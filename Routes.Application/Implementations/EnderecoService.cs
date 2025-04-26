using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Configuration;
using Routes.Service.Exceptions;

namespace Routes.Service.Implementations;

public class EnderecoService : IEnderecoService
{
    private readonly IMapper _mapper;
    private readonly SecretManager _secretManager;
    private readonly IUserContext _userContext;
    private readonly IBaseRepository<Endereco> _enderecoRepository;
    private readonly IAuthApi _authApi;
    private readonly HttpClient _googleMapsCliente;
    public EnderecoService(
        IMapper mapper,
        IAuthApi authApi,
        IBaseRepository<Endereco> enderecoRepository,
        SecretManager secretManager,
        IHttpClientFactory httpClientFactory,
        IUserContext userContext)
    {
        _secretManager = secretManager;
        _googleMapsCliente = httpClientFactory.CreateClient("api-googlemaps"); ;
        _mapper = mapper;
        _userContext = userContext;
        _authApi = authApi;
        _enderecoRepository = enderecoRepository;
    }

    public async Task AdicionarAsync(EnderecoAdicionarViewModel enderecoAdicionarViewModel)
    {
        var model = _mapper.Map<Endereco>(enderecoAdicionarViewModel);
        model.UsuarioId = _userContext.UserId;
        model.Status = Domain.Enums.StatusEntityEnum.Ativo;

        var marcador = await ObterMarcadorAsync(model.ObterEnderecoCompleto());
        model.Latitude = marcador.Latitude;
        model.Longitude = marcador.Longitude;

        await _enderecoRepository.AdicionarAsync(model);
    }

    public async Task AtualizarAsync(EnderecoAtualizarViewModel enderecoAtualizarViewModel)
    {
        var model = await _enderecoRepository.ObterPorIdAsync(enderecoAtualizarViewModel.Id);

        if ((model.Rua?.Trim() ?? string.Empty) != (enderecoAtualizarViewModel.Rua?.Trim() ?? string.Empty) ||
            (model.Numero ?? string.Empty) != (enderecoAtualizarViewModel.Numero ?? string.Empty) ||
            (model.Bairro?.Trim() ?? string.Empty) != (enderecoAtualizarViewModel.Bairro?.Trim() ?? string.Empty) ||
            (model.Cidade?.Trim() ?? string.Empty) != (enderecoAtualizarViewModel.Cidade?.Trim() ?? string.Empty) ||
            (model.Estado?.Trim() ?? string.Empty) != (enderecoAtualizarViewModel.Estado?.Trim() ?? string.Empty) ||
            (model.CEP ?? string.Empty) != (enderecoAtualizarViewModel.CEP ?? string.Empty))
        {
            var enderecoRequest = $"{model.Rua ?? string.Empty} {model.Numero ?? string.Empty}, {model.Bairro ?? string.Empty}, {model.Cidade ?? string.Empty}, {model.Estado ?? string.Empty}, {model.CEP ?? string.Empty}, Brazil";
            var marcador = await ObterMarcadorAsync(enderecoRequest);
            model.Latitude = marcador.Latitude;
            model.Longitude = marcador.Longitude;
        }

        model.Rua = enderecoAtualizarViewModel.Rua.Trim();
        model.Numero = enderecoAtualizarViewModel.Numero;
        model.Complemento = enderecoAtualizarViewModel.Complemento;
        model.Bairro = enderecoAtualizarViewModel.Bairro;
        model.Cidade = enderecoAtualizarViewModel.Cidade;
        model.Estado = enderecoAtualizarViewModel.Estado;
        model.CEP = enderecoAtualizarViewModel.CEP;
        model.Pais = enderecoAtualizarViewModel.Pais;
        model.TipoEndereco = enderecoAtualizarViewModel.TipoEndereco.Value;

        await _enderecoRepository.AtualizarAsync(model);
    }

    public async Task DeletarAsync(int id)
    {
        await _enderecoRepository.DeletarAsync(id);
    }

    public async Task<EnderecoViewModel> Obter(int id)
        => _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorIdAsync(id));

    public async Task<List<EnderecoViewModel>> Obter()
    {
        var obterUsuarioResponse = await _authApi.ObterUsuarioAsync(_userContext.UserId);
        if (!obterUsuarioResponse.Sucesso || obterUsuarioResponse.Data == null)
            throw new BusinessRuleException(obterUsuarioResponse.Mensagem);

        var usuario = obterUsuarioResponse.Data;
        var enderecos = usuario.Enderecos.Where(x => x.Status == Domain.Enums.StatusEntityEnum.Ativo);
        return _mapper.Map<List<EnderecoViewModel>>(enderecos);
    }

    private async Task<Marcador> ObterMarcadorAsync(string endereco)
    {
        var requestUri = $"{_secretManager.Google.BaseUrl}/geocode/json?address={Uri.EscapeDataString(endereco)}&key={_secretManager.Google.Key}";
        var geocodeResponse = await _googleMapsCliente.GetFromJsonAsync<GoogleGeocodeResponse>(requestUri);

        // Verifica se a resposta tem sucesso (status OK)
        if (geocodeResponse?.Status != "OK")
        {
            Console.WriteLine($"Erro na resposta da API: {geocodeResponse?.Status}");
            return null;
        }

        // Verifica se 'Results' contém itens
        if (geocodeResponse?.Results == null || !geocodeResponse.Results.Any())
        {
            Console.WriteLine("Nenhum resultado encontrado.");
            return null;
        }

        // Retorna o primeiro marcador encontrado
        return geocodeResponse.Results
            .Select(x => new Marcador { Latitude = x.Geometry.Location.Lat, Longitude = x.Geometry.Location.Lng })
            .FirstOrDefault();
    }
}