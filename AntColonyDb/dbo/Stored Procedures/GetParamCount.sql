Create procedure GetParamCount            
                                           @idTask int
                                           AS
                                           BEGIN
                                           	SELECT COUNT(*) FROM  TaskParams
                                            WHERE IdTask = @idTask
                                           END
                                          
