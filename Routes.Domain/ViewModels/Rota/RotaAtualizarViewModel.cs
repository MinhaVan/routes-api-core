using System;
using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels;

public class RotaAtualizarViewModel
{
    public int Id { get; set; }
    public int? VeiculoId { get; set; }
    public string Nome { get; set; }
    public bool DeveBuscarRotaNoGoogleMaps { get; set; }
    public StatusEntityEnum Status { get; set; }
    public DiaSemanaEnum DiaSemana { get; set; }
    public TimeOnly Horario { get; set; }
    public TipoRotaEnum TipoRota { get; set; }
}