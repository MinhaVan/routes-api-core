using System.Collections.Generic;
using Routes.Domain.ViewModels;
using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels.Rota;

public class Marcador
{
    public int EnderecoId { get; set; }
    public TipoMarcadorEnum TipoMarcador { get; set; }
    public string Titulo { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public virtual AlunoViewModel Aluno { get; set; }
    public virtual List<AlunoViewModel> Alunos { get; set; } = new();
}