using System;
using Routes.Service.Implementations;
using Routes.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Routes.API.Filters;
using Routes.API.Converters;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using Routes.Service.Configuration;
using Routes.Application.Implementations;

namespace Routes.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton<ILocalizacaoCache, LocalizacaoCacheEmMemoria>();
        services.AddScoped<IEnderecoService, EnderecoService>();
        services.AddScoped<IRotaService, RotaService>();
        services.AddScoped<ITrajetoService, TrajetoService>();
        services.AddScoped<IGoogleDirectionsService, GoogleDirectionsService>();
        services.AddScoped<IAjusteEnderecoService, AjusteEnderecoService>();
        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<IAlunoRotaService, AlunoRotaService>();
        services.AddScoped<IMotoristaRotaService, MotoristaRotaService>();
        services.AddScoped<IGestaoTrajetoService, GestaoTrajetoService>();
        services.AddScoped<IMarcadorService, MarcadorService>();
        services.AddScoped<IOrdemTrajetoService, OrdemTrajetoService>();
        services.AddScoped<IRelatorioTrajetoService, RelatorioTrajetoService>();
        services.AddScoped<IRotaOnlineService, RotaOnlineService>();

        Console.WriteLine("Configuração das services realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddCustomMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }

    public static IServiceCollection AddControllersWithFilters(this IServiceCollection services, SecretManager secretManager)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
        });

        services.AddSignalR()
            .AddStackExchangeRedis(secretManager.Infra.Redis, options =>
            {
                options.Configuration.ChannelPrefix = "Websockets";
            });

        services.AddHttpClient();

        services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}