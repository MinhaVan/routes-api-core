using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using Routes.Domain.Enums;

public class AuthorizeRolesAttribute : ActionFilterAttribute
{
    private readonly PerfilEnum[] _requiredRoles;

    public AuthorizeRolesAttribute(params PerfilEnum[] requiredRoles)
    {
        _requiredRoles = requiredRoles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userRoles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        bool hasRequiredRole = _requiredRoles
            .Select(r => ((int)r).ToString())
            .Any(requiredRole => userRoles.Contains(requiredRole));

        if (!hasRequiredRole)
        {
            context.Result = new ForbidResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}