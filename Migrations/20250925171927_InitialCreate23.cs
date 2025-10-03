using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur",
                column: "IdUserLogin",
                principalTable: "UserLogin",
                principalColumn: "IdUserLogin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur",
                column: "IdUserLogin",
                principalTable: "UserLogin",
                principalColumn: "IdUserLogin",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
