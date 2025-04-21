using System;
using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels;

public class MotoristaViewModel : UsuarioViewModel
{
    public string CNH { get; set; }
    public DateTime Vencimento { get; set; }
    public TipoCNHEnum TipoCNH { get; set; }
    public string Foto { get; set; }
}