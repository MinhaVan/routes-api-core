
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Routes.Application.Implementations;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;
using Routes.Service.Exceptions;
using Xunit;

namespace Routes.Tests.Unitary;

public class GestaoTrajetoServiceTests
{
    private readonly Mock<IBaseRepository<Rota>> _rotaRepositoryMock = new();
    private readonly Mock<IBaseRepository<RotaHistorico>> _rotaHistoricoRepositoryMock = new();
    private readonly Mock<ILogger<GestaoTrajetoService>> _loggerMock = new();

    private GestaoTrajetoService CreateService() =>
        new(_rotaRepositoryMock.Object, _rotaHistoricoRepositoryMock.Object, _loggerMock.Object);

    [Fact]
    public async Task FinalizarTrajetoAsync_DeveFinalizarQuandoExisteTrajetoEmAndamento()
    {
        // Arrange
        var rotaId = 1;
        var trajeto = new RotaHistorico { RotaId = rotaId, EmAndamento = true };
        _rotaHistoricoRepositoryMock
            .Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<RotaHistorico, bool>>>(), It.IsAny<Expression<Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync(trajeto);

        var service = CreateService();

        // Act
        await service.FinalizarTrajetoAsync(rotaId);

        // Assert
        Assert.False(trajeto.EmAndamento);
        Assert.NotNull(trajeto.DataFim);
        _rotaHistoricoRepositoryMock.Verify(r => r.AtualizarAsync(trajeto), Times.Once);
    }

    [Fact]
    public async Task FinalizarTrajetoAsync_DeveLancarExcecaoQuandoNaoExisteTrajeto()
    {
        // Arrange
        var rotaId = 1;
        _rotaHistoricoRepositoryMock
            .Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<RotaHistorico, bool>>>(), It.IsAny<Expression<Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync((RotaHistorico)null);

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => service.FinalizarTrajetoAsync(rotaId));
    }

    [Fact]
    public async Task IniciarTrajetoAsync_DeveCriarNovoTrajetoQuandoNaoExiste()
    {
        // Arrange
        var rotaId = 1;
        var rota = new Rota { Id = rotaId, TipoRota = TipoRotaEnum.Ida };
        _rotaRepositoryMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Rota, bool>>>(), It.IsAny<Expression<Func<Rota, object>>[]>()))
            .ReturnsAsync(rota);
        _rotaHistoricoRepositoryMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<RotaHistorico, bool>>>(), It.IsAny<Expression<Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync((RotaHistorico)null);

        var service = CreateService();

        // Act
        await service.IniciarTrajetoAsync(rotaId);

        // Assert
        _rotaHistoricoRepositoryMock.Verify(r => r.AdicionarAsync(It.Is<RotaHistorico>(h => h.RotaId == rotaId && h.EmAndamento)), Times.Once);
    }

    [Fact]
    public async Task IniciarTrajetoAsync_DeveFinalizarTrajetoAntigoSeForaDoIntervalo()
    {
        // Arrange
        var rotaId = 1;
        var rota = new Rota { Id = rotaId, TipoRota = TipoRotaEnum.Ida };
        var antigo = new RotaHistorico
        {
            RotaId = rotaId,
            EmAndamento = true,
            DataRealizacao = DateTime.UtcNow.AddHours(-1)
        };
        _rotaRepositoryMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Rota, bool>>>(), It.IsAny<Expression<Func<Rota, object>>[]>()))
            .ReturnsAsync(rota);
        _rotaHistoricoRepositoryMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<RotaHistorico, bool>>>(), It.IsAny<Expression<Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync(antigo);

        var service = CreateService();

        // Act
        await service.IniciarTrajetoAsync(rotaId);

        // Assert
        Assert.False(antigo.EmAndamento);
        Assert.NotNull(antigo.DataFim);
        _rotaHistoricoRepositoryMock.Verify(r => r.AtualizarAsync(antigo), Times.Once);
        _rotaHistoricoRepositoryMock.Verify(r => r.AdicionarAsync(It.Is<RotaHistorico>(h => h.RotaId == rotaId && h.EmAndamento)), Times.Once);
    }
}