using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.APIs;

public interface IPessoasAPI
{
    Task<BaseResponse<List<AlunoViewModel>>> ObterAlunoPorIdAsync(List<int> alunosId);
    Task<BaseResponse<List<AlunoViewModel>>> ObterAlunoPorResponsavelIdAsync();
    Task<BaseResponse<MotoristaViewModel>> ObterMotoristaPorIdAsync(int motoristaId, bool completarDadosDoUsuario = false);
    Task<BaseResponse<MotoristaViewModel>> ObterMotoristaPorUsuarioIdAsync(int usuarioId, bool completarDadosDoUsuario = false);
}