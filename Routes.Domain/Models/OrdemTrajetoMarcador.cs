using Routes.Domain.Enums;

namespace Routes.Domain.Models;

public class OrdemTrajetoMarcador : Entity
{
    public TipoMarcadorEnum TipoMarcador { get; set; }
    public int OrdemTrajetoId { get; set; }
    public int EnderecoId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    //
    public virtual OrdemTrajeto OrdemTrajeto { get; set; }
}