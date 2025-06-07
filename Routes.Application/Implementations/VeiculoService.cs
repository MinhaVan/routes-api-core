using System.Collections.Generic;
using System.Linq;
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

public class VeiculoService : IVeiculoService
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IPessoasAPI _pessoasAPI;
    private readonly IBaseRepository<Veiculo> _veiculoRepository;
    private readonly IBaseRepository<MotoristaRota> _motoristaRotaRepository;
    public VeiculoService(
        IMapper mapper,
        IUserContext userContext,
        IPessoasAPI pessoasAPI,
        IBaseRepository<Veiculo> veiculoRepository,
        IBaseRepository<MotoristaRota> motoristaRotaRepository)
    {
        _motoristaRotaRepository = motoristaRotaRepository;
        _userContext = userContext;
        _mapper = mapper;
        _pessoasAPI = pessoasAPI;
        _veiculoRepository = veiculoRepository;
    }

    public async Task AdicionarAsync(List<VeiculoAdicionarViewModel> veiculosViewModels)
    {
        var veiculos = _mapper.Map<List<Veiculo>>(veiculosViewModels);
        await Parallel.ForEachAsync(veiculos, async (item, token) =>
        {
            item.EmpresaId = _userContext.Empresa;
            await Task.CompletedTask;
        });
        await _veiculoRepository.AdicionarAsync(veiculos);
    }

    public async Task AtualizarAsync(List<VeiculoAtualizarViewModel> veiculosViewModels)
    {
        var veiculos = _mapper.Map<List<Veiculo>>(veiculosViewModels);
        await Parallel.ForEachAsync(veiculos, async (item, token) =>
        {
            item.EmpresaId = _userContext.Empresa;
            await Task.CompletedTask;
        });
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

    public async Task<VeiculoViewModel> ObterAsync(int veiculoId, int rotaId, bool completarDadosDoUsuario = false)
    {
        var veiculo = await _veiculoRepository.BuscarUmAsync(x => x.Id == veiculoId && x.EmpresaId == _userContext.Empresa);
        var configuracao = await _motoristaRotaRepository.BuscarUmAsync(x => x.RotaId == rotaId && x.Status == StatusEntityEnum.Ativo);
        var motoristaResponse = await _pessoasAPI.ObterMotoristaPorIdAsync(configuracao.MotoristaId, completarDadosDoUsuario);
        if (motoristaResponse == null || !motoristaResponse.Sucesso || motoristaResponse.Data == null)
        {
            throw new BusinessRuleException(motoristaResponse.Mensagem ?? "Motorista não encontrado ou não está ativo.");
        }

        var motorista = motoristaResponse.Data;
        var dto = _mapper.Map<VeiculoViewModel>(veiculo);
        dto.Motorista = _mapper.Map<MotoristaViewModel>(motorista);
        dto.Motorista.CNH = motorista.CNH;
        dto.Motorista.Vencimento = motorista.Vencimento;
        dto.Motorista.TipoCNH = motorista.TipoCNH;
        dto.Motorista.Foto = motorista.Foto;

        return dto;
    }
}
