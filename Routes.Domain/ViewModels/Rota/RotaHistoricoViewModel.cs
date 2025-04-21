using System;
using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels;

public class RotaHistoricoViewModel
{
    public int Id { get; set; }
    public int RotaId { get; set; }
    public DateTime DataRealizacao { get; set; }
    public DateTime? DataFim { get; set; }
    public bool EmAndamento { get; set; }
    public virtual Rota2ViewModel? Rota { get; set; }
}

public class Rota2ViewModel
{
    public int Id { get; set; }
    public int EnderecoId { get; set; }
    public int VeiculoId { get; set; }
    public string Nome { get; set; }
    public DiaSemanaEnum DiaSemana { get; set; }
    public TimeOnly Horario { get; set; }
    public TipoRotaEnum TipoRota { get; set; }
    //
    public virtual List<AlunoRotaViewModel> AlunoRotas { get; set; }
}