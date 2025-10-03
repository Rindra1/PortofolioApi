using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Utilisateur_UtilisateursIdUser",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Lien_Projet_ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.DropIndex(
                name: "IX_Lien_ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.DropIndex(
                name: "IX_Contact_UtilisateursIdUser",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.DropColumn(
                name: "UtilisateursIdUser",
                table: "Contact");

            migrationBuilder.CreateIndex(
                name: "IX_Lien_IdProjet",
                table: "Lien",
                column: "IdProjet");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_IdUser",
                table: "Contact",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Utilisateur_IdUser",
                table: "Contact",
                column: "IdUser",
                principalTable: "Utilisateur",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lien_Projet_IdProjet",
                table: "Lien",
                column: "IdProjet",
                principalTable: "Projet",
                principalColumn: "IdProjet",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Utilisateur_IdUser",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Lien_Projet_IdProjet",
                table: "Lien");

            migrationBuilder.DropIndex(
                name: "IX_Lien_IdProjet",
                table: "Lien");

            migrationBuilder.DropIndex(
                name: "IX_Contact_IdUser",
                table: "Contact");

            migrationBuilder.AddColumn<int>(
                name: "ProjetsIdProjet",
                table: "Lien",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateursIdUser",
                table: "Contact",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lien_ProjetsIdProjet",
                table: "Lien",
                column: "ProjetsIdProjet");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_UtilisateursIdUser",
                table: "Contact",
                column: "UtilisateursIdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Utilisateur_UtilisateursIdUser",
                table: "Contact",
                column: "UtilisateursIdUser",
                principalTable: "Utilisateur",
                principalColumn: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Lien_Projet_ProjetsIdProjet",
                table: "Lien",
                column: "ProjetsIdProjet",
                principalTable: "Projet",
                principalColumn: "IdProjet");
        }
    }
}
