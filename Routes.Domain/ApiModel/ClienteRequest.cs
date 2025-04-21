namespace Routes.Domain.ApiModel;

public class ClienteRequest
{
    public string Nome { get; set; }
    public string CpfCnpj { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
}