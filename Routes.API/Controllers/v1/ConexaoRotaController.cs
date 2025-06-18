using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Utils;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.WebSocket;

namespace Routes.API.Controllers.v1;

[ApiController]
[Authorize("Bearer")]
[Route("v1/[controller]")]
public class ConexaoRotaController(
    ILogger<ConexaoRotaController> _logger,
    IRabbitMqRepository _rabbitMqRepository,
    IRedisRepository _redisRepository) : BaseController
{
    [HttpPost("Localizacao")]
    public async Task<IActionResult> EnviarLocalizacaoAsync([FromBody] EnviarLocalizacaoWebSocketRequest data)
    {
        if (data is null)
            return ObterRespostaErro("Dados da localização inválido!");

        var localizacaoNoCache = await _redisRepository.GetAsync<EnviarLocalizacaoWebSocketResponse>(KeyRedis.EnviarLocalizacao(data.RotaId));
        if (localizacaoNoCache is not null && localizacaoNoCache.TipoMensagem == "finalizarCorrida")
            return Success();

        var response = new BaseResponse<EnviarLocalizacaoWebSocketResponse>
        {
            Data = new EnviarLocalizacaoWebSocketResponse(
                data.Latitude,
                data.Longitude,
                data.RotaId,
                data.AlunoId,
                data.Destino.Latitude,
                data.Destino.Longitude,
                data.TipoMensagem
            ),
            Mensagem = "Localização recebida com sucesso!",
            Sucesso = true
        };

        var mensagem = new BaseQueue<EnviarLocalizacaoWebSocketResponse>
        {
            Mensagem = response.Data,
            Retry = 0
        };

        _rabbitMqRepository.Publish(RabbitMqQueues.EnviarLocalizacao, mensagem, shouldThrowException: false);

        await _redisRepository.SetAsync(KeyRedis.EnviarLocalizacao(data.RotaId), response.Data);
        return Success();
    }

    [HttpGet("Responsavel/{responsavelId}/Rota/{rotaId}")]
    public async Task<IActionResult> ObterLocalizacaoAsync([FromRoute] int responsavelId, [FromRoute] int rotaId)
    {
        if (responsavelId <= 0 || rotaId <= 0)
            return ObterRespostaErro("IDs inválidos para o responsável ou rota.");

        try
        {
            var localizacaoCache = await _redisRepository.GetAsync<EnviarLocalizacaoWebSocketResponse>(KeyRedis.EnviarLocalizacao(rotaId));
            var mensagem = localizacaoCache is null ? "Nenhuma localização disponível no momento." : "Localização recebida com sucesso!";

            return Success(localizacaoCache, mensagem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar o responsável {ResponsavelId} na rota {RotaId}", responsavelId, rotaId);
            return ObterRespostaErro("Ocorreu um erro ao tentar receber os dados da localização!");
        }
    }

    private IActionResult ObterRespostaErro(string mensagem)
    {
        return Success(new BaseResponse<EnviarLocalizacaoWebSocketResponse>
        {
            Data = null,
            Mensagem = mensagem,
            Sucesso = false
        });
    }
}