using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class MotoristaController : BaseController
{
    private readonly IMotoristaService _motoristaService;
    private readonly ITrajetoService _trajetoService;
    public MotoristaController(ITrajetoService trajetoService, IMotoristaService motoristaService)
    {
        _trajetoService = trajetoService;
        _motoristaService = motoristaService;
    }

    [HttpPut("desvincular")]
    public async Task<IActionResult> DesvincularAsync([FromBody] MotoristaVincularViewModel request)
    {
        await _motoristaService.DesvincularAsync(request);
        return Success();
    }

    [HttpPut("vincular")]
    public async Task<IActionResult> VincularAsync([FromBody] MotoristaVincularViewModel request)
    {
        await _motoristaService.VincularAsync(request);
        return Success();
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarAsync([FromBody] MotoristaNovoViewModel usuario)
    {
        await _motoristaService.AdicionarAsync(usuario);
        return Success();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarAsync([FromBody] MotoristaAtualizarViewModel usuario)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _motoristaService.AtualizarAsync(usuario);
        return Success();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarAsync(int id)
    {
        // Migrado para a Auth API
        throw new System.Exception("Migrado para a Auth API");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterAsync(int id)
    {
        // Migrado para a Auth API
        throw new System.Exception("Migrado para a Auth API");
    }

    [HttpGet("{pagina}/{tamanho}")]
    public async Task<IActionResult> ObterTodosMotoristasAsync(int pagina, int tamanho)
    {
        // Migrado para a Auth API
        throw new System.Exception("Migrado para a Auth API");
    }

    [HttpGet("rota/online")]
    public async Task<IActionResult> RotaOnlineAsync()
    {
        return Success(await _trajetoService.RotaOnlineParaMotoristaAsync());
    }
}