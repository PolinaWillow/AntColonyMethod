using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntColonyClient.Service.Migrations
{
    /// <inheritdoc />
    public partial class NewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParamElems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdParam = table.Column<int>(type: "int", nullable: false),
                    ValueParam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParamElems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskParams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTask = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    NumParam = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    TypeParam = table.Column<int>(type: "int", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskParams", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParamElems");

            migrationBuilder.DropTable(
                name: "TaskParams");
        }
    }
}
