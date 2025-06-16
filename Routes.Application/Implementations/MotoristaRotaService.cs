using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    IPessoasAPI _pessoasAPI,
    IBaseRepository<MotoristaRota> _motoristaRotaRepository,
    IUserContext _userContext) : IMotoristaRotaService
{

    public async Task VincularAsync(MotoristaVincularViewModel request)
    {
        var configuracao = await _motoristaRotaRepository.BuscarUmAsync(x =>
            x.MotoristaId == request.MotoristaId &&
            x.RotaId == request.RotaId);

        if (configuracao is not null && configuracao.Id > 0 && configuracao.Status == StatusEntityEnum.Ativo)
        {
            throw new BusinessRuleException("Motorista já está configurado para essa rota");
        }

        // Se configuracao existir e estiver desativada
        if (configuracao is not null && configuracao.Status == StatusEntityEnum.Deletado)
        {
            configuracao.Status = StatusEntityEnum.Ativo;
            await _motoristaRotaRepository.AtualizarAsync(configuracao);
        }
        else
        {
            var model = new MotoristaRota
            {
                MotoristaId = request.MotoristaId.Value,
                RotaId = request.RotaId
            };

            await _motoristaRotaRepository.AdicionarAsync(model);
        }
    }

    public async Task DesvincularAsync(MotoristaVincularViewModel request)
    {
        var configuracao = await _motoristaRotaRepository
            .BuscarUmAsync(x => x.MotoristaId == request.MotoristaId && x.RotaId == request.RotaId);

        if (configuracao is null)
        {
            throw new BusinessRuleException("Motorista não está configurado para essa rota");
        }

        configuracao.Status = StatusEntityEnum.Deletado;
        await _motoristaRotaRepository.AtualizarAsync(configuracao);
    }

    public async Task<MotoristaViewModel> BuscarMotoristaPorRotaAsync(int rotaId)
    {
        var motoristas = await _motoristaRotaRepository.BuscarAsync(x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo);
        if (motoristas is null || motoristas.Count() == 0)
        {
            return new MotoristaViewModel();
        }

        var motoristaId = motoristas.First().MotoristaId;

        var motoristaResponse = await _pessoasAPI.ObterMotoristaPorIdAsync(motoristaId, completarDadosDoUsuario: true);
        if (motoristaResponse is null || motoristaResponse.Data is null)
        {
            throw new BusinessRuleException(motoristaResponse.Mensagem);
        }

        return motoristaResponse.Data;
    }
}