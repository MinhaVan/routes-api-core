using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class VeiculoController : BaseController
{
    private readonly IVeiculoService _veiculoService;

    public VeiculoController(IVeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    [HttpPost]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte)]
    public async Task<IActionResult> AdicionarAsync([FromBody] List<VeiculoAdicionarViewModel> veiculoAdicionarViewModel)
    {
        await _veiculoService.AdicionarAsync(veiculoAdicionarViewModel);
        return Success();
    }

    [HttpPut("{id}")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte)]
    public async Task<IActionResult> AtualizarAsync([FromBody] List<VeiculoAtualizarViewModel> veiculoAdicionarViewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _veiculoService.AtualizarAsync(veiculoAdicionarViewModel);
        return Success();
    }

    [HttpDelete("{id}")]
    // [AuthorizeRoles(PerfilEnum.Administrador, PerfilEnum.Suporte)]
    public async Task<IActionResult> DeletarAsync(int id)
    {
        await _veiculoService.DeletarAsync(id);
        return Success();
    }

    [HttpGet]
    public async Task<IActionResult> ObterAsync()
    {
        var veiculo = await _veiculoService.ObterAsync();
        return Success(veiculo);
    }

    [HttpGet("motorista/{veiculoId}/rota/{rotaId}")]
    public async Task<IActionResult> ObterAsync(int veiculoId, int rotaId)
    {
        var veiculo = await _veiculoService.ObterAsync(veiculoId, rotaId);
        return Success(veiculo);
    }
}