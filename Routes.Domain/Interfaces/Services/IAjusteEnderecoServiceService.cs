using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Domain.Interfaces.Services;

public interface IAjusteEnderecoService
{
    Task<List<RotaAjusteEnderecoViewModel>> ObterAjusteEnderecoAsync(int AlunoId, int rotaId);
    Task AdicionarAjusteEnderecoAsync(RotaAdicionarAjusteEnderecoViewModel alterarEnderecoViewModel);
    Task AlterarAjusteEnderecoAsync(RotaAlterarAjusteEnderecoViewModel alterarAjusteEnderecoViewModel);
}