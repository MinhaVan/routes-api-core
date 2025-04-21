using Routes.Domain.Enums;

namespace Routes.Domain.Models;

public class EnderecoAdicionarViewModel
{
    public string Complemento { get; set; }
    public string Numero { get; set; }
    public string CEP { get; set; }
    public string Rua { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Pais { get; set; }
    private TipoEnderecoEnum? tipoEndereco;
    public TipoEnderecoEnum? TipoEndereco
    {
        get => tipoEndereco ?? TipoEnderecoEnum.Outros;
        set => tipoEndereco = value;
    }
}