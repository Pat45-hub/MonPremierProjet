// fichier 20231109134248_Migration2.cs
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MonPremierProjet.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_testtable",
                table: "testtable");

            migrationBuilder.RenameTable(
                name: "testtable",
                newName: "TestTables");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestTables",
                table: "TestTables",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ajustementenergie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomAjustement = table.Column<string>(type: "text", nullable: false),
                    Valeur = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ajustementenergie", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ajustementenergie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestTables",
                table: "TestTables");

            migrationBuilder.RenameTable(
                name: "TestTables",
                newName: "testtable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_testtable",
                table: "testtable",
                column: "Id");
        }
    }
}
