using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Service.Exceptions;

namespace Routes.Application.Implementations;

public class GestaoTrajetoService(
    IBaseRepository<Rota> rotaRepository,
    IBaseRepository<RotaHistorico> rotaHistoricoRepository,
    ILogger<GestaoTrajetoService> logger
) : IGestaoTrajetoService
{
    private readonly IBaseRepository<Rota> _rotaRepository = rotaRepository;
    private readonly IBaseRepository<RotaHistorico> _rotaHistoricoRepository = rotaHistoricoRepository;
    private readonly ILogger<GestaoTrajetoService> _logger = logger;
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
}