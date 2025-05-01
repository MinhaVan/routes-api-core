using System;
using System.IO;
using AspNetCoreRateLimit;
using Routes.API.Extensions;
using Routes.Service.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Iniciando a API no ambiente '{environment}'");

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Adiciona as configurações do Secrets Manager
var secretManager = builder.Services.AddSecretManager(builder.Configuration);

// Configura os serviços
builder.Services.AddCustomAuthentication(secretManager)
                .AddCustomAuthorization()
                .AddCustomDbContext(secretManager)
                .AddCustomSwagger()
                .AddCustomRateLimiting(secretManager)
                .AddCustomResponseCompression()
                .AddCustomCors()
                .AddCustomServices(secretManager)
                .AddCustomRepository(secretManager)
                .AddCustomMapper()
                .AddControllersWithFilters()
                .AddCustomHttp(secretManager);

// Configura o logger
builder.Logging.ClearProviders().AddConsole().AddDebug();

var app = builder.Build();

// Configurações específicas para desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configurações gerais
#if !DEBUG
app.UsePathBase("/routes");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/routes/swagger/v1/swagger.json", "Routes.API v1");
    c.RoutePrefix = "swagger";
});
#else
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Routes.API v1"));
#endif

app.UseResponseCompression();
app.UseRouting();
app.MapHub<RotaHub>("websocket/rotas");
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.MapMetrics();
app.UseCors("CorsPolicy");
app.UseWebSockets();
app.MapControllers();

Console.WriteLine("Configuração de API finalizada com sucesso!");

app.Run();