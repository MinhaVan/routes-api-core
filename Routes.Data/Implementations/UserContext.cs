using System;
using System.Linq;
using Routes.Domain.Interfaces.Repository;
using Microsoft.AspNetCore.Http;

namespace Routes.Data.Implementations;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Token
    {
        get
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                // Tenta pegar do header Authorization (HTTP padrão)
                var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    return authorizationHeader.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                }

                // Tenta pegar da query string (caso WebSocket ou fallback)
                var queryToken = httpContext.Request.Query["access_token"].FirstOrDefault();
                if (!string.IsNullOrEmpty(queryToken))
                {
                    return queryToken;
                }

                return null;
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException("Erro ao acessar o token.");
            }
        }
    }


    public int Empresa
    {
        get
        {
            try
            {
                var claims = _httpContextAccessor.HttpContext.User.Claims;
                var empresaClaim = claims.FirstOrDefault(c => c.Type == "Empresa");
                return int.Parse(empresaClaim.Value);
            }
            catch (System.Exception)
            {
                throw new UnauthorizedAccessException("Erro ao acessar a empresa do token.");
            }
        }
    }

    public int UserId
    {
        get
        {
            try
            {
                var claims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = claims.FirstOrDefault(c => c.Type == "UserId");
                return int.Parse(userId.Value);
            }
            catch (System.Exception)
            {
                throw new UnauthorizedAccessException("rro ao acessar o usuário do token.");
            }
        }
    }
}
