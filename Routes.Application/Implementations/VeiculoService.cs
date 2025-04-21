using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;

namespace Routes.Service.Implementations;

public class VeiculoService : IVeiculoService
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IBaseRepository<Veiculo> _veiculoRepository;
    private readonly IBaseRepository<MotoristaRota> _motoristaRotaRepository;
    public VeiculoService(IMapper mapper, IUserContext userContext, IBaseRepository<Veiculo> veiculoRepository, IBaseRepository<MotoristaRota> motoristaRotaRepository)
    {
        _motoristaRotaRepository = motoristaRotaRepository;
        _userContext = userContext;
        _mapper = mapper;
        _veiculoRepository = veiculoRepository;
    }

    public async Task AdicionarAsync(List<VeiculoAdicionarViewModel> veiculosViewModels)
    {
        var veiculos = _mapper.Map<List<Veiculo>>(veiculosViewModels);
        foreach (var item in veiculos)
        {
            item.EmpresaId = _userContext.Empresa;
        }

        await _veiculoRepository.AdicionarAsync(veiculos);
    }

    public async Task AtualizarAsync(List<VeiculoAtualizarViewModel> veiculosViewModels)
    {
        var veiculos = _mapper.Map<List<Veiculo>>(veiculosViewModels);
        foreach (var item in veiculos)
        {
            item.EmpresaId = _userContext.Empresa;
        }

        await _veiculoRepository.AtualizarAsync(veiculos);
    }

    public async Task DeletarAsync(int id)
    {
        var model = await _veiculoRepository.ObterPorIdAsync(id);
        model.Status = Domain.Enums.StatusEntityEnum.Deletado;
        await _veiculoRepository.AtualizarAsync(model);
    }

    public async Task<List<VeiculoViewModel>> ObterAsync()
    {
        var veiculos = await _veiculoRepository.BuscarAsync(x => x.EmpresaId == _userContext.Empresa);
        return _mapper.Map<List<VeiculoViewModel>>(veiculos);
    }

    public async Task<VeiculoViewModel> ObterAsync(int motoristaId, int rotaId)
    {
        var veiculo = await _veiculoRepository.BuscarUmAsync(x => x.Id == motoristaId && x.EmpresaId == _userContext.Empresa);

        var configuracao = await _motoristaRotaRepository.BuscarUmAsync(x =>
            x.MotoristaId == motoristaId && x.RotaId == rotaId,
            x => x.Motorista, x => x.Motorista.Usuario);

        var dto = _mapper.Map<VeiculoViewModel>(veiculo);
        dto.Motorista = _mapper.Map<MotoristaViewModel>(configuracao.Motorista.Usuario);
        dto.Motorista.CNH = configuracao.Motorista.CNH;
        dto.Motorista.Vencimento = configuracao.Motorista.Vencimento;
        dto.Motorista.TipoCNH = configuracao.Motorista.TipoCNH;
        dto.Motorista.Foto = configuracao.Motorista.Foto;

        return dto;
    }
}
