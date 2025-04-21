using System.Threading.Tasks;
using Refit;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.APIs;

public interface IAuthApi
{
    [Post("v1/usuario")]
    Task<BaseResponse<UsuarioViewModel>> RegistrarAsync(UsuarioNovoViewModel user);
    [Put("v1/usuario")]
    Task<BaseResponse<object>> AtualizarAsync(UsuarioAtualizarViewModel user);
    [Get("v1/usuario/{userId}")]
    Task<BaseResponse<UsuarioViewModel>> ObterUsuarioAsync(int userId);
}