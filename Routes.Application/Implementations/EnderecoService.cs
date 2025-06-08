using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;

namespace Routes.Service.Implementations;

public class EnderecoService(
    IMapper mapper,
    IBaseRepository<Endereco> enderecoRepository,
    IGoogleDirectionsService googleDirectionsService,
    IUserContext userContext) : IEnderecoService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserContext _userContext = userContext;
    private readonly IBaseRepository<Endereco> _enderecoRepository = enderecoRepository;
    private readonly IGoogleDirectionsService _googleDirectionsService = googleDirectionsService;
    public async Task AdicionarAsync(EnderecoAdicionarViewModel enderecoAdicionarViewModel)
    {
        var model = _mapper.Map<Endereco>(enderecoAdicionarViewModel);
        model.UsuarioId = enderecoAdicionarViewModel.UsuarioId.HasValue ? enderecoAdicionarViewModel.UsuarioId : _userContext.UserId;
        model.Status = Domain.Enums.StatusEntityEnum.Ativo;

        var marcador = await _googleDirectionsService.ObterMarcadorAsync(model.ObterEnderecoCompleto());
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
            var marcador = await _googleDirectionsService.ObterMarcadorAsync(enderecoRequest);
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

    public async Task<List<EnderecoViewModel>> Obter(List<int> ids)
        => _mapper.Map<List<EnderecoViewModel>>(await _enderecoRepository.BuscarAsync(x => ids.Contains(x.Id)));

    public async Task<List<EnderecoViewModel>> Obter()
    {
        var enderecos = await _enderecoRepository.BuscarAsync(x => x.Status == Domain.Enums.StatusEntityEnum.Ativo && x.UsuarioId == _userContext.UserId);
        return _mapper.Map<List<EnderecoViewModel>>(enderecos);
    }
}