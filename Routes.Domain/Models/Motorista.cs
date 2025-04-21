using System;
using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.Models;

public class Motorista : Entity
{
    public int UsuarioId { get; set; }
    public string CNH { get; set; }
    public DateTime Vencimento { get; set; }
    public TipoCNHEnum TipoCNH { get; set; }
    public string Foto { get; set; }
    //
    public virtual Usuario Usuario { get; set; }
    public virtual List<MotoristaRota> MotoristaRotas { get; set; }
}