using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class MotoristaRotaController : BaseController
{
    private readonly IMotoristaRotaService _motoristaRotaService;
    public MotoristaRotaController(IMotoristaRotaService motoristaRotaService)
    {
        _motoristaRotaService = motoristaRotaService;
    }

    [HttpGet("Motorista/Rota/{rotaId}")]
    public async Task<IActionResult> BuscarMotoristaPorRotaAsync([FromRoute] int rotaId)
    {
        MotoristaViewModel motorista = await _motoristaRotaService.BuscarMotoristaPorRotaAsync(rotaId);
        return Success(motorista);
    }

    [HttpPut("Vincular")]
    public async Task<IActionResult> VincularAsync([FromBody] MotoristaVincularViewModel request)
    {
        await _motoristaRotaService.VincularAsync(request);
        return Success();
    }

    [HttpPut("Desvincular")]
    public async Task<IActionResult> DesvincularAsync([FromBody] MotoristaVincularViewModel request)
    {
        await _motoristaRotaService.DesvincularAsync(request);
        return Success();
    }
}