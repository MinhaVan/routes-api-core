using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Routes.Application.Implementations;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Xunit;

namespace Routes.Tests.Unitary;

public class MarcadorServiceTest
{
    private readonly Mock<IPessoasAPI> _pessoasApiMock = new();
    private readonly Mock<IBaseRepository<Rota>> _rotaRepoMock = new();
    private readonly Mock<IBaseRepository<AjusteAlunoRota>> _ajusteRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private MarcadorService CreateService() =>
        new(_pessoasApiMock.Object, _rotaRepoMock.Object, _ajusteRepoMock.Object, _mapperMock.Object);

    private static AlunoViewModel CreateAluno(int id, double latPartida, double lngPartida, double latDestino, double lngDestino)
    {
        return new AlunoViewModel
        {
            Id = id,
            EnderecoPartida = new EnderecoViewModel { Latitude = latPartida, Longitude = lngPartida },
            EnderecoDestino = new EnderecoViewModel { Latitude = latDestino, Longitude = lngDestino },
            EnderecoPartidaId = id * 10 + 1,
            EnderecoDestinoId = id * 10 + 2
        };
    }

    [Fact]
    public async Task ObterTodosMarcadoresParaRotasAsync_ReturnsMarcadores()
    {
        // Arrange
        var rotaId = 1;
        var alunoId = 100;
        var rota = new Rota
        {
            Id = rotaId,
            TipoRota = TipoRotaEnum.Ida,
            AlunoRotas = new List<AlunoRota>
            {
                new() { AlunoId = alunoId, Status = StatusEntityEnum.Ativo }
            }
        };
        var alunos = new List<AlunoViewModel> { CreateAluno(alunoId, 1.1, 2.2, 3.3, 4.4) };
        var alunosResponse = new BaseResponse<List<AlunoViewModel>> { Data = alunos };

        _rotaRepoMock.Setup(r => r.BuscarUmAsync(It.IsAny<Expression<Func<Rota, bool>>>(), It.IsAny<Expression<Func<Rota, object>>>()))
            .ReturnsAsync(rota);

        _pessoasApiMock.Setup(p => p.ObterAlunoPorIdAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(alunosResponse);

        _ajusteRepoMock.Setup(a => a.BuscarAsync(It.IsAny<Expression<Func<AjusteAlunoRota, bool>>>(),
            It.IsAny<Expression<Func<AjusteAlunoRota, object>>>(),
            It.IsAny<Expression<Func<AjusteAlunoRota, object>>>(),
            It.IsAny<Expression<Func<AjusteAlunoRota, object>>>()))
            .ReturnsAsync(new List<AjusteAlunoRota>());

        _mapperMock.Setup(m => m.Map<AlunoViewModel>(It.IsAny<AlunoViewModel>()))
            .Returns<AlunoViewModel>(a => a);

        var service = CreateService();

        // Act
        var marcadores = await service.ObterTodosMarcadoresParaRotasAsync(rotaId);

        // Assert
        Assert.NotNull(marcadores);
        Assert.All(marcadores, m => Assert.NotNull(m.Alunos));
        Assert.Contains(marcadores, m => m.TipoMarcador == TipoMarcadorEnum.Partida);
        Assert.Contains(marcadores, m => m.TipoMarcador == TipoMarcadorEnum.Destino);
    }

    [Fact]
    public void ObterMarcadorPorRotaDirecao_Ida_AgrupaPorLocalizacao()
    {
        // Arrange
        var alunos = new List<AlunoViewModel>
        {
            CreateAluno(1, 1.1, 2.2, 3.3, 4.4),
            CreateAluno(2, 1.1, 2.2, 3.3, 4.4) // mesmo endereço
        };
        _mapperMock.Setup(m => m.Map<AlunoViewModel>(It.IsAny<AlunoViewModel>()))
            .Returns<AlunoViewModel>(a => a);

        var service = CreateService();

        // Act
        var marcadores = service.ObterMarcadorPorRotaDirecao(alunos, TipoRotaEnum.Ida, 1);

        // Assert
        Assert.Equal(2, marcadores.Count); // 1 partida, 1 destino
        Assert.All(marcadores, m => Assert.Equal(2, m.Alunos.Count));
    }

    [Fact]
    public void ObterMarcadorPorRotaDirecao_Volta_ComRetorno()
    {
        // Arrange
        var aluno = CreateAluno(1, 1.1, 2.2, 3.3, 4.4);
        aluno.EnderecoRetorno = new EnderecoViewModel { Latitude = 5.5, Longitude = 6.6 };
        aluno.EnderecoRetornoId = 99;
        _mapperMock.Setup(m => m.Map<AlunoViewModel>(It.IsAny<AlunoViewModel>()))
            .Returns<AlunoViewModel>(a => a);

        var service = CreateService();

        // Act
        var marcadores = service.ObterMarcadorPorRotaDirecao(new[] { aluno }, TipoRotaEnum.Volta, 1);

        // Assert
        Assert.Contains(marcadores, m => m.TipoMarcador == TipoMarcadorEnum.InicioRetorno);
        Assert.Contains(marcadores, m => m.TipoMarcador == TipoMarcadorEnum.Retorno);
    }

    [Fact]
    public void ObterMarcadorPorRotaDirecao_SemAjuste_UsaEnderecoPadrao()
    {
        // Arrange
        var aluno = CreateAluno(1, 10, 20, 30, 40);
        _mapperMock.Setup(m => m.Map<AlunoViewModel>(It.IsAny<AlunoViewModel>()))
            .Returns<AlunoViewModel>(a => a);

        var service = CreateService();

        // Act
        var marcadores = service.ObterMarcadorPorRotaDirecao(new[] { aluno }, TipoRotaEnum.Ida, 1);

        // Assert
        Assert.Contains(marcadores, m => m.Latitude == 10 && m.Longitude == 20);
        Assert.Contains(marcadores, m => m.Latitude == 30 && m.Longitude == 40);
    }
}