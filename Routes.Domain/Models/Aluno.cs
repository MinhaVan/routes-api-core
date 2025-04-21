using System.Collections.Generic;

namespace Routes.Domain.Models;

public class Aluno : Entity
{
    public string PrimeiroNome { get; set; }
    public string CPF { get; set; }
    public string UltimoNome { get; set; }
    public string Contato { get; set; }
    public string Email { get; set; }
    public int ResponsavelId { get; set; }
    public int EmpresaId { get; set; }
    public int EnderecoPartidaId { get; set; }
    public int EnderecoDestinoId { get; set; }
    public int? EnderecoRetornoId { get; set; }
    //
    public virtual List<AlunoRota> AlunoRotas { get; set; }
    public virtual List<AlunoRotaHistorico> AlunoRotaHistoricos { get; set; }
    public virtual List<OrdemTrajeto> OrdemTrajetos { get; set; }
    public virtual Usuario Responsavel { get; set; }
    public virtual Empresa Empresa { get; set; }
    public virtual Endereco EnderecoPartida { get; set; }
    public virtual Endereco EnderecoDestino { get; set; }
    public virtual Endereco? EnderecoRetorno { get; set; }

    public string NomeInteiro() => this.PrimeiroNome.Trim() + " " + this.UltimoNome.Trim();
}