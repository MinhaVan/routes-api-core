using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.Enums;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Domain.Interfaces.Services;

public interface IMarcadorService
{
    Task<List<Marcador>> ObterTodosMarcadoresParaRotasAsync(int rotaId);
    List<Marcador> ObterMarcadorPorRotaDirecao(
        IEnumerable<AlunoViewModel> alunos, TipoRotaEnum tipoRota,
        int rotaId, IEnumerable<AjusteAlunoRota> ajusteAlunoRota = null
    );
}