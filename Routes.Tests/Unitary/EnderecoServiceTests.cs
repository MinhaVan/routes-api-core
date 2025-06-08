using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Implementations;
using Xunit;

namespace Routes.Tests.Unitary;

public class EnderecoServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IBaseRepository<Endereco>> _enderecoRepoMock = new();
    private readonly Mock<IGoogleDirectionsService> _googleDirectionsServiceMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();

    private EnderecoService CreateService()
    {
        return new EnderecoService(
            _mapperMock.Object,
            _enderecoRepoMock.Object,
            _googleDirectionsServiceMock.Object,
            _userContextMock.Object
        );
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarEnderecoComMarcador()
    {
        // Arrange
        var viewModel = new EnderecoAdicionarViewModel { UsuarioId = null };
        var endereco = new Endereco();
        var marcador = new Marcador { Latitude = 1.1, Longitude = 2.2 };

        _mapperMock.Setup(m => m.Map<Endereco>(viewModel)).Returns(endereco);
        _userContextMock.SetupGet(u => u.UserId).Returns(42);
        _googleDirectionsServiceMock.Setup(g => g.ObterMarcadorAsync(It.IsAny<string>())).ReturnsAsync(marcador);
        _enderecoRepoMock.Setup(r => r.AdicionarAsync(endereco)).Returns(Task.CompletedTask);

        var service = CreateService();

        // Act
        await service.AdicionarAsync(viewModel);

        // Assert
        Assert.Equal(42, endereco.UsuarioId);
        Assert.Equal(1.1, endereco.Latitude);
        Assert.Equal(2.2, endereco.Longitude);
        _enderecoRepoMock.Verify(r => r.AdicionarAsync(endereco), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarEnderecoERecalcularMarcador_SeEnderecoMudou()
    {
        // Arrange
        var viewModel = new EnderecoAtualizarViewModel
        {
            Id = 1,
            Rua = "Nova Rua",
            Numero = "123",
            Bairro = "Centro",
            Cidade = "Cidade",
            Estado = "UF",
            CEP = "00000-000",
            Pais = "Brasil",
            TipoEndereco = Domain.Enums.TipoEnderecoEnum.Casa
        };
        var endereco = new Endereco
        {
            Id = 1,
            Rua = "Antiga Rua",
            Numero = "321",
            Bairro = "Bairro",
            Cidade = "Cidade",
            Estado = "UF",
            CEP = "11111-111"
        };
        var marcador = new Marcador { Latitude = 9.9, Longitude = 8.8 };

        _enderecoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(endereco);
        _googleDirectionsServiceMock.Setup(g => g.ObterMarcadorAsync(It.IsAny<string>())).ReturnsAsync(marcador);
        _enderecoRepoMock.Setup(r => r.AtualizarAsync(endereco)).Returns(Task.CompletedTask);

        var service = CreateService();

        // Act
        await service.AtualizarAsync(viewModel);

        // Assert
        Assert.Equal("Nova Rua", endereco.Rua);
        Assert.Equal("123", endereco.Numero);
        Assert.Equal(9.9, endereco.Latitude);
        Assert.Equal(8.8, endereco.Longitude);
        _enderecoRepoMock.Verify(r => r.AtualizarAsync(endereco), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_NaoChamaMarcador_SeEnderecoNaoMudou()
    {
        // Arrange
        var viewModel = new EnderecoAtualizarViewModel
        {
            Id = 1,
            Rua = "Rua",
            Numero = "123",
            Bairro = "Bairro",
            Cidade = "Cidade",
            Estado = "UF",
            CEP = "00000-000",
            Pais = "Brasil",
            TipoEndereco = Domain.Enums.TipoEnderecoEnum.Casa
        };
        var endereco = new Endereco
        {
            Id = 1,
            Rua = "Rua",
            Numero = "123",
            Bairro = "Bairro",
            Cidade = "Cidade",
            Estado = "UF",
            CEP = "00000-000"
        };

        _enderecoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(endereco);
        _enderecoRepoMock.Setup(r => r.AtualizarAsync(endereco)).Returns(Task.CompletedTask);

        var service = CreateService();

        // Act
        await service.AtualizarAsync(viewModel);

        // Assert
        _googleDirectionsServiceMock.Verify(g => g.ObterMarcadorAsync(It.IsAny<string>()), Times.Never);
        _enderecoRepoMock.Verify(r => r.AtualizarAsync(endereco), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_DeveChamarDeletar()
    {
        // Arrange
        _enderecoRepoMock.Setup(r => r.DeletarAsync(1)).Returns(Task.CompletedTask);
        var service = CreateService();

        // Act
        await service.DeletarAsync(1);

        // Assert
        _enderecoRepoMock.Verify(r => r.DeletarAsync(1), Times.Once);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarViewModel()
    {
        // Arrange
        var endereco = new Endereco { Id = 1 };
        var viewModel = new EnderecoViewModel { Id = 1 };

        _enderecoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(endereco);
        _mapperMock.Setup(m => m.Map<EnderecoViewModel>(endereco)).Returns(viewModel);

        var service = CreateService();

        // Act
        var result = await service.Obter(1);

        // Assert
        Assert.Equal(viewModel, result);
    }

    [Fact]
    public async Task ObterPorListaIds_DeveRetornarViewModels()
    {
        // Arrange
        var ids = new List<int> { 1, 2 };
        var enderecos = new List<Endereco> { new Endereco { Id = 1 }, new Endereco { Id = 2 } };
        var viewModels = new List<EnderecoViewModel> { new EnderecoViewModel { Id = 1 }, new EnderecoViewModel { Id = 2 } };

        _enderecoRepoMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Endereco, bool>>>())).ReturnsAsync(enderecos);
        _mapperMock.Setup(m => m.Map<List<EnderecoViewModel>>(enderecos)).Returns(viewModels);

        var service = CreateService();

        // Act
        var result = await service.Obter(ids);

        // Assert
        Assert.Equal(viewModels, result);
    }

    [Fact]
    public async Task Obter_DeveRetornarEnderecosDoUsuarioAtivo()
    {
        // Arrange
        var userId = 42;
        var enderecos = new List<Endereco> { new Endereco { Id = 1, UsuarioId = userId } };
        var viewModels = new List<EnderecoViewModel> { new EnderecoViewModel { Id = 1 } };

        _userContextMock.SetupGet(u => u.UserId).Returns(userId);
        _enderecoRepoMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Endereco, bool>>>())).ReturnsAsync(enderecos);
        _mapperMock.Setup(m => m.Map<List<EnderecoViewModel>>(enderecos)).Returns(viewModels);

        var service = CreateService();

        // Act
        var result = await service.Obter();

        // Assert
        Assert.Equal(viewModels, result);
    }
}