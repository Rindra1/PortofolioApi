using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurId",
                table: "Projet");

            migrationBuilder.RenameColumn(
                name: "UtilisateurId",
                table: "Projet",
                newName: "IdUser");

            migrationBuilder.RenameIndex(
                name: "IX_Projet_UtilisateurId",
                table: "Projet",
                newName: "IX_Projet_IdUser");

            migrationBuilder.AddColumn<int>(
                name: "IdUser",
                table: "Contact",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Projet_Utilisateur_IdUser",
                table: "Projet",
                column: "IdUser",
                principalTable: "Utilisateur",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projet_Utilisateur_IdUser",
                table: "Projet");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "IdUser",
                table: "Projet",
                newName: "UtilisateurId");

            migrationBuilder.RenameIndex(
                name: "IX_Projet_IdUser",
                table: "Projet",
                newName: "IX_Projet_UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projet_Utilisateur_UtilisateurId",
                table: "Projet",
                column: "UtilisateurId",
                principalTable: "Utilisateur",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
