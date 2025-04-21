using System;

namespace Routes.Domain.ViewModels;

public class RotaAdicionarAjusteEnderecoViewModel
{
    public int AlunoId { get; set; }
    public int RotaId { get; set; }
    public int? EnderecoPartidaId { get; set; }
    public int? EnderecoDestinoId { get; set; }
    public int? EnderecoRetornoId { get; set; }
    public DateTime Data { get; set; }
}