using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class AddExperienceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "resume",
                table: "Utilisateur",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResumerProjet",
                table: "Projet",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Experience",
                columns: table => new
                {
                    IdExperience = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TitreExperience = table.Column<string>(type: "TEXT", nullable: false),
                    DetailExperience = table.Column<string>(type: "TEXT", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdUser = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experience", x => x.IdExperience);
                    table.ForeignKey(
                        name: "FK_Experience_Utilisateur_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Utilisateur",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Experience_IdUser",
                table: "Experience",
                column: "IdUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Experience");

            migrationBuilder.DropColumn(
                name: "resume",
                table: "Utilisateur");

            migrationBuilder.DropColumn(
                name: "ResumerProjet",
                table: "Projet");
        }
    }
}
