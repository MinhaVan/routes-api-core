using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Routes.Application.Implementations;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Configuration;
using Xunit;

namespace Routes.Tests.Unitary;

public class GoogleDirectionsServiceTests
{
    private GoogleDirectionsService CreateService(HttpResponseMessage responseMessage, SecretManager secretManager = null)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var httpClient = new HttpClient(handlerMock.Object);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(f => f.CreateClient("api-googlemaps")).Returns(httpClient);

        secretManager ??= new SecretManager
        {
            Google = new Google
            {
                BaseUrl = "https://fake-google.com",
                Key = "fake-key"
            }
        };

        return new GoogleDirectionsService(httpClientFactoryMock.Object, secretManager);
    }

    [Fact]
    public async Task ObterMarcadorAsync_DeveRetornarMarcador_QuandoStatusOkEResultadoExiste()
    {
        // Arrange
        var fakeResponse = new GoogleGeocodeResponse
        {
            Status = "OK",
            Results = new List<Result>
            {
                new Result
                {
                    Geometry = new Geometry
                    {
                        Location = new Location { Lat = 1.23, Lng = 4.56 }
                    }
                }
            }
        };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(fakeResponse)
        };
        var service = CreateService(responseMessage);

        // Act
        var marcador = await service.ObterMarcadorAsync("Rua Teste");

        // Assert
        Assert.NotNull(marcador);
        Assert.Equal(1.23, marcador.Latitude);
        Assert.Equal(4.56, marcador.Longitude);
    }

    [Fact]
    public async Task ObterMarcadorAsync_DeveRetornarNull_QuandoStatusNaoOk()
    {
        // Arrange
        var fakeResponse = new GoogleGeocodeResponse { Status = "ZERO_RESULTS", Results = new List<Result>() };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(fakeResponse)
        };
        var service = CreateService(responseMessage);

        // Act
        var marcador = await service.ObterMarcadorAsync("Rua Inexistente");

        // Assert
        Assert.Null(marcador);
    }

    [Fact]
    public async Task ObterMarcadorAsync_DeveRetornarNull_QuandoNaoHaResultados()
    {
        // Arrange
        var fakeResponse = new GoogleGeocodeResponse { Status = "OK", Results = new List<Result>() };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(fakeResponse)
        };
        var service = CreateService(responseMessage);

        // Act
        var marcador = await service.ObterMarcadorAsync("Rua Vazia");

        // Assert
        Assert.Null(marcador);
    }

    [Fact]
    public async Task ObterRotaIdealAsync_DeveRetornarRotaIdeal_QuandoRespostaValida()
    {
        // Arrange
        var origem = new Marcador { Latitude = 1, Longitude = 2 };
        var destino = new Marcador { Latitude = 3, Longitude = 4 };
        var pontos = new List<Marcador>
        {
            new Marcador { Latitude = 5, Longitude = 6 },
            new Marcador { Latitude = 7, Longitude = 8 }
        };

        var fakeDirectionsResponse = new GoogleDirectionsResponse
        {
            Routes = new List<GoogleDirectionsResponse.Route>
            {
                new GoogleDirectionsResponse.Route { WaypointOrder = new List<int> { 1, 0 } }
            }
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(fakeDirectionsResponse))
        };
        var service = CreateService(responseMessage);

        // Act
        var result = await service.ObterRotaIdealAsync(origem, destino, pontos);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(4, result.Data.Count); // origem + 2 pontos + destino
        Assert.Equal(origem, result.Data[0]);
        Assert.Equal(pontos[1], result.Data[1]); // ordem otimizada
        Assert.Equal(pontos[0], result.Data[2]);
        Assert.Equal(destino, result.Data[3]);
    }

    [Fact]
    public async Task ObterRotaIdealAsync_DeveLancarExcecao_QuandoNaoHaWaypoints()
    {
        // Arrange
        var origem = new Marcador { Latitude = 1, Longitude = 2 };
        var destino = new Marcador { Latitude = 3, Longitude = 4 };
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.ObterRotaIdealAsync(origem, destino, new List<Marcador>()));
    }

    [Fact]
    public async Task ObterRotaIdealAsync_DeveLancarExcecao_QuandoNaoHaRota()
    {
        // Arrange
        var origem = new Marcador { Latitude = 1, Longitude = 2 };
        var destino = new Marcador { Latitude = 3, Longitude = 4 };
        var pontos = new List<Marcador> { new Marcador { Latitude = 5, Longitude = 6 } };

        var fakeDirectionsResponse = new GoogleDirectionsResponse { Routes = null };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(fakeDirectionsResponse))
        };
        var service = CreateService(responseMessage);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.ObterRotaIdealAsync(origem, destino, pontos));
    }
}