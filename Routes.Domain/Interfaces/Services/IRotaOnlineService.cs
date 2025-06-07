using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IRotaOnlineService
{
    Task<RotaViewModel> RotaOnlineParaMotoristaAsync();
}