using System.Collections.Generic;

namespace Routes.Domain.Models;

public class Permissao : Entity
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public int EmpresaId { get; set; }
    public bool EditarPlanos { get; set; }
    public bool EditarVeiculos { get; set; }
    public bool PadraoPassageiros { get; set; }
    public bool PadraoSuporte { get; set; }
    public bool PadraoResponsavel { get; set; }
    public bool PadraoMotorista { get; set; }
    // 
    public virtual IList<UsuarioPermissao> Usuarios { get; set; } = new List<UsuarioPermissao>();
}