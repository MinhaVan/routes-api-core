using System.Threading.Tasks;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;

namespace Routes.Domain.Interfaces.Repositories;

public interface IMotoristaRotaRepository : IBaseRepository<MotoristaRota>
{
    Task AtualizarStatusAsync(int motoristaId, int rotaId, StatusEntityEnum status);
}