using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.Utils;
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
            await Task.WhenAll(
                _redisRepository.DeleteAsync(KeyRedis.Veiculos.Veiculo(veiculo.Id)),
                _redisRepository.SetAsync($"veiculo:{veiculo.Id}", veiculo, "veiculos")
            );
        }

        await Task.WhenAll(
            _redisRepository.DeleteAsync(KeyRedis.Veiculos.VeiculosEmpresa(_userContext.Empresa, false)),
            _redisRepository.DeleteAsync(KeyRedis.Veiculos.VeiculosEmpresa(_userContext.Empresa, true))
        );
    }

    public async Task DeletarAsync(int id)
    {
        var model = await _veiculoRepository.ObterPorIdAsync(id);
        model.Status = StatusEntityEnum.Deletado;
        await _veiculoRepository.AtualizarAsync(model);

        await Task.WhenAll(
            _redisRepository.DeleteAsync(KeyRedis.Veiculos.Veiculo(model.Id)),
            _redisRepository.DeleteAsync(KeyRedis.Veiculos.VeiculosEmpresa(_userContext.Empresa, false)),
            _redisRepository.DeleteAsync(KeyRedis.Veiculos.VeiculosEmpresa(_userContext.Empresa, true))
        );
    }

    public async Task<List<VeiculoViewModel>> ObterAsync(bool incluirDeletados = false)
    {
        var status = new List<StatusEntityEnum>
        {
            StatusEntityEnum.Ativo
        };

        if (incluirDeletados)
            status.Add(StatusEntityEnum.Deletado);

        var chave = KeyRedis.Veiculos.VeiculosEmpresa(_userContext.Empresa, incluirDeletados);
        var veiculos = await _redisRepository.GetAsync<IEnumerable<Veiculo>>(chave);

        if (veiculos is null || !veiculos.Any())
        {
            veiculos = await _veiculoRepository.BuscarAsync(x =>
                x.EmpresaId == _userContext.Empresa && status.Contains(x.Status)
            );

            await _redisRepository.SetAsync(chave, veiculos, "veiculos");
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

    private async Task<Veiculo> ObterVeiculoAsync(int veiculoId)
    {
        var veiculo = await _redisRepository.GetAsync<Veiculo>($"veiculo:{veiculoId}");
        if (veiculo is null)
        {
            veiculo = await _veiculoRepository.BuscarUmAsync(x => x.Id == veiculoId && x.EmpresaId == _userContext.Empresa);
            await _redisRepository.SetAsync(KeyRedis.Veiculos.Veiculo(veiculo.Id), veiculo, "veiculos");
        }

        return veiculo;
    }
}
