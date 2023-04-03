using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntColonyClient.Service.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUserTaskSQLProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Create procedure DeleteUserTask
                                           @id int
                                           AS
                                           BEGIN TRANSACTION
                                           	DELETE FROM  UserTasks 
                                                  WHERE Id = @id
                                            DELETE FROM  TaskParams 
                                                  WHERE IdTask = @id
                                            DELETE FROM  ParamElems 
                                                  WHERE Id = (SELECT Id FROM TaskParams WHERE IdTask = @id)

                                            IF @@ERROR<>0 -- Если возникла любая ошибка во время исполнения
                                             BEGIN
                                                ROLLBACK -- Откатываем назад сделанные изменения(то есть не удаляем запись с ID = 1)
                                             END
                                            ELSE
                                             BEGIN
                                                COMMIT -- Подтверждаем сделанные изменения, так как никаких ошибок нет
                                             END

                                          
                                           ";
            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Drop procedure DeleteUserTask";
            migrationBuilder.Sql(procedure);
        }
    }
}
