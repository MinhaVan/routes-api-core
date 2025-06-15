using System.Threading.Tasks;
using Routes.Domain.ViewModels.WebSocket;

namespace Routes.Domain.Interfaces.Services;

public interface ILocalizacaoTrajetoService
{
    Task AdicionarLocalizacaoTrajetoAsync(EnviarLocalizacaoWebSocketResponse localizacao);
}