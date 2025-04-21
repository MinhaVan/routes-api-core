using System.Collections.Generic;

namespace Routes.Domain.Models;

public class AlunoRota : Entity
{
    public int AlunoId { get; set; }
    public int RotaId { get; set; }
    //
    public virtual Aluno Aluno { get; set; }
    public virtual Rota Rota { get; set; }
    public virtual List<AjusteAlunoRota> AjusteAlunoRotas { get; set; }
}