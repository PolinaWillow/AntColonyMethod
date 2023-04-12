Create procedure AddNewUserTask
                                           @name nvarchar(50),
                                           @createData nvarchar(50), 
                                           @inputMethod int
                                           AS
                                           BEGIN
                                           	INSERT INTO UserTasks (Name, Create_Data, InputMethod)
                                            VALUES (@name,@createData,@inputMethod)
                                           END
                                          
