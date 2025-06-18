using System;
using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels;

public class RotaViewModel
{
    public int Id { get; set; }
    public StatusEntityEnum Status { get; set; }
    public int EnderecoId { get; set; }
    public int VeiculoId { get; set; }
    public string Nome { get; set; }
    public bool EmAndamento { get; set; }
    public DiaSemanaEnum DiaSemana { get; set; }
    public TimeOnly Horario { get; set; }
    public TipoRotaEnum TipoRota { get; set; }
    //
    public virtual VeiculoViewModel Veiculo { get; set; }
    public virtual List<AlunoRotaViewModel> AlunoRotas { get; set; }
    public virtual List<RotaHistoricoViewModel> Historicos { get; set; }
}