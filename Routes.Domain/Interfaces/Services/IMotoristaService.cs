using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IMotoristaService
{
    Task VincularAsync(MotoristaVincularViewModel request);
    Task DesvincularAsync(MotoristaVincularViewModel request);
    Task<MotoristaViewModel> AdicionarAsync(MotoristaNovoViewModel usuarioAdicionarViewModel);
    Task AtualizarAsync(MotoristaAtualizarViewModel usuarioAdicionarViewModel);
}