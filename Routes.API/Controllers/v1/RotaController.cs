using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class RotaController : BaseController
{
    private readonly IRotaService _rotaService;
    private readonly IAjusteEnderecoService _ajusteEnderecoService;

    public RotaController(
        IAjusteEnderecoService ajusteEnderecoService,
        IRotaService rotaService)
    {
        _ajusteEnderecoService = ajusteEnderecoService;
        _rotaService = rotaService;
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarAsync([FromBody] RotaAdicionarViewModel rotaAdicionarViewModel)
    {
        return Success(
            await _rotaService.AdicionarAsync(rotaAdicionarViewModel)
        );
    }

    [HttpPut]
    public async Task<IActionResult> AtualizarAsync([FromBody] RotaAtualizarViewModel rotaAtualizarViewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _rotaService.AtualizarAsync(rotaAtualizarViewModel);
        return Success();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarAsync(int id)
    {
        await _rotaService.DeletarAsync(id);
        return Success();
    }

    [HttpGet("Empresa/{empresaId}")]
    public async Task<IActionResult> ObterTodosAsync(
        [FromRoute] int empresaId,
        [FromQuery] bool incluirDeletados = false,
        [FromQuery] bool incluirDetalhes = false
    )
    {
        var rotas = await _rotaService.ObterTodosAsync(empresaId, incluirDeletados, incluirDetalhes);
        return Success(rotas);
    }

    [HttpGet("ObterRotasDosFilhos")]
    public async Task<IActionResult> ObterRotasDosFilhosAsync()
    {
        var rotas = await _rotaService.ObterRotasDosFilhosAsync();
        return Success(rotas);
    }

    [HttpGet("Online")]
    public async Task<IActionResult> ObterRotasOnlineAsync()
    {
        var rotas = await _rotaService.ObterRotasOnlineAsync();
        return Success(rotas);
    }

    [HttpGet("{rotaId}")]
    public async Task<IActionResult> ObterAsync(int rotaId)
    {
        var rota = await _rotaService.ObterPorRotaIdAsync(rotaId);
        return Success(rota);
    }

    [HttpGet("{id}/detalhe")]
    public async Task<IActionResult> ObterDetalheAsync([FromRoute] int id)
    {
        var rota = await _rotaService.ObterDetalheAsync(id);
        return Success(rota);
    }

    [HttpGet("motorista/{motoristaId}")]
    public async Task<IActionResult> ObterRotaDoMotoristaAsync([FromRoute] int motoristaId, [FromQuery] bool? filtrarApenasHoje)
    {
        var rota = await _rotaService.ObterRotaDoMotoristaAsync(motoristaId, filtrarApenasHoje ?? true);
        return Success(rota);
    }

    [HttpGet("aluno/{id}")]
    public async Task<IActionResult> ObterPorAlunoIdAsync(int id)
    {
        var rotas = await _rotaService.ObterPorAlunoIdAsync(id);
        return Success(rotas);
    }

    #region Ajuste Rota

    [HttpGet("{rotaId}/AjusteRota/Aluno/{alunoId}")]
    public async Task<IActionResult> ObterAjusteEnderecoAsync(int rotaId, int alunoId)
    {
        var response = await _ajusteEnderecoService.ObterAjusteEnderecoAsync(alunoId, rotaId);
        return Success(response);
    }

    [HttpPost("{rotaId}/AjusteRota")]
    public async Task<IActionResult> AdicionarAjusteEnderecoAsync([FromRoute] int rotaId, [FromBody] RotaAdicionarAjusteEnderecoViewModel alterarEnderecoViewModel)
    {
        alterarEnderecoViewModel.RotaId = rotaId;
        await _ajusteEnderecoService.AdicionarAjusteEnderecoAsync(alterarEnderecoViewModel);
        return Success();
    }

    [HttpPut("{rotaId}/AjusteRota/{ajusteRotaId}")]
    public async Task<IActionResult> AlterarAjusteEnderecoAsync([FromRoute] int rotaId, [FromRoute] int ajusteRotaId, [FromBody] RotaAlterarAjusteEnderecoViewModel alterarEnderecoViewModel)
    {
        alterarEnderecoViewModel.RotaId = rotaId;
        alterarEnderecoViewModel.Id = ajusteRotaId;
        await _ajusteEnderecoService.AlterarAjusteEnderecoAsync(alterarEnderecoViewModel);
        return Success();
    }

    #endregion
}