using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: true),
                    Prenom = table.Column<string>(type: "TEXT", nullable: true),
                    APropos = table.Column<string>(type: "TEXT", nullable: true),
                    UserImage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    IdContact = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdresseContact = table.Column<string>(type: "TEXT", nullable: true),
                    UtilisateursIdUser = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.IdContact);
                    table.ForeignKey(
                        name: "FK_Contact_Utilisateur_UtilisateursIdUser",
                        column: x => x.UtilisateursIdUser,
                        principalTable: "Utilisateur",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateTable(
                name: "Projet",
                columns: table => new
                {
                    IdProjet = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TitreProjet = table.Column<string>(type: "TEXT", nullable: true),
                    DetailProjet = table.Column<string>(type: "TEXT", nullable: true),
                    ImageProjet = table.Column<string>(type: "TEXT", nullable: true),
                    UtilisateurIdUser = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projet", x => x.IdProjet);
                    table.ForeignKey(
                        name: "FK_Projet_Utilisateur_UtilisateurIdUser",
                        column: x => x.UtilisateurIdUser,
                        principalTable: "Utilisateur",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    IdUserLogin = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Pseudo = table.Column<string>(type: "TEXT", nullable: true),
                    Role = table.Column<string>(type: "TEXT", nullable: true),
                    MotDePasse = table.Column<string>(type: "TEXT", nullable: true),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => x.IdUserLogin);
                    table.ForeignKey(
                        name: "FK_UserLogin_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lien",
                columns: table => new
                {
                    IdLien = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheminLien = table.Column<string>(type: "TEXT", nullable: true),
                    ProjetsIdProjet = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lien", x => x.IdLien);
                    table.ForeignKey(
                        name: "FK_Lien_Projet_ProjetsIdProjet",
                        column: x => x.ProjetsIdProjet,
                        principalTable: "Projet",
                        principalColumn: "IdProjet");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contact_UtilisateursIdUser",
                table: "Contact",
                column: "UtilisateursIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Lien_ProjetsIdProjet",
                table: "Lien",
                column: "ProjetsIdProjet");

            migrationBuilder.CreateIndex(
                name: "IX_Projet_UtilisateurIdUser",
                table: "Projet",
                column: "UtilisateurIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UtilisateurId",
                table: "UserLogin",
                column: "UtilisateurId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Lien");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "Projet");

            migrationBuilder.DropTable(
                name: "Utilisateur");
        }
    }
}
