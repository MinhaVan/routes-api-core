using System;

namespace Routes.Domain.ViewModels;

public class AlunoAdicionarViewModel
{
    public string PrimeiroNome { get; set; }
    public string CPF { get; set; }
    public string UltimoNome { get; set; }
    public string Contato { get; set; }
    public string Email { get; set; }
    public int EnderecoPartidaId { get; set; }
    public int EnderecoDestinoId { get; set; }
    public int? EnderecoRetornoId { get; set; }
}