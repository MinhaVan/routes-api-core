using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Domain.Interfaces.Services;

public interface ITrajetoService
{
    Task<List<Marcador>> GerarMelhorTrajetoAsync(int rotaId);
    Task AtualizarStatusAlunoTrajetoAsync(int alunoId, int rotaId, bool alunoEntrouNaVan);
    Task<RotaHistoricoViewModel> ObterTrajetoEmAndamentoAsync(int rotaId);
    Task<List<Marcador>> ObterDestinoAsync(int rotaId, bool validarRotaOnline = true);
}