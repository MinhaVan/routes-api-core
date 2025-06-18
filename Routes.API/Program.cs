using System;
using System.IO;
using AspNetCoreRateLimit;
using Routes.API.Extensions;
// using Routes.Service.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prometheus;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Routes.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

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
Console.WriteLine($"Secret: '{JsonConvert.SerializeObject(secretManager)}'");

// Configura os serviços
builder.Services.AddCustomAuthentication(secretManager)
                .AddCustomAuthorization()
                .AddCustomDbContext(secretManager)
                .AddCustomSwagger()
                .AddCustomRateLimiting(secretManager)
                .AddCustomResponseCompression()
                .AddCustomCors()
                .AddCustomServices()
                .AddCache(secretManager)
                .AddCustomRepository()
                .AddQueue(secretManager)
                .AddCustomMapper()
                .AddControllersWithFilters(secretManager)
                .AddCustomHttp(secretManager);

// Configura o logger
builder.Logging.ClearProviders().AddConsole().AddDebug();

#if DEBUG
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(12000); // aceita conexões externas na porta 12000
});
#endif

var app = builder.Build();

// Configurações específicas para desenvolvimento
if (environment == "local")
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Routes.API v1"));
}
else
{
    using (var scope = app.Services.CreateScope())
    {
        Console.WriteLine($"Rodando migrations '{environment}'");
        var db = scope.ServiceProvider.GetRequiredService<APIContext>();
        db.Database.Migrate();
        Console.WriteLine($"Migrations '{environment}' executadas com sucesso");
    }

    app.UsePathBase("/routes");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/routes/swagger/v1/swagger.json", "Routes.API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseResponseCompression();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
// app.MapHub<RotaHub>("v1/Websocket/Rotas");
app.MapMetrics();
app.MapControllers();

Console.WriteLine("Configuração de API finalizada com sucesso!");

app.Run();