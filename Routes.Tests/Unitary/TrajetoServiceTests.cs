using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Exceptions;
using Routes.Service.Implementations;
using Xunit;
using System.Linq.Expressions;

namespace Routes.Tests.Unitary;

public class TrajetoServiceTests
{
    private readonly Mock<ILogger<TrajetoService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPessoasAPI> _pessoasApiMock = new();
    private readonly Mock<IMarcadorService> _marcadorServiceMock = new();
    private readonly Mock<IOrdemTrajetoService> _ordemTrajetoServiceMock = new();
    private readonly Mock<IBaseRepository<AjusteAlunoRota>> _ajusteAlunoRotaRepoMock = new();
    private readonly Mock<IBaseRepository<Endereco>> _enderecoRepoMock = new();
    private readonly Mock<IRotaHistoricoRepository> _rotaHistoricoRepoMock = new();
    private readonly Mock<IBaseRepository<OrdemTrajeto>> _ordemTrajetoRepoMock = new();
    private readonly Mock<IBaseRepository<AlunoRotaHistorico>> _alunoRotaHistoricoRepoMock = new();
    private readonly Mock<IGoogleDirectionsService> _googleDirectionsServiceMock = new();
    private readonly Mock<IBaseRepository<Rota>> _rotaRepoMock = new();

    private TrajetoService CreateService() => new(
        _loggerMock.Object,
        _mapperMock.Object,
        _pessoasApiMock.Object,
        _marcadorServiceMock.Object,
        _ordemTrajetoServiceMock.Object,
        _ajusteAlunoRotaRepoMock.Object,
        _enderecoRepoMock.Object,
        _rotaHistoricoRepoMock.Object,
        _ordemTrajetoRepoMock.Object,
        _alunoRotaHistoricoRepoMock.Object,
        _googleDirectionsServiceMock.Object,
        _rotaRepoMock.Object
    );

    [Fact]
    public async Task AtualizarStatusAlunoTrajetoAsync_AlunoJaComMesmoStatus_NaoAtualiza()
    {
        // Arrange
        var service = CreateService();
        var rotaHistorico = new RotaHistorico { Id = 1, RotaId = 2, EmAndamento = true };
        var alunoRotaHistorico = new AlunoRotaHistorico { AlunoId = 3, RotaHistoricoId = 1, EntrouNaVan = true };
        _rotaHistoricoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<RotaHistorico, bool>>>(), It.IsAny<Expression<Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync(rotaHistorico);
        _alunoRotaHistoricoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<AlunoRotaHistorico, bool>>>(), It.IsAny<Expression<Func<AlunoRotaHistorico, object>>[]>()))
            .ReturnsAsync(alunoRotaHistorico);

        // Act
        await service.AtualizarStatusAlunoTrajetoAsync(3, 2, true);

        // Assert
        _alunoRotaHistoricoRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<AlunoRotaHistorico>()), Times.Never);
        _alunoRotaHistoricoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<AlunoRotaHistorico>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarStatusAlunoTrajetoAsync_AlunoNaoExiste_Adiciona()
    {
        // Arrange
        var service = CreateService();
        var rotaHistorico = new RotaHistorico { Id = 1, RotaId = 2, EmAndamento = true };
        _rotaHistoricoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<RotaHistorico, bool>>>(), It.IsAny<Expression<Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync(rotaHistorico);
        _alunoRotaHistoricoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<AlunoRotaHistorico, bool>>>(), It.IsAny<Expression<Func<AlunoRotaHistorico, object>>[]>()))
            .ReturnsAsync((AlunoRotaHistorico)null);
        _pessoasApiMock.Setup(p => p.ObterAlunoPorIdAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new BaseResponse<List<AlunoViewModel>> { Data = new List<AlunoViewModel> { new AlunoViewModel { Id = 3 } } });

        // Act
        await service.AtualizarStatusAlunoTrajetoAsync(3, 2, true);

        // Assert
        _alunoRotaHistoricoRepoMock.Verify(r => r.AdicionarAsync(It.Is<AlunoRotaHistorico>(a => a.AlunoId == 3 && a.EntrouNaVan)), Times.Once);
    }

    [Fact]
    public async Task ObterTrajetoEmAndamentoAsync_RetornaViewModel()
    {
        // Arrange
        var service = CreateService();
        var rotaHistorico = new RotaHistorico { Id = 1, RotaId = 2, EmAndamento = true };
        var viewModel = new RotaHistoricoViewModel();
        _rotaHistoricoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<RotaHistorico, bool>>>(), It.IsAny<Expression<Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync(rotaHistorico);
        _mapperMock.Setup(m => m.Map<RotaHistoricoViewModel>(rotaHistorico)).Returns(viewModel);

        // Act
        var result = await service.ObterTrajetoEmAndamentoAsync(2);

        // Assert
        Assert.Equal(viewModel, result);
    }

    [Fact]
    public async Task ObterDestinoAsync_OrdemTrajetoNula_ValidaOnline()
    {
        // Arrange
        var service = CreateService();
        var rota = new Rota { Id = 1, TipoRota = TipoRotaEnum.Ida, AlunoRotas = new List<AlunoRota> { new AlunoRota { AlunoId = 10 } } };
        _rotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Rota, bool>>>(), It.IsAny<Expression<Func<Rota, object>>[]>()))
            .ReturnsAsync(rota);
        _pessoasApiMock.Setup(p => p.ObterAlunoPorIdAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new BaseResponse<List<AlunoViewModel>> { Data = new List<AlunoViewModel> { new AlunoViewModel { Id = 10 } } });
        _ordemTrajetoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<OrdemTrajeto, bool>>>(), It.IsAny<Expression<Func<OrdemTrajeto, object>>[]>()))
            .ReturnsAsync((OrdemTrajeto)null);
        _ajusteAlunoRotaRepoMock.Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<AjusteAlunoRota, bool>>>(), It.IsAny<Expression<Func<AjusteAlunoRota, object>>[]>()))
            .ReturnsAsync(new List<AjusteAlunoRota>());
        _rotaHistoricoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<RotaHistorico, bool>>>(), It.IsAny<Expression<Func<RotaHistorico, object>>[]>()))
            .ReturnsAsync(new RotaHistorico { Id = 1, EmAndamento = true });
        _marcadorServiceMock.Setup(m => m.ObterMarcadorPorRotaDirecao(It.IsAny<IEnumerable<AlunoViewModel>>(), It.IsAny<TipoRotaEnum>(), It.IsAny<int>(), It.IsAny<IEnumerable<AjusteAlunoRota>>()))
            .Returns(new List<Marcador> { new Marcador() });

        // Act
        var result = await service.ObterDestinoAsync(1);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GerarMelhorTrajetoAsync_MenosQueDoisMarcadores_DisparaExcecao()
    {
        // Arrange
        var service = CreateService();
        var rota = new Rota { Id = 1, TipoRota = TipoRotaEnum.Ida, AlunoRotas = new List<AlunoRota> { new AlunoRota { AlunoId = 10 } } };
        _rotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Rota, bool>>>(), It.IsAny<Expression<Func<Rota, object>>[]>()))
            .ReturnsAsync(rota);
        _pessoasApiMock.Setup(p => p.ObterAlunoPorIdAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new BaseResponse<List<AlunoViewModel>> { Data = new List<AlunoViewModel> { new AlunoViewModel { Id = 10 } } });
        _ordemTrajetoRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<OrdemTrajeto, bool>>>(), It.IsAny<Expression<Func<OrdemTrajeto, object>>[]>()))
            .ReturnsAsync((OrdemTrajeto)null);
        _marcadorServiceMock.Setup(m => m.ObterMarcadorPorRotaDirecao(It.IsAny<IEnumerable<AlunoViewModel>>(), It.IsAny<TipoRotaEnum>(), It.IsAny<int>(), null))
            .Returns(new List<Marcador> { new Marcador() });

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => service.GerarMelhorTrajetoAsync(1));
    }
}