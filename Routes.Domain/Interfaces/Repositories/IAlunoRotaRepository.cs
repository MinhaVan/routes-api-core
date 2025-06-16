using System.Threading.Tasks;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Models;

namespace Routes.Domain.Interfaces.Repositories;

public interface IAlunoRotaRepository : IBaseRepository<AlunoRota>
{
    Task AtualizarStatusAsync(AlunoRota alunoRota);
}