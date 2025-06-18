using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IVeiculoService
{
    Task AdicionarAsync(List<VeiculoAdicionarViewModel> veiculosViewModels);
    Task AtualizarAsync(List<VeiculoAtualizarViewModel> veiculosViewModels);
    Task<List<VeiculoViewModel>> ObterAsync();
    Task<IEnumerable<VeiculoViewModel>> ObterVeiculoAsync(IEnumerable<int> veiculoIds);
    Task DeletarAsync(int id);
    Task<VeiculoViewModel> ObterAsync(int veiculoId, int rotaId, bool completarDadosDoUsuario = false);
}