using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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