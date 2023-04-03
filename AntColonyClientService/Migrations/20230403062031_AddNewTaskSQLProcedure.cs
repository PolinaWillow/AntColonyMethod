using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntColonyClient.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTaskSQLProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Переменная для хранения процедуры
            string addNewTaskProcedure = @"Create procedure AddNewUserTask
                                           @name nvarchar(50),
                                           @createData nvarchar(50), 
                                           @inputMethod int
                                           AS
                                           BEGIN
                                           	INSERT INTO UserTasks (Name, Create_Data, InputMethod)
                                            VALUES (@name,@createData,@inputMethod)
                                           END
                                          ";
            migrationBuilder.Sql(addNewTaskProcedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string addNewTaskProcedure = @"Drop procedure AddNewUserTask";
            migrationBuilder.Sql(addNewTaskProcedure);
        }
    }
}
