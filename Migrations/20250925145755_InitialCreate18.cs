using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lien_Projet_IdProjet",
                table: "Lien");

            migrationBuilder.DropForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurId",
                table: "Projet");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur");

            migrationBuilder.DropIndex(
                name: "IX_Lien_IdProjet",
                table: "Lien");

            migrationBuilder.AddColumn<int>(
                name: "ProjetsIdProjet",
                table: "Lien",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lien_ProjetsIdProjet",
                table: "Lien",
                column: "ProjetsIdProjet");

            migrationBuilder.AddForeignKey(
                name: "FK_Lien_Projet_ProjetsIdProjet",
                table: "Lien",
                column: "ProjetsIdProjet",
                principalTable: "Projet",
                principalColumn: "IdProjet");

            migrationBuilder.AddForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurId",
                table: "Projet",
                column: "UtilisateurId",
                principalTable: "Utilisateur",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur",
                column: "IdUserLogin",
                principalTable: "UserLogin",
                principalColumn: "IdUserLogin",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lien_Projet_ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.DropForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurId",
                table: "Projet");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur");

            migrationBuilder.DropIndex(
                name: "IX_Lien_ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.DropColumn(
                name: "ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.CreateIndex(
                name: "IX_Lien_IdProjet",
                table: "Lien",
                column: "IdProjet");

            migrationBuilder.AddForeignKey(
                name: "FK_Lien_Projet_IdProjet",
                table: "Lien",
                column: "IdProjet",
                principalTable: "Projet",
                principalColumn: "IdProjet");

            migrationBuilder.AddForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurId",
                table: "Projet",
                column: "UtilisateurId",
                principalTable: "Utilisateur",
                principalColumn: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur",
                column: "IdUserLogin",
                principalTable: "UserLogin",
                principalColumn: "IdUserLogin");
        }
    }
}
