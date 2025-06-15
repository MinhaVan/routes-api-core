using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels.Rota;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class TrajetoController(
    ITrajetoService trajetoService,
    IGestaoTrajetoService gestaoTrajetoService,
    IRelatorioTrajetoService relatorioTrajetoService,
    IRotaOnlineService rotaOnlineService,
    IMarcadorService marcadorService,
    IOrdemTrajetoService ordemTrajetoService) : BaseController
{
    private readonly IGestaoTrajetoService _gestaoTrajetoService = gestaoTrajetoService;
    private readonly IRelatorioTrajetoService _relatorioTrajetoService = relatorioTrajetoService;
    private readonly IRotaOnlineService _rotaOnlineService = rotaOnlineService;
    private readonly ITrajetoService _trajetoService = trajetoService;
    private readonly IMarcadorService _marcadorService = marcadorService;
    private readonly IOrdemTrajetoService _ordemTrajetoService = ordemTrajetoService;

    /// <summary>
    /// Gera o melhor trajeto para a rota especificada.
    /// Este método calcula o trajeto ideal com base nos marcadores associados à rota.
    /// O trajeto é otimizado para minimizar a distância e o tempo de viagem, considerando as coordenadas geográficas dos marcadores.
    /// O resultado é salvo no banco de dados e pode ser utilizado para planejamento de rotas.
    /// </summary>
    /// <param name="rotaId">Identificador da Rota</param>
    /// <returns></returns>
    [HttpPost("Rota/{rotaId}/Gerar")]
    public async Task<IActionResult> GerarMelhorTrajetoAsync(int rotaId)
    {
        await _trajetoService.GerarMelhorTrajetoAsync(rotaId);
        return Success();
    }

    [HttpGet("Rota/{rotaId}/Destino")]
    public async Task<IActionResult> ObterDestinoAsync([FromRoute] int rotaId, [FromQuery] bool validarRotaOnline)
    {
        var rota = await _trajetoService.ObterDestinoAsync(rotaId, validarRotaOnline);
        return Success(rota);
    }

    [HttpPost("Rota/{rotaId}")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> SalvarOrdemDoTrajetoAsync([FromRoute] int rotaId, [FromBody] List<Marcador> body)
    {
        await _ordemTrajetoService.SalvarOrdemDoTrajetoAsync(rotaId, body);
        return Success();
    }

    [HttpGet("Rota/{rotaId}/marcadores")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> ObterTodosMarcadoresParaRotasAsync([FromRoute] int rotaId)
    {
        var response = await _marcadorService.ObterTodosMarcadoresParaRotasAsync(rotaId);
        return Success(response);
    }

    [HttpGet("Rota/{rotaId}")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> ObterTrajetoEmAndamentoAsync([FromRoute] int rotaId)
    {
        var response = await _trajetoService.ObterTrajetoEmAndamentoAsync(rotaId);
        return Success(response);
    }

    [HttpPost("Rota/{rotaId}/iniciar")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> IniciarTrajetoAsync([FromRoute] int rotaId)
    {
        await _gestaoTrajetoService.IniciarTrajetoAsync(rotaId);
        return Success();
    }

    [HttpPost("Rota/{rotaId}/finalizar")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> FinalizarTrajetoAsync([FromRoute] int rotaId)
    {
        await _gestaoTrajetoService.FinalizarTrajetoAsync(rotaId);
        return Success();
    }

    [HttpPut("rota/{rotaId}/aluno/{alunoId}/{alunoEntrouNaVan}")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> AtualizarStatusAlunoTrajetoAsync([FromRoute] int alunoId, [FromRoute] int rotaId, [FromRoute] bool alunoEntrouNaVan)
    {
        await _trajetoService.AtualizarStatusAlunoTrajetoAsync(alunoId, rotaId, alunoEntrouNaVan);
        return Success();
    }

    [HttpGet("Relatorio/Rota/{rotaId}")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> RelatorioTrajetoAsync([FromRoute] int rotaId)
    {
        var response = await _relatorioTrajetoService.RelatorioUltimoTrajetoAsync(rotaId);
        return Success(response);
    }

    [HttpGet("Rota/Online")]
    public async Task<IActionResult> RotaOnlineAsync()
    {
        return Success(await _rotaOnlineService.RotaOnlineParaMotoristaAsync());
    }
}