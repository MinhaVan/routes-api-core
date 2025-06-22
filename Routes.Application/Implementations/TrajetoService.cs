using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Exceptions;
using Microsoft.Extensions.Logging;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Utils;

namespace Routes.Service.Implementations;

public class TrajetoService(
    ILogger<TrajetoService> _logger,
    IMapper _mapper,
    IPessoasAPI _pessoasAPI,
    IMarcadorService _marcadorService,
    IRabbitMqRepository _rabbitMqRepository,
    IOrdemTrajetoService _ordemTrajetoService,
    IBaseRepository<AjusteAlunoRota> _ajusteAlunoRotaRepository,
    IBaseRepository<Endereco> _enderecoRepository,
    IRotaHistoricoRepository _rotaHistoricoRepository,
    IAuthApi _authApi,
    IBaseRepository<OrdemTrajeto> _ordemTrajetoRepository,
    IBaseRepository<AlunoRotaHistorico> _alunoRotaHistoricoRepository,
    IGoogleDirectionsService _googleDirectionsService,
    IBaseRepository<Rota> _rotaRepository
) : ITrajetoService
{
    public async Task AtualizarStatusAlunoTrajetoAsync(int alunoId, int rotaId, bool alunoEntrouNaVan)
    {
        try
        {
            var trajetoEmAndamento = await _rotaHistoricoRepository.BuscarUmAsync(r => r.RotaId == rotaId && r.EmAndamento);
            _ = trajetoEmAndamento ?? throw new BusinessRuleException("Não há um trajeto em andamento para essa rota.");

            var alunoRotaHistorico = await _alunoRotaHistoricoRepository.BuscarUmAsync(x => x.AlunoId == alunoId && x.RotaHistoricoId == trajetoEmAndamento.Id);
            if (alunoRotaHistorico is not null && alunoRotaHistorico.EntrouNaVan == alunoEntrouNaVan)
                return;

            var alunos = await _pessoasAPI.ObterAlunoPorIdAsync(new List<int> { alunoId });
            var aluno = alunos.Data.FirstOrDefault();
            _ = aluno ?? throw new BusinessRuleException("Aluno não encontrado!");

            if (alunoRotaHistorico is not null)
            {
                alunoRotaHistorico.EntrouNaVan = alunoEntrouNaVan;
                await _alunoRotaHistoricoRepository.AtualizarAsync(alunoRotaHistorico);
            }
            else
            {
                var model = new AlunoRotaHistorico
                {
                    EntrouNaVan = alunoEntrouNaVan,
                    RotaHistoricoId = trajetoEmAndamento.Id,
                    AlunoId = aluno.Id
                };

                await _alunoRotaHistoricoRepository.AdicionarAsync(model);
                alunoRotaHistorico = model;
            }

            if (alunoEntrouNaVan is false)
            {
                var responsavel = await _authApi.ObterUsuarioAsync(aluno.ResponsavelId);
                var request = new
                {
                    Nome = $"{responsavel.Data.PrimeiroNome} {responsavel.Data.UltimoNome}",
                    Contato = responsavel.Data.Contato,
                    Email = responsavel.Data.Email,
                    TipoNotificacao = TipoNotificacaoEnum.AlunoNaoEntrouNaVan,
                };

                _rabbitMqRepository.Publish(
                    RabbitMqQueues.EnviarNotificacao,
                    new BaseQueue<object>
                    {
                        Mensagem = request,
                        Retry = 0
                    }
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FinalizarTrajetoAsync] Ocorreu um erro ao finalizar o trajeto da rota {0}.", rotaId);
            throw;
        }
    }

    public async Task<RotaHistoricoViewModel> ObterTrajetoEmAndamentoAsync(int rotaId)
    {
        var trajetoEmAndamento = await _rotaHistoricoRepository.BuscarUmAsync(
            r => r.RotaId == rotaId && r.EmAndamento
        );

        return _mapper.Map<RotaHistoricoViewModel>(trajetoEmAndamento);
    }

    public async Task<List<Marcador>> ObterDestinoAsync(int rotaId, bool validarRotaOnline = true)
    {
        var rota = await _rotaRepository.BuscarUmAsync(x => x.Id == rotaId, z => z.AlunoRotas);
        var alunosId = rota.AlunoRotas.Select(x => x.AlunoId).ToList();
        var alunos = (await _pessoasAPI.ObterAlunoPorIdAsync(alunosId)).Data;

        if (rota.DeveBuscarRotaNoGoogleMaps)
        {
            return await GerarMelhorTrajetoAsync(rotaId);
        }

        var ordemTrajeto = await _ordemTrajetoRepository.BuscarUmAsync(
            x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo, x => x.Marcadores);

        if (ordemTrajeto is null)
        {
            var nowUtc = DateTime.UtcNow;
            var ajusteAlunoRota = await _ajusteAlunoRotaRepository.BuscarAsync(
                x => alunosId.Contains(x.AlunoId) && x.RotaId == rotaId && x.Data.Date == nowUtc.Date,
                z => z.EnderecoDestino,
                z => z.EnderecoPartida,
                z => z.EnderecoRetorno);

            if (validarRotaOnline)
            {
                var trajetoOnline = await _rotaHistoricoRepository.BuscarUmAsync(
                    x => x.RotaId == rotaId && x.EmAndamento, x => x.Rota);
                if (trajetoOnline is null || trajetoOnline.Id == 0)
                    throw new BusinessRuleException("Trajeto não está online!");
            }

            return _marcadorService.ObterMarcadorPorRotaDirecao(alunos, rota.TipoRota, rotaId, ajusteAlunoRota);
        }
        else
        {
            var enderecosIds = ordemTrajeto.Marcadores.Select(x => x.EnderecoId).ToList();
            var enderecos = await _enderecoRepository.BuscarAsync(x => enderecosIds.Contains(x.Id));

            return ordemTrajeto.Marcadores.Select(m =>
            {
                var endereco = enderecos.FirstOrDefault(z => z.Id == m.EnderecoId);
                var alunosMarcador = alunos.Where(a =>
                    m.TipoMarcador == TipoMarcadorEnum.Partida ? a.EnderecoPartida.Id == m.EnderecoId :
                    m.TipoMarcador == TipoMarcadorEnum.Destino || m.TipoMarcador == TipoMarcadorEnum.InicioRetorno ? a.EnderecoDestinoId == m.EnderecoId :
                    m.TipoMarcador == TipoMarcadorEnum.Retorno ? a.EnderecoRetornoId == m.EnderecoId : false
                ).ToList();

                return new Marcador
                {
                    EnderecoId = m.EnderecoId,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    TipoMarcador = m.TipoMarcador,
                    Titulo = endereco?.ObterEndereco() ?? string.Empty,
                    Alunos = _mapper.Map<List<AlunoViewModel>>(alunosMarcador),
                    Aluno = _mapper.Map<AlunoViewModel>(alunosMarcador.FirstOrDefault())
                };
            }).ToList();
        }
    }

    /// <summary>
    /// Gera o melhor trajeto para a rota especificada, otimizando a ordem dos marcadores.
    /// Se a ordem do trajeto já existir, ela será atualizada com a nova rota ideal.
    /// </summary>
    /// <param name="rotaId"></param>
    /// <returns></returns>
    /// <exception cref="BusinessRuleException"></exception>
    public async Task<List<Marcador>> GerarMelhorTrajetoAsync(int rotaId)
    {
        var rota = await _rotaRepository.BuscarUmAsync(x => x.Id == rotaId, z => z.AlunoRotas)
            ?? throw new BusinessRuleException("Rota não encontrada.");

        var alunosId = rota.AlunoRotas.Select(x => x.AlunoId).ToList();
        var alunos = (await _pessoasAPI.ObterAlunoPorIdAsync(alunosId)).Data;

        // Remove ordem antiga se existir
        var ordemExistente = await _ordemTrajetoRepository.BuscarUmAsync(
            x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo,
            x => x.Marcadores
        );

        if (ordemExistente is not null && ordemExistente.Marcadores.Any())
        {
            ordemExistente.SetDeletado();
            await _ordemTrajetoRepository.AtualizarAsync(ordemExistente);
        }

        var novaOrdem = new OrdemTrajeto
        {
            RotaId = rotaId,
            Status = StatusEntityEnum.Ativo,
            Marcadores = new()
        };

        var marcadores = _marcadorService.ObterMarcadorPorRotaDirecao(alunos, rota.TipoRota, rotaId);
        if (marcadores.Count < 2)
            throw new BusinessRuleException("Não há marcadores suficientes para gerar o trajeto.");

        var origem = marcadores.First();
        var destino = marcadores.Last();
        var intermediarios = marcadores.Skip(1).Take(marcadores.Count - 2).ToList();

        var rotaIdeal = intermediarios.Count > 1
            ? await ObterRotaIdeal(origem, destino, intermediarios, novaOrdem)
            : [origem, .. intermediarios, destino];

        return await _ordemTrajetoService.CriarOrdemTrajetoAsync(novaOrdem, rotaId, rotaIdeal);
    }

    private async Task<List<Marcador>> ObterRotaIdeal(Marcador origem, Marcador destino, List<Marcador> pontos, OrdemTrajeto ordem)
    {
        var resposta = await _googleDirectionsService.ObterRotaIdealAsync(origem, destino, pontos);
        if (!resposta.Sucesso || resposta.Data is null)
            throw new BusinessRuleException("Não foi possível obter a rota ideal.");

        ordem.GeradoAutomaticamente = true;
        return resposta.Data;
    }
}