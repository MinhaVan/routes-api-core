using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IMotoristaRotaService
{
    Task VincularAsync(MotoristaVincularViewModel request);
    Task DesvincularAsync(MotoristaVincularViewModel request);
    Task<List<MotoristaViewModel>> BuscarMotoristasPorRotaAsync(int rotaId);
}