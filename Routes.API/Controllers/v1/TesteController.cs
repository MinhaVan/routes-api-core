using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Routes.Domain.ViewModels;
using Routes.Domain.Interfaces.Repositories;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
// [Authorize("Bearer")]
public class TesteController(IRabbitMqRepository rabbitMq) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> AdicionarAsync([FromBody] object data)
    {
        rabbitMq.Publish("teste", data);
        return Ok();
    }
}