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
            ordemTrajeto.SetDeletado();
            await _ordemTrajetoRepository.AtualizarAsync(ordemTrajeto);
        }

        var novaOrdemTrajeto = new OrdemTrajeto
        {
            RotaId = rotaId,
            GeradoAutomaticamente = false,
            Status = StatusEntityEnum.Ativo,
            Marcadores = marcadoresOrdenados.Select(x => new OrdemTrajetoMarcador
            {
                Ordem = x.Ordem,
                EnderecoId = x.EnderecoId ?? 0,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                TipoMarcador = x.TipoMarcador,
                Status = StatusEntityEnum.Ativo
            }).ToList()
        };

        await _ordemTrajetoRepository.AdicionarAsync(novaOrdemTrajeto);
    }


    public async Task<List<Marcador>> CriarOrdemTrajetoAsync(OrdemTrajeto ordemTrajeto, int rotaId, List<Marcador> rotaIdeal)
    {
        var ordemTrajetoModel = await _ordemTrajetoRepository
            .BuscarUmAsync(x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo, x => x.Marcadores);

        if (ordemTrajetoModel is not null)
        {
            foreach (var marcador in ordemTrajetoModel.Marcadores)
                marcador.Status = StatusEntityEnum.Deletado;

            ordemTrajetoModel.Status = StatusEntityEnum.Deletado;
            await _ordemTrajetoRepository.AtualizarAsync(ordemTrajetoModel);
        }

        await _ordemTrajetoRepository.AdicionarAsync(ordemTrajeto);
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
        return rotaIdeal;
    }
}