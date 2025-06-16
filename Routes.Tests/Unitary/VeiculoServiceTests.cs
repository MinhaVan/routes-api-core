using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;
using Routes.Service.Implementations;
using Xunit;

namespace Routes.Tests.Unitary;

public class VeiculoServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IPessoasAPI> _pessoasApiMock = new();
    private readonly Mock<IBaseRepository<Veiculo>> _veiculoRepoMock = new();
    private readonly Mock<IBaseRepository<MotoristaRota>> _motoristaRotaRepoMock = new();

    private VeiculoService CreateService()
    {
        return new VeiculoService(
            _mapperMock.Object,
            _userContextMock.Object,
            _pessoasApiMock.Object,
            _veiculoRepoMock.Object,
            _motoristaRotaRepoMock.Object
        );
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarVeiculosComEmpresaId()
    {
        // Arrange
        var veiculosViewModels = new List<VeiculoAdicionarViewModel> { new VeiculoAdicionarViewModel() };
        var veiculos = new List<Veiculo> { new Veiculo() };
        _mapperMock.Setup(m => m.Map<List<Veiculo>>(veiculosViewModels)).Returns(veiculos);
        _userContextMock.SetupGet(u => u.Empresa).Returns(42);

        var service = CreateService();

        // Act
        await service.AdicionarAsync(veiculosViewModels);

        // Assert
        Assert.All(veiculos, v => Assert.Equal(42, v.EmpresaId));
        _veiculoRepoMock.Verify(r => r.AdicionarAsync(veiculos), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarVeiculosComEmpresaId()
    {
        // Arrange
        var veiculosViewModels = new List<VeiculoAtualizarViewModel> { new VeiculoAtualizarViewModel() };
        var veiculos = new List<Veiculo> { new Veiculo() };
        _mapperMock.Setup(m => m.Map<List<Veiculo>>(veiculosViewModels)).Returns(veiculos);
        _userContextMock.SetupGet(u => u.Empresa).Returns(99);

        var service = CreateService();

        // Act
        await service.AtualizarAsync(veiculosViewModels);

        // Assert
        Assert.All(veiculos, v => Assert.Equal(99, v.EmpresaId));
        _veiculoRepoMock.Verify(r => r.AtualizarAsync(veiculos), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_DeveAtualizarStatusParaDeletado()
    {
        // Arrange
        var veiculo = new Veiculo { Id = 1, Status = StatusEntityEnum.Ativo };
        _veiculoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(veiculo);

        var service = CreateService();

        // Act
        await service.DeletarAsync(1);

        // Assert
        Assert.Equal(StatusEntityEnum.Deletado, veiculo.Status);
        _veiculoRepoMock.Verify(r => r.AtualizarAsync(veiculo), Times.Once);
    }

    [Fact]
    public async Task ObterAsync_DeveRetornarVeiculosDaEmpresa()
    {
        // Arrange
        var veiculos = new List<Veiculo> { new Veiculo() };
        var veiculosViewModel = new List<VeiculoViewModel> { new VeiculoViewModel() };
        _userContextMock.SetupGet(u => u.Empresa).Returns(7);
        _veiculoRepoMock.Setup(r => r.BuscarAsync(It.IsAny<Expression<System.Func<Veiculo, bool>>>(), It.IsAny<Expression<System.Func<Veiculo, object>>[]>()))
            .ReturnsAsync(veiculos);
        _mapperMock.Setup(m => m.Map<List<VeiculoViewModel>>(veiculos)).Returns(veiculosViewModel);

        var service = CreateService();

        // Act
        var result = await service.ObterAsync();

        // Assert
        Assert.Equal(veiculosViewModel, result);
    }

    [Fact]
    public async Task ObterAsync_ComRota_DeveRetornarVeiculoComMotorista()
    {
        // Arrange
        var veiculo = new Veiculo { Id = 1, EmpresaId = 2 };
        var motoristaRota = new MotoristaRota { MotoristaId = 10, Status = StatusEntityEnum.Ativo };
        var motorista = new MotoristaViewModel { CNH = "123", Vencimento = System.DateTime.Now, TipoCNH = TipoCNHEnum.B, Foto = "foto.jpg" };
        var veiculoViewModel = new VeiculoViewModel();
        var motoristaResponse = new BaseResponse<MotoristaViewModel>
        {
            Sucesso = true,
            Data = motorista
        };

        _userContextMock.SetupGet(u => u.Empresa).Returns(2);
        _veiculoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<System.Func<Veiculo, bool>>>(), It.IsAny<Expression<System.Func<Veiculo, object>>[]>()))
            .ReturnsAsync(veiculo);
        _motoristaRotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<System.Func<MotoristaRota, bool>>>(), It.IsAny<Expression<System.Func<MotoristaRota, object>>[]>()))
            .ReturnsAsync(motoristaRota);
        _pessoasApiMock.Setup(p => p.ObterMotoristaPorIdAsync(10, false)).ReturnsAsync(motoristaResponse);
        _mapperMock.Setup(m => m.Map<VeiculoViewModel>(veiculo)).Returns(veiculoViewModel);
        _mapperMock.Setup(m => m.Map<MotoristaViewModel>(motorista)).Returns(motorista);

        var service = CreateService();

        // Act
        var result = await service.ObterAsync(1, 1);

        // Assert
        Assert.Equal(motorista.CNH, result.Motorista.CNH);
        Assert.Equal(motorista.Vencimento, result.Motorista.Vencimento);
        Assert.Equal(motorista.TipoCNH, result.Motorista.TipoCNH);
        Assert.Equal(motorista.Foto, result.Motorista.Foto);
    }

    [Fact]
    public async Task ObterAsync_ComRota_DeveLancarExcecao_SeMotoristaNaoEncontrado()
    {
        // Arrange
        var veiculo = new Veiculo { Id = 1, EmpresaId = 2 };
        var motoristaRota = new MotoristaRota { MotoristaId = 10, Status = StatusEntityEnum.Ativo };
        var motoristaResponse = new BaseResponse<MotoristaViewModel>
        {
            Sucesso = false,
            Mensagem = "Motorista não encontrado",
            Data = null
        };

        _userContextMock.SetupGet(u => u.Empresa).Returns(2);
        _veiculoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<System.Func<Veiculo, bool>>>(), It.IsAny<Expression<System.Func<Veiculo, object>>[]>()))
            .ReturnsAsync(veiculo);
        _motoristaRotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<System.Func<MotoristaRota, bool>>>(), It.IsAny<Expression<System.Func<MotoristaRota, object>>[]>()))
            .ReturnsAsync(motoristaRota);
        _pessoasApiMock.Setup(p => p.ObterMotoristaPorIdAsync(10, false)).ReturnsAsync(motoristaResponse);

        var service = CreateService();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => service.ObterAsync(1, 1));
        Assert.Contains("Motorista não encontrado", ex.Message);
    }
}