namespace Routes.Domain.Models;

public class MotoristaRota : Entity
{
    public int MotoristaId { get; set; }
    public int RotaId { get; set; }
    //
    public virtual Motorista Motorista { get; set; }
    public virtual Rota Rota { get; set; }
}