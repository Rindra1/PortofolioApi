using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToUtilisateurAndProjet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StackMaitrise",
                table: "Utilisateur",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StackTechnique",
                table: "Utilisateur",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Titre",
                table: "Utilisateur",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fonctionnalite",
                table: "Projet",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageProjet1",
                table: "Projet",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageProjet2",
                table: "Projet",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lien",
                table: "Projet",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stack",
                table: "Projet",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StackMaitrise",
                table: "Utilisateur");

            migrationBuilder.DropColumn(
                name: "StackTechnique",
                table: "Utilisateur");

            migrationBuilder.DropColumn(
                name: "Titre",
                table: "Utilisateur");

            migrationBuilder.DropColumn(
                name: "Fonctionnalite",
                table: "Projet");

            migrationBuilder.DropColumn(
                name: "ImageProjet1",
                table: "Projet");

            migrationBuilder.DropColumn(
                name: "ImageProjet2",
                table: "Projet");

            migrationBuilder.DropColumn(
                name: "Lien",
                table: "Projet");

            migrationBuilder.DropColumn(
                name: "Stack",
                table: "Projet");
        }
    }
}
