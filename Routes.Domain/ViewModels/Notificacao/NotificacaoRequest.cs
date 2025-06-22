using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels.Notificacao;

public class NotificacaoRequest
{
    public TipoNotificacaoEnum TipoNotificacao { get; set; }
    public TipoContatoNotificacaoEnum TipoContatoNotificacao { get; set; }
    public string Data { get; set; } = string.Empty;
    public List<string> Destinos { get; set; } = new();
    public string Assunto { get; set; } = string.Empty;
}