using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Routes.API.Filters;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels.WebSocket;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
public class LocalizacaoTrajetoController(
    ILocalizacaoTrajetoService _localizacaoTrajetoService
) : BaseController
{
    [HttpPost]
    [ApiKeyAuth]
    public async Task<IActionResult> AdicionarAsync([FromBody] EnviarLocalizacaoWebSocketResponse localizacao)
    {
        await _localizacaoTrajetoService.AdicionarLocalizacaoTrajetoAsync(localizacao);
        return Success();
    }
}