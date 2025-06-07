using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels;

namespace Routes.Application.Implementations;

public class RelatorioTrajetoService(
    IMapper mapper,
    IRotaHistoricoRepository rotaHistoricoRepository) : IRelatorioTrajetoService
{
    private readonly IMapper _mapper = mapper;
    private readonly IRotaHistoricoRepository _rotaHistoricoRepository = rotaHistoricoRepository;
    public async Task<RotaHistoricoViewModel> RelatorioUltimoTrajetoAsync(int rotaId)
    {
        var rotaHistorico = await _rotaHistoricoRepository.ObterUltimoTrajetoAsync(rotaId);
        return _mapper.Map<RotaHistoricoViewModel>(rotaHistorico);
    }
}