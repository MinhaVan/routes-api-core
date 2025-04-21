using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Configuration;

namespace Routes.Service.Implementations;

public class EnderecoService : IEnderecoService
{
    private readonly IMapper _mapper;
    private readonly SecretManager _secretManager;
    private readonly IUserContext _userContext;
    private readonly IBaseRepository<Aluno> _alunoRepository;
    private readonly IBaseRepository<Endereco> _enderecoRepository;
    private readonly IBaseRepository<Usuario> _usuarioRepository;
    private readonly HttpClient _googleMapsCliente;
    public EnderecoService(
        IMapper mapper,
        IBaseRepository<Aluno> alunoRepository,
        IBaseRepository<Usuario> usuarioRepository,
        IBaseRepository<Endereco> enderecoRepository,
        SecretManager secretManager,
        IHttpClientFactory httpClientFactory,
        IUserContext userContext)
    {
        _secretManager = secretManager;
        _googleMapsCliente = httpClientFactory.CreateClient("api-googlemaps"); ;
        _alunoRepository = alunoRepository;
        _mapper = mapper;
        _userContext = userContext;
        _usuarioRepository = usuarioRepository;
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
        // var usuariosComEnderecoVinculado = await _usuarioRepository
        //     .BuscarAsync(x => x.EnderecoPrincipalId == id && x.Status != Domain.Enums.StatusEntityEnum.Deletado);

        // if (usuariosComEnderecoVinculado is not null && usuariosComEnderecoVinculado.Any())
        //     throw new Exception("Troque seu endereço principal antes de deletar!");

        // var AlunosComEnderecoVinculado = await _alunoRepository
        //     .BuscarAsync(x => (x.EnderecoPartidaId == id || x.EnderecoRetornoId == id) && x.Status != Domain.Enums.StatusEntityEnum.Deletado);

        // if (AlunosComEnderecoVinculado is not null && AlunosComEnderecoVinculado.Any())
        //     throw new Exception("Existe algum aluno com esse endereço!");

        var model = await _enderecoRepository.ObterPorIdAsync(id);
        model.Status = Domain.Enums.StatusEntityEnum.Deletado;
        await _enderecoRepository.AtualizarAsync(model);
    }

    public async Task<EnderecoViewModel> Obter(int id)
        => _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorIdAsync(id));

    public async Task<List<EnderecoViewModel>> Obter()
    {
        var usuario = await _usuarioRepository.BuscarAsync(x => x.Id == _userContext.UserId, y => y.Enderecos);
        var enderecos = usuario.SelectMany(x => x.Enderecos).Where(x => x.Status == Domain.Enums.StatusEntityEnum.Ativo).ToList();
        return _mapper.Map<List<EnderecoViewModel>>(enderecos);
    }

    public async Task<Marcador> ObterMarcadorAsync(string endereco)
    {
        var requestUri = $"{_secretManager.Google.BaseUrl}/geocode/json?address={Uri.EscapeDataString(endereco)}&key={_secretManager.Google.Key}";
        var geocodeResponse = await _googleMapsCliente.GetFromJsonAsync<GoogleGeocodeResponse>(requestUri);

        // Exibe o conteúdo da resposta para depuração
        // Console.WriteLine("Resposta da API:");
        // Console.WriteLine(response);

        // var geocodeResponse = JsonSerializer.Deserialize<GoogleGeocodeResponse>(response);

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