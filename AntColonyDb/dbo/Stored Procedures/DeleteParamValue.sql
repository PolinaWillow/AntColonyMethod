Create procedure DeleteParamValue
                                           @id int
                                           AS
                                           BEGIN TRANSACTION                                           	
                                            DELETE FROM  ParamElems 
                                                  WHERE Id = @id

                                            IF @@ERROR<>0 -- Если возникла любая ошибка во время исполнения
                                             BEGIN
                                                ROLLBACK -- Откатываем назад сделанные изменения(то есть не удаляем запись с ID = 1)
                                             END
                                            ELSE
                                             BEGIN
                                                COMMIT -- Подтверждаем сделанные изменения, так как никаких ошибок нет
                                             END

                                          
                                           
