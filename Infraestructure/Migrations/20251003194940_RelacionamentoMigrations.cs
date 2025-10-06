using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class RelacionamentoMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropIndex(
                name: "IX_agendamento_contato_id",
                table: "agendamento");

            migrationBuilder.CreateIndex(
                name: "IX_agendamento_contato_id",
                table: "agendamento",
                column: "contato_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_agendamento_contato_id",
                table: "agendamento");

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    codg = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dscr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.codg);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agendamento_contato_id",
                table: "agendamento",
                column: "contato_id");
        }
    }
}
