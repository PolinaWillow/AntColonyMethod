Create procedure GetValueCount            
                                           @idParam int
                                           AS
                                           BEGIN
                                           	SELECT COUNT(*) FROM  ParamElems
                                            WHERE IdParam = @idParam
                                           END