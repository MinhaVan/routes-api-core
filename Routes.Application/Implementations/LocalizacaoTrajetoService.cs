using System.Threading.Tasks;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels.WebSocket;

namespace Routes.Application.Implementations;

public class LocalizacaoTrajetoService(
    IBaseRepository<LocalizacaoTrajeto> _localizacaoTrajetoRepository,
    IBaseRepository<RotaHistorico> _rotaHistoricoRepository,
    IRedisRepository _redisRepository
) : ILocalizacaoTrajetoService
{
    public async Task AdicionarLocalizacaoTrajetoAsync(EnviarLocalizacaoWebSocketResponse localizacao)
    {
        var chave = $"rotaHistorico:{localizacao.RotaId}";
        var rotaHistorico = await _redisRepository.GetAsync<RotaHistorico>(chave);
        if (rotaHistorico is null || rotaHistorico.EmAndamento == false)
        {
            await _redisRepository.DeleteAsync(chave);
            rotaHistorico = await _rotaHistoricoRepository.BuscarUmAsync(x => x.RotaId == localizacao.RotaId && x.EmAndamento);
            if (rotaHistorico == null)
                return;

            await _redisRepository.SetAsync(chave, rotaHistorico);
        }

        var localizacaoTrajeto = new LocalizacaoTrajeto
        {
            RotaId = localizacao.RotaId,
            RotaHistoricoId = rotaHistorico.Id,
            Latitude = localizacao.Latitude,
            Longitude = localizacao.Longitude
        };

        await _localizacaoTrajetoRepository.AdicionarAsync(localizacaoTrajeto);
    }
}