using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels;

public class EnderecoViewModel
{
    public int Id { get; set; }
    public string Complemento { get; set; }
    public string Numero { get; set; }
    public string CEP { get; set; }
    public string Rua { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Pais { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public StatusEntityEnum Status { get; set; }
    public TipoEnderecoEnum TipoEndereco { get; set; }

    public void Sanitizar()
    {
        Rua = Rua.Trim() ?? string.Empty;
        Bairro = Bairro.Trim() ?? string.Empty;
        Complemento = Complemento.Trim() ?? string.Empty;
        Cidade = Cidade.Trim() ?? string.Empty;
        Estado = Estado.Trim() ?? string.Empty;
        CEP = CEP.Trim() ?? string.Empty;
        Pais = Pais.Trim() ?? string.Empty;
    }

    public string ObterEndereco()
    {
        return string.Format("{0}, {1}, {2}, {3}", Rua, Numero, Bairro, Complemento);
    }

    public string ObterEnderecoCompleto()
    {
        return $"{this.Rua ?? string.Empty} {this.Numero ?? string.Empty}, {this.Bairro ?? string.Empty}, {this.Cidade ?? string.Empty}, {this.Estado ?? string.Empty}, {this.CEP ?? string.Empty}, Brazil";
    }
}