using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Utils;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Configuration;

namespace Routes.Application.Implementations;

public class GoogleDirectionsService(
    IHttpClientFactory httpClientFactory,
    SecretManager _secretManager
) : IGoogleDirectionsService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("api-googlemaps");
    public async Task<Marcador> ObterMarcadorAsync(string endereco)
    {
        var requestUri = $"{_secretManager.Google.BaseUrl}/geocode/json?address={Uri.EscapeDataString(endereco)}&key={_secretManager.Google.Key}";
        var geocodeResponse = await _httpClient.GetFromJsonAsync<GoogleGeocodeResponse>(requestUri);

        // Verifica se a resposta tem sucesso (status OK)
        if (geocodeResponse?.Status != "OK")
        {
            Console.WriteLine($"Erro na resposta da API: {geocodeResponse?.Status}");
            return null;
        }

        // Verifica se 'Results' contém itens
        if (geocodeResponse?.Results == null || !geocodeResponse.Results.Any())
        {
            Console.WriteLine("Nenhum resultado encontrado.");
            return null;
        }

        // Retorna o primeiro marcador encontrado
        return geocodeResponse.Results
            .Select(x => new Marcador { Latitude = x.Geometry.Location.Lat, Longitude = x.Geometry.Location.Lng })
            .FirstOrDefault();
    }

    public async Task<BaseResponse<List<Marcador>>> ObterRotaIdealAsync(Marcador origem, Marcador destino, List<Marcador> pontosIntermediarios)
    {
        if (pontosIntermediarios == null || pontosIntermediarios.Count == 0)
            throw new ArgumentException("Precisa de pelo menos 1 waypoint para otimizar.");

        var pontosOrdenados = pontosIntermediarios.OrdenarComDependencias();
        var waypointsString = string.Join("|", pontosOrdenados.Select(p => p.ToString()));

        var url = $"https://maps.googleapis.com/maps/api/directions/json?" +
                  $"origin={origem}&destination={destino}&waypoints={Uri.EscapeDataString(waypointsString)}&key={_secretManager.Google.Key}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<GoogleDirectionsResponse>(json);

        if (data?.Routes?.FirstOrDefault() is not { } route)
            throw new Exception("Não foi possível encontrar rota.");

        var rotaFinal = new List<Marcador> { origem };
        rotaFinal.AddRange(pontosOrdenados);
        rotaFinal.Add(destino);

        return new BaseResponse<List<Marcador>>
        {
            Data = rotaFinal,
            Sucesso = true,
            Mensagem = "Rota ideal obtida com sucesso."
        };
    }
}