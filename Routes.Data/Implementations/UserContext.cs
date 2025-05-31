using System;
using System.Linq;
using Routes.Domain.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Routes.Data.Utils;

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
                Console.WriteLine("Request: " + _httpContextAccessor.HttpContext.Request.ToJson());

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.Request?.Headers != null && httpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
                {
                    var authorizationHeader = authHeaderValues.FirstOrDefault();
                    if (!string.IsNullOrEmpty(authorizationHeader))
                    {
                        return authorizationHeader;
                    }
                }

                if (httpContext?.Request?.Query != null && httpContext.Request.Query.TryGetValue("access_token", out var queryTokenValues))
                {
                    var queryToken = queryTokenValues.FirstOrDefault();
                    if (!string.IsNullOrEmpty(queryToken))
                    {
                        return queryToken;
                    }
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
