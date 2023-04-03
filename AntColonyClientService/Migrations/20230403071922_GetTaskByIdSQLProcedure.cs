using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntColonyClient.Service.Migrations
{
    /// <inheritdoc />
    public partial class GetTaskByIdSQLProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Create procedure GetTaskById
                                           @id int
                                           AS
                                           BEGIN
                                           	SELECT Id, Name, Create_Data, InputMethod FROM  UserTasks 
                                            WHERE Id = @id
                                           END
                                          ";
            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Drop procedure GetTaskById";
            migrationBuilder.Sql(procedure);
        }
    }
}
