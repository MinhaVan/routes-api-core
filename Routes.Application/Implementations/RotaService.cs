using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using Routes.Domain.Interfaces.APIs;

namespace Routes.Service.Implementations;

public class RotaService : IRotaService
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IBaseRepository<Rota> _rotaRepository;
    private readonly IBaseRepository<AlunoRota> _alunoRotaRepository;
    private readonly IPessoasAPI _pessoasAPI;
    private readonly IRotaHistoricoRepository _rotaHistoricoRepository;
    private readonly IBaseRepository<MotoristaRota> _motoristaRotaRepository;
    public RotaService(
        IMapper mapper,
        IUserContext userContext,
        IPessoasAPI pessoasAPI,
        IRotaHistoricoRepository rotaHistoricoRepository,
        IBaseRepository<MotoristaRota> motoristaRotaRepository,
        IBaseRepository<AlunoRota> alunoRotaRepository,
        IBaseRepository<Rota> rotaRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _pessoasAPI = pessoasAPI;
        _rotaRepository = rotaRepository;
        _alunoRotaRepository = alunoRotaRepository;
        _motoristaRotaRepository = motoristaRotaRepository;
        _rotaHistoricoRepository = rotaHistoricoRepository;
    }

    public async Task<RotaViewModel> AdicionarAsync(RotaAdicionarViewModel rotaAdicionarViewModel)
    {
        var model = _mapper.Map<Rota>(rotaAdicionarViewModel);
        model.EmpresaId = _userContext.Empresa;
        model.Status = StatusEntityEnum.Ativo;
        model.TipoRota = rotaAdicionarViewModel.TipoRota;

        await _rotaRepository.AdicionarAsync(model);
        return _mapper.Map<RotaViewModel>(model); ;
    }

    public async Task AtualizarAsync(RotaAtualizarViewModel rotaAtualizarViewModel)
    {
        var model = await _rotaRepository.BuscarUmAsync(x => x.Id == rotaAtualizarViewModel.Id);

        model.VeiculoId = rotaAtualizarViewModel.VeiculoId.GetValueOrDefault(0) <= 0 ? null : rotaAtualizarViewModel.VeiculoId.Value;
        model.Nome = rotaAtualizarViewModel.Nome;
        model.DiaSemana = rotaAtualizarViewModel.DiaSemana;
        model.Horario = rotaAtualizarViewModel.Horario;
        model.TipoRota = rotaAtualizarViewModel.TipoRota;

        await _rotaRepository.AtualizarAsync(model);
    }

    public async Task DeletarAsync(int id)
    {
        var model = await _rotaRepository.BuscarUmAsync(x => x.Id == id, w => w.AlunoRotas, z => z.MotoristaRotas);

        model.Status = StatusEntityEnum.Deletado;
        model.AlunoRotas.ToList().ForEach(item => item.Status = StatusEntityEnum.Deletado);
        model.MotoristaRotas.ToList().ForEach(item => item.Status = StatusEntityEnum.Deletado);

        await _rotaRepository.AtualizarAsync(model);
    }

    public async Task<RotaViewModel> ObterAsync(int id)
    {
        var response = await _rotaRepository.BuscarUmAsync(x =>
            x.Id == id && x.EmpresaId == _userContext.Empresa && x.Status == StatusEntityEnum.Ativo,
            x => x.AlunoRotas,
            x => x.Historicos,
            x => x.Historicos.OrderByDescending(x => x.DataCriacao));

        return _mapper.Map<RotaViewModel>(response);
    }

    public async Task<RotaDetalheViewModel> ObterDetalheAsync(int id)
    {
        Console.WriteLine($"Obtendo detalhes da rota com ID: {id}");
        var rota = await _rotaRepository.BuscarUmAsync(x =>
            x.Id == id && x.EmpresaId == _userContext.Empresa && x.Status == StatusEntityEnum.Ativo,
            x => x.AlunoRotas.Where(x => x.Status == StatusEntityEnum.Ativo),
            x => x.Historicos.OrderByDescending(x => x.DataCriacao));

        var alunosRotas = await _alunoRotaRepository.BuscarAsync(x => x.RotaId == id);
        if (alunosRotas is null || !alunosRotas.Any())
            return default!;

        var alunosIds = alunosRotas.Select(x => x.AlunoId).ToList();
        Console.WriteLine($"Obtendo alunos com IDs: {string.Join(", ", alunosIds)}");
        var alunos = await _pessoasAPI.ObterAlunoPorIdAsync(alunosIds);

        Console.WriteLine($"Alunos obtidos: {alunos.Data.Count()} alunos encontrados.");

        var response = _mapper.Map<RotaDetalheViewModel>(rota);
        response.Alunos = _mapper.Map<List<AlunoDetalheViewModel>>(alunos.Data);

        var trajetoOnline = await _rotaHistoricoRepository.BuscarUmAsync(x => x.RotaId == id);
        response.EmAndamento = trajetoOnline is not null && trajetoOnline.Id > 0 && trajetoOnline.DataFim.HasValue == false;

        return response;
    }

    public async Task<List<RotaViewModel>> ObterAsync()
    {
        var Alunos = await _pessoasAPI.ObterAlunoPorResponsavelIdAsync();
        var AlunosId = Alunos.Data.Select(x => x.Id).ToList();

        var response = await _rotaRepository.BuscarAsync(
            x => x.EmpresaId == _userContext.Empresa
                 && x.Status == StatusEntityEnum.Ativo
                 && x.AlunoRotas.Any(f => AlunosId.Contains(f.AlunoId)),
            x => x.AlunoRotas,
            x => x.Historicos.OrderByDescending(x => x.DataCriacao)
        );

        if (!response.Any(x => x.Historicos.Any()))
            return default!;

        return _mapper.Map<List<RotaViewModel>>(response);
    }

    public async Task<List<RotaViewModel>> ObterRotasOnlineAsync()
    {
        var alunos = await _pessoasAPI.ObterAlunoPorResponsavelIdAsync();
        var alunosId = alunos.Data.Select(x => x.Id).ToList();
        if (!alunosId.Any())
            return default!;

        var response = await _rotaRepository.BuscarAsync(
            x => x.EmpresaId == _userContext.Empresa
                 && x.Status == StatusEntityEnum.Ativo
                 && x.AlunoRotas.Any(f => alunosId.Contains(f.AlunoId))
                 && x.Historicos.Any(z => z.DataFim == null && z.EmAndamento),
            x => x.AlunoRotas,
            x => x.Historicos.OrderByDescending(x => x.DataCriacao)
        );

        if (!response.Any(x => x.Historicos.Any()))
            return default!;

        return _mapper.Map<List<RotaViewModel>>(response);
    }

    public async Task<List<RotaViewModel>> ObterPorAlunoIdAsync(int id)
    {
        var alunos = await _rotaRepository.BuscarAsync(x => x.AlunoRotas.Any(y => y.AlunoId == id), x => x.AlunoRotas);
        var rotasId = alunos.SelectMany(x => x.AlunoRotas).Select(x => x.RotaId);

        var rotas = await _rotaRepository.BuscarAsync(
            x => x.Status == StatusEntityEnum.Ativo && rotasId.Contains(x.Id),
            x => x.Historicos);

        return _mapper.Map<List<RotaViewModel>>(rotas);
    }

    public async Task<List<RotaViewModel>> ObterRotaDoMotoristaAsync(int usuarioId, bool filtrarApenasHoje = true)
    {
        var saoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        var nowUtc = DateTime.UtcNow;
        var hoje = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, saoPauloTimeZone);

        var diaDaSemanaAtual = (DiaSemanaEnum)(hoje.DayOfWeek + 1); // Para alinhar com o enum

        var motorista = await _pessoasAPI.ObterMotoristaPorIdAsync(usuarioId);
        var motoristaRotas = await _motoristaRotaRepository.BuscarAsync(x => x.MotoristaId == motorista.Data.Id && x.Status == StatusEntityEnum.Ativo);

        if (motoristaRotas is null || !motoristaRotas.Any())
            return Enumerable.Empty<RotaViewModel>().ToList();

        var rotasId = motoristaRotas.Select(x => x.RotaId);
        var rotasDoDiaParaMotorista = await _rotaRepository.BuscarAsync(x =>
            rotasId.Contains(x.Id) && // Buscando rotas que o MOTORISTA esta cadastrado
            x.Status == StatusEntityEnum.Ativo && // Rota precisa estar ativa
                                                  // Logica para buscar as ROTAS apenas do dia de HOJE
            (filtrarApenasHoje ? (x.DiaSemana == DiaSemanaEnum.Todos) ||
            (x.DiaSemana == diaDaSemanaAtual) ||
            (diaDaSemanaAtual >= DiaSemanaEnum.Segunda &&
            diaDaSemanaAtual <= DiaSemanaEnum.Sexta && x.DiaSemana == DiaSemanaEnum.DiasUteis) : true)
        );

        // Mapear as rotas filtradas para RotaViewModel
        var rotasViewModel = _mapper.Map<List<RotaViewModel>>(rotasDoDiaParaMotorista);

        return rotasViewModel;
    }
}