using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.Models;

public class OrdemTrajeto : Entity
{
    public int RotaId { get; set; }
    public bool GeradoAutomaticamente { get; set; }
    //
    public virtual List<OrdemTrajetoMarcador> Marcadores { get; set; }
    public virtual Rota Rota { get; set; }

    public void SetDeletado()
    {
        Status = StatusEntityEnum.Deletado;
        Marcadores?.ForEach(x => x.Status = StatusEntityEnum.Deletado);
    }
}