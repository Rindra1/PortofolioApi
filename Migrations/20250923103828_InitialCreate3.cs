using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUserLogin",
                table: "Utilisateur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUserLogin",
                table: "Utilisateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
