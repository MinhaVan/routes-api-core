using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.WebSocket;
using Routes.Domain.Interfaces.APIs;
using System.Collections.Concurrent;
using Routes.Domain.Interfaces.Services;

namespace Routes.Service.Hubs;

public class RotaHub : Hub
{
    private readonly ILogger<RotaHub> _logger;
    private readonly IBaseRepository<Rota> _rotaRepository;
    private readonly IAuthApi _authApi;
    private static readonly ConcurrentDictionary<int, BaseResponse<EnviarLocalizacaoWebSocketResponse>> _ultimaLocalizacaoPorRota = new();
    private readonly ILocalizacaoCache _localizacaoCache;

    public RotaHub(
        ILogger<RotaHub> logger,
        IAuthApi authApi,
        IBaseRepository<Rota> rotaRepository,
        ILocalizacaoCache localizacaoCache)
    {
        _logger = logger;
        _localizacaoCache = localizacaoCache;
        _authApi = authApi;
        _rotaRepository = rotaRepository;
    }

    #region Public Methods

    public async Task EnviarLocalizacao(EnviarLocalizacaoWebSocketRequest data)
    {
        if (data == null)
            return;

        var response = new BaseResponse<EnviarLocalizacaoWebSocketResponse>
        {
            Data = new EnviarLocalizacaoWebSocketResponse(
                data.Latitude,
                data.Longitude,
                data.RotaId,
                data.AlunoId,
                data.Destino.Latitude,
                data.Destino.Longitude,
                data.TipoMensagem
            ),
            Mensagem = "Localização recebida com sucesso!",
            Sucesso = true
        };

        await _localizacaoCache.SalvarUltimaLocalizacaoAsync(data.RotaId, response);

        var tasks = new[]
        {
            Clients.Group(data.RotaId.ToString()).SendAsync("ReceberLocalizacao", response)
        };

        await Task.WhenAll(tasks).ContinueWith(task =>
        {
            if (task.IsFaulted)
                _logger.LogError(task.Exception, "Erro ao enviar localização para o grupo {RotaId}", data.RotaId);
        });
    }

    public async Task AdicionarResponsavelNaRota(int responsavelId, int rotaId)
    {
        if (responsavelId <= 0 || rotaId <= 0)
        {
            await EnviarRespostaErro("IDs inválidos para o responsável ou rota.");
            return;
        }

        try
        {
            var autorizado = await ValidarResponsavel(responsavelId, rotaId);

            if (!autorizado)
            {
                await EnviarRespostaErro("Você ainda não pode receber os dados da localização!");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, rotaId.ToString());

            var ultimaLocalizacao = await _localizacaoCache.ObterUltimaLocalizacaoAsync(rotaId)
                ?? new BaseResponse<EnviarLocalizacaoWebSocketResponse>
                {
                    Data = null,
                    Mensagem = "Nenhuma localização disponível no momento.",
                    Sucesso = true
                };

            await Clients.Caller.SendAsync("ReceberLocalizacao", ultimaLocalizacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar o responsável {ResponsavelId} na rota {RotaId}", responsavelId, rotaId);
            await EnviarRespostaErro("Ocorreu um erro ao tentar receber os dados da localização!");
        }
    }


    public async Task RemoverResponsavelDaRota(int rotaId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, rotaId.ToString());
    }

    #endregion Public Methods

    #region Private Methods

    private async Task<bool> ValidarResponsavel(int responsavelId, int rotaId)
    {
        var obterUsuarioResponse = await _authApi.ObterUsuarioAsync(responsavelId);
        var rota = await _rotaRepository.BuscarUmAsync(r => r.Id == rotaId, r => r.AlunoRotas);

        if (!obterUsuarioResponse.Sucesso || obterUsuarioResponse.Data == null)
            return false;

        var usuario = obterUsuarioResponse.Data;
        var idsAlunosResponsavel = usuario.Alunos.Select(a => a.Id).ToHashSet();

        return rota.AlunoRotas.Any(ar => idsAlunosResponsavel.Contains(ar.AlunoId));
    }

    private async Task EnviarRespostaErro(string mensagem)
    {
        await Clients.Caller.SendAsync("ReceberLocalizacao", new BaseResponse<EnviarLocalizacaoWebSocketResponse>
        {
            Data = null,
            Mensagem = mensagem,
            Sucesso = false
        });
    }
    #endregion Private Methods
}
