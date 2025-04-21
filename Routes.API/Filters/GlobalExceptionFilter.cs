using System;
using System.Collections.Generic;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Routes.API.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is BusinessRuleException businessRuleException)
        {
            _logger.LogWarning(businessRuleException, "Uma regra de negócio foi violada.");

            var response = new BaseResponse<object>
            {
                Sucesso = false,
                Data = null,
                Mensagem = "Uma regra de negócio foi violada.",
                Erros = new List<string> { businessRuleException.Message }
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        else if (context.Exception is UnauthorizedAccessException unauthorizedAccessException)
        {
            _logger.LogError(unauthorizedAccessException, "Usuário não autenticado.");

            var response = new BaseResponse<object>
            {
                Sucesso = false,
                Data = null,
                Mensagem = unauthorizedAccessException.Message,
                Erros = new List<string> { unauthorizedAccessException.Message }
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
        else
        {
            _logger.LogError(context.Exception, "Ocorreu um erro não tratado.");

            var response = new BaseResponse<object>
            {
                Sucesso = false,
                Data = null,
                Mensagem = "Ocorreu um erro ao processar a sua solicitação.",
                Erros = new List<string> { context.Exception.Message }
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        context.ExceptionHandled = true;
    }
}