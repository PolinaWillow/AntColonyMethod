Create procedure AddNewParamValue
                                           @idParam int,
                                           @valueParam nvarchar(50)
                                           AS
                                           BEGIN
                                           	INSERT INTO ParamElems (IdParam, ValueParam)
                                            VALUES (@idParam,@valueParam)
                                           END
                                          
