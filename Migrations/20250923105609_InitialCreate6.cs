using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortofolioApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_Utilisateur_IdUserLogin",
                table: "UserLogin");

            migrationBuilder.AlterColumn<int>(
                name: "IdUserLogin",
                table: "UserLogin",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateur_IdUserLogin",
                table: "Utilisateur",
                column: "IdUserLogin",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur",
                column: "IdUserLogin",
                principalTable: "UserLogin",
                principalColumn: "IdUserLogin",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateur_UserLogin_IdUserLogin",
                table: "Utilisateur");

            migrationBuilder.DropIndex(
                name: "IX_Utilisateur_IdUserLogin",
                table: "Utilisateur");

            migrationBuilder.AlterColumn<int>(
                name: "IdUserLogin",
                table: "UserLogin",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_Utilisateur_IdUserLogin",
                table: "UserLogin",
                column: "IdUserLogin",
                principalTable: "Utilisateur",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
