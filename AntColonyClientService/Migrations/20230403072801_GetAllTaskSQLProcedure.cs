using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntColonyClient.Service.Migrations
{
    /// <inheritdoc />
    public partial class GetAllTaskSQLProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Create procedure GetAllTasks                                       
                                           AS
                                           BEGIN
                                           	SELECT * FROM  UserTasks                                            
                                           END
                                          ";
            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Drop procedure GetAllTasks";
            migrationBuilder.Sql(procedure);
        }
    }
}
