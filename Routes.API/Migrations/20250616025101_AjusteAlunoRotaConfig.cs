using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Routes.API.Migrations
{
    /// <inheritdoc />
    public partial class AjusteAlunoRotaConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ajusteAlunoRota",
                table: "ajusteAlunoRota");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ajusteAlunoRota");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ajusteAlunoRota",
                table: "ajusteAlunoRota",
                columns: new[] { "AlunoId", "RotaId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ajusteAlunoRota",
                table: "ajusteAlunoRota");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ajusteAlunoRota",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ajusteAlunoRota",
                table: "ajusteAlunoRota",
                column: "Id");
        }
    }
}
