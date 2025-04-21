using System;

namespace Routes.Domain.Models;

public class AjusteAlunoRota : Entity
{
    public int AlunoId { get; set; }
    public int RotaId { get; set; }
    public int? NovoEnderecoPartidaId { get; set; }
    public int? NovoEnderecoDestinoId { get; set; }
    public int? NovoEnderecoRetornoId { get; set; }
    public DateTime Data { get; set; }
    //
    public virtual Endereco EnderecoPartida { get; set; }
    public virtual Endereco EnderecoDestino { get; set; }
    public virtual Endereco EnderecoRetorno { get; set; }
}
