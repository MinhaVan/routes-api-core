using System.Threading.Tasks;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.WebSocket;

namespace Routes.Domain.Interfaces.Services;

public interface ILocalizacaoCache
{
    Task<BaseResponse<EnviarLocalizacaoWebSocketResponse>> ObterUltimaLocalizacaoAsync(int rotaId);
    Task SalvarUltimaLocalizacaoAsync(int rotaId, BaseResponse<EnviarLocalizacaoWebSocketResponse> localizacao);
}