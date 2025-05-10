using System;
using Routes.Domain.Interfaces.APIs;
using Routes.Service.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Routes.Data.APIs;

namespace Routes.API.Extensions;

public static class HttpExtensions
{
    public static IServiceCollection AddCustomHttp(this IServiceCollection services, SecretManager secretManager)
    {
        services.AddHttpClient("api-asaas", client =>
        {
            client.BaseAddress = new Uri(secretManager.Asaas.Url);
            client.DefaultRequestHeaders.Add("User-Agent", "VanCoreAPI");
            client.DefaultRequestHeaders.Add("access_token", secretManager.Asaas.AcessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddHttpClient("api-nominatim", client =>
        {
            client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddHttpClient("api-googlemaps", client =>
        {
            client.BaseAddress = new Uri(secretManager.Google.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddHttpClient("api-pessoas", client =>
        {
            client.BaseAddress = new Uri(secretManager.URL.PessoasAPI);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddHttpClient("api-auth", client =>
        {
            client.BaseAddress = new Uri(secretManager.URL.AuthAPI);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddScoped<IPessoasAPI, PessoasAPI>();
        services.AddScoped<IAuthApi, AuthAPI>();

        Console.WriteLine("Configuração das APIs consumidas realizada com sucesso!");

        return services;
    }
}
