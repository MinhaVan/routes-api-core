using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Services;

public interface IEnderecoService
{
    Task AdicionarAsync(EnderecoAdicionarViewModel enderecoAdicionarViewModel);
    Task AtualizarAsync(EnderecoAtualizarViewModel enderecoAdicionarViewModel);
    Task DeletarAsync(int id);
    Task<EnderecoViewModel> Obter(int id);
    Task<List<EnderecoViewModel>> Obter(List<int> ids);
    Task<List<EnderecoViewModel>> Obter();
}