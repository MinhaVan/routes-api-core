using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IRotaService
{
    Task<List<RotaViewModel>> ObterTodosAsync();
    Task<RotaViewModel> AdicionarAsync(RotaAdicionarViewModel rotaAdicionarViewModel);
    Task AtualizarAsync(RotaAtualizarViewModel rotaAtualizarViewModel);
    Task DeletarAsync(int id);
    Task<RotaViewModel> ObterAsync(int id);
    Task<RotaDetalheViewModel> ObterDetalheAsync(int id);
    Task<List<RotaViewModel>> ObterAsync();
    Task<List<RotaViewModel>> ObterRotasOnlineAsync();
    Task<List<RotaViewModel>> ObterPorAlunoIdAsync(int id);
    Task<List<RotaViewModel>> ObterRotaDoMotoristaAsync(int motoristaId, bool filtrarApenasHoje);
}