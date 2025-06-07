using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bogus;
using Moq;
using Routes.Application.Implementations;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Models;
using Routes.Domain.ViewModels.Rota;
using Xunit;

namespace Routes.Tests.Unitary;

public class OrdemTrajetoServiceTests
{
    private readonly Mock<IBaseRepository<OrdemTrajetoMarcador>> _ordemTrajetoMarcadorRepoMock;
    private readonly Mock<IBaseRepository<OrdemTrajeto>> _ordemTrajetoRepoMock;
    private readonly OrdemTrajetoService _service;

    public OrdemTrajetoServiceTests()
    {
        _ordemTrajetoMarcadorRepoMock = new Mock<IBaseRepository<OrdemTrajetoMarcador>>();
        _ordemTrajetoRepoMock = new Mock<IBaseRepository<OrdemTrajeto>>();
        _service = new OrdemTrajetoService(_ordemTrajetoMarcadorRepoMock.Object, _ordemTrajetoRepoMock.Object);
    }

    [Fact]
    public async Task SalvarOrdemDoTrajetoAsync_DeletaAntigaEAdicionaNova()
    {
        // Arrange
        int rotaId = new Faker().Random.Int(1, 1000);
        var marcadores = new List<Marcador>
        {
            new Marcador { EnderecoId = 10, Latitude = 1.1, Longitude = 2.2, TipoMarcador = TipoMarcadorEnum.Partida },
            new Marcador { EnderecoId = 20, Latitude = 3.3, Longitude = 4.4, TipoMarcador = TipoMarcadorEnum.Destino }
        };

        var ordemExistente = new OrdemTrajeto
        {
            Id = new Faker().Random.Int(1, 1000),
            RotaId = rotaId,
            Status = StatusEntityEnum.Ativo,
            Marcadores = new List<OrdemTrajetoMarcador>()
        };

        _ordemTrajetoRepoMock
            .Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<OrdemTrajeto, bool>>>(), It.IsAny<Expression<Func<OrdemTrajeto, object>>[]>()))
            .ReturnsAsync(ordemExistente);

        _ordemTrajetoRepoMock
            .Setup(r => r.AtualizarAsync(It.IsAny<OrdemTrajeto>()))
            .Returns(Task.CompletedTask);

        _ordemTrajetoRepoMock
            .Setup(r => r.AdicionarAsync(It.IsAny<OrdemTrajeto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SalvarOrdemDoTrajetoAsync(rotaId, marcadores);

        // Assert
        _ordemTrajetoRepoMock.Verify(r => r.AtualizarAsync(It.Is<OrdemTrajeto>(o => o.Status == StatusEntityEnum.Deletado)), Times.Once);
        _ordemTrajetoRepoMock.Verify(r => r.AdicionarAsync(It.Is<OrdemTrajeto>(o =>
            o.RotaId == rotaId &&
            o.Status == StatusEntityEnum.Ativo &&
            o.Marcadores.Count == marcadores.Count &&
            o.Marcadores.First().EnderecoId == marcadores.First().EnderecoId
        )), Times.Once);
    }
}