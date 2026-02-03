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
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [AppRoles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AppRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [AppUsers] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AppUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AppRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AppRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AppUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AppUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AppRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AppRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AppUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AppUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AppUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [AppUserId] int NULL,
        [Name] nvarchar(250) NOT NULL,
        [Username] nvarchar(100) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [Avatar] nvarchar(500) NULL,
        [Bio] nvarchar(max) NULL,
        [IsOnline] bit NOT NULL DEFAULT CAST(0 AS bit),
        [LastSeen] datetime NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_AppUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [AppUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [BlockedUsers] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [BlockedUserId] int NOT NULL,
        [BlockedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_BlockedUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BlockedUsers_Users_BlockedUserId] FOREIGN KEY ([BlockedUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BlockedUsers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [Calls] (
        [Id] int NOT NULL IDENTITY,
        [CallerId] int NOT NULL,
        [ReceiverId] int NOT NULL,
        [CallType] nvarchar(50) NOT NULL DEFAULT N'voice',
        [Status] nvarchar(50) NOT NULL DEFAULT N'ringing',
        [StartedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [EndedAt] datetime NULL,
        [Duration] int NULL,
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Calls] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Calls_Users_CallerId] FOREIGN KEY ([CallerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Calls_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [Contacts] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ContactUserId] int NOT NULL,
        [AddedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Contacts_Users_ContactUserId] FOREIGN KEY ([ContactUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Contacts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [Groups] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Avatar] nvarchar(500) NULL,
        [CreatedById] int NOT NULL,
        [IsMutedForAll] bit NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Groups] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Group_UpdatedAt] CHECK ([UpdatedAt] >= [CreatedAt]),
        CONSTRAINT [FK_Groups_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [Messages] (
        [Id] int NOT NULL IDENTITY,
        [SenderId] int NOT NULL,
        [ReceiverId] int NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [IsRead] bit NOT NULL DEFAULT CAST(0 AS bit),
        [SentAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [ReadAt] datetime NULL,
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Messages_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Messages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [Stories] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ImageUrl] nvarchar(1000) NOT NULL,
        [Caption] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [ExpiresAt] datetime2 NOT NULL,
        [Deleted] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Stories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Stories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [GroupMembers] (
        [Id] int NOT NULL IDENTITY,
        [GroupId] int NOT NULL,
        [UserId] int NOT NULL,
        [IsAdmin] bit NOT NULL DEFAULT CAST(0 AS bit),
        [IsMuted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [JoinedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_GroupMembers] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_GroupMember_JoinedAt] CHECK ([JoinedAt] <= GETUTCDATE()),
        CONSTRAINT [FK_GroupMembers_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_GroupMembers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [GroupMessages] (
        [Id] int NOT NULL IDENTITY,
        [GroupId] int NOT NULL,
        [SenderId] int NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [IsSystemMessage] bit NOT NULL DEFAULT CAST(0 AS bit),
        [SentAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_GroupMessages] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_GroupMessage_Content] CHECK (LEN([Content]) > 0),
        CONSTRAINT [CK_GroupMessage_SentAt] CHECK ([SentAt] <= GETUTCDATE()),
        CONSTRAINT [FK_GroupMessages_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_GroupMessages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE TABLE [StoryViews] (
        [Id] int NOT NULL IDENTITY,
        [StoryId] int NOT NULL,
        [UserId] int NOT NULL,
        [ViewedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_StoryViews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StoryViews_Stories_StoryId] FOREIGN KEY ([StoryId]) REFERENCES [Stories] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StoryViews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AppRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AppUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AppUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BlockedUsers_BlockedUserId] ON [BlockedUsers] ([BlockedUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BlockedUsers_UserId] ON [BlockedUsers] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BlockedUsers_UserId_BlockedUserId] ON [BlockedUsers] ([UserId], [BlockedUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_Caller_Started] ON [Calls] ([CallerId], [StartedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_CallerId] ON [Calls] ([CallerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_Deleted] ON [Calls] ([Deleted]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_Receiver_Started] ON [Calls] ([ReceiverId], [StartedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_ReceiverId] ON [Calls] ([ReceiverId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_StartedAt] ON [Calls] ([StartedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_Status] ON [Calls] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Contacts_ContactUserId] ON [Contacts] ([ContactUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Contacts_UserId] ON [Contacts] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Contacts_UserId_ContactUserId] ON [Contacts] ([UserId], [ContactUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_GroupMember_Group_IsAdmin] ON [GroupMembers] ([GroupId], [IsAdmin]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [idx_GroupMember_Group_User_Deleted] ON [GroupMembers] ([GroupId], [UserId], [Deleted]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_GroupMember_GroupId] ON [GroupMembers] ([GroupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_GroupMember_UserId] ON [GroupMembers] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_GroupMessage_Group_Deleted_SentAt] ON [GroupMessages] ([GroupId], [Deleted], [SentAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_GroupMessage_Group_IsSystem] ON [GroupMessages] ([GroupId], [IsSystemMessage]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_GroupMessage_Group_SentAt] ON [GroupMessages] ([GroupId], [SentAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_GroupMessage_SenderId] ON [GroupMessages] ([SenderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_Group_CreatedAt] ON [Groups] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_Group_CreatedById] ON [Groups] ([CreatedById]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [idx_Group_Name_Deleted] ON [Groups] ([Name], [Deleted]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_Group_UpdatedAt] ON [Groups] ([UpdatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_IsRead] ON [Messages] ([IsRead]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [idx_Name_Deleted] ON [Messages] ([Deleted]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_ReceiverId] ON [Messages] ([ReceiverId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_SenderId] ON [Messages] ([SenderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_SentAt] ON [Messages] ([SentAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Stories_CreatedAt] ON [Stories] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Stories_ExpiresAt] ON [Stories] ([ExpiresAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Stories_UserId] ON [Stories] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StoryViews_StoryId] ON [StoryViews] ([StoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StoryViews_StoryId_UserId] ON [StoryViews] ([StoryId], [UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StoryViews_UserId] ON [StoryViews] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [idx_AppUserId] ON [Users] ([AppUserId]) WHERE [AppUserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_CreatedAt] ON [Users] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [idx_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE INDEX [idx_IsOnline] ON [Users] ([IsOnline]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [idx_Name_Deleted] ON [Users] ([Name], [Deleted]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [idx_Username] ON [Users] ([Username]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127170644_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260127170644_InitialCreate', N'8.0.22');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201114228_UpdateAvatarColumnsToMax'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260201114228_UpdateAvatarColumnsToMax', N'8.0.22');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201114732_FixAvatarMaxLength'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Avatar');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Users] ALTER COLUMN [Avatar] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201114732_FixAvatarMaxLength'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Groups]') AND [c].[name] = N'Avatar');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Groups] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Groups] ALTER COLUMN [Avatar] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201114732_FixAvatarMaxLength'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260201114732_FixAvatarMaxLength', N'8.0.22');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE TABLE [MovieRooms] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(250) NOT NULL,
        [Description] nvarchar(max) NULL,
        [YouTubeUrl] nvarchar(500) NOT NULL,
        [YouTubeVideoId] nvarchar(50) NOT NULL,
        [CreatedById] int NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CurrentTime] float NOT NULL DEFAULT 0.0E0,
        [IsPlaying] bit NOT NULL DEFAULT CAST(0 AS bit),
        [CreatedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_MovieRooms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MovieRooms_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE TABLE [MovieRoomMessages] (
        [Id] int NOT NULL IDENTITY,
        [MovieRoomId] int NOT NULL,
        [SenderId] int NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [SentAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_MovieRoomMessages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MovieRoomMessages_MovieRooms_MovieRoomId] FOREIGN KEY ([MovieRoomId]) REFERENCES [MovieRooms] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MovieRoomMessages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE TABLE [MovieRoomParticipants] (
        [Id] int NOT NULL IDENTITY,
        [MovieRoomId] int NOT NULL,
        [UserId] int NOT NULL,
        [JoinedAt] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [Deleted] int NOT NULL DEFAULT 0,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_MovieRoomParticipants] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MovieRoomParticipants_MovieRooms_MovieRoomId] FOREIGN KEY ([MovieRoomId]) REFERENCES [MovieRooms] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MovieRoomParticipants_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE INDEX [IX_MovieRoomMessages_MovieRoomId] ON [MovieRoomMessages] ([MovieRoomId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE INDEX [IX_MovieRoomMessages_SenderId] ON [MovieRoomMessages] ([SenderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE INDEX [IX_MovieRoomMessages_SentAt] ON [MovieRoomMessages] ([SentAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE INDEX [IX_MovieRoomParticipants_UserId] ON [MovieRoomParticipants] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE UNIQUE INDEX [idx_MovieRoomParticipant_Unique] ON [MovieRoomParticipants] ([MovieRoomId], [UserId], [Deleted]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE INDEX [IX_MovieRooms_CreatedAt] ON [MovieRooms] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE INDEX [IX_MovieRooms_CreatedById] ON [MovieRooms] ([CreatedById]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    CREATE INDEX [IX_MovieRooms_IsActive] ON [MovieRooms] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201121144_AddMovieRoomFeature'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260201121144_AddMovieRoomFeature', N'8.0.22');
END;
GO

COMMIT;
GO

