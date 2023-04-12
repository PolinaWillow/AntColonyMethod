CREATE TABLE [dbo].[UserTasks] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    [Create_Data] NVARCHAR (50) NOT NULL,
    [InputMethod] INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserTasks] PRIMARY KEY CLUSTERED ([Id] ASC)
);

