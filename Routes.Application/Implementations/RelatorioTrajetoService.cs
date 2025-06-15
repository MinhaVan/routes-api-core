using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels;

namespace Routes.Application.Implementations;

public class RelatorioTrajetoService(
    IMapper _mapper,
    IRotaHistoricoRepository _rotaHistoricoRepository) : IRelatorioTrajetoService
{
    public async Task<RotaHistoricoViewModel> RelatorioUltimoTrajetoAsync(int rotaId)
    {
        var rotaHistorico = await _rotaHistoricoRepository.ObterUltimoTrajetoAsync(rotaId);
        return _mapper.Map<RotaHistoricoViewModel>(rotaHistorico);
    }
}