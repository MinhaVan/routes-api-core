using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;

namespace Routes.Service.Implementations;

public class EnderecoService(
    IMapper _mapper,
    IRedisRepository _redisRepository,
    IBaseRepository<Endereco> _enderecoRepository,
    IGoogleDirectionsService _googleDirectionsService,
    IUserContext _userContext) : IEnderecoService
{
    public async Task AdicionarAsync(EnderecoAdicionarViewModel enderecoAdicionarViewModel)
    {
        var model = _mapper.Map<Endereco>(enderecoAdicionarViewModel);
        model.UsuarioId = enderecoAdicionarViewModel.UsuarioId.HasValue ? enderecoAdicionarViewModel.UsuarioId : _userContext.UserId;
        model.Status = Domain.Enums.StatusEntityEnum.Ativo;

        var marcador = await _googleDirectionsService.ObterMarcadorAsync(model.ObterEnderecoCompleto());
        model.Latitude = marcador.Latitude;
        model.Longitude = marcador.Longitude;

        await _enderecoRepository.AdicionarAsync(model);

        await _redisRepository.DeleteAsync($"endereco:{model.Id}");
        await _redisRepository.DeleteAsync($"enderecos:usuario:{model.UsuarioId}");
        await _redisRepository.SetAsync($"endereco:{model.Id}", model, "enderecos");
    }

    public async Task AtualizarAsync(EnderecoAtualizarViewModel enderecoAtualizarViewModel)
    {
        var model = await _enderecoRepository.ObterPorIdAsync(enderecoAtualizarViewModel.Id);

        if ((model.Rua?.Trim() ?? string.Empty) != (enderecoAtualizarViewModel.Rua?.Trim() ?? string.Empty) ||
            (model.Numero ?? string.Empty) != (enderecoAtualizarViewModel.Numero ?? string.Empty) ||
            (model.Bairro?.Trim() ?? string.Empty) != (enderecoAtualizarViewModel.Bairro?.Trim() ?? string.Empty) ||
            (model.Cidade?.Trim() ?? string.Empty) != (enderecoAtualizarViewModel.Cidade?.Trim() ?? string.Empty) ||
            (model.Estado?.Trim() ?? string.Empty) != (enderecoAtualizarViewModel.Estado?.Trim() ?? string.Empty) ||
            (model.CEP ?? string.Empty) != (enderecoAtualizarViewModel.CEP ?? string.Empty))
        {
            var enderecoRequest = $"{model.Rua ?? string.Empty} {model.Numero ?? string.Empty}, {model.Bairro ?? string.Empty}, {model.Cidade ?? string.Empty}, {model.Estado ?? string.Empty}, {model.CEP ?? string.Empty}, Brazil";
            var marcador = await _googleDirectionsService.ObterMarcadorAsync(enderecoRequest);
            model.Latitude = marcador.Latitude;
            model.Longitude = marcador.Longitude;
        }

        model.Rua = enderecoAtualizarViewModel.Rua.Trim();
        model.Numero = enderecoAtualizarViewModel.Numero;
        model.Complemento = enderecoAtualizarViewModel.Complemento;
        model.Bairro = enderecoAtualizarViewModel.Bairro;
        model.Cidade = enderecoAtualizarViewModel.Cidade;
        model.Estado = enderecoAtualizarViewModel.Estado;
        model.CEP = enderecoAtualizarViewModel.CEP;
        model.Pais = enderecoAtualizarViewModel.Pais;
        model.TipoEndereco = enderecoAtualizarViewModel.TipoEndereco.Value;

        await _enderecoRepository.AtualizarAsync(model);

        await _redisRepository.DeleteAsync($"enderecos:usuario:{model.UsuarioId}");
        await _redisRepository.DeleteAsync($"endereco:{model.Id}");
        await _redisRepository.SetAsync($"endereco:{model.Id}", model, "enderecos");
    }

    public async Task DeletarAsync(int id)
    {
        await _enderecoRepository.DeletarAsync(id);
        await _redisRepository.DeleteAsync($"endereco:{id}");
        await _redisRepository.DeleteAsync($"enderecos:usuario:{_userContext.UserId}");
    }

    public async Task<EnderecoViewModel> Obter(int id)
    {
        var model = await _redisRepository.GetAsync<Endereco>($"endereco:{id}");
        if (model is null)
        {
            model = await _enderecoRepository.ObterPorIdAsync(id);
            await _redisRepository.SetAsync($"endereco:{model.Id}", model, "enderecos");
        }

        return _mapper.Map<EnderecoViewModel>(model);
    }

    public async Task<List<EnderecoViewModel>> Obter(List<int> ids)
    {
        var keys = ids.Select(id => $"endereco:{id}").ToList();
        var cached = await _redisRepository.GetListAsync<Endereco>(keys);

        var cachedIds = cached.Select(c => c.Id).ToHashSet();
        var missingIds = ids.Except(cachedIds).ToList();

        if (missingIds.Any())
        {
            var fromDb = await _enderecoRepository.BuscarAsync(x => missingIds.Contains(x.Id));
            foreach (var item in fromDb)
                await _redisRepository.SetAsync($"endereco:{item.Id}", item, "enderecos");

            cached.AddRange(fromDb);
        }

        return _mapper.Map<List<EnderecoViewModel>>(cached);
    }

    public async Task<List<EnderecoViewModel>> Obter()
    {
        var enderecos = await _redisRepository.GetAsync<IEnumerable<Endereco>>($"enderecos:usuario:{_userContext.UserId}");
        if (enderecos is null || !enderecos.Any())
        {
            enderecos = await _enderecoRepository.BuscarAsync(x => x.Status == StatusEntityEnum.Ativo && x.UsuarioId == _userContext.UserId);
            await _redisRepository.SetAsync($"enderecos:usuario:{_userContext.UserId}", enderecos, "enderecos");
        }

        return _mapper.Map<List<EnderecoViewModel>>(enderecos);
    }
}