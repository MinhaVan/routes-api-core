using System;
using System.Collections.Generic;
using Routes.Domain.Enums;
using Routes.Domain.ViewModels.Motorista;

namespace Routes.Domain.ViewModels;

public class MotoristaViewModel : UsuarioViewModel
{
    public string CNH { get; set; }
    public DateTime Vencimento { get; set; }
    public TipoCNHEnum TipoCNH { get; set; }
    public string Foto { get; set; }
    public List<MotoristaRotaViewModel> MotoristaRotas { get; set; } = new();
}