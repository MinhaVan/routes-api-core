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
    IUserContext userContext,
    IAuthApi authApi,
    IPessoasAPI pessoasAPI,
    IBaseRepository<AjusteAlunoRota> ajusteAlunoRotaRepository,
    IBaseRepository<Endereco> enderecoRepository,
    IBaseRepository<OrdemTrajetoMarcador> ordemTrajetoMarcadorRepository,
    IRotaHistoricoRepository rotaHistoricoRepository,
    IBaseRepository<OrdemTrajeto> ordemTrajetoRepository,
    IBaseRepository<MotoristaRota> motoristaRotaRepository,
    IBaseRepository<AlunoRotaHistorico> alunoRotaHistoricoRepository,
    IGoogleDirectionsService googleDirectionsService,
    IBaseRepository<Rota> rotaRepository
) : ITrajetoService
{
    private readonly ILogger<TrajetoService> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IUserContext _userContext = userContext;
    private readonly IAuthApi _authApi = authApi;
    private readonly IPessoasAPI _pessoasAPI = pessoasAPI;
    private readonly IBaseRepository<AjusteAlunoRota> _ajusteAlunoRotaRepository = ajusteAlunoRotaRepository;
    private readonly IBaseRepository<Endereco> _enderecoRepository = enderecoRepository;
    private readonly IBaseRepository<OrdemTrajetoMarcador> _ordemTrajetoMarcadorRepository = ordemTrajetoMarcadorRepository;
    private readonly IRotaHistoricoRepository _rotaHistoricoRepository = rotaHistoricoRepository;
    private readonly IBaseRepository<OrdemTrajeto> _ordemTrajetoRepository = ordemTrajetoRepository;
    private readonly IBaseRepository<MotoristaRota> _motoristaRotaRepository = motoristaRotaRepository;
    private readonly IBaseRepository<AlunoRotaHistorico> _alunoRotaHistoricoRepository = alunoRotaHistoricoRepository;
    private readonly IGoogleDirectionsService _googleDirectionsService = googleDirectionsService;
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
        int rotaId,
        IEnumerable<AjusteAlunoRota> ajusteAlunoRota = null)
    {
        ajusteAlunoRota = ajusteAlunoRota ?? new List<AjusteAlunoRota>();
        var marcadoresPartidas = new Dictionary<string, Marcador>();
        var marcadoresDestinos = new Dictionary<string, Marcador>();
        var now = DateTime.Now;

        foreach (var aluno in alunos)
        {
            Func<AjusteAlunoRota, bool> predicate = x => x.AlunoId == aluno.Id && x.Data.Date == now.Date && x.RotaId == rotaId;
            var ajuste = ajusteAlunoRota.Any(predicate) ? ajusteAlunoRota.FirstOrDefault(predicate) : null;
            if (tipoRota == TipoRotaEnum.Ida)
            {
                // Adiciona todas as partidas primeiro
                var latPartida = ajuste?.EnderecoPartida?.Latitude ?? aluno.EnderecoPartida.Latitude;
                var lngPartida = ajuste?.EnderecoPartida?.Longitude ?? aluno.EnderecoPartida.Longitude;
                var chavePartida = $"{latPartida},{lngPartida}";

                if (!marcadoresPartidas.ContainsKey(chavePartida))
                {
                    marcadoresPartidas[chavePartida] = new Marcador
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
                    marcadoresPartidas[chavePartida].Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }
            else
            {
                // Adiciona todos os destinos primeiro
                var latDestino = ajuste?.EnderecoDestino?.Latitude ?? aluno.EnderecoDestino.Latitude;
                var lngDestino = ajuste?.EnderecoDestino?.Longitude ?? aluno.EnderecoDestino.Longitude;
                var chaveDestino = $"{latDestino},{lngDestino}";

                if (!marcadoresPartidas.ContainsKey(chaveDestino))
                {
                    marcadoresPartidas[chaveDestino] = new Marcador
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
                    marcadoresPartidas[chaveDestino].Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }

            // Adiciona os destinos ou retornos, dependendo do tipo de rota
            if (tipoRota == TipoRotaEnum.Ida)
            {
                var latDestino = ajuste?.EnderecoDestino?.Latitude ?? aluno.EnderecoDestino.Latitude;
                var lngDestino = ajuste?.EnderecoDestino?.Longitude ?? aluno.EnderecoDestino.Longitude;
                var chaveDestino = $"{latDestino},{lngDestino}";

                if (!marcadoresDestinos.ContainsKey(chaveDestino))
                {
                    marcadoresDestinos[chaveDestino] = new Marcador
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
                    marcadoresDestinos[chaveDestino].Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }
            else
            {
                var latRetorno = aluno.EnderecoRetorno.Latitude;
                var lngRetorno = aluno.EnderecoRetorno.Longitude;
                var chaveRetorno = $"{latRetorno},{lngRetorno}";

                if (!marcadoresDestinos.ContainsKey(chaveRetorno))
                {
                    marcadoresDestinos[chaveRetorno] = new Marcador
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
                    marcadoresDestinos[chaveRetorno].Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }
        }

        // Unifica os marcadoresDestinos dentro de marcadoresPartidas
        foreach (var kvp in marcadoresDestinos)
        {
            if (marcadoresPartidas.TryGetValue(kvp.Key, out var marcadorExistente))
            {
                // Junta os alunos de destinos na lista existente
                marcadorExistente.Alunos.AddRange(kvp.Value.Alunos);
            }
            else
            {
                // Se não existe, adiciona normalmente
                marcadoresPartidas[kvp.Key] = kvp.Value;
            }
        }


        return marcadoresPartidas.Values.ToList();
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

        marcadores.AddRange(ObterMarcadorPorRotaDirecao(alunos, rota.TipoRota, rotaId));
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

            marcadores = ObterMarcadorPorRotaDirecao(alunos, rota.TipoRota, rotaId, ajusteAlunoRota);
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

    /// <summary>
    /// Gera o melhor trajeto para a rota especificada, otimizando a ordem dos marcadores.
    /// Se a ordem do trajeto já existir, ela será atualizada com a nova rota ideal.
    /// </summary>
    /// <param name="rotaId"></param>
    /// <returns></returns>
    /// <exception cref="BusinessRuleException"></exception>
    public async Task GerarMelhorTrajetoAsync(int rotaId)
    {
        var rota = await _rotaRepository
            .BuscarUmAsync(x => x.Id == rotaId, z => z.AlunoRotas);

        var alunosId = rota.AlunoRotas.Select(x => x.AlunoId).ToList();

        var alunosResponse = await _pessoasAPI.ObterAlunoPorIdAsync(alunosId);
        var alunos = alunosResponse.Data;

        var ordemTrajeto = await _ordemTrajetoRepository.BuscarUmAsync(x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo, x => x.Marcadores);

        // var saoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        // var nowUtc = DateTime.UtcNow;
        // var ajusteAlunoRota = await _ajusteAlunoRotaRepository
        //     .BuscarAsync(x =>
        //         alunosId.Contains(x.AlunoId) && x.RotaId == rotaId && x.Data.Date == nowUtc.Date,
        //         z => z.EnderecoDestino,
        //         z => z.EnderecoPartida,
        //         z => z.EnderecoRetorno);

        var marcadores = ObterMarcadorPorRotaDirecao(alunos, rota.TipoRota, rotaId);
        var origem = marcadores.First();
        var destino = marcadores.Last();
        var pontosIntermediarios = marcadores.Skip(1).Take(marcadores.Count - 2).ToList();
        var rotaIdeal = new List<Marcador>();

        if (pontosIntermediarios.Count > 1)
        {
            var rotaIdealResponse = await _googleDirectionsService.ObterRotaIdealAsync(origem, destino, pontosIntermediarios);
            if (rotaIdealResponse.Sucesso is false || rotaIdealResponse.Data is null)
            {
                throw new BusinessRuleException("Não foi possível obter a rota ideal.");
            }

            rotaIdeal = rotaIdealResponse.Data;
        }
        else
        {
            rotaIdeal = [origem, .. pontosIntermediarios, destino];
        }

        if (ordemTrajeto is null)
        {
            ordemTrajeto = new OrdemTrajeto
            {
                RotaId = rotaId,
            };
            await _ordemTrajetoRepository.AdicionarAsync(ordemTrajeto);
        }
        else
        {
            await Parallel.ForEachAsync(ordemTrajeto.Marcadores, async (ordemTrajetoMarcador, _) =>
            {
                ordemTrajetoMarcador.Status = StatusEntityEnum.Deletado;
                await Task.CompletedTask;
            });

            await _ordemTrajetoMarcadorRepository.AtualizarAsync(ordemTrajeto.Marcadores);
        }

        var ordemTrajetoMarcador = rotaIdeal.Select(rota =>
        {
            return new OrdemTrajetoMarcador
            {
                Status = StatusEntityEnum.Ativo,
                OrdemTrajetoId = ordemTrajeto.Id,
                TipoMarcador = rota.TipoMarcador,
                Latitude = rota.Latitude,
                Longitude = rota.Longitude,
            };
        });

        await _ordemTrajetoMarcadorRepository.AdicionarAsync(ordemTrajetoMarcador);
    }

    public async Task<RotaHistoricoViewModel> RelatorioUltimoTrajetoAsync(int rotaId)
    {
        var rotaHistorico = await _rotaHistoricoRepository.ObterUltimoTrajetoAsync(rotaId);
        return _mapper.Map<RotaHistoricoViewModel>(rotaHistorico);
    }

    public async Task<RotaViewModel> RotaOnlineParaMotoristaAsync()
    {
        var obterMotoristaPorIdResponse = await _pessoasAPI.ObterMotoristaPorUsuarioIdAsync(_userContext.UserId);
        if (!obterMotoristaPorIdResponse.Sucesso)
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