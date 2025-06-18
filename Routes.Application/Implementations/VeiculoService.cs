using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;

namespace Routes.Service.Implementations;

public class VeiculoService(
    IMapper _mapper,
    IUserContext _userContext,
    IPessoasAPI _pessoasAPI,
    IRedisRepository _redisRepository,
    IBaseRepository<Veiculo> _veiculoRepository,
    IBaseRepository<MotoristaRota> _motoristaRotaRepository) : IVeiculoService
{

    public async Task AdicionarAsync(List<VeiculoAdicionarViewModel> veiculosViewModels)
    {
        var veiculos = _mapper.Map<List<Veiculo>>(veiculosViewModels);
        await Parallel.ForEachAsync(veiculos, async (item, token) =>
        {
            item.EmpresaId = _userContext.Empresa;
            await Task.CompletedTask;
        });
        await _veiculoRepository.AdicionarAsync(veiculos);
        foreach (var veiculo in veiculos)
        {
            await _redisRepository.DeleteAsync($"veiculo:{veiculo.Id}");
            await _redisRepository.DeleteAsync($"veiculos:empresa:{_userContext.Empresa}");
            await _redisRepository.SetAsync($"veiculo:{veiculo.Id}", veiculo, "veiculos");
        }
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
        foreach (var veiculo in veiculos)
        {
            await _redisRepository.DeleteAsync($"veiculo:{veiculo.Id}");
            await _redisRepository.DeleteAsync($"veiculos:empresa:{_userContext.Empresa}");
            await _redisRepository.SetAsync($"veiculo:{veiculo.Id}", veiculo, "veiculos");
        }
    }

    public async Task DeletarAsync(int id)
    {
        var model = await _veiculoRepository.ObterPorIdAsync(id);
        model.Status = StatusEntityEnum.Deletado;
        await _veiculoRepository.AtualizarAsync(model);
        await _redisRepository.DeleteAsync($"veiculo:{model.Id}");
        await _redisRepository.DeleteAsync($"veiculos:empresa:{_userContext.Empresa}");
    }

    public async Task<List<VeiculoViewModel>> ObterAsync()
    {
        var veiculos = await _redisRepository.GetAsync<IEnumerable<Veiculo>>($"veiculos:empresa:{_userContext.Empresa}");
        if (veiculos is null || veiculos.Count() == 0)
        {
            veiculos = await _veiculoRepository.BuscarAsync(x => x.EmpresaId == _userContext.Empresa);
            await _redisRepository.SetAsync($"veiculos:empresa:{_userContext.Empresa}", veiculos, "veiculos");
        }

        var dtos = _mapper.Map<List<VeiculoViewModel>>(veiculos);
        return dtos;
    }

    public async Task<VeiculoViewModel> ObterAsync(int veiculoId, int rotaId, bool completarDadosDoUsuario = false)
    {
        var veiculo = await ObterVeiculoAsync(veiculoId);
        var motoristaRota = await _redisRepository.GetAsync<MotoristaRota>($"motorista_rota:{rotaId}:{veiculoId}");
        if (motoristaRota is null)
        {
            motoristaRota = await _motoristaRotaRepository.BuscarUmAsync(x => x.RotaId == rotaId);
            await _redisRepository.SetAsync($"motorista_rota:{rotaId}:{veiculoId}", motoristaRota, "motoristas_rotas");
        }

        if (motoristaRota.Status != StatusEntityEnum.Ativo)
            return default;

        var motoristaResponse = await _pessoasAPI.ObterMotoristaPorIdAsync(motoristaRota.MotoristaId, completarDadosDoUsuario);
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

    public async Task<IEnumerable<VeiculoViewModel>> ObterVeiculoAsync(IEnumerable<int> veiculoIds)
    {
        var veiculos = await _veiculoRepository.BuscarAsync(x => veiculoIds.Contains(x.Id) && x.EmpresaId == _userContext.Empresa);
        return _mapper.Map<IEnumerable<VeiculoViewModel>>(veiculos);
    }

    private async Task<Veiculo> ObterVeiculoAsync(int veiculoId)
    {
        var veiculo = await _redisRepository.GetAsync<Veiculo>($"veiculo:{veiculoId}");
        if (veiculo is null)
        {
            veiculo = await _veiculoRepository.BuscarUmAsync(x => x.Id == veiculoId && x.EmpresaId == _userContext.Empresa);
            await _redisRepository.SetAsync($"veiculo:{veiculo.Id}", veiculo, "veiculos");
        }

        return veiculo;
    }
}
