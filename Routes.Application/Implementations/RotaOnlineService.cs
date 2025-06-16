using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;

namespace Routes.Application.Implementations;

public class RotaOnlineService(
    IPessoasAPI _pessoasAPI,
    IUserContext _userContext,
    IBaseRepository<MotoristaRota> _motoristaRotaRepository,
    IBaseRepository<RotaHistorico> _rotaHistoricoRepository,
    IMapper _mapper
) : IRotaOnlineService
{
    public async Task<RotaViewModel> RotaOnlineParaMotoristaAsync()
    {
        var obterMotoristaPorIdResponse = await _pessoasAPI.ObterMotoristaPorUsuarioIdAsync(_userContext.UserId);
        if (!obterMotoristaPorIdResponse.Sucesso)
            throw new BusinessRuleException(obterMotoristaPorIdResponse.Mensagem);

        var motorista = obterMotoristaPorIdResponse.Data;
        var motoristaRotas = await _motoristaRotaRepository.BuscarAsync(x => x.MotoristaId == motorista.Id, z => z.Rota);
        var rotasId = motoristaRotas.Select(x => x.RotaId);

        var trajetoOnline = await _rotaHistoricoRepository.BuscarUmAsync(x =>
            rotasId.Contains(x.RotaId) &&
            x.EmAndamento == true &&
            x.DataFim == null,
            x => x.Rota
        );

        if (trajetoOnline is not null && trajetoOnline.Id > 0)
        {
            var viewModel = _mapper.Map<RotaViewModel>(trajetoOnline.Rota);
            return viewModel;
        }

        return null;
    }
}