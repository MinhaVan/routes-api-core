using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IAlunoRotaService
{
    Task VincularRotaAsync(int rotaId, int alunoId);
    Task DesvincularRotaAsync(int rotaId, int alunoId);
    Task<List<AlunoRotaViewModel>> ObterRotasPorAlunoAsync(int alunoId, int rotaId);
}