using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Routes.Domain.ViewModels.Rota;

public class GoogleDirectionsResponse
{
    [JsonPropertyName("routes")]
    public List<Route> Routes { get; set; }

    public class Route
    {
        [JsonPropertyName("waypoint_order")]
        public List<int> WaypointOrder { get; set; }
    }
}