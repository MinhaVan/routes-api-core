namespace Routes.Domain.ViewModels.WebSocket;

public class EnviarLocalizacaoWebSocketResponse
{
    public EnviarLocalizacaoWebSocketResponse() { }
    public EnviarLocalizacaoWebSocketResponse(double latitude, double longitude, int rotaId, int proximoAlunoId, double destinoLatitude, double destinoLongitude)
    {
        Latitude = latitude;
        RotaId = rotaId;
        Longitude = longitude;
        Destino = new DestinoWebSocketRequest
        {
            ProximoAlunoId = proximoAlunoId,
            Latitude = destinoLatitude,
            Longitude = destinoLongitude
        };
    }
    public EnviarLocalizacaoWebSocketResponse(double latitude, double longitude, int rotaId, int proximoAlunoId, double destinoLatitude, double destinoLongitude, string tipoMensagem)
        : this(latitude, longitude, rotaId, proximoAlunoId, destinoLatitude, destinoLongitude)
    {
        TipoMensagem = tipoMensagem;
    }

    public string TipoMensagem { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RotaId { get; set; }
    public DestinoWebSocketRequest Destino { get; set; }
}