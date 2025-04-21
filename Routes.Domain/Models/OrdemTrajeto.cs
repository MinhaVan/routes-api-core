using System.Collections.Generic;

namespace Routes.Domain.Models;

public class OrdemTrajeto : Entity
{
    public int RotaId { get; set; }
    //
    public virtual List<OrdemTrajetoMarcador> Marcadores { get; set; }
    public virtual Rota Rota { get; set; }
}