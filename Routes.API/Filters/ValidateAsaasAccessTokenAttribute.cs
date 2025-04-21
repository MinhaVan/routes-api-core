using Routes.Service.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Routes.API.Filters;

public class ValidateAsaasAccessTokenAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var secretManager = context.HttpContext.RequestServices.GetRequiredService<SecretManager>();

        context.HttpContext.Request.Headers.TryGetValue("asaas-access-token", out var token);
        if (token != secretManager.Asaas.TokenWebHookAsaas)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}


