IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE TABLE [Members] (
        [Id] int NOT NULL IDENTITY,
        [Role] nvarchar(20) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [WorkEmail] nvarchar(256) NOT NULL,
        [WorkPhone] nvarchar(50) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE TABLE [Systems] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [BusinessOwnerId] int NOT NULL,
        [SystemAdminId] int NULL,
        [ItDeveloperId] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Systems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Systems_Members_BusinessOwnerId] FOREIGN KEY ([BusinessOwnerId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Systems_Members_ItDeveloperId] FOREIGN KEY ([ItDeveloperId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Systems_Members_SystemAdminId] FOREIGN KEY ([SystemAdminId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE TABLE [Environments] (
        [Id] int NOT NULL IDENTITY,
        [AppSystemId] int NOT NULL,
        [Type] nvarchar(20) NOT NULL,
        [Label] nvarchar(100) NULL,
        [Url] nvarchar(500) NOT NULL,
        [DeploymentLocation] nvarchar(500) NOT NULL,
        [Notes] nvarchar(1000) NULL,
        CONSTRAINT [PK_Environments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Environments_Systems_AppSystemId] FOREIGN KEY ([AppSystemId]) REFERENCES [Systems] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Environments_AppSystemId] ON [Environments] ([AppSystemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Members_WorkEmail] ON [Members] ([WorkEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Systems_BusinessOwnerId] ON [Systems] ([BusinessOwnerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Systems_ItDeveloperId] ON [Systems] ([ItDeveloperId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Systems_Name] ON [Systems] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Systems_SystemAdminId] ON [Systems] ([SystemAdminId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261010212927_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261010212927_InitialCreate', N'10.0.12');
END;

COMMIT;
GO

