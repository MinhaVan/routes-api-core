using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;

namespace Routes.Service.Implementations;

public class MotoristaService : IMotoristaService
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IAuthApi _authApi;
    private readonly IBaseRepository<MotoristaRota> _motoristaRotaRepository;
    private readonly IBaseRepository<Motorista> _motoristaRepository;
    public MotoristaService(
        IMapper mapper,
        IAuthApi authApi,
        IBaseRepository<Usuario> usuarioRepository,
        IBaseRepository<MotoristaRota> motoristaRotaRepository,
        IBaseRepository<Motorista> motoristaRepository,
        IUserContext userContext)
    {
        _motoristaRepository = motoristaRepository;
        _motoristaRotaRepository = motoristaRotaRepository;
        _authApi = authApi;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<MotoristaViewModel> AdicionarAsync(MotoristaNovoViewModel usuarioNovoViewModel)
    {
        usuarioNovoViewModel.Perfil = PerfilEnum.Motorista;
        var response = await _authApi.RegistrarAsync(_mapper.Map<UsuarioNovoViewModel>(usuarioNovoViewModel));

        var model = _mapper.Map<Motorista>(usuarioNovoViewModel);
        model.UsuarioId = response.Data.Id;
        await _motoristaRepository.AdicionarAsync(model);

        return _mapper.Map<MotoristaViewModel>(response);
    }

    public async Task AtualizarAsync(MotoristaAtualizarViewModel usuarioAtualizarViewModel)
    {
        await _authApi.AtualizarAsync(usuarioAtualizarViewModel);

        var motorista = await _motoristaRepository.BuscarUmAsync(x => x.UsuarioId == usuarioAtualizarViewModel.Id);
        motorista.CNH = usuarioAtualizarViewModel.CNH;
        motorista.Vencimento = usuarioAtualizarViewModel.Vencimento;
        motorista.TipoCNH = usuarioAtualizarViewModel.TipoCNH;
        motorista.Foto = usuarioAtualizarViewModel.Foto;
        await _motoristaRepository.AtualizarAsync(motorista);
    }

    public async Task VincularAsync(MotoristaVincularViewModel request)
    {
        // TODO: Caso o usuario que está fzendo a acao nao for o motorista, vai ficar errado o vinculo da rota x usuario
        var motorista = await _motoristaRepository.BuscarUmAsync(x => x.UsuarioId == _userContext.UserId);
        var configuracao = await _motoristaRotaRepository.BuscarUmAsync(x =>
            x.MotoristaId == motorista.Id &&
            x.RotaId == request.RotaId);

        if (configuracao is not null && configuracao.Id > 0 && configuracao.Status == StatusEntityEnum.Ativo)
        {
            throw new BusinessRuleException("Motorista já está configurado para essa rota");
        }

        // Se configuracao existir e estiver desativada
        if (configuracao is not null && configuracao.Status == StatusEntityEnum.Ativo)
        {
            configuracao.Status = StatusEntityEnum.Ativo;
            await _motoristaRotaRepository.AtualizarAsync(configuracao);
        }
        else
        {
            var model = new MotoristaRota
            {
                MotoristaId = motorista.Id,
                RotaId = request.RotaId
            };

            await _motoristaRotaRepository.AdicionarAsync(model);
        }
    }

    public async Task DesvincularAsync(MotoristaVincularViewModel request)
    {
        // TODO: Caso o usuario que está fzendo a acao nao for o motorista, vai ficar errado o vinculo da rota x usuario
        var motorista = await _motoristaRepository.BuscarUmAsync(x => x.UsuarioId == _userContext.UserId);
        var configuracao = await _motoristaRotaRepository
            .BuscarUmAsync(x => x.MotoristaId == motorista.Id && x.RotaId == request.RotaId);

        if (configuracao is null || configuracao.Id <= 0)
        {
            throw new BusinessRuleException("Motorista não está configurado para essa rota");
        }

        configuracao.Status = StatusEntityEnum.Deletado;
        await _motoristaRotaRepository.AtualizarAsync(configuracao);
    }
}