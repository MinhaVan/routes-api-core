using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using Moq;
using Routes.Application.Implementations;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;
using Xunit;

namespace Routes.Tests.Unitary;

public class RotaOnlineServiceTests
{
    private readonly Mock<IPessoasAPI> _pessoasApiMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IBaseRepository<MotoristaRota>> _motoristaRotaRepoMock = new();
    private readonly Mock<IBaseRepository<RotaHistorico>> _rotaHistoricoRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private RotaOnlineService CreateService() =>
        new(
            _pessoasApiMock.Object,
            _userContextMock.Object,
            _motoristaRotaRepoMock.Object,
            _rotaHistoricoRepoMock.Object,
            _mapperMock.Object
        );

    [Fact]
    public async Task RotaOnlineParaMotoristaAsync_DeveRetornarViewModel_QuandoTrajetoOnlineExiste()
    {
        // Arrange
        var userId = 123;
        var motoristaId = 10;
        var rotaId = 99;
        var rota = new Rota { Id = rotaId };
        var motorista = new MotoristaViewModel { Id = motoristaId };
        var motoristaRotas = new List<MotoristaRota> { new() { MotoristaId = motoristaId, RotaId = rotaId } };
        var trajetoOnline = new RotaHistorico { Id = 1, RotaId = rotaId, EmAndamento = true, DataFim = null, Rota = rota };
        var rotaViewModel = new RotaViewModel();

        _userContextMock.SetupGet(x => x.UserId).Returns(userId);
        _pessoasApiMock.Setup(x => x.ObterMotoristaPorUsuarioIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Sucesso = true, Data = motorista });
        _motoristaRotaRepoMock.Setup(x => x.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MotoristaRota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<MotoristaRota, object>>[]>()))
            .ReturnsAsync(motoristaRotas);
        _rotaHistoricoRepoMock.Setup(x => x.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<RotaHistorico, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync(trajetoOnline);
        _mapperMock.Setup(m => m.Map<RotaViewModel>(rota)).Returns(rotaViewModel);

        var service = CreateService();

        // Act
        var result = await service.RotaOnlineParaMotoristaAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(rotaViewModel, result);
    }

    [Fact]
    public async Task RotaOnlineParaMotoristaAsync_DeveRetornarNull_QuandoNaoExisteTrajetoOnline()
    {
        // Arrange
        var userId = 123;
        var motoristaId = 10;
        var rotaId = 99;
        var motorista = new MotoristaViewModel { Id = motoristaId };
        var motoristaRotas = new List<MotoristaRota> { new() { MotoristaId = motoristaId, RotaId = rotaId } };

        _userContextMock.SetupGet(x => x.UserId).Returns(userId);
        _pessoasApiMock.Setup(x => x.ObterMotoristaPorUsuarioIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Sucesso = true, Data = motorista });
        _motoristaRotaRepoMock.Setup(x => x.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MotoristaRota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<MotoristaRota, object>>[]>()))
            .ReturnsAsync(motoristaRotas);
        _rotaHistoricoRepoMock.Setup(x => x.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<RotaHistorico, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync((RotaHistorico)null);

        var service = CreateService();

        // Act
        var result = await service.RotaOnlineParaMotoristaAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RotaOnlineParaMotoristaAsync_DeveLancarExcecao_QuandoApiFalha()
    {
        // Arrange
        var userId = new Faker().Random.Int(1, 1000);
        _userContextMock.SetupGet(x => x.UserId).Returns(userId);
        _pessoasApiMock.Setup(x => x.ObterMotoristaPorUsuarioIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Sucesso = false, Mensagem = "Erro" });

        var service = CreateService();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => service.RotaOnlineParaMotoristaAsync());
        Assert.Equal("Erro", ex.Message);
    }
}