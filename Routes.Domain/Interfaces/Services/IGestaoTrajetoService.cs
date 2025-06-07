using System.Threading.Tasks;

namespace Routes.Domain.Interfaces.Services;

public interface IGestaoTrajetoService
{
    Task FinalizarTrajetoAsync(int rotaId);
    Task IniciarTrajetoAsync(int rotaId);
}