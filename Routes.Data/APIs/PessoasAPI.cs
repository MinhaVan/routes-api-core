using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Routes.Data.Utils;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.ViewModels;

namespace Routes.Data.APIs;

public class PessoasAPI : IPessoasAPI
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PessoasAPI> _logger;
    private readonly IUserContext _context;

    public PessoasAPI(IHttpClientFactory httpClientFactory, IUserContext userContext, ILogger<PessoasAPI> logger)
    {
        _logger = logger;
        _context = userContext;
        _httpClient = httpClientFactory.CreateClient("api-pessoas");
    }

    public async Task<BaseResponse<List<AlunoViewModel>>> ObterAlunoPorIdAsync(List<int> alunosId)
    {
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var query = string.Join("&", alunosId.Select(id => $"alunosIds={id}"));

        var response = await _httpClient.GetAsync($"v1/Aluno/Lista?{query}");
        if (response.IsSuccessStatusCode)
        {
            var alunos = await response.Content.ReadFromJsonAsync<BaseResponse<List<AlunoViewModel>>>();
            _logger.LogInformation($"Resposta da requisição para obter dados do aluno - Dados: {alunos.ToJson()}");
            return alunos;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao obter dados do aluno - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao tentar obter dados do aluno!");
        }
    }

    public async Task<BaseResponse<List<AlunoViewModel>>> ObterAlunoPorResponsavelIdAsync(bool completarDadosDoUsuario = true, string token = null)
    {
        _logger.LogInformation($"Enviando requisição para obter todos os alunos do responsavel - _context.Token: {token}");
        if (string.IsNullOrEmpty(token))
        {
            token = _context.Token;
        }

        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", token);
        var response = await _httpClient.GetAsync($"v1/Aluno?completarDadosDoUsuario={completarDadosDoUsuario}");
        _logger.LogInformation($"Retorno obter todos os alunos do responsavel");

        if (response.IsSuccessStatusCode)
        {
            var aluno = await response.Content.ReadFromJsonAsync<BaseResponse<List<AlunoViewModel>>>();
            _logger.LogInformation($"Resposta da requisição para obter todos os alunos do responsavel - Dados: {aluno.ToJson()}");
            return aluno;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao obter dados do aluno - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao tentar obter todos os alunos do responsavel");
        }
    }

    public async Task<BaseResponse<MotoristaViewModel>> ObterMotoristaPorIdAsync(int motoristaId, bool completarDadosDoUsuario = false)
    {
        _logger.LogInformation($"Enviando requisição para obter dados do motorista - MotoristaId: {motoristaId} ComplementarDados: {completarDadosDoUsuario}");
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var response = await _httpClient.GetAsync($"v1/Motorista/{motoristaId}?completarDadosDoUsuario={completarDadosDoUsuario}");

        if (response.IsSuccessStatusCode)
        {
            var motorista = await response.Content.ReadFromJsonAsync<BaseResponse<MotoristaViewModel>>();
            _logger.LogInformation($"Resposta da requisição para obter dados do motorista - Dados: {motorista.ToJson()}");
            return motorista;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao obter dados do motorista - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao tentar obter dados do motorista!");
        }
    }

    public async Task<BaseResponse<MotoristaViewModel>> ObterMotoristaPorUsuarioIdAsync(int usuarioId, bool completarDadosDoUsuario = false)
    {
        _logger.LogInformation($"Enviando requisição para obter dados do motorista pelo usuarioId - Dados: {usuarioId} {completarDadosDoUsuario}");
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var response = await _httpClient.GetAsync($"v1/Motorista/Usuario/{usuarioId}?completarDadosDoUsuario={completarDadosDoUsuario}");

        if (response.IsSuccessStatusCode)
        {
            var motorista = await response.Content.ReadFromJsonAsync<BaseResponse<MotoristaViewModel>>();
            _logger.LogInformation($"Resposta da requisição para obter dados do motorista pelo usuarioId - Dados: {motorista.ToJson()}");
            return motorista;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao obter dados do motorista pelo usuarioId - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao tentar obter dados do motorista pelo usuarioId!");
        }
    }
}