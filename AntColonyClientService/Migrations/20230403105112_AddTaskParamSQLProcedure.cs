using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntColonyClient.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskParamSQLProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string addNewTaskProcedure = @"Create procedure AddTaskParam
                                           @idTask int,
                                           @numParam int, 
                                           @typeParam int
                                           AS
                                           BEGIN
                                           	INSERT INTO TaskParams (IdTask, NumParam, TypeParam)
                                            VALUES (@idTask,@numParam,@typeParam)
                                           END
                                          ";
            migrationBuilder.Sql(addNewTaskProcedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string addNewTaskProcedure = @"Drop procedure AddTaskParam";
            migrationBuilder.Sql(addNewTaskProcedure);
        }
    }
}
