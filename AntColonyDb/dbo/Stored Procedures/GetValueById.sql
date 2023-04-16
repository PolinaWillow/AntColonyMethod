Create procedure GetValueById
                                           @id int
                                           AS
                                           BEGIN
                                           	SELECT Id, IdParam, ValueParam FROM  ParamElems 
                                            WHERE Id = @id
                                           END
                                          
