using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntColonyClient.Service.Migrations
{
    /// <inheritdoc />
    public partial class GetAllTaskParams_SQLProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Create procedure GetAllTaskParams                                       
                                           @idTask int
                                           AS
                                           BEGIN
                                           	SELECT * FROM  TaskParams 
                                            WHERE IdTask = @idTask
                                           END
                                          ";
            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Drop procedure GetAllTaskParams";
            migrationBuilder.Sql(procedure);
        }
    }
}
