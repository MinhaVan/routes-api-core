using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.Models;

public class Veiculo : Entity
{
    public int Ano { get; set; }
    public int AnoModelo { get; set; }
    public string Cor { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Placa { get; set; }
    public TipoVeiculoEnum TipoVeiculo { get; set; }
    public int EmpresaId { get; set; }
    //
    public virtual Empresa Empresa { get; set; }
    public virtual List<Rota> Rotas { get; set; }
}