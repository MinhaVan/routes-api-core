using System.Text.Json.Serialization;

namespace Routes.Domain.ViewModels.Rota;

public class MarcadorResponse
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longitude { get; set; }
}