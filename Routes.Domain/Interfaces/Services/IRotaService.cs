using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IRotaService
{
    Task<List<RotaViewModel>> ObterTodosAsync(int empresaId, bool incluirDeletados = false, bool incluirDetalhes = false);
    Task<RotaViewModel> AdicionarAsync(RotaAdicionarViewModel rotaAdicionarViewModel);
    Task AtualizarAsync(RotaAtualizarViewModel rotaAtualizarViewModel);
    Task DeletarAsync(int id);
    Task<List<RotaViewModel>> ObterRotasDosFilhosAsync();
    Task<RotaDetalheViewModel> ObterDetalheAsync(int id);
    Task<List<RotaViewModel>> ObterPorEmpresaAsync(int empresaId);
    Task<List<RotaViewModel>> ObterRotasOnlineAsync();
    Task<RotaViewModel> ObterPorRotaIdAsync(int rotaId);
    Task<List<RotaViewModel>> ObterPorAlunoIdAsync(int id);
    Task<List<RotaViewModel>> ObterRotaDoMotoristaAsync(int motoristaId, bool filtrarApenasHoje);
}