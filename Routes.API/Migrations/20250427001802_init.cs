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
                });

            migrationBuilder.CreateTable(
                name: "veiculo",
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
                    table.PrimaryKey("PK_veiculo", x => x.Id);
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
                        name: "FK_rota_endereco_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "endereco",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_rota_veiculo_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "veiculo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "alunoRota",
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
                    table.PrimaryKey("PK_alunoRota", x => new { x.AlunoId, x.RotaId });
                    table.ForeignKey(
                        name: "FK_alunoRota_rota_RotaId",
                        column: x => x.RotaId,
                        principalTable: "rota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "motoristaRota",
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
                    table.PrimaryKey("PK_motoristaRota", x => new { x.MotoristaId, x.RotaId });
                    table.ForeignKey(
                        name: "FK_motoristaRota_rota_RotaId",
                        column: x => x.RotaId,
                        principalTable: "rota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ordemTrajeto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RotaId = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordemTrajeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ordemTrajeto_rota_RotaId",
                        column: x => x.RotaId,
                        principalTable: "rota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rotaHistorico",
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
                    table.PrimaryKey("PK_rotaHistorico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rotaHistorico_rota_RotaId",
                        column: x => x.RotaId,
                        principalTable: "rota",
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
                    table.ForeignKey(
                        name: "FK_ajusteAlunoRota_alunoRota_AlunoRotaAlunoId_AlunoRotaRotaId",
                        columns: x => new { x.AlunoRotaAlunoId, x.AlunoRotaRotaId },
                        principalTable: "alunoRota",
                        principalColumns: new[] { "AlunoId", "RotaId" });
                    table.ForeignKey(
                        name: "FK_ajusteAlunoRota_endereco_NovoEnderecoDestinoId",
                        column: x => x.NovoEnderecoDestinoId,
                        principalTable: "endereco",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ajusteAlunoRota_endereco_NovoEnderecoPartidaId",
                        column: x => x.NovoEnderecoPartidaId,
                        principalTable: "endereco",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ajusteAlunoRota_endereco_NovoEnderecoRetornoId",
                        column: x => x.NovoEnderecoRetornoId,
                        principalTable: "endereco",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ordemTrajeto_marcador",
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
                    table.PrimaryKey("PK_ordemTrajeto_marcador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ordemTrajeto_marcador_ordemTrajeto_OrdemTrajetoId",
                        column: x => x.OrdemTrajetoId,
                        principalTable: "ordemTrajeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alunoRota_historico",
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
                    table.PrimaryKey("PK_alunoRota_historico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alunoRota_historico_rotaHistorico_RotaHistoricoId",
                        column: x => x.RotaHistoricoId,
                        principalTable: "rotaHistorico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "localizacaoTrajeto",
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
                    table.PrimaryKey("PK_localizacaoTrajeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_localizacaoTrajeto_rotaHistorico_RotaHistoricoId",
                        column: x => x.RotaHistoricoId,
                        principalTable: "rotaHistorico",
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
                name: "IX_alunoRota_RotaId",
                table: "alunoRota",
                column: "RotaId");

            migrationBuilder.CreateIndex(
                name: "IX_alunoRota_historico_RotaHistoricoId",
                table: "alunoRota_historico",
                column: "RotaHistoricoId");

            migrationBuilder.CreateIndex(
                name: "IX_localizacaoTrajeto_RotaHistoricoId",
                table: "localizacaoTrajeto",
                column: "RotaHistoricoId");

            migrationBuilder.CreateIndex(
                name: "IX_motoristaRota_RotaId",
                table: "motoristaRota",
                column: "RotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ordemTrajeto_RotaId",
                table: "ordemTrajeto",
                column: "RotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ordemTrajeto_marcador_OrdemTrajetoId",
                table: "ordemTrajeto_marcador",
                column: "OrdemTrajetoId");

            migrationBuilder.CreateIndex(
                name: "IX_rota_EnderecoId",
                table: "rota",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_rota_VeiculoId",
                table: "rota",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_rotaHistorico_RotaId",
                table: "rotaHistorico",
                column: "RotaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ajusteAlunoRota");

            migrationBuilder.DropTable(
                name: "alunoRota_historico");

            migrationBuilder.DropTable(
                name: "localizacaoTrajeto");

            migrationBuilder.DropTable(
                name: "motoristaRota");

            migrationBuilder.DropTable(
                name: "ordemTrajeto_marcador");

            migrationBuilder.DropTable(
                name: "alunoRota");

            migrationBuilder.DropTable(
                name: "rotaHistorico");

            migrationBuilder.DropTable(
                name: "ordemTrajeto");

            migrationBuilder.DropTable(
                name: "rota");

            migrationBuilder.DropTable(
                name: "endereco");

            migrationBuilder.DropTable(
                name: "veiculo");
        }
    }
}
