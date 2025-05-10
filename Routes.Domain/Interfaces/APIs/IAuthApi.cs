using System.Threading.Tasks;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.APIs;

public interface IAuthApi
{
    Task<BaseResponse<UsuarioViewModel>> RegistrarAsync(UsuarioNovoViewModel user);
    Task<BaseResponse<object>> AtualizarAsync(UsuarioAtualizarViewModel user);
    Task<BaseResponse<UsuarioViewModel>> ObterUsuarioAsync(int userId);
}