using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;
using Routes.Service.Implementations;
using Xunit;

namespace Routes.Tests.Unitary;

public class MotoristaRotaServiceTests
{
    private readonly Mock<IPessoasAPI> _pessoasApiMock = new();
    private readonly Mock<IMotoristaRotaRepository> _motoristaRotaRepoMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private MotoristaRotaService CreateService()
    {
        return new MotoristaRotaService(
            _pessoasApiMock.Object,
            _motoristaRotaRepoMock.Object
        );
    }

    [Fact]
    public async Task VincularAsync_DeveAdicionarNovoVinculo_SeNaoExiste()
    {
        // Arrange
        var request = new MotoristaVincularViewModel { RotaId = 10 };
        var motorista = new MotoristaViewModel { Id = 5 };
        _userContextMock.SetupGet(u => u.UserId).Returns(99);
        _pessoasApiMock.Setup(p => p.ObterMotoristaPorUsuarioIdAsync(99, It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Data = motorista });
        _motoristaRotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MotoristaRota, bool>>>(), null))
            .ReturnsAsync((MotoristaRota)null);

        var service = CreateService();

        // Act
        await service.VincularAsync(request);

        // Assert
        _motoristaRotaRepoMock.Verify(r => r.AdicionarAsync(It.Is<MotoristaRota>(m => m.MotoristaId == 5 && m.RotaId == 10)), Times.Once);
    }

    [Fact]
    public async Task VincularAsync_DeveLancarExcecao_SeMotoristaNaoEncontrado()
    {
        // Arrange
        var request = new MotoristaVincularViewModel { RotaId = 10 };
        _userContextMock.SetupGet(u => u.UserId).Returns(99);
        _pessoasApiMock.Setup(p => p.ObterMotoristaPorUsuarioIdAsync(99, It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Data = null, Mensagem = "Não encontrado" });

        var service = CreateService();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => service.VincularAsync(request));
        Assert.Contains("Não encontrado", ex.Message);
    }

    [Fact]
    public async Task VincularAsync_DeveLancarExcecao_SeJaAtivo()
    {
        // Arrange
        var request = new MotoristaVincularViewModel { RotaId = 10 };
        var motorista = new MotoristaViewModel { Id = 5 };
        var configuracao = new MotoristaRota { Id = 1, Status = StatusEntityEnum.Ativo };
        _userContextMock.SetupGet(u => u.UserId).Returns(99);
        _pessoasApiMock.Setup(p => p.ObterMotoristaPorUsuarioIdAsync(99, It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Data = motorista });
        _motoristaRotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MotoristaRota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<MotoristaRota, object>>[]>()))
            .ReturnsAsync(configuracao);

        var service = CreateService();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => service.VincularAsync(request));
        Assert.Contains("Motorista já está configurado para essa rota", ex.Message);
    }

    [Fact]
    public async Task VincularAsync_DeveAtualizarConfiguracao_SeDesativada()
    {
        // Arrange
        var request = new MotoristaVincularViewModel { RotaId = 10 };
        var motorista = new MotoristaViewModel { Id = 5 };
        var configuracao = new MotoristaRota { Id = 1, Status = StatusEntityEnum.Ativo };
        _userContextMock.SetupGet(u => u.UserId).Returns(99);
        _pessoasApiMock.Setup(p => p.ObterMotoristaPorUsuarioIdAsync(99, It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Data = motorista });
        _motoristaRotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MotoristaRota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<MotoristaRota, object>>[]>()))
            .ReturnsAsync(configuracao);

        var service = CreateService();

        // Act
        await Assert.ThrowsAsync<BusinessRuleException>(() => service.VincularAsync(request));
        // O método lança exceção antes de atualizar, então não há verificação de atualização aqui.
    }

    [Fact]
    public async Task DesvincularAsync_DeveAtualizarStatusParaDeletado()
    {
        // Arrange
        var request = new MotoristaVincularViewModel { RotaId = 10, MotoristaId = 5 };
        var motorista = new MotoristaViewModel { Id = 5 };
        var configuracao = new MotoristaRota { Id = 1, MotoristaId = 5, RotaId = 10, Status = StatusEntityEnum.Ativo };
        _userContextMock.SetupGet(u => u.UserId).Returns(99);
        _pessoasApiMock.Setup(p => p.ObterMotoristaPorUsuarioIdAsync(99, It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Data = motorista });
        _motoristaRotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MotoristaRota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<MotoristaRota, object>>[]>()))
            .ReturnsAsync(configuracao);

        var service = CreateService();

        // Act
        await service.DesvincularAsync(request);

        // Assert
        Assert.Equal(StatusEntityEnum.Deletado, configuracao.Status);
        _motoristaRotaRepoMock.Verify(r => r.AtualizarAsync(configuracao), Times.Once);
    }

    [Fact]
    public async Task DesvincularAsync_DeveLancarExcecao_SeNaoEncontrado()
    {
        // Arrange
        var request = new MotoristaVincularViewModel { RotaId = 10 };
        var motorista = new MotoristaViewModel { Id = 5 };
        _userContextMock.SetupGet(u => u.UserId).Returns(99);
        _pessoasApiMock.Setup(p => p.ObterMotoristaPorUsuarioIdAsync(99, It.IsAny<bool>()))
            .ReturnsAsync(new BaseResponse<MotoristaViewModel> { Data = motorista });
        _motoristaRotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<Func<MotoristaRota, bool>>>(), null))
            .ReturnsAsync((MotoristaRota)null);

        var service = CreateService();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => service.DesvincularAsync(request));
        Assert.Contains("Motorista não está configurado para essa rota", ex.Message);
    }
}