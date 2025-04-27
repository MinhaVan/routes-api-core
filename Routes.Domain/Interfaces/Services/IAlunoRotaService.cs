using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IAlunoRotaService
{
    Task AdicionarAsync(AlunoRotaViewModel alunoRota);
    Task AtualizarAsync(AlunoRotaViewModel alunoRota);
    Task<List<AlunoRotaViewModel>> ObterRotasPorAlunoAsync(int rotaId, int? alunoId = null);

    Task VincularRotaAsync(int rotaId, int alunoId);
    Task DesvincularRotaAsync(int rotaId, int alunoId);
}