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

public class TrajetoService : ITrajetoService
{
    private readonly IMapper _mapper;
    private readonly ILogger<TrajetoService> _logger;
    private readonly IUserContext _userContext;
    private readonly IBaseRepository<Rota> _rotaRepository;
    private readonly IAuthApi _authApi;
    private readonly IPessoasAPI _pessoasAPI;
    private readonly IBaseRepository<OrdemTrajeto> _ordemTrajetoRepository;
    private readonly IBaseRepository<Endereco> _enderecoRepository;
    private readonly IBaseRepository<AjusteAlunoRota> _ajusteAlunoRotaRepository;
    private readonly IBaseRepository<AlunoRotaHistorico> _alunoRotaHistoricoRepository;
    private readonly IRotaHistoricoRepository _rotaHistoricoRepository;
    private readonly IBaseRepository<MotoristaRota> _motoristaRotaRepository;
    public TrajetoService(
        ILogger<TrajetoService> logger,
        IMapper mapper,
        IUserContext userContext,
        IAuthApi authApi,
        IPessoasAPI pessoasAPI,
        IBaseRepository<AjusteAlunoRota> ajusteAlunoRotaRepository,
        IBaseRepository<Endereco> enderecoRepository,
        IRotaHistoricoRepository rotaHistoricoRepository,
        IBaseRepository<OrdemTrajeto> ordemTrajetoRepository,
        IBaseRepository<MotoristaRota> motoristaRotaRepository,
        IBaseRepository<AlunoRotaHistorico> alunoRotaHistoricoRepository,
        IBaseRepository<Rota> rotaRepository)
    {
        _logger = logger;
        _userContext = userContext;
        _mapper = mapper;
        _authApi = authApi;
        _pessoasAPI = pessoasAPI;
        _enderecoRepository = enderecoRepository;
        _ordemTrajetoRepository = ordemTrajetoRepository;
        _alunoRotaHistoricoRepository = alunoRotaHistoricoRepository;
        _motoristaRotaRepository = motoristaRotaRepository;
        _ajusteAlunoRotaRepository = ajusteAlunoRotaRepository;
        _rotaRepository = rotaRepository;
        _rotaHistoricoRepository = rotaHistoricoRepository;
    }

    public async Task AtualizarStatusAlunoTrajetoAsync(int alunoId, int rotaId, bool alunoEntrouNaVan)
    {
        try
        {
            var trajetoEmAndamento = await _rotaHistoricoRepository.BuscarUmAsync(r => r.RotaId == rotaId && r.EmAndamento);
            _ = trajetoEmAndamento ?? throw new BusinessRuleException("Não há um trajeto em andamento para essa rota.");

            var alunoRotaHistorico = await _alunoRotaHistoricoRepository.BuscarUmAsync(x => x.AlunoId == alunoId && x.RotaHistoricoId == trajetoEmAndamento.Id);
            if (alunoRotaHistorico is not null && alunoRotaHistorico.EntrouNaVan == alunoEntrouNaVan)
                return;

            var aluno = await _pessoasAPI.ObterAlunoPorIdAsync(alunoId);
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
                    AlunoId = aluno.Data.Id
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

    public async Task FinalizarTrajetoAsync(int rotaId)
    {
        try
        {
            // _logger.LogInformation("[FinalizarTrajetoAsync] Finalizando trajeto {0}", rotaId);
            var trajetoEmAndamento = await _rotaHistoricoRepository.BuscarUmAsync(
                r => r.RotaId == rotaId && r.EmAndamento
            );

            if (trajetoEmAndamento == null)
            {
                _logger.LogInformation("[FinalizarTrajetoAsync] Não há um trajeto em andamento para a rota {0}.", rotaId);
                throw new BusinessRuleException("Não há um trajeto em andamento para essa rota.");
            }

            trajetoEmAndamento.DataFim = DateTime.UtcNow;
            trajetoEmAndamento.EmAndamento = false;

            // _logger.LogInformation("[FinalizarTrajetoAsync] Atualizando trajeto {0}", JsonConvert.SerializeObject(trajetoEmAndamento, Formatting.None));
            await _rotaHistoricoRepository.AtualizarAsync(trajetoEmAndamento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FinalizarTrajetoAsync] Ocorreu um erro ao finalizar o trajeto da rota {0}.", rotaId);
            throw;
        }
    }

    public async Task IniciarTrajetoAsync(int rotaId)
    {
        try
        {
            var saoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            var nowUtc = DateTime.UtcNow;
            var now = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, saoPauloTimeZone);
            var rota = await _rotaRepository.BuscarUmAsync(x => x.Id == rotaId);
            var trajetoEmAndamento = await _rotaHistoricoRepository.BuscarUmAsync(
                r => r.RotaId == rotaId && r.EmAndamento
            );

            if (trajetoEmAndamento is not null)
            {
                // Já existe um trajeto em andamento para a rota
                var trintaMinutosAtras = nowUtc.AddMinutes(-30);

                if (trajetoEmAndamento.DataRealizacao.Year == nowUtc.Year &&
                    trajetoEmAndamento.DataRealizacao.Month == nowUtc.Month &&
                    trajetoEmAndamento.DataRealizacao.Day == nowUtc.Day &&
                    trintaMinutosAtras <= trajetoEmAndamento.DataRealizacao &&
                    trajetoEmAndamento.DataRealizacao <= nowUtc)
                {
                    return;
                }

                trajetoEmAndamento.EmAndamento = false;
                trajetoEmAndamento.DataFim = DateTime.UtcNow;
                await _rotaHistoricoRepository.AtualizarAsync(trajetoEmAndamento);
            }

            var currentTime = TimeOnly.FromDateTime(now);
            var trintaMinAntes = currentTime.AddMinutes(-30);
            var trintaMinDepois = currentTime.AddMinutes(30);

            var novoTrajeto = new RotaHistorico
            {
                RotaId = rotaId,
                DataRealizacao = DateTime.UtcNow,
                EmAndamento = true,
                TipoRota = rota.TipoRota
            };

            // _logger.LogInformation("[IniciarTrajetoAsync] Salvando novo trajeto {0}", JsonConvert.SerializeObject(novoTrajeto, Formatting.None));
            await _rotaHistoricoRepository.AdicionarAsync(novoTrajeto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[IniciarTrajetoAsync] Ocorreu um erro ao iniciar o trajeto da rota {0}.", rotaId);
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

    private List<Marcador> ObterMarcadorPorRotaDirecao(
        IEnumerable<AlunoViewModel> alunos,
        TipoRotaEnum tipoRota,
        IEnumerable<AjusteAlunoRota> ajusteAlunoRota = null)
    {
        var marcadores = new Dictionary<string, Marcador>();

        foreach (var aluno in alunos)
        {
            if (tipoRota == TipoRotaEnum.Ida)
            {
                // Adiciona todas as partidas primeiro
                var latPartida = aluno.EnderecoPartida.Latitude;
                var lngPartida = aluno.EnderecoPartida.Longitude;
                var chavePartida = $"{latPartida},{lngPartida}";

                if (!marcadores.ContainsKey(chavePartida))
                {
                    marcadores[chavePartida] = new Marcador
                    {
                        EnderecoId = aluno.EnderecoPartidaId,
                        Titulo = aluno.EnderecoPartida.ObterEndereco(),
                        TipoMarcador = TipoMarcadorEnum.Partida,
                        Latitude = latPartida,
                        Longitude = lngPartida,
                        Aluno = _mapper.Map<AlunoViewModel>(aluno),
                        Alunos = new List<AlunoViewModel> { _mapper.Map<AlunoViewModel>(aluno) }
                    };
                }
                else
                {
                    // Adiciona o aluno à lista existente
                    marcadores[chavePartida].Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }
            else
            {
                // Adiciona todos os destinos primeiro
                var latDestino = aluno.EnderecoDestino.Latitude;
                var lngDestino = aluno.EnderecoDestino.Longitude;
                var chaveDestino = $"{latDestino},{lngDestino}";

                if (!marcadores.ContainsKey(chaveDestino))
                {
                    marcadores[chaveDestino] = new Marcador
                    {
                        EnderecoId = aluno.EnderecoDestinoId,
                        Titulo = aluno.EnderecoDestino.ObterEndereco(),
                        TipoMarcador = TipoMarcadorEnum.InicioRetorno,
                        Latitude = latDestino,
                        Longitude = lngDestino,
                        Aluno = _mapper.Map<AlunoViewModel>(aluno),
                        Alunos = new List<AlunoViewModel> { _mapper.Map<AlunoViewModel>(aluno) }
                    };
                }
                else
                {
                    // Adiciona o aluno à lista existente
                    marcadores[chaveDestino].Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }
        }

        // Adiciona os destinos ou retornos, dependendo do tipo de rota
        foreach (var aluno in alunos)
        {
            if (tipoRota == TipoRotaEnum.Ida)
            {
                var latDestino = aluno.EnderecoDestino.Latitude;
                var lngDestino = aluno.EnderecoDestino.Longitude;
                var chaveDestino = $"{latDestino},{lngDestino}";

                if (!marcadores.ContainsKey(chaveDestino))
                {
                    marcadores[chaveDestino] = new Marcador
                    {
                        EnderecoId = aluno.EnderecoDestinoId,
                        Titulo = aluno.EnderecoDestino.ObterEndereco(),
                        TipoMarcador = TipoMarcadorEnum.Destino,
                        Latitude = latDestino,
                        Longitude = lngDestino,
                        Aluno = _mapper.Map<AlunoViewModel>(aluno),
                        Alunos = new List<AlunoViewModel> { _mapper.Map<AlunoViewModel>(aluno) }
                    };
                }
                else
                {
                    // Adiciona o aluno à lista existente
                    marcadores[chaveDestino].Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }
            else
            {
                var latRetorno = aluno.EnderecoRetorno.Latitude;
                var lngRetorno = aluno.EnderecoRetorno.Longitude;
                var chaveRetorno = $"{latRetorno},{lngRetorno}";

                if (!marcadores.ContainsKey(chaveRetorno))
                {
                    marcadores[chaveRetorno] = new Marcador
                    {
                        EnderecoId = aluno.EnderecoRetornoId.Value,
                        Titulo = aluno.EnderecoRetorno.ObterEndereco(),
                        TipoMarcador = TipoMarcadorEnum.Retorno,
                        Latitude = latRetorno,
                        Longitude = lngRetorno,
                        Aluno = _mapper.Map<AlunoViewModel>(aluno),
                        Alunos = new List<AlunoViewModel> { _mapper.Map<AlunoViewModel>(aluno) }
                    };
                }
                else
                {
                    // Adiciona o aluno à lista existente
                    marcadores[chaveRetorno].Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }
        }

        return marcadores.Values.ToList();
    }

    public async Task SalvarOrdemDoTrajetoAsync(int rotaId, List<Marcador> marcadoresOrdenados)
    {
        var ordemTrajeto = await _ordemTrajetoRepository.BuscarUmAsync(x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo);
        if (ordemTrajeto is not null)
        {
            ordemTrajeto.Status = StatusEntityEnum.Deletado;
            await _ordemTrajetoRepository.AtualizarAsync(ordemTrajeto);
        }

        var novaOrdemTrajeto = new OrdemTrajeto
        {
            RotaId = rotaId,
            Status = StatusEntityEnum.Ativo,
            Marcadores = marcadoresOrdenados.Select(x => new OrdemTrajetoMarcador
            {
                EnderecoId = x.EnderecoId,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                TipoMarcador = x.TipoMarcador,
                Status = StatusEntityEnum.Ativo
            }).ToList()
        };

        await _ordemTrajetoRepository.AdicionarAsync(novaOrdemTrajeto);
    }

    public async Task<List<Marcador>> ObterTodosMarcadoresParaRotasAsync(int rotaId)
    {
        var marcadores = new List<Marcador>();
        var rota = await _rotaRepository.BuscarUmAsync(x => x.Id == rotaId, z => z.AlunoRotas.Where(x => x.Status == StatusEntityEnum.Ativo));

        var alunosId = rota.AlunoRotas.Select(x => x.AlunoId).ToList();
        var alunosResponse = await _pessoasAPI.ObterAlunoPorIdAsync(alunosId);
        var alunos = alunosResponse.Data;

        var ajusteAlunoRota = await _ajusteAlunoRotaRepository
            .BuscarAsync(x => alunosId.Contains(x.AlunoId) && x.RotaId == rotaId,
                z => z.EnderecoDestino,
                z => z.EnderecoPartida,
                z => z.EnderecoRetorno);

        switch (rota.TipoRota)
        {
            case TipoRotaEnum.Ida:
                marcadores.AddRange(ObterMarcadorPorRotaDirecao(alunos, TipoRotaEnum.Ida));
                break;
            case TipoRotaEnum.Volta:
                marcadores.AddRange(ObterMarcadorPorRotaDirecao(alunos, TipoRotaEnum.Volta));
                break;
            default:
                throw new BusinessRuleException("Tipo de rota inválida!");
        }

        return marcadores;
    }

    public async Task<List<Marcador>> ObterDestinoAsync(int rotaId, bool validarRotaOnline = true)
    {
        List<Marcador> marcadores = new();
        var rota = await _rotaRepository
            .BuscarUmAsync(x => x.Id == rotaId, z => z.AlunoRotas);

        var alunosId = rota.AlunoRotas.Select(x => x.AlunoId).ToList();

        var alunosResponse = await _pessoasAPI.ObterAlunoPorIdAsync(alunosId);
        var alunos = alunosResponse.Data;

        var ordemTrajeto = await _ordemTrajetoRepository.BuscarUmAsync(x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo, x => x.Marcadores);
        if (ordemTrajeto is null)
        {
            var saoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            var nowUtc = DateTime.UtcNow;

            var ajusteAlunoRota = await _ajusteAlunoRotaRepository
                .BuscarAsync(x =>
                    alunosId.Contains(x.AlunoId) && x.RotaId == rotaId && x.Data.Date == nowUtc.Date,
                    z => z.EnderecoDestino,
                    z => z.EnderecoPartida,
                    z => z.EnderecoRetorno);

            if (validarRotaOnline)
            {
                var trajetoOnline = await _rotaHistoricoRepository.BuscarUmAsync(x => x.RotaId == rotaId && x.EmAndamento, x => x.Rota);
                if (trajetoOnline is null || trajetoOnline.Id == 0)
                {
                    throw new BusinessRuleException("Trajeto não está online!");
                }
            }

            marcadores = ObterMarcadorPorRotaDirecao(alunos, rota.TipoRota, ajusteAlunoRota);
        }
        else
        {
            var enderecosIds = ordemTrajeto.Marcadores.Select(x => x.EnderecoId);
            var enderecos = await _enderecoRepository.BuscarAsync(x => enderecosIds.Contains(x.Id));

            marcadores = ordemTrajeto.Marcadores.Select(m =>
                new Marcador
                {
                    EnderecoId = m.EnderecoId,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    TipoMarcador = m.TipoMarcador,
                    Titulo = enderecos.First(z => z.Id == m.EnderecoId).ObterEndereco() ?? string.Empty,
                    Alunos = _mapper.Map<List<AlunoViewModel>>(
                        alunos.Where(a =>
                            m.TipoMarcador == TipoMarcadorEnum.Partida ?
                                a.EnderecoPartida.Id == m.EnderecoId :
                            m.TipoMarcador == TipoMarcadorEnum.Destino || m.TipoMarcador == TipoMarcadorEnum.InicioRetorno ?
                                a.EnderecoDestinoId == m.EnderecoId :
                            m.TipoMarcador == TipoMarcadorEnum.Retorno ?
                                a.EnderecoRetornoId == m.EnderecoId : false
                        )
                    ),
                    Aluno = _mapper.Map<AlunoViewModel>(
                        alunos.FirstOrDefault(a =>
                            m.TipoMarcador == TipoMarcadorEnum.Partida ?
                                a.EnderecoPartida.Id == m.EnderecoId :
                            m.TipoMarcador == TipoMarcadorEnum.Destino || m.TipoMarcador == TipoMarcadorEnum.InicioRetorno ?
                                a.EnderecoDestinoId == m.EnderecoId :
                            m.TipoMarcador == TipoMarcadorEnum.Retorno ?
                                a.EnderecoRetornoId == m.EnderecoId : false
                        )
                    )
                }
            ).ToList();
        }

        return marcadores;
    }

    public async Task<RotaHistoricoViewModel> RelatorioUltimoTrajetoAsync(int rotaId)
    {
        var rotaHistorico = await _rotaHistoricoRepository.ObterUltimoTrajetoAsync(rotaId);
        return _mapper.Map<RotaHistoricoViewModel>(rotaHistorico);
    }

    public async Task<RotaViewModel> RotaOnlineParaMotoristaAsync()
    {
        var obterUsuarioResponse = await _authApi.ObterUsuarioAsync(_userContext.UserId);
        if (!obterUsuarioResponse.Sucesso || obterUsuarioResponse.Data == null)
            throw new BusinessRuleException(obterUsuarioResponse.Mensagem);

        var usuario = obterUsuarioResponse.Data;

        var obterMotoristaPorIdResponse = await _pessoasAPI.ObterMotoristaPorIdAsync(usuario.Id);
        if (!obterMotoristaPorIdResponse.Sucesso || obterMotoristaPorIdResponse.Data == null)
            throw new BusinessRuleException(obterMotoristaPorIdResponse.Mensagem);

        var motorista = obterMotoristaPorIdResponse.Data;
        var motoristaRotas = await _motoristaRotaRepository.BuscarAsync(x => x.MotoristaId == motorista.Id, z => z.Rota);
        var rotasId = motoristaRotas.Select(x => x.RotaId);

        var trajetoOnline = await _rotaHistoricoRepository.BuscarUmAsync(x =>
            rotasId.Contains(x.RotaId) &&
            x.EmAndamento == true &&
            x.DataFim == null,
            x => x.Rota
        );

        if (trajetoOnline is not null && trajetoOnline.Id > 0)
        {
            var viewModel = _mapper.Map<RotaViewModel>(trajetoOnline.Rota);
            return viewModel;
        }

        return null;
    }
}