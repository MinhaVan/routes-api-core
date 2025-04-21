using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.APIs;

public interface IAuthApi
{
    [Post("v1/token/login")]
    Task<BaseResponse<TokenViewModel>> LoginAsync(UsuarioLoginViewModel body);

    [Post("v1/token/refreshToken")]
    Task<BaseResponse<TokenViewModel>> RefreshTokenAsync(UsuarioLoginViewModel body);


    [Get("v1/perfil/refreshToken")]
    Task<BaseResponse<Dictionary<string, int>>> ObterPerfilsAsync();




    [Get("v1/usuario/{userId}")]
    Task<BaseResponse<UsuarioViewModel>> ObterUsuarioAsync(int userId);
    [Get("v1/usuario/me")]
    Task<BaseResponse<UsuarioViewModel>> ObterMeusDadosAsync();
    [Post("v1/usuario")]
    Task<BaseResponse<UsuarioViewModel>> RegistrarAsync(UsuarioNovoViewModel user);
    [Put("v1/usuario")]
    Task<BaseResponse<object>> AtualizarAsync(UsuarioAtualizarViewModel user);
}