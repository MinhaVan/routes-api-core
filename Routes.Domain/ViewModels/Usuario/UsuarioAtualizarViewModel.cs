using System;
using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels;

public class UsuarioAtualizarViewModel
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Contato { get; set; }
    public string CPF { get; set; }
    public string Email { get; set; }
    public string PrimeiroNome { get; set; }
    public string UltimoNome { get; set; }
    public PerfilEnum Perfil { get; set; }
    public int? PlanoId { get; set; }
    public int? EnderecoPrincipalId { get; set; }
    public bool UsuarioValidado { get; set; }
    public string Senha { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}