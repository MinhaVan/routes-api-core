using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Routes.API.Migrations
{
    /// <inheritdoc />
    public partial class addCampoOrdem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ordem",
                table: "ordemTrajetoMarcador",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ordem",
                table: "ordemTrajetoMarcador");
        }
    }
}
