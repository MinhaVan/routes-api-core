using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Application.Implementations;

public class OrdemTrajetoService(
    IBaseRepository<OrdemTrajetoMarcador> _ordemTrajetoMarcadorRepository,
    IBaseRepository<OrdemTrajeto> _ordemTrajetoRepository) : IOrdemTrajetoService
{
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
                EnderecoId = x.EnderecoId ?? 0,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                TipoMarcador = x.TipoMarcador,
                Status = StatusEntityEnum.Ativo
            }).ToList()
        };

        await _ordemTrajetoRepository.AdicionarAsync(novaOrdemTrajeto);
    }


    public async Task AtualizarOuCriarOrdemTrajeto(OrdemTrajeto ordemTrajeto, int rotaId, List<Marcador> rotaIdeal)
    {
        if (ordemTrajeto is null)
        {
            ordemTrajeto = new OrdemTrajeto
            {
                RotaId = rotaId,
                Status = StatusEntityEnum.Ativo
            };
            await _ordemTrajetoRepository.AdicionarAsync(ordemTrajeto);
        }
        else
        {
            foreach (var marcador in ordemTrajeto.Marcadores)
                marcador.Status = StatusEntityEnum.Deletado;

            await _ordemTrajetoMarcadorRepository.AtualizarAsync(ordemTrajeto.Marcadores);
        }

        var ordemTrajetoMarcadores = rotaIdeal.Select(rota => new OrdemTrajetoMarcador
        {
            Status = StatusEntityEnum.Ativo,
            OrdemTrajetoId = ordemTrajeto.Id,
            TipoMarcador = rota.TipoMarcador,
            Latitude = rota.Latitude,
            Longitude = rota.Longitude,
            EnderecoId = rota.EnderecoId ?? 0
        });

        await _ordemTrajetoMarcadorRepository.AdicionarAsync(ordemTrajetoMarcadores);
    }
}