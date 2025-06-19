using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Routes.API.Migrations
{
    /// <inheritdoc />
    public partial class addQtdPessoasSuportada_Veiculos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantidadePessoasSuportada",
                table: "veiculo",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantidadePessoasSuportada",
                table: "veiculo");
        }
    }
}
