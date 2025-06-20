using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Routes.API.Migrations
{
    /// <inheritdoc />
    public partial class addCampoGeradoAutomaticamente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GeradoAutomaticamente",
                table: "ordemTrajeto",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeradoAutomaticamente",
                table: "ordemTrajeto");
        }
    }
}
