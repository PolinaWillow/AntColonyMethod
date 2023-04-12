CREATE TABLE [dbo].[TaskParams] (
    [Id]        INT IDENTITY (1, 1) NOT NULL,
    [IdTask]    INT NOT NULL,
    [NumParam]  INT NOT NULL,
    [TypeParam] INT NOT NULL,
    CONSTRAINT [PK_TaskParams] PRIMARY KEY CLUSTERED ([Id] ASC)
);

