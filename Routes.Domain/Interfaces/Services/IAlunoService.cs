using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IAlunoService
{
    Task AdicionarAsync(int responsavelId, AlunoAdicionarViewModel Alunos);
    Task AtualizarAsync(int responsavelId, AlunoViewModel Alunos);
    Task DeletarAsync(int responsavelId, int AlunoId);
    Task<IList<AlunoViewModel>> ObterTodos(int responsavelId);
    Task<IList<AlunoViewModel>> ObterAluno(int responsavelId, int AlunoId);
    Task VincularRotaAsync(int rotaId, int AlunoId);
    Task DesvincularRotaAsync(int rotaId, int AlunoId);
    Task<List<AlunoViewModel>> ObterAlunosPorFiltro(int rotaId, string filtro);
}