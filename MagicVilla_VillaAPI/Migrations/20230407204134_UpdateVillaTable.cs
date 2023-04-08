using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVillaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Villas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Villas");
        }
    }
}
