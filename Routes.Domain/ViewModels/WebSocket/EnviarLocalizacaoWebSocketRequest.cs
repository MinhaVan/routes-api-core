namespace Routes.Domain.ViewModels.WebSocket;

public class EnviarLocalizacaoWebSocketRequest
{
    public string TipoMensagem { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RotaId { get; set; }
    public int AlunoId { get; set; }
    public DestinoWebSocketRequest Destino { get; set; }
}
