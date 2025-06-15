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
using Microsoft.AspNetCore.Authorization;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Utils;

namespace Routes.Service.Hubs;

[Authorize]
public class RotaHub(
    ILogger<RotaHub> _logger,
    IPessoasAPI _pessoasAPI,
    IBaseRepository<Rota> _rotaRepository,
    IRabbitMqRepository _rabbitMqRepository,
    IRedisRepository _localizacaoCache) : Hub
{

    #region Public Methods

    public async Task EnviarLocalizacao(EnviarLocalizacaoWebSocketRequest data)
    {
        if (data is null)
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

        var mensagem = new BaseQueue<EnviarLocalizacaoWebSocketResponse>
        {
            Mensagem = response.Data,
            Retry = 0
        };
        _rabbitMqRepository.Publish(RabbitMqQueues.EnviarLocalizacao, mensagem, shouldThrowException: false);

        var tasks = new[]
        {
            _localizacaoCache.SetAsync(ObterRedisKey(data.RotaId), response.Data),
            Clients.Group(data.RotaId.ToString()).SendAsync("ReceberLocalizacao", response)
        };

        await Task.WhenAll(tasks).ContinueWith(task =>
        {
            if (task.IsFaulted)
                _logger.LogError(task.Exception, "Erro ao enviar localização para o grupo {RotaId}", data.RotaId);
        });
    }

    public async Task AdicionarResponsavelNaRota(int responsavelId, int rotaId, string accessToken)
    {
        if (responsavelId <= 0 || rotaId <= 0)
        {
            await EnviarRespostaErro("IDs inválidos para o responsável ou rota.");
            return;
        }

        try
        {
            var autorizado = await ValidarResponsavel(rotaId, accessToken);
            if (!autorizado)
            {
                await EnviarRespostaErro("Você ainda não pode receber os dados da localização!");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, rotaId.ToString());

            var localizacaoCache = await _localizacaoCache.GetAsync<EnviarLocalizacaoWebSocketResponse>(ObterRedisKey(rotaId));
            var ultimaLocalizacao = new BaseResponse<EnviarLocalizacaoWebSocketResponse>
            {
                Data = localizacaoCache,
                Mensagem = localizacaoCache is null ? "Nenhuma localização disponível no momento." : "Localização recebida com sucesso!",
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

    public async Task RemoverResponsavelDaRota(int rotaId, string accessToken)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, rotaId.ToString());
    }

    #endregion Public Methods

    #region Private Methods

    public string ObterRedisKey(int rotaId)
        => string.Format(KeyRedis.EnviarLocalizacao, rotaId);

    private async Task<bool> ValidarResponsavel(int rotaId, string accessToken)
    {
        var alunosResponse = await _pessoasAPI.ObterAlunoPorResponsavelIdAsync(completarDadosDoUsuario: true, token: string.Format("Bearer {0}", accessToken));
        if (!alunosResponse.Sucesso || alunosResponse.Data == null)
            return false;

        var rota = await _rotaRepository.BuscarUmAsync(r => r.Id == rotaId, r => r.AlunoRotas);
        var alunos = alunosResponse.Data;
        var idsAlunosResponsavel = alunos.Select(a => a.Id).ToHashSet();
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
