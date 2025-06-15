namespace Routes.Domain.ViewModels.WebSocket;

public class DestinoWebSocketRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? ProximoAlunoId { get; set; }
}