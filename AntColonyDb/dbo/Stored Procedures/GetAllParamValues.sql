Create procedure GetAllParamValues                                       
                                           @idParam int
                                           AS
                                           BEGIN
                                           	SELECT * FROM  ParamElems 
                                            WHERE IdParam = @idParam
                                           END