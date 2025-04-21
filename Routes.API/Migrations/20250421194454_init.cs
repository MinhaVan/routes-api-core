using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Routes.API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CNPJ = table.Column<string>(type: "text", nullable: true),
                    NomeExibicao = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    NomeFantasia = table.Column<string>(type: "text", nullable: true),
                    RazaoSocial = table.Column<string>(type: "text", nullable: true),
                    Apelido = table.Column<string>(type: "text", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "permissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    EditarPlanos = table.Column<bool>(type: "boolean", nullable: false),
                    EditarVeiculos = table.Column<bool>(type: "boolean", nullable: false),
                    PadraoPassageiros = table.Column<bool>(type: "boolean", nullable: false),
                    PadraoSuporte = table.Column<bool>(type: "boolean", nullable: false),
                    PadraoResponsavel = table.Column<bool>(type: "boolean", nullable: false),
                    PadraoMotorista = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "plano",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plano", x => x.Id);
                    table.ForeignKey(
                        name: "FK_plano_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "veiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    AnoModelo = table.Column<int>(type: "integer", nullable: false),
                    Cor = table.Column<string>(type: "text", nullable: true),
                    Marca = table.Column<string>(type: "text", nullable: true),
                    Modelo = table.Column<string>(type: "text", nullable: true),
                    Placa = table.Column<string>(type: "text", nullable: true),
                    TipoVeiculo = table.Column<int>(type: "integer", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_veiculos_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ajusteAlunoRota",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlunoId = table.Column<int>(type: "integer", nullable: false),
                    RotaId = table.Column<int>(type: "integer", nullable: false),
                    NovoEnderecoPartidaId = table.Column<int>(type: "integer", nullable: true),
                    NovoEnderecoDestinoId = table.Column<int>(type: "integer", nullable: true),
                    NovoEnderecoRetornoId = table.Column<int>(type: "integer", nullable: true),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AlunoRotaAlunoId = table.Column<int>(type: "integer", nullable: true),
                    AlunoRotaRotaId = table.Column<int>(type: "integer", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ajusteAlunoRota", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aluno_rota",
                columns: table => new
                {
                    AlunoId = table.Column<int>(type: "integer", nullable: false),
                    RotaId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aluno_rota", x => new { x.AlunoId, x.RotaId });
                });

            migrationBuilder.CreateTable(
                name: "aluno_rota_historico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RotaHistoricoId = table.Column<int>(type: "integer", nullable: false),
                    AlunoId = table.Column<int>(type: "integer", nullable: false),
                    DataRealizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EntrouNaVan = table.Column<bool>(type: "boolean", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aluno_rota_historico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrimeiroNome = table.Column<string>(type: "text", nullable: true),
                    CPF = table.Column<string>(type: "text", nullable: true),
                    UltimoNome = table.Column<string>(type: "text", nullable: true),
                    Contato = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    ResponsavelId = table.Column<int>(type: "integer", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    EnderecoPartidaId = table.Column<int>(type: "integer", nullable: false),
                    EnderecoDestinoId = table.Column<int>(type: "integer", nullable: false),
                    EnderecoRetornoId = table.Column<int>(type: "integer", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alunos_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assinaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdExterno = table.Column<string>(type: "text", nullable: true),
                    Vencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    PlanoId = table.Column<int>(type: "integer", nullable: false),
                    AlunoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    NumeroCartao = table.Column<string>(type: "text", nullable: true),
                    TipoPagamento = table.Column<int>(type: "integer", nullable: false),
                    CopiaCola = table.Column<string>(type: "text", nullable: true),
                    Imagem = table.Column<string>(type: "text", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assinaturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assinaturas_plano_PlanoId",
                        column: x => x.PlanoId,
                        principalTable: "plano",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataVencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StatusPagamento = table.Column<int>(type: "integer", nullable: false),
                    AssinaturaId = table.Column<int>(type: "integer", nullable: false),
                    PagamentoIdExternal = table.Column<string>(type: "text", nullable: true),
                    AssinaturaExternal = table.Column<string>(type: "text", nullable: true),
                    TipoFaturamento = table.Column<string>(type: "text", nullable: true),
                    FaturaURL = table.Column<string>(type: "text", nullable: true),
                    NumeroFatura = table.Column<string>(type: "text", nullable: true),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pagamentos_assinaturas_AssinaturaId",
                        column: x => x.AssinaturaId,
                        principalTable: "assinaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "endereco",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Rua = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: true),
                    CEP = table.Column<string>(type: "text", nullable: true),
                    Complemento = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    TipoEndereco = table.Column<int>(type: "integer", nullable: false),
                    EnderecoPrincipal = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    EmpresaId = table.Column<int>(type: "integer", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_endereco", x => x.Id);
                    table.ForeignKey(
                        name: "FK_endereco_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "rota",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    VeiculoId = table.Column<int>(type: "integer", nullable: true),
                    DiaSemana = table.Column<int>(type: "integer", nullable: false),
                    Horario = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TipoRota = table.Column<int>(type: "integer", nullable: false),
                    EnderecoId = table.Column<int>(type: "integer", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rota", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rota_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rota_endereco_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "endereco",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_rota_veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "veiculos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CPF = table.Column<string>(type: "text", nullable: true),
                    Senha = table.Column<string>(type: "text", nullable: true),
                    Contato = table.Column<string>(type: "text", nullable: true),
                    PrimeiroNome = table.Column<string>(type: "text", nullable: true),
                    UltimoNome = table.Column<string>(type: "text", nullable: true),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioValidado = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    ClientIdPaymentGateway = table.Column<string>(type: "text", nullable: true),
                    Perfil = table.Column<int>(type: "integer", nullable: false),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlanoId = table.Column<int>(type: "integer", nullable: true),
                    EnderecoPrincipalId = table.Column<int>(type: "integer", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_endereco_EnderecoPrincipalId",
                        column: x => x.EnderecoPrincipalId,
                        principalTable: "endereco",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuarios_plano_PlanoId",
                        column: x => x.PlanoId,
                        principalTable: "plano",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ordem_trajeto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RotaId = table.Column<int>(type: "integer", nullable: false),
                    AlunoId = table.Column<int>(type: "integer", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordem_trajeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ordem_trajeto_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ordem_trajeto_rota_RotaId",
                        column: x => x.RotaId,
                        principalTable: "rota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rota_historico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RotaId = table.Column<int>(type: "integer", nullable: false),
                    TipoRota = table.Column<int>(type: "integer", nullable: false),
                    DataRealizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmAndamento = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rota_historico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rota_historico_rota_RotaId",
                        column: x => x.RotaId,
                        principalTable: "rota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "motoristas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    CNH = table.Column<string>(type: "text", nullable: true),
                    Vencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoCNH = table.Column<int>(type: "integer", nullable: false),
                    Foto = table.Column<string>(type: "text", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motoristas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_motoristas_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_permissao",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    PermissaoId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_permissao", x => new { x.UsuarioId, x.PermissaoId });
                    table.ForeignKey(
                        name: "FK_usuario_permissao_permissoes_PermissaoId",
                        column: x => x.PermissaoId,
                        principalTable: "permissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_permissao_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ordem_trajeto_marcador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoMarcador = table.Column<int>(type: "integer", nullable: false),
                    OrdemTrajetoId = table.Column<int>(type: "integer", nullable: false),
                    EnderecoId = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordem_trajeto_marcador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ordem_trajeto_marcador_ordem_trajeto_OrdemTrajetoId",
                        column: x => x.OrdemTrajetoId,
                        principalTable: "ordem_trajeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "localizacao_trajeto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RotaId = table.Column<int>(type: "integer", nullable: false),
                    RotaHistoricoId = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_localizacao_trajeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_localizacao_trajeto_rota_historico_RotaHistoricoId",
                        column: x => x.RotaHistoricoId,
                        principalTable: "rota_historico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "motorista_rota",
                columns: table => new
                {
                    MotoristaId = table.Column<int>(type: "integer", nullable: false),
                    RotaId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motorista_rota", x => new { x.MotoristaId, x.RotaId });
                    table.ForeignKey(
                        name: "FK_motorista_rota_motoristas_MotoristaId",
                        column: x => x.MotoristaId,
                        principalTable: "motoristas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_motorista_rota_rota_RotaId",
                        column: x => x.RotaId,
                        principalTable: "rota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ajusteAlunoRota_AlunoRotaAlunoId_AlunoRotaRotaId",
                table: "ajusteAlunoRota",
                columns: new[] { "AlunoRotaAlunoId", "AlunoRotaRotaId" });

            migrationBuilder.CreateIndex(
                name: "IX_ajusteAlunoRota_NovoEnderecoDestinoId",
                table: "ajusteAlunoRota",
                column: "NovoEnderecoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_ajusteAlunoRota_NovoEnderecoPartidaId",
                table: "ajusteAlunoRota",
                column: "NovoEnderecoPartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_ajusteAlunoRota_NovoEnderecoRetornoId",
                table: "ajusteAlunoRota",
                column: "NovoEnderecoRetornoId");

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_rota_RotaId",
                table: "Aluno_rota",
                column: "RotaId");

            migrationBuilder.CreateIndex(
                name: "IX_aluno_rota_historico_AlunoId",
                table: "aluno_rota_historico",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_aluno_rota_historico_RotaHistoricoId",
                table: "aluno_rota_historico",
                column: "RotaHistoricoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_EmpresaId",
                table: "Alunos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_EnderecoDestinoId",
                table: "Alunos",
                column: "EnderecoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_EnderecoPartidaId",
                table: "Alunos",
                column: "EnderecoPartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_EnderecoRetornoId",
                table: "Alunos",
                column: "EnderecoRetornoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_ResponsavelId",
                table: "Alunos",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_PlanoId",
                table: "assinaturas",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_UsuarioId",
                table: "assinaturas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_endereco_EmpresaId",
                table: "endereco",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_endereco_UsuarioId",
                table: "endereco",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_localizacao_trajeto_RotaHistoricoId",
                table: "localizacao_trajeto",
                column: "RotaHistoricoId");

            migrationBuilder.CreateIndex(
                name: "IX_motorista_rota_RotaId",
                table: "motorista_rota",
                column: "RotaId");

            migrationBuilder.CreateIndex(
                name: "IX_motoristas_UsuarioId",
                table: "motoristas",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ordem_trajeto_AlunoId",
                table: "ordem_trajeto",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_ordem_trajeto_RotaId",
                table: "ordem_trajeto",
                column: "RotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ordem_trajeto_marcador_OrdemTrajetoId",
                table: "ordem_trajeto_marcador",
                column: "OrdemTrajetoId");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_AssinaturaId",
                table: "pagamentos",
                column: "AssinaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_plano_EmpresaId",
                table: "plano",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_rota_EmpresaId",
                table: "rota",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_rota_EnderecoId",
                table: "rota",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_rota_VeiculoId",
                table: "rota",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_rota_historico_RotaId",
                table: "rota_historico",
                column: "RotaId");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_permissao_PermissaoId",
                table: "usuario_permissao",
                column: "PermissaoId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_EmpresaId",
                table: "usuarios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_EnderecoPrincipalId",
                table: "usuarios",
                column: "EnderecoPrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_PlanoId",
                table: "usuarios",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_EmpresaId",
                table: "veiculos",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ajusteAlunoRota_Aluno_rota_AlunoRotaAlunoId_AlunoRotaRotaId",
                table: "ajusteAlunoRota",
                columns: new[] { "AlunoRotaAlunoId", "AlunoRotaRotaId" },
                principalTable: "Aluno_rota",
                principalColumns: new[] { "AlunoId", "RotaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ajusteAlunoRota_endereco_NovoEnderecoDestinoId",
                table: "ajusteAlunoRota",
                column: "NovoEnderecoDestinoId",
                principalTable: "endereco",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ajusteAlunoRota_endereco_NovoEnderecoPartidaId",
                table: "ajusteAlunoRota",
                column: "NovoEnderecoPartidaId",
                principalTable: "endereco",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ajusteAlunoRota_endereco_NovoEnderecoRetornoId",
                table: "ajusteAlunoRota",
                column: "NovoEnderecoRetornoId",
                principalTable: "endereco",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Aluno_rota_Alunos_AlunoId",
                table: "Aluno_rota",
                column: "AlunoId",
                principalTable: "Alunos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Aluno_rota_rota_RotaId",
                table: "Aluno_rota",
                column: "RotaId",
                principalTable: "rota",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_aluno_rota_historico_Alunos_AlunoId",
                table: "aluno_rota_historico",
                column: "AlunoId",
                principalTable: "Alunos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_aluno_rota_historico_rota_historico_RotaHistoricoId",
                table: "aluno_rota_historico",
                column: "RotaHistoricoId",
                principalTable: "rota_historico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_endereco_EnderecoDestinoId",
                table: "Alunos",
                column: "EnderecoDestinoId",
                principalTable: "endereco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_endereco_EnderecoPartidaId",
                table: "Alunos",
                column: "EnderecoPartidaId",
                principalTable: "endereco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_endereco_EnderecoRetornoId",
                table: "Alunos",
                column: "EnderecoRetornoId",
                principalTable: "endereco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_usuarios_ResponsavelId",
                table: "Alunos",
                column: "ResponsavelId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_assinaturas_usuarios_UsuarioId",
                table: "assinaturas",
                column: "UsuarioId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_endereco_usuarios_UsuarioId",
                table: "endereco",
                column: "UsuarioId",
                principalTable: "usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_endereco_EnderecoPrincipalId",
                table: "usuarios");

            migrationBuilder.DropTable(
                name: "ajusteAlunoRota");

            migrationBuilder.DropTable(
                name: "aluno_rota_historico");

            migrationBuilder.DropTable(
                name: "localizacao_trajeto");

            migrationBuilder.DropTable(
                name: "motorista_rota");

            migrationBuilder.DropTable(
                name: "ordem_trajeto_marcador");

            migrationBuilder.DropTable(
                name: "pagamentos");

            migrationBuilder.DropTable(
                name: "usuario_permissao");

            migrationBuilder.DropTable(
                name: "Aluno_rota");

            migrationBuilder.DropTable(
                name: "rota_historico");

            migrationBuilder.DropTable(
                name: "motoristas");

            migrationBuilder.DropTable(
                name: "ordem_trajeto");

            migrationBuilder.DropTable(
                name: "assinaturas");

            migrationBuilder.DropTable(
                name: "permissoes");

            migrationBuilder.DropTable(
                name: "Alunos");

            migrationBuilder.DropTable(
                name: "rota");

            migrationBuilder.DropTable(
                name: "veiculos");

            migrationBuilder.DropTable(
                name: "endereco");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "plano");

            migrationBuilder.DropTable(
                name: "empresas");
        }
    }
}
