using System.Threading.Tasks;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;

namespace Routes.Service.Implementations;

public class MotoristaRotaService(
        IPessoasAPI pessoasAPI,
        IBaseRepository<MotoristaRota> motoristaRotaRepository,
        IUserContext userContext) : IMotoristaRotaService
{
    private readonly IUserContext _userContext = userContext;
    private readonly IBaseRepository<MotoristaRota> _motoristaRotaRepository = motoristaRotaRepository;
    private readonly IPessoasAPI _pessoasAPI = pessoasAPI;

    public async Task VincularAsync(MotoristaVincularViewModel request)
    {
        // TODO: Caso o usuario que está fzendo a acao nao for o motorista, vai ficar errado o vinculo da rota x usuario
        var motoristaResponse = await _pessoasAPI.ObterMotoristaPorUsuarioIdAsync(_userContext.UserId);
        if (motoristaResponse is null || motoristaResponse.Data is null)
        {
            throw new BusinessRuleException(motoristaResponse.Mensagem);
        }

        var motorista = motoristaResponse.Data;
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
        var motoristaResponse = await _pessoasAPI.ObterMotoristaPorUsuarioIdAsync(_userContext.UserId);
        if (motoristaResponse is null || motoristaResponse.Data is null)
        {
            throw new BusinessRuleException(motoristaResponse.Mensagem);
        }

        var motorista = motoristaResponse.Data;
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