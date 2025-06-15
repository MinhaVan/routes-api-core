using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Routes.API.Filters;

public class ApiKeyAuthAttribute : Attribute, IAsyncAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-Consumer-Key";
    private const string ExpectedKey = "<ue@pZV@Y|{^]_<rJ<r8LygZoGH;wAu|";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var hasHeader = context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey);

        if (!hasHeader || extractedApiKey != ExpectedKey)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
