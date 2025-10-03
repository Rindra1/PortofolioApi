using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurIdUser",
                table: "Projet");

            migrationBuilder.DropIndex(
                name: "IX_Projet_UtilisateurIdUser",
                table: "Projet");

            migrationBuilder.DropColumn(
                name: "UtilisateurIdUser",
                table: "Projet");

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "Projet",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projet_UtilisateurId",
                table: "Projet",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurId",
                table: "Projet",
                column: "UtilisateurId",
                principalTable: "Utilisateur",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurId",
                table: "Projet");

            migrationBuilder.DropIndex(
                name: "IX_Projet_UtilisateurId",
                table: "Projet");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Projet");

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurIdUser",
                table: "Projet",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projet_UtilisateurIdUser",
                table: "Projet",
                column: "UtilisateurIdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurIdUser",
                table: "Projet",
                column: "UtilisateurIdUser",
                principalTable: "Utilisateur",
                principalColumn: "IdUser");
        }
    }
}
