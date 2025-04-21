using System;

namespace Routes.Domain.ViewModels;

public class RotaAlterarAjusteEnderecoViewModel
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public int RotaId { get; set; }
    public int EnderecoDestinoId { get; set; }
    public int EnderecoRetornoId { get; set; }
    public int EnderecoPartidaId { get; set; }
    public DateTime Data { get; set; }
    public bool Deletado { get; set; }
}