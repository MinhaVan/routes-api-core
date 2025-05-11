using System;
using Routes.Service.Implementations;
using Routes.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Routes.API.Filters;
using Routes.API.Converters;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using StackExchange.Redis;
using Routes.Service.Configuration;

namespace Routes.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, SecretManager secretManager)
    {
        services.AddHttpContextAccessor();
        services.AddCache(secretManager);

        services.AddScoped<IEnderecoService, EnderecoService>();
        services.AddScoped<IRotaService, RotaService>();
        services.AddScoped<ITrajetoService, TrajetoService>();
        services.AddScoped<IAjusteEnderecoService, AjusteEnderecoService>();
        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<IAlunoRotaService, AlunoRotaService>();
        services.AddScoped<IMotoristaRotaService, MotoristaRotaService>();

        Console.WriteLine("Configuração das services realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddCache(this IServiceCollection services, SecretManager secretManager)
    {
        services.AddSignalR();

        // services.AddSingleton<IConnectionMultiplexer>(sp =>
        // {
        //     var configuration = secretManager.ConnectionStrings.RedisConnection;
        //     return ConnectionMultiplexer.Connect(configuration);
        // });

        Console.WriteLine("Configuração do Redis realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddCustomMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }

    public static IServiceCollection AddControllersWithFilters(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
        });

        services.AddSignalR();
        services.AddHttpClient();

        services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}