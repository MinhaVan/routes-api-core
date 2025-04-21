using System;
using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.Models;

public class Usuario : Entity
{
    public string Email { get; set; }
    public string CPF { get; set; }
    public string Senha { get; set; }
    public string Contato { get; set; }
    public string PrimeiroNome { get; set; }
    public string UltimoNome { get; set; }
    public int EmpresaId { get; set; }
    public bool UsuarioValidado { get; set; }
    public string RefreshToken { get; set; }
    public string ClientIdPaymentGateway { get; set; }
    public PerfilEnum Perfil { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public int? PlanoId { get; set; }
    public int? EnderecoPrincipalId { get; set; }
    //
    public virtual IList<UsuarioPermissao> Permissoes { get; set; } = new List<UsuarioPermissao>();
    public virtual IList<Aluno> Alunos { get; set; } = new List<Aluno>();
    public virtual IList<Endereco> Enderecos { get; set; } = new List<Endereco>();
    public virtual Endereco EnderecoPrincipal { get; set; } = new Endereco();
    public virtual Empresa Empresa { get; set; }
    public virtual Motorista Motorista { get; set; }
    //
    public string ObterNomeInteiro()
    {
        return string.Concat(PrimeiroNome, " ", UltimoNome);
    }
}