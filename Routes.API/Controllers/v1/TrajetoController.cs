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
public class TrajetoController : BaseController
{
    private readonly ITrajetoService _trajetoService;

    public TrajetoController(ITrajetoService trajetoService)
    {
        _trajetoService = trajetoService;
    }

    [HttpGet("rota/{rotaId}/destino")]
    public async Task<IActionResult> ObterDestinoAsync([FromRoute] int rotaId, [FromQuery] bool validarRotaOnline)
    {
        var rota = await _trajetoService.ObterDestinoAsync(rotaId, validarRotaOnline);
        return Success(rota);
    }

    [HttpPost("rota/{rotaId}")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> SalvarOrdemDoTrajetoAsync([FromRoute] int rotaId, [FromBody] List<Marcador> body)
    {
        await _trajetoService.SalvarOrdemDoTrajetoAsync(rotaId, body);
        return Success();
    }

    [HttpGet("Rota/{rotaId}/marcadores")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> ObterTodosMarcadoresParaRotasAsync([FromRoute] int rotaId)
    {
        var response = await _trajetoService.ObterTodosMarcadoresParaRotasAsync(rotaId);
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
        await _trajetoService.IniciarTrajetoAsync(rotaId);
        return Success();
    }

    [HttpPost("Rota/{rotaId}/finalizar")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte, PerfilEnum.Motorista)]
    public async Task<IActionResult> FinalizarTrajetoAsync([FromRoute] int rotaId)
    {
        await _trajetoService.FinalizarTrajetoAsync(rotaId);
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
        var response = await _trajetoService.RelatorioUltimoTrajetoAsync(rotaId);
        return Success(response);
    }

    [HttpGet("rota/online")]
    public async Task<IActionResult> RotaOnlineAsync()
    {
        return Success(await _trajetoService.RotaOnlineParaMotoristaAsync());
    }
}