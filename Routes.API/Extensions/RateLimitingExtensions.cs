using System;
using System.Collections.Generic;
using AspNetCoreRateLimit;
using Routes.Service.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Routes.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services, SecretManager secretManager)
    {
        services.AddOptions();
        services.AddMemoryCache();

        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = secretManager.IpRateLimiting.EnableEndpointRateLimiting;
            options.StackBlockedRequests = secretManager.IpRateLimiting.StackBlockedRequests;
            options.RealIpHeader = secretManager.IpRateLimiting.RealIpHeader;
            options.ClientIdHeader = secretManager.IpRateLimiting.ClientIdHeader;
            options.GeneralRules = MapGeneralRulesToRateLimitRules(secretManager.IpRateLimiting.GeneralRules);
        });

        services.Configure<IpRateLimitPolicy>(options =>
        {
            options.Rules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = secretManager.AuthenticatedRateLimit.Period,
                    Limit = secretManager.AuthenticatedRateLimit.Limit,
                    MonitorMode = false
                }
            };
        });

        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

        Console.WriteLine("Configuração de ratelimit o realizada com sucesso!");

        return services;
    }


    private static List<RateLimitRule> MapGeneralRulesToRateLimitRules(List<GeneralRule> generalRules)
    {
        var rateLimitRules = new List<RateLimitRule>();

        foreach (var rule in generalRules)
        {
            var rateLimitRule = new RateLimitRule
            {
                Endpoint = rule.Endpoint,
                Period = rule.Period,
                Limit = rule.Limit
            };
            rateLimitRules.Add(rateLimitRule);
        }

        return rateLimitRules;
    }
}


