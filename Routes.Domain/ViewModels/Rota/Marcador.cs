using System.Collections.Generic;
using Routes.Domain.Enums;
using System.Globalization;
using System;

namespace Routes.Domain.ViewModels.Rota;

public class Marcador
{
    public int? Id { get; set; }
    public Guid IdTemporario { get; set; } = Guid.NewGuid();
    public int Ordem { get; set; }
    public int? EnderecoId { get; set; }
    public TipoMarcadorEnum TipoMarcador { get; set; }
    public string Titulo { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<Guid> Prerequisitos { get; set; } = new();
    public virtual AlunoViewModel Aluno { get; set; }
    public virtual List<AlunoViewModel> Alunos { get; set; } = new();

    public override string ToString() => $"{Latitude.ToString(CultureInfo.InvariantCulture)},{Longitude.ToString(CultureInfo.InvariantCulture)}";
}