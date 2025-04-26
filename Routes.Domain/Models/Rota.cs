using System;
using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.Models;

public class Rota : Entity
{
    public string Nome { get; set; }
    public int EmpresaId { get; set; }
    public int? VeiculoId { get; set; }
    public DiaSemanaEnum DiaSemana { get; set; }
    public TimeOnly Horario { get; set; }
    public TipoRotaEnum TipoRota { get; set; }
    //
    public virtual List<RotaHistorico> Historicos { get; set; }
    public virtual List<AlunoRota> AlunoRotas { get; set; }
    public virtual List<MotoristaRota> MotoristaRotas { get; set; }
    public virtual List<OrdemTrajeto> OrdemTrajetos { get; set; }
    public virtual Veiculo Veiculo { get; set; }
}