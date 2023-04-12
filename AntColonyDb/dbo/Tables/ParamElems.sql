CREATE TABLE [dbo].[ParamElems] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [IdParam]    INT           NOT NULL,
    [ValueParam] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ParamElems] PRIMARY KEY CLUSTERED ([Id] ASC)
);

