Create procedure AddTaskParam
                                           @idTask int,
                                           @numParam int, 
                                           @typeParam int
                                           AS
                                           BEGIN
                                           	INSERT INTO TaskParams (IdTask, NumParam, TypeParam)
                                            VALUES (@idTask,@numParam,@typeParam)
                                           END
                                          
