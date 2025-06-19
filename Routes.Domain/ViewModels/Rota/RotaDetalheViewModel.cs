using System;
using System.Collections.Generic;
using Routes.Domain.Enums;
using Routes.Domain.Models;

namespace Routes.Domain.ViewModels;

public class RotaDetalheViewModel
{
    public int Id { get; set; }
    public int EnderecoId { get; set; }
    public int VeiculoId { get; set; }
    public string Nome { get; set; }
    public DiaSemanaEnum DiaSemana { get; set; }
    public TimeOnly Horario { get; set; }
    public TipoRotaEnum TipoRota { get; set; }
    public bool EmAndamento { get; set; }
    //
    public virtual List<AlunoDetalheViewModel> Alunos { get; set; }
}

public class AlunoDetalheViewModel
{
    public int Id { get; set; }
    public string PrimeiroNome { get; set; }
    public string UltimoNome { get; set; }
    public string Contato { get; set; }
    public string Email { get; set; }
    public int ResponsavelId { get; set; }
    public int EnderecoPartidaId { get; set; }
    public int EnderecoRetornoId { get; set; }
    public int EnderecoDestinoId { get; set; }

    public EnderecoViewModel EnderecoPartida { get; set; }
    public EnderecoViewModel EnderecoDestino { get; set; }
    public EnderecoViewModel EnderecoRetorno { get; set; }
    public UsuarioDetalheViewModel Responsavel { get; set; }
}

public class UsuarioDetalheViewModel
{
    public int Id { get; set; }
    public string CPF { get; set; }
    public string Contato { get; set; }
    public string Email { get; set; }
    public string PrimeiroNome { get; set; }
    public string UltimoNome { get; set; }
    public PerfilEnum Perfil { get; set; }
    public int PlanoId { get; set; }
    public bool UsuarioValidado { get; set; }
    public int? EnderecoPrincipalId { get; set; }
    public string Senha { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}