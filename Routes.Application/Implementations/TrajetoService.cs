using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Exceptions;
using Microsoft.Extensions.Logging;
using Routes.Domain.Interfaces.APIs;

namespace Routes.Service.Implementations;

public class TrajetoService(
    ILogger<TrajetoService> logger,
    IMapper mapper,
    IPessoasAPI pessoasAPI,
    IMarcadorService marcadorService,
    IOrdemTrajetoService ordemTrajetoService,
    IBaseRepository<AjusteAlunoRota> ajusteAlunoRotaRepository,
    IBaseRepository<Endereco> enderecoRepository,
    IRotaHistoricoRepository rotaHistoricoRepository,
    IBaseRepository<OrdemTrajeto> ordemTrajetoRepository,
    IBaseRepository<AlunoRotaHistorico> alunoRotaHistoricoRepository,
    IGoogleDirectionsService googleDirectionsService,
    IBaseRepository<Rota> rotaRepository
) : ITrajetoService
{
    private readonly IMarcadorService _marcadorService = marcadorService;
    private readonly ILogger<TrajetoService> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IPessoasAPI _pessoasAPI = pessoasAPI;
    private readonly IBaseRepository<AjusteAlunoRota> _ajusteAlunoRotaRepository = ajusteAlunoRotaRepository;
    private readonly IBaseRepository<Endereco> _enderecoRepository = enderecoRepository;
    private readonly IRotaHistoricoRepository _rotaHistoricoRepository = rotaHistoricoRepository;
    private readonly IBaseRepository<OrdemTrajeto> _ordemTrajetoRepository = ordemTrajetoRepository;
    private readonly IBaseRepository<AlunoRotaHistorico> _alunoRotaHistoricoRepository = alunoRotaHistoricoRepository;
    private readonly IGoogleDirectionsService _googleDirectionsService = googleDirectionsService;
    private readonly IOrdemTrajetoService _ordemTrajetoService = ordemTrajetoService;
    private readonly IBaseRepository<Rota> _rotaRepository = rotaRepository;

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
    public async Task GerarMelhorTrajetoAsync(int rotaId)
    {
        var rota = await _rotaRepository.BuscarUmAsync(x => x.Id == rotaId, z => z.AlunoRotas);
        var alunosId = rota.AlunoRotas.Select(x => x.AlunoId).ToList();
        var alunos = (await _pessoasAPI.ObterAlunoPorIdAsync(alunosId)).Data;

        var ordemTrajeto = await _ordemTrajetoRepository.BuscarUmAsync(
            x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo, x => x.Marcadores);

        var marcadores = _marcadorService.ObterMarcadorPorRotaDirecao(alunos, rota.TipoRota, rotaId);

        if (marcadores.Count < 2)
            throw new BusinessRuleException("Não há marcadores suficientes para gerar o trajeto.");

        var origem = marcadores.First();
        var destino = marcadores.Last();
        var pontosIntermediarios = marcadores.Skip(1).Take(marcadores.Count - 2).ToList();

        List<Marcador> rotaIdeal;
        if (pontosIntermediarios.Count > 1)
        {
            var rotaIdealResponse = await _googleDirectionsService.ObterRotaIdealAsync(origem, destino, pontosIntermediarios);
            if (rotaIdealResponse.Sucesso is false || rotaIdealResponse.Data is null)
                throw new BusinessRuleException("Não foi possível obter a rota ideal.");

            rotaIdeal = rotaIdealResponse.Data;
        }
        else
        {
            rotaIdeal = [origem, .. pontosIntermediarios, destino];
        }

        await _ordemTrajetoService.AtualizarOuCriarOrdemTrajeto(ordemTrajeto, rotaId, rotaIdeal);
    }
}