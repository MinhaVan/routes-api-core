using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Routes.Domain.ViewModels;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class AlunoRotaController : BaseController
{
    private readonly IAlunoRotaService _alunoRotaService;
    public AlunoRotaController(IAlunoRotaService alunoRotaService)
    {
        _alunoRotaService = alunoRotaService;
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarAsync([FromBody] AlunoRotaViewModel alunoRota)
    {
        await _alunoRotaService.AdicionarAsync(alunoRota);
        return Success();
    }

    [HttpPut]
    public async Task<IActionResult> AtualizarAsync([FromBody] AlunoRotaViewModel alunoRota)
    {
        await _alunoRotaService.AtualizarAsync(alunoRota);
        return Success();
    }

    [HttpGet]
    public async Task<IActionResult> ObterRotasPorAlunoAsync([FromQuery] int rotaId, [FromQuery] int? alunoId = null)
    {
        var response = await _alunoRotaService.ObterRotasPorAlunoAsync(rotaId, alunoId);
        return Success(response);
    }

    [HttpPut("Desvincular")]
    public async Task<IActionResult> DesvincularAsync(int rotaId, int alunoId)
    {
        await _alunoRotaService.DesvincularRotaAsync(rotaId, alunoId);
        return Success();
    }

    [HttpPut("Vincular")]
    public async Task<IActionResult> VincularAsync(int rotaId, int alunoId)
    {
        await _alunoRotaService.VincularRotaAsync(rotaId, alunoId);
        return Success();
    }
}