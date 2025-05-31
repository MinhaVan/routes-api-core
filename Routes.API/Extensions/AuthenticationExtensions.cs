using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Routes.Service.Configuration;
using System.Threading.Tasks;

namespace Routes.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, SecretManager secretManager)
    {
        var tokenConfigurations = secretManager.TokenConfigurations;

        services.AddSingleton(tokenConfigurations);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenConfigurations.Issuer,
                ValidAudience = tokenConfigurations.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    Console.WriteLine($"Query string: {context.Request.QueryString}");

                    var path = context.HttpContext.Request.Path.Value;
                    Console.WriteLine($"Path: {path}");

                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWith("/v1/Websocket/Rotas", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        Console.WriteLine("Configuração da autenticação realizada com sucesso!");

        return services;
    }
}
