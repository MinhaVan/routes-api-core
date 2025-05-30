using System.Collections.Concurrent;
using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.WebSocket;

namespace Routes.Application.Implementations;

public class LocalizacaoCacheEmMemoria : ILocalizacaoCache
{
    private readonly ConcurrentDictionary<int, BaseResponse<EnviarLocalizacaoWebSocketResponse>> _cache = new();

    public Task<BaseResponse<EnviarLocalizacaoWebSocketResponse>> ObterUltimaLocalizacaoAsync(int rotaId)
    {
        _cache.TryGetValue(rotaId, out var localizacao);
        return Task.FromResult(localizacao);
    }

    public Task SalvarUltimaLocalizacaoAsync(int rotaId, BaseResponse<EnviarLocalizacaoWebSocketResponse> localizacao)
    {
        _cache[rotaId] = localizacao;
        return Task.CompletedTask;
    }
}