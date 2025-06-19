using Routes.Domain.ViewModels;

namespace Routes.Domain.ViewModels;

public class AlunoViewModel
{
    public int Id { get; set; }
    public string PrimeiroNome { get; set; }
    public string UltimoNome { get; set; }
    public string Contato { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public int ResponsavelId { get; set; }
    public int EnderecoPartidaId { get; set; }
    public int EnderecoDestinoId { get; set; }
    public int? EnderecoRetornoId { get; set; }
    //
    public UsuarioViewModel Responsavel { get; set; }
    public EnderecoViewModel EnderecoPartida { get; set; }
    public EnderecoViewModel EnderecoDestino { get; set; }
    public EnderecoViewModel? EnderecoRetorno { get; set; }
}