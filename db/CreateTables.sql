USE [ContactUsDb] 
GO
DROP TABLE IF EXISTS [dbo].[ContactMessages];
GO
CREATE TABLE [dbo].[ContactMessages]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
);