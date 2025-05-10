using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class EnderecoController : BaseController
{
    private readonly IEnderecoService _enderecoService;

    public EnderecoController(IEnderecoService enderecoService)
    {
        _enderecoService = enderecoService;
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarAsync([FromBody] EnderecoAdicionarViewModel enderecoAdicionarViewModel)
    {
        await _enderecoService.AdicionarAsync(enderecoAdicionarViewModel);
        return Success();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarAsync([FromRoute] int id, [FromBody] EnderecoAtualizarViewModel enderecoAtualizarViewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        enderecoAtualizarViewModel.Id = id;
        await _enderecoService.AtualizarAsync(enderecoAtualizarViewModel);
        return Success();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarAsync(int id)
    {
        await _enderecoService.DeletarAsync(id);
        return Success();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterAsync(int id)
    {
        var endereco = await _enderecoService.Obter(id);
        return Success(endereco);
    }

    [HttpGet("Lista/{ids}")]
    public async Task<IActionResult> ObterAsync(List<int> ids)
    {
        var enderecos = await _enderecoService.Obter(ids);
        return Success(enderecos);
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodosEnderecosAsync()
    {
        var enderecos = await _enderecoService.Obter();
        return Success(enderecos);
    }
}