using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntColonyClient.Service.Migrations
{
    /// <inheritdoc />
    public partial class GetTaskCountSQLProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Create procedure GetTaskCount                                       
                                           AS
                                           BEGIN
                                           	SELECT COUNT(*) FROM  UserTasks                                            
                                           END
                                          ";
            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Drop procedure GetTaskCount";
            migrationBuilder.Sql(procedure);
        }
    }
}
