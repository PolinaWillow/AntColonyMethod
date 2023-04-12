Create procedure GetAllTaskParams                                       
                                           @idTask int
                                           AS
                                           BEGIN
                                           	SELECT * FROM  TaskParams 
                                            WHERE IdTask = @idTask
                                           END