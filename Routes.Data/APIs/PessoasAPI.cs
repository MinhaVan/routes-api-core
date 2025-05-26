using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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

    public async Task<BaseResponse<IEnumerable<AlunoViewModel>>> ObterAlunoPorIdAsync(List<int> alunosId)
    {
        _logger.LogInformation($"Enviando requisição para obter dados do aluno - Dados: {alunosId.ToJson()}");
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var idsQuery = string.Join("&alunosId=", alunosId);
        var response = await _httpClient.GetAsync($"v1/Aluno?alunosId={idsQuery}");

        if (response.IsSuccessStatusCode)
        {
            var aluno = await response.Content.ReadFromJsonAsync<BaseResponse<IEnumerable<AlunoViewModel>>>();
            _logger.LogInformation($"Resposta da requisição para obter dados do aluno - Dados: {aluno.ToJson()}");
            return aluno;
        }
        else
        {
            var mensagemErro = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Erro ao obter dados do aluno - Mensagem: {mensagemErro}");
            throw new Exception("Ocorreu um erro ao tentar obter dados do aluno!");
        }
    }

    public async Task<BaseResponse<List<AlunoViewModel>>> ObterAlunoPorResponsavelIdAsync()
    {
        _logger.LogInformation($"Enviando requisição para obter todos os alunos do responsavel");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var response = await _httpClient.GetAsync($"v1/Aluno/");

        if (response.IsSuccessStatusCode)
        {
            var xxxx = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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

    public async Task<BaseResponse<MotoristaViewModel>> ObterMotoristaPorIdAsync(int motoristaId)
    {
        _logger.LogInformation($"Enviando requisição para obter dados do motorista - Dados: {motoristaId}");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var response = await _httpClient.GetAsync($"v1/Motorista/Usuario/{motoristaId}");

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

    public async Task<BaseResponse<MotoristaViewModel>> ObterMotoristaPorUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation($"Enviando requisição para obter dados do motorista pelo usuarioId - Dados: {usuarioId}");
        _httpClient.DefaultRequestHeaders.Add("Authorization", _context.Token);
        var response = await _httpClient.GetAsync($"v1/Motorista/Usuario/{usuarioId}");

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