using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IRelatorioTrajetoService
{
    Task<RotaHistoricoViewModel> RelatorioUltimoTrajetoAsync(int rotaId);
}