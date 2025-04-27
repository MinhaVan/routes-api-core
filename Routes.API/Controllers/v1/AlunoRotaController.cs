using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public async Task<IActionResult> ObterRotasPorAlunoAsync(int rotaId, int alunoId)
    {
        await _alunoRotaService.ObterRotasPorAlunoAsync(rotaId, alunoId);
        return Success();
    }

    [HttpPut("desvincular")]
    public async Task<IActionResult> DesvincularAsync(int rotaId, int alunoId)
    {
        await _alunoRotaService.DesvincularRotaAsync(rotaId, alunoId);
        return Success();
    }

    [HttpPut("vincular")]
    public async Task<IActionResult> VincularAsync(int rotaId, int alunoId)
    {
        await _alunoRotaService.VincularRotaAsync(rotaId, alunoId);
        return Success();
    }
}