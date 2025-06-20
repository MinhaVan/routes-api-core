using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.ViewModels.Rota;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Routes.API.Controllers.v1;

[ApiController]
[Route("v1/[controller]")]
[Authorize("Bearer")]
public class TrajetoController(
    ITrajetoService trajetoService,
    IGestaoTrajetoService gestaoTrajetoService,
    IRelatorioTrajetoService relatorioTrajetoService,
    IRotaOnlineService rotaOnlineService,
    IMarcadorService marcadorService,
    IOrdemTrajetoService ordemTrajetoService) : BaseController
{
    private readonly IGestaoTrajetoService _gestaoTrajetoService = gestaoTrajetoService;
    private readonly IRelatorioTrajetoService _relatorioTrajetoService = relatorioTrajetoService;
    private readonly IRotaOnlineService _rotaOnlineService = rotaOnlineService;
    private readonly ITrajetoService _trajetoService = trajetoService;
    private readonly IMarcadorService _marcadorService = marcadorService;
    private readonly IOrdemTrajetoService _ordemTrajetoService = ordemTrajetoService;

    /// <summary>
    /// Gera o melhor trajeto para a rota especificada.
    /// Este método calcula o trajeto ideal com base nos marcadores associados à rota.
    /// O trajeto é otimizado para minimizar a distância e o tempo de viagem, considerando as coordenadas geográficas dos marcadores.
    /// O resultado é salvo no banco de dados e pode ser utilizado para planejamento de rotas.
    /// </summary>
    /// <param name="rotaId">Identificador da Rota</param>
    /// <returns></returns>
    [HttpPost("Rota/{rotaId}/GerarMelhorTrajeto")]
    public async Task<IActionResult> GerarMelhorTrajetoAsync(int rotaId)
    {
        await _trajetoService.GerarMelhorTrajetoAsync(rotaId);
        return Success();
    }

    [HttpGet("Rota/{rotaId}/Destino")]
    public async Task<IActionResult> ObterDestinoAsync([FromRoute] int rotaId, [FromQuery] bool validarRotaOnline)
    {
        var rota = await _trajetoService.ObterDestinoAsync(rotaId, validarRotaOnline);
        return Success(rota);
    }

    /// <summary>
    /// Salva a ordem do trajeto para a rota especificada (Feita manualmente pelo usuário).
    /// Este método permite que o usuário defina a ordem dos marcadores no trajeto de uma rota específica.
    /// A ordem é salva no banco de dados e pode ser utilizada para otimização de rotas.
    /// </summary>
    /// <param name="rotaId">Identificador da rota</param>
    /// <param name="body">Marcadores otimizados pelo usuário</param>
    /// <returns>Objeto informando sucesso/falha</returns>
    [HttpPost("Rota/{rotaId}")]
    public async Task<IActionResult> SalvarOrdemDoTrajetoAsync([FromRoute] int rotaId, [FromBody] List<Marcador> body)
    {
        await _ordemTrajetoService.SalvarOrdemDoTrajetoAsync(rotaId, body);
        return Success();
    }

    /// <summary>
    /// Obtém todos os marcadores associados a uma rota específica.
    /// Este método retorna uma lista de marcadores que estão associados à rota especificada.
    /// Os marcadores podem incluir pontos de interesse, paradas ou outros locais relevantes para a rota.
    /// A lista é utilizada para visualização e planejamento de rotas.
    /// </summary>
    /// <param name="rotaId">Identificador da rota</param>
    /// <returns>Lista de marcadores</returns>
    [HttpGet("Rota/{rotaId}/Marcadores")]
    public async Task<IActionResult> ObterTodosMarcadoresParaRotasAsync([FromRoute] int rotaId)
    {
        var response = await _marcadorService.ObterTodosMarcadoresParaRotasAsync(rotaId);
        return Success(response);
    }

    [HttpGet("Rota/{rotaId}")]
    public async Task<IActionResult> ObterTrajetoEmAndamentoAsync([FromRoute] int rotaId)
    {
        var response = await _trajetoService.ObterTrajetoEmAndamentoAsync(rotaId);
        return Success(response);
    }

    [HttpPost("Rota/{rotaId}/iniciar")]
    public async Task<IActionResult> IniciarTrajetoAsync([FromRoute] int rotaId)
    {
        await _gestaoTrajetoService.IniciarTrajetoAsync(rotaId);
        return Success();
    }

    [HttpPost("Rota/{rotaId}/finalizar")]
    public async Task<IActionResult> FinalizarTrajetoAsync([FromRoute] int rotaId)
    {
        await _gestaoTrajetoService.FinalizarTrajetoAsync(rotaId);
        return Success();
    }

    /// <summary>
    /// Atualiza o status de um aluno no trajeto da rota especificada.
    /// Este método permite marcar se um aluno entrou ou não na van durante o trajeto.
    /// O status é atualizado no banco de dados e pode ser utilizado para monitoramento do trajeto
    /// </summary>
    /// <param name="alunoId">Identificador do aluno</param>
    /// <param name="rotaId">Identificador da rota</param>
    /// <param name="alunoEntrouNaVan">Status informando se aluno entrou ou não na van</param>
    /// <returns>Objeto HTTP com sucesso/falha</returns>
    [HttpPut("rota/{rotaId}/aluno/{alunoId}/{alunoEntrouNaVan}")]
    public async Task<IActionResult> AtualizarStatusAlunoTrajetoAsync([FromRoute] int alunoId, [FromRoute] int rotaId, [FromRoute] bool alunoEntrouNaVan)
    {
        await _trajetoService.AtualizarStatusAlunoTrajetoAsync(alunoId, rotaId, alunoEntrouNaVan);
        return Success();
    }

    /// <summary>
    /// Gera um relatório do último trajeto realizado para a rota especificada.
    /// Este método é utilizado ao final de um trajeto para compilar informações sobre o percurso,
    /// incluindo pontos de partida, chegada e quaisquer paradas intermediárias.
    /// O relatório pode incluir detalhes como distância percorrida, tempo gasto e status dos alunos durante o trajeto.
    /// O resultado é utilizado para análise e melhoria dos trajetos futuros.
    /// </summary>
    /// <param name="rotaId"></param>
    /// <returns></returns>
    [HttpGet("Relatorio/Rota/{rotaId}")]
    public async Task<IActionResult> RelatorioTrajetoAsync([FromRoute] int rotaId)
    {
        var response = await _relatorioTrajetoService.RelatorioUltimoTrajetoAsync(rotaId);
        return Success(response);
    }

    [HttpGet("Rota/Online")]
    public async Task<IActionResult> RotaOnlineAsync()
    {
        return Success(await _rotaOnlineService.RotaOnlineParaMotoristaAsync());
    }
}