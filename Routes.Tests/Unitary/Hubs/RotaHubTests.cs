using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.WebSocket;
using Routes.Service.Hubs;
using Xunit;

namespace Routes.Tests.Unitary.Hubs;

public class RotaHubTests
{
    private readonly Mock<ILogger<RotaHub>> _loggerMock = new();
    private readonly Mock<IPessoasAPI> _pessoasApiMock = new();
    private readonly Mock<IBaseRepository<Rota>> _rotaRepoMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<ILocalizacaoCache> _localizacaoCacheMock = new();
    private readonly Mock<IHubCallerClients> _clientsMock = new();
    private readonly Mock<IGroupManager> _groupsMock = new();
    private readonly Mock<HubCallerContext> _contextMock = new();

    private RotaHub CreateHub()
    {
        var hub = new RotaHub(
            _loggerMock.Object,
            _pessoasApiMock.Object,
            _rotaRepoMock.Object,
            _httpContextAccessorMock.Object,
            _localizacaoCacheMock.Object
        )
        {
            Clients = _clientsMock.Object,
            Groups = _groupsMock.Object,
            Context = _contextMock.Object
        };
        return hub;
    }

    [Fact]
    public async Task EnviarLocalizacao_DeveEnviarParaGrupo()
    {
        // Arrange
        var hub = CreateHub();
        var request = new EnviarLocalizacaoWebSocketRequest
        {
            Latitude = 1,
            Longitude = 2,
            RotaId = 10,
            AlunoId = 20,
            Destino = new DestinoWebSocketRequest { Latitude = 3, Longitude = 4 },
            TipoMensagem = "teste"
        };

        var groupClientMock = new Mock<IClientProxy>();
        _clientsMock.Setup(c => c.Group("10")).Returns(groupClientMock.Object);

        _localizacaoCacheMock.Setup(c => c.SalvarUltimaLocalizacaoAsync(10, It.IsAny<BaseResponse<EnviarLocalizacaoWebSocketResponse>>()))
            .Returns(Task.CompletedTask);

        groupClientMock.Setup(c => c.SendCoreAsync("ReceberLocalizacao", It.IsAny<object[]>(), default))
            .Returns(Task.CompletedTask);

        // Act
        await hub.EnviarLocalizacao(request);

        // Assert
        _clientsMock.Verify(c => c.Group("10"), Times.Once);
        groupClientMock.Verify(c => c.SendCoreAsync("ReceberLocalizacao", It.IsAny<object[]>(), default), Times.Once);
        _localizacaoCacheMock.Verify(c => c.SalvarUltimaLocalizacaoAsync(10, It.IsAny<BaseResponse<EnviarLocalizacaoWebSocketResponse>>()), Times.Once);
    }

    [Fact]
    public async Task AdicionarResponsavelNaRota_DeveAdicionarAoGrupoEEnviarUltimaLocalizacao()
    {
        // Arrange
        var hub = CreateHub();
        _contextMock.SetupGet(c => c.ConnectionId).Returns("conn1");
        _pessoasApiMock.Setup(api => api.ObterAlunoPorResponsavelIdAsync(true, It.IsAny<string>()))
            .ReturnsAsync(new BaseResponse<System.Collections.Generic.List<AlunoViewModel>>
            {
                Sucesso = true,
                Data = new System.Collections.Generic.List<AlunoViewModel> { new AlunoViewModel { Id = 1 } }
            });
        _rotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Rota, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Rota, object>>[]>()))
            .ReturnsAsync(new Rota { AlunoRotas = new System.Collections.Generic.List<AlunoRota> { new AlunoRota { AlunoId = 1 } } });

        _groupsMock.Setup(g => g.AddToGroupAsync("conn1", "10", default)).Returns(Task.CompletedTask);
        _localizacaoCacheMock.Setup(c => c.ObterUltimaLocalizacaoAsync(10)).ReturnsAsync((BaseResponse<EnviarLocalizacaoWebSocketResponse>)null);

        var callerMock = new Mock<ISingleClientProxy>();
        _clientsMock.Setup(c => c.Caller).Returns(callerMock.Object);
        callerMock.Setup(c => c.SendCoreAsync("ReceberLocalizacao", It.IsAny<object[]>(), default)).Returns(Task.CompletedTask);

        // Act
        await hub.AdicionarResponsavelNaRota(1, 10, "token");

        // Assert
        _groupsMock.Verify(g => g.AddToGroupAsync("conn1", "10", default), Times.Once);
        _clientsMock.Verify(c => c.Caller, Times.Once);
        callerMock.Verify(c => c.SendCoreAsync("ReceberLocalizacao", It.IsAny<object[]>(), default), Times.Once);
    }

    [Fact]
    public async Task AdicionarResponsavelNaRota_DeveEnviarErro_SeNaoAutorizado()
    {
        // Arrange
        var hub = CreateHub();
        _contextMock.SetupGet(c => c.ConnectionId).Returns("conn1");
        _pessoasApiMock.Setup(api => api.ObterAlunoPorResponsavelIdAsync(true, It.IsAny<string>()))
            .ReturnsAsync(new BaseResponse<System.Collections.Generic.List<AlunoViewModel>>
            {
                Sucesso = false,
                Data = null
            });

        var callerMock = new Mock<ISingleClientProxy>();
        _clientsMock.Setup(c => c.Caller).Returns(callerMock.Object);
        callerMock.Setup(c => c.SendCoreAsync("ReceberLocalizacao", It.IsAny<object[]>(), default)).Returns(Task.CompletedTask);

        // Act
        await hub.AdicionarResponsavelNaRota(1, 10, "token");

        // Assert
        callerMock.Verify(c => c.SendCoreAsync("ReceberLocalizacao", It.IsAny<object[]>(), default), Times.Once);
    }

    [Fact]
    public async Task AdicionarResponsavelNaRota_DeveEnviarErro_SeIdsInvalidos()
    {
        // Arrange
        var hub = CreateHub();
        var callerMock = new Mock<ISingleClientProxy>();
        _clientsMock.Setup(c => c.Caller).Returns(callerMock.Object);
        callerMock.Setup(c => c.SendCoreAsync("ReceberLocalizacao", It.IsAny<object[]>(), default)).Returns(Task.CompletedTask);

        // Act
        await hub.AdicionarResponsavelNaRota(0, 0, "token");

        // Assert
        callerMock.Verify(c => c.SendCoreAsync("ReceberLocalizacao", It.IsAny<object[]>(), default), Times.Once);
    }

    [Fact]
    public async Task RemoverResponsavelDaRota_DeveRemoverDoGrupo()
    {
        // Arrange
        var hub = CreateHub();
        _contextMock.SetupGet(c => c.ConnectionId).Returns("conn1");
        _groupsMock.Setup(g => g.RemoveFromGroupAsync("conn1", "10", default)).Returns(Task.CompletedTask);

        // Act
        await hub.RemoverResponsavelDaRota(10);

        // Assert
        _groupsMock.Verify(g => g.RemoveFromGroupAsync("conn1", "10", default), Times.Once);
    }
}