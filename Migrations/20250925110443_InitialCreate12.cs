using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lien_Projet_ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.DropIndex(
                name: "IX_Lien_ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.DropColumn(
                name: "ProjetsIdProjet",
                table: "Lien");

            migrationBuilder.AddColumn<int>(
                name: "IdProjet",
                table: "Lien",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Lien_IdProjet",
                table: "Lien",
                column: "IdProjet");

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
                name: "FK_Lien_Projet_IdProjet",
                table: "Lien");

            migrationBuilder.DropIndex(
                name: "IX_Lien_IdProjet",
                table: "Lien");

            migrationBuilder.DropColumn(
                name: "IdProjet",
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
        }
    }
}
