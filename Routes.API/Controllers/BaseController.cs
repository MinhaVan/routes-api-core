using Microsoft.AspNetCore.Mvc;
using Routes.Domain.ViewModels;
using System.Collections.Generic;

namespace Routes.API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected ActionResult Default(int statusCode, string message, bool sucesso, object data = null, List<string> errors = null)
    {
        var response = new BaseResponse<object>
        {
            Sucesso = sucesso,
            Data = data,
            Mensagem = message,
            Erros = errors ?? new List<string>()
        };
        return StatusCode(statusCode, response);
    }

    protected ActionResult Success(string message = null) => this.Success(new { }, message);
    protected ActionResult Success<T>(T data, string message = "Operação realizada com sucesso")
    {
        var response = new BaseResponse<T>
        {
            Sucesso = true,
            Data = data,
            Mensagem = message,
            Erros = null
        };
        return Ok(response);
    }

    protected ActionResult Error(string message, List<string> errors = null)
    {
        var response = new BaseResponse<object>
        {
            Sucesso = false,
            Data = null,
            Mensagem = message,
            Erros = errors ?? new List<string>()
        };
        return BadRequest(response);
    }
}
