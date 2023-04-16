Create procedure GetParamById
                                           @id int
                                           AS
                                           BEGIN
                                           	SELECT Id, IdTask, NumParam, TypeParam FROM  TaskParams 
                                            WHERE Id = @id
                                           END
                                          
