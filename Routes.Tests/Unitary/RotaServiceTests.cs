using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Implementations;
using Xunit;

namespace Routes.Tests.Unitary;

public class RotaServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IBaseRepository<Rota>> _rotaRepoMock = new();
    private readonly Mock<IBaseRepository<AlunoRota>> _alunoRotaRepoMock = new();
    private readonly Mock<IPessoasAPI> _pessoasApiMock = new();
    private readonly Mock<IRotaHistoricoRepository> _rotaHistoricoRepoMock = new();
    private readonly Mock<IBaseRepository<MotoristaRota>> _motoristaRotaRepoMock = new();
    private readonly Mock<IRedisRepository> _redisRepository = new();

    private RotaService CreateService()
    {
        return new RotaService(
            _mapperMock.Object,
            _userContextMock.Object,
            _pessoasApiMock.Object,
            _redisRepository.Object,
            _rotaHistoricoRepoMock.Object,
            _motoristaRotaRepoMock.Object,
            _alunoRotaRepoMock.Object,
            _rotaRepoMock.Object
        );
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarRota()
    {
        var viewModel = new RotaAdicionarViewModel { TipoRota = TipoRotaEnum.Ida };
        var rota = new Rota();
        var rotaViewModel = new RotaViewModel();

        _mapperMock.Setup(m => m.Map<Rota>(viewModel)).Returns(rota);
        _userContextMock.SetupGet(u => u.Empresa).Returns(1);
        _rotaRepoMock.Setup(r => r.AdicionarAsync(rota)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<RotaViewModel>(rota)).Returns(rotaViewModel);

        var service = CreateService();
        var result = await service.AdicionarAsync(viewModel);

        Assert.Equal(rotaViewModel, result);
        Assert.Equal(1, rota.EmpresaId);
        Assert.Equal(StatusEntityEnum.Ativo, rota.Status);
        Assert.Equal(viewModel.TipoRota, rota.TipoRota);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarRota()
    {
        var viewModel = new RotaAtualizarViewModel { Id = 1, VeiculoId = 2, Nome = "Teste", DiaSemana = DiaSemanaEnum.Todos, Horario = new TimeOnly(8, 0), TipoRota = TipoRotaEnum.Ida };
        var rota = new Rota();

        _rotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rota);
        _rotaRepoMock.Setup(r => r.AtualizarAsync(rota)).Returns(Task.CompletedTask);

        var service = CreateService();
        await service.AtualizarAsync(viewModel);

        Assert.Equal(viewModel.VeiculoId, rota.VeiculoId);
        Assert.Equal(viewModel.Nome, rota.Nome);
        Assert.Equal(viewModel.DiaSemana, rota.DiaSemana);
        Assert.Equal(viewModel.Horario, rota.Horario);
        Assert.Equal(viewModel.TipoRota, rota.TipoRota);
    }

    [Fact]
    public async Task DeletarAsync_DeveDeletarRotaEAlunosEMotoristas()
    {
        var rota = new Rota
        {
            AlunoRotas = new List<AlunoRota> { new AlunoRota() },
            MotoristaRotas = new List<MotoristaRota> { new MotoristaRota() }
        };

        _rotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rota);
        _rotaRepoMock.Setup(r => r.AtualizarAsync(rota)).Returns(Task.CompletedTask);

        var service = CreateService();
        await service.DeletarAsync(1);

        Assert.Equal(StatusEntityEnum.Deletado, rota.Status);
        Assert.All(rota.AlunoRotas, a => Assert.Equal(StatusEntityEnum.Deletado, a.Status));
        Assert.All(rota.MotoristaRotas, m => Assert.Equal(StatusEntityEnum.Deletado, m.Status));
    }

    [Fact]
    public async Task ObterAsync_Id_DeveRetornarRotaViewModel()
    {
        var rota = new Rota();
        var rotaViewModel = new RotaViewModel();

        _userContextMock.SetupGet(u => u.Empresa).Returns(1);
        _rotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rota);
        _mapperMock.Setup(m => m.Map<RotaViewModel>(rota)).Returns(rotaViewModel);

        var service = CreateService();
        var result = await service.ObterAsync(1);

        Assert.Equal(rotaViewModel, result);
    }

    [Fact]
    public async Task ObterDetalheAsync_DeveRetornarDetalhe()
    {
        var rota = new Rota { Id = 1 };
        var alunosRotas = new List<AlunoRota> { new AlunoRota { AlunoId = 2 } };
        var alunos = new BaseResponse<List<AlunoViewModel>> { Data = new List<AlunoViewModel> { new AlunoViewModel { Id = 2 } } };
        var detalheViewModel = new RotaDetalheViewModel();
        var alunoDetalheViewModels = new List<AlunoDetalheViewModel> { new AlunoDetalheViewModel() };

        _userContextMock.SetupGet(u => u.Empresa).Returns(1);
        _rotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rota);
        _alunoRotaRepoMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AlunoRota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<AlunoRota, object>>[]>())).ReturnsAsync(alunosRotas);
        _pessoasApiMock.Setup(p => p.ObterAlunoPorIdAsync(It.IsAny<List<int>>())).ReturnsAsync(alunos);
        _mapperMock.Setup(m => m.Map<RotaDetalheViewModel>(rota)).Returns(detalheViewModel);
        _mapperMock.Setup(m => m.Map<List<AlunoDetalheViewModel>>(alunos.Data)).Returns(alunoDetalheViewModels);
        _rotaHistoricoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RotaHistorico, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<RotaHistorico, object>>[]>())).ReturnsAsync(new RotaHistorico { Id = 1, DataFim = null });

        var service = CreateService();
        var result = await service.ObterDetalheAsync(1);

        Assert.Equal(detalheViewModel, result);
        Assert.Equal(alunoDetalheViewModels, result.Alunos);
        Assert.True(result.EmAndamento);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarTodasRotas()
    {
        var rotas = new List<Rota> { new Rota() };
        var rotasViewModel = new List<RotaViewModel> { new RotaViewModel() };

        _userContextMock.SetupGet(u => u.Empresa).Returns(1);
        _rotaRepoMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rotas);
        _mapperMock.Setup(m => m.Map<List<RotaViewModel>>(rotas)).Returns(rotasViewModel);

        var service = CreateService();
        var result = await service.ObterTodosAsync();

        Assert.Equal(rotasViewModel, result);
    }

    [Fact]
    public async Task ObterAsync_DeveRetornarRotasPorResponsavel()
    {
        var alunos = new BaseResponse<List<AlunoViewModel>> { Data = new List<AlunoViewModel> { new AlunoViewModel { Id = 2 } } };
        var rotas = new List<Rota> { new Rota { Historicos = new List<RotaHistorico> { new RotaHistorico() } } };
        var rotasViewModel = new List<RotaViewModel> { new RotaViewModel() };

        _pessoasApiMock.Setup(p => p.ObterAlunoPorResponsavelIdAsync(It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(alunos);
        _userContextMock.SetupGet(u => u.Empresa).Returns(1);
        _rotaRepoMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rotas);
        _mapperMock.Setup(m => m.Map<List<RotaViewModel>>(rotas)).Returns(rotasViewModel);

        var service = CreateService();
        var result = await service.ObterAsync();

        Assert.Equal(rotasViewModel, result);
    }

    [Fact]
    public async Task ObterRotasOnlineAsync_DeveRetornarRotasOnline()
    {
        var alunos = new BaseResponse<List<AlunoViewModel>> { Data = new List<AlunoViewModel> { new AlunoViewModel { Id = 2 } } };
        var rotas = new List<Rota> { new Rota { Historicos = new List<RotaHistorico> { new RotaHistorico { EmAndamento = true } } } };
        var rotasViewModel = new List<RotaViewModel> { new RotaViewModel() };

        _pessoasApiMock.Setup(p => p.ObterAlunoPorResponsavelIdAsync(It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(alunos);
        _userContextMock.SetupGet(u => u.Empresa).Returns(1);
        _rotaRepoMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rotas);
        _mapperMock.Setup(m => m.Map<List<RotaViewModel>>(rotas)).Returns(rotasViewModel);

        var service = CreateService();
        var result = await service.ObterRotasOnlineAsync();

        Assert.Equal(rotasViewModel, result);
    }

    [Fact]
    public async Task ObterPorAlunoIdAsync_DeveRetornarRotasPorAluno()
    {
        var rotasAluno = new List<Rota> { new Rota { AlunoRotas = new List<AlunoRota> { new AlunoRota { AlunoId = 1, RotaId = 2 } } } };
        var rotas = new List<Rota> { new Rota { Id = 2, Status = StatusEntityEnum.Ativo } };
        var rotasViewModel = new List<RotaViewModel> { new RotaViewModel() };

        _rotaRepoMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rotasAluno);
        _rotaRepoMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>())).ReturnsAsync(rotas);
        _mapperMock.Setup(m => m.Map<List<RotaViewModel>>(rotas)).Returns(rotasViewModel);

        var service = CreateService();
        var result = await service.ObterPorAlunoIdAsync(1);

        Assert.Equal(rotasViewModel, result);
    }

    [Fact]
    public async Task ObterRotaDoMotoristaAsync_DeveRetornarRotasDoDia()
    {
        int usuarioId = 123;
        var motoristaId = 456;
        var motoristaRotas = new List<MotoristaRota>
        {
            new MotoristaRota { MotoristaId = motoristaId, RotaId = 1, Status = StatusEntityEnum.Ativo }
        };
        var rotas = new List<Rota>
        {
            new Rota { Id = 1, Status = StatusEntityEnum.Ativo, DiaSemana = DiaSemanaEnum.Todos }
        };
        var rotasViewModel = new List<RotaViewModel> { new RotaViewModel { Id = 1 } };

        _pessoasApiMock.Setup(api => api.ObterMotoristaPorUsuarioIdAsync(usuarioId, It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Data = new MotoristaViewModel { Id = motoristaId } });

        _motoristaRotaRepoMock.Setup(repo => repo.BuscarAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<MotoristaRota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<MotoristaRota, object>>[]>()))
            .ReturnsAsync(motoristaRotas);

        _rotaRepoMock.Setup(repo => repo.BuscarAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>()))
            .ReturnsAsync(rotas);

        _mapperMock.Setup(m => m.Map<List<RotaViewModel>>(rotas)).Returns(rotasViewModel);

        var service = CreateService();
        var result = await service.ObterRotaDoMotoristaAsync(usuarioId);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }
}