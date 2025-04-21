namespace Routes.Domain.Models;

public class UsuarioPermissao : Entity
{
    public int UsuarioId { get; set; }
    public int PermissaoId { get; set; }
    // 
    public virtual Usuario Usuario { get; set; }
    public virtual Permissao Permissao { get; set; }
}