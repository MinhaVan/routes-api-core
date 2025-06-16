using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.ViewModels;
using Routes.Data.Utils;
using Routes.Domain.Interfaces.Repositories;

namespace Routes.Data.APIs;

public class AuthAPI(
    IHttpClientFactory httpClientFactory,
    IUserContext userContext,
    ILogger<AuthAPI> logger) : IAuthApi
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("api-auth");
    private readonly ILogger<AuthAPI> _logger = logger;
    private readonly IUserContext _context = userContext;

    public async Task<BaseResponse<UsuarioViewModel>> RegistrarAsync(UsuarioNovoViewModel user)
    {
        _logger.LogInformation($"Enviando requisição para registrar usuário - Dados: {user.ToJson()}");
        var response = await _httpClient.PostAsJsonAsync("v1/usuario", user);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<BaseResponse<UsuarioViewModel>>();
            _logger.LogInformation($"Usuário registrado com sucesso - Dados: {result.ToJson()}");
            return result;
        }

        var mensagemErro = await response.Content.ReadAsStringAsync();
        _logger.LogError($"Erro ao registrar usuário - Mensagem: {mensagemErro}");
        throw new Exception("Erro ao registrar usuário.");
    }

    public async Task<BaseResponse<object>> AtualizarAsync(UsuarioAtualizarViewModel user)
    {
        _logger.LogInformation($"Enviando requisição para atualizar usuário - Dados: {user.ToJson()}");
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var response = await _httpClient.PutAsJsonAsync("v1/usuario", user);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<BaseResponse<object>>();
            _logger.LogInformation($"Usuário atualizado com sucesso - Dados: {result.ToJson()}");
            return result;
        }

        var mensagemErro = await response.Content.ReadAsStringAsync();
        _logger.LogError($"Erro ao atualizar usuário - Mensagem: {mensagemErro}");
        throw new Exception("Erro ao atualizar usuário.");
    }

    public async Task<BaseResponse<UsuarioViewModel>> ObterUsuarioAsync(int userId)
    {
        _logger.LogInformation($"Enviando requisição para obter usuário - ID: {userId}");
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var response = await _httpClient.GetAsync($"v1/usuario/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<BaseResponse<UsuarioViewModel>>();
            _logger.LogInformation($"Usuário obtido com sucesso - Dados: {result.ToJson()}");
            return result;
        }

        var mensagemErro = await response.Content.ReadAsStringAsync();
        _logger.LogError($"Erro ao obter usuário - Mensagem: {mensagemErro}");
        throw new Exception("Erro ao obter usuário.");
    }
}
