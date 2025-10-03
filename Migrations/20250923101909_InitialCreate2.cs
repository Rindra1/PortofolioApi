using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_Utilisateur_IdUserLogin",
                table: "UserLogin");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "UserLogin");

            migrationBuilder.AddColumn<int>(
                name: "IdUserLogin",
                table: "Utilisateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_Utilisateur_IdUserLogin",
                table: "UserLogin",
                column: "IdUserLogin",
                principalTable: "Utilisateur",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_Utilisateur_IdUserLogin",
                table: "UserLogin");

            migrationBuilder.DropColumn(
                name: "IdUserLogin",
                table: "Utilisateur");

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "UserLogin",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_Utilisateur_IdUserLogin",
                table: "UserLogin",
                column: "IdUserLogin",
                principalTable: "Utilisateur",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
