using System;
using Routes.Domain.Interfaces.APIs;
using Routes.Service.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Routes.API.Extensions;

public static class HttpExtensions
{
    public static IServiceCollection AddCustomHttp(this IServiceCollection services, SecretManager secretManager)
    {
        var url = secretManager.Asaas.Url;
        var asaasToken = secretManager.Asaas.AcessToken;
        services.AddHttpClient("api-asaas", client =>
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Add("User-Agent", "VanCoreAPI");
            client.DefaultRequestHeaders.Add("access_token", asaasToken);
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

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(secretManager.URL.AuthAPI));

        Console.WriteLine("Configuração das APIs consumidas realizada com sucesso!");

        return services;
    }
}
