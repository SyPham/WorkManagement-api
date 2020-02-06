USE [WorkManagement]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Managers]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Managers](
	[UserID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
	[OCID] [int] NULL,
 CONSTRAINT [PK_Managers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OCs]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Level] [int] NOT NULL,
	[ParentID] [int] NOT NULL,
 CONSTRAINT [PK_OCs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OCUsers]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCUsers](
	[UserID] [int] NOT NULL,
	[OCID] [int] NOT NULL,
 CONSTRAINT [PK_OCUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[OCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Projects]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Projects](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tags]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tags](
	[UserID] [int] NOT NULL,
	[TaskID] [int] NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[TaskID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[From] [nvarchar](max) NULL,
	[CreatedBy] [int] NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[Status] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[DueDate] [datetime2](7) NOT NULL,
	[ParentID] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[Seen] [bit] NOT NULL,
	[ProjectID] [int] NOT NULL,
	[JobName] [nvarchar](max) NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamMembers]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamMembers](
	[UserID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
	[OCID] [int] NULL,
 CONSTRAINT [PK_TeamMembers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2/6/2020 4:34:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](max) NULL,
	[PasswordHash] [varbinary](max) NULL,
	[PasswordSalt] [varbinary](max) NULL,
	[Email] [nvarchar](max) NULL,
	[RoleID] [int] NOT NULL,
	[OCID] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200205050733_Version1.0.0', N'3.1.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200205051152_Version1.0.1', N'3.1.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200205051831_Version1.0.2', N'3.1.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200205052517_Version1.0.3', N'3.1.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200206012923_Version1.0.4', N'3.1.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200206084654_Version1.0.5', N'3.1.1')
GO
SET IDENTITY_INSERT [dbo].[Projects] ON 
GO
INSERT [dbo].[Projects] ([ID], [Name]) VALUES (1, N'KPI System')
GO
INSERT [dbo].[Projects] ([ID], [Name]) VALUES (2, N'Mixing Room')
GO
INSERT [dbo].[Projects] ([ID], [Name]) VALUES (3, N'Work Mangement')
GO
SET IDENTITY_INSERT [dbo].[Projects] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 
GO
INSERT [dbo].[Roles] ([ID], [Name]) VALUES (1, N'admin')
GO
INSERT [dbo].[Roles] ([ID], [Name]) VALUES (2, N'user')
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
INSERT [dbo].[Tags] ([UserID], [TaskID]) VALUES (2, 4)
GO
INSERT [dbo].[Tags] ([UserID], [TaskID]) VALUES (2, 5)
GO
INSERT [dbo].[Tags] ([UserID], [TaskID]) VALUES (3, 4)
GO
INSERT [dbo].[Tags] ([UserID], [TaskID]) VALUES (3, 5)
GO
SET IDENTITY_INSERT [dbo].[Tasks] ON 
GO
INSERT [dbo].[Tasks] ([ID], [Description], [From], [CreatedBy], [Remark], [Status], [CreatedDate], [DueDate], [ParentID], [Level], [Seen], [ProjectID], [JobName]) VALUES (4, N'To Do List', N'Swook', 2, NULL, 1, CAST(N'2020-02-05T00:00:00.0000000' AS DateTime2), CAST(N'2020-02-10T00:00:00.0000000' AS DateTime2), 0, 1, 0, 1, NULL)
GO
INSERT [dbo].[Tasks] ([ID], [Description], [From], [CreatedBy], [Remark], [Status], [CreatedDate], [DueDate], [ParentID], [Level], [Seen], [ProjectID], [JobName]) VALUES (5, N'To Do List', N'Swook', 1, NULL, 1, CAST(N'2020-02-05T00:00:00.0000000' AS DateTime2), CAST(N'2020-02-10T00:00:00.0000000' AS DateTime2), 4, 1, 0, 1, NULL)
GO
SET IDENTITY_INSERT [dbo].[Tasks] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([ID], [Username], [PasswordHash], [PasswordSalt], [Email], [RoleID], [OCID]) VALUES (1, N'admin', NULL, NULL, N'asdas', 1, 1)
GO
INSERT [dbo].[Users] ([ID], [Username], [PasswordHash], [PasswordSalt], [Email], [RoleID], [OCID]) VALUES (2, N'user', NULL, NULL, N'asdas', 2, 2)
GO
INSERT [dbo].[Users] ([ID], [Username], [PasswordHash], [PasswordSalt], [Email], [RoleID], [OCID]) VALUES (3, N'user', NULL, NULL, N'asdas', 2, 2)
GO
INSERT [dbo].[Users] ([ID], [Username], [PasswordHash], [PasswordSalt], [Email], [RoleID], [OCID]) VALUES (4, N'user', NULL, NULL, N'asdas', 2, 2)
GO
INSERT [dbo].[Users] ([ID], [Username], [PasswordHash], [PasswordSalt], [Email], [RoleID], [OCID]) VALUES (7, N'henry', 0x0313CF2E879BE5F2E7D34C19BB98301D3896702A43982F1233FBCF92F9A8B806D6197781B1FE2D5A5F1F220ED5F4F352ACEE34872F1EB33579E92FFB5229ACDE, 0xB834274B9A7A78D1039A53A711C68529756342F0872905336619898257052CEDAB431CD0F42DE69C5FA630DE5ECA816792F7B05E4A5787C7A43C0E9BC29666F20457A120B754758FF67478E3C7FF24EB96197A8AB7F90011E84E54976DFB38971CA280EB2F094DE6F3FB4E31177255ED1C7EC86F73283CF21D3196E33BB08C20, NULL, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [OCID]
GO
ALTER TABLE [dbo].[Managers]  WITH CHECK ADD  CONSTRAINT [FK_Managers_OCs_OCID] FOREIGN KEY([OCID])
REFERENCES [dbo].[OCs] ([ID])
GO
ALTER TABLE [dbo].[Managers] CHECK CONSTRAINT [FK_Managers_OCs_OCID]
GO
ALTER TABLE [dbo].[Managers]  WITH CHECK ADD  CONSTRAINT [FK_Managers_Projects_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Managers] CHECK CONSTRAINT [FK_Managers_Projects_ProjectID]
GO
ALTER TABLE [dbo].[Managers]  WITH CHECK ADD  CONSTRAINT [FK_Managers_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Managers] CHECK CONSTRAINT [FK_Managers_Users_UserID]
GO
ALTER TABLE [dbo].[OCUsers]  WITH CHECK ADD  CONSTRAINT [FK_OCUsers_OCs_OCID] FOREIGN KEY([OCID])
REFERENCES [dbo].[OCs] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCUsers] CHECK CONSTRAINT [FK_OCUsers_OCs_OCID]
GO
ALTER TABLE [dbo].[OCUsers]  WITH CHECK ADD  CONSTRAINT [FK_OCUsers_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[OCUsers] CHECK CONSTRAINT [FK_OCUsers_Users_UserID]
GO
ALTER TABLE [dbo].[Tags]  WITH CHECK ADD  CONSTRAINT [FK_Tags_Tasks_TaskID] FOREIGN KEY([TaskID])
REFERENCES [dbo].[Tasks] ([ID])
GO
ALTER TABLE [dbo].[Tags] CHECK CONSTRAINT [FK_Tags_Tasks_TaskID]
GO
ALTER TABLE [dbo].[Tags]  WITH CHECK ADD  CONSTRAINT [FK_Tags_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Tags] CHECK CONSTRAINT [FK_Tags_Users_UserID]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Projects_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_Projects_ProjectID]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Users_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_Users_CreatedBy]
GO
ALTER TABLE [dbo].[TeamMembers]  WITH CHECK ADD  CONSTRAINT [FK_TeamMembers_OCs_OCID] FOREIGN KEY([OCID])
REFERENCES [dbo].[OCs] ([ID])
GO
ALTER TABLE [dbo].[TeamMembers] CHECK CONSTRAINT [FK_TeamMembers_OCs_OCID]
GO
ALTER TABLE [dbo].[TeamMembers]  WITH CHECK ADD  CONSTRAINT [FK_TeamMembers_Projects_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeamMembers] CHECK CONSTRAINT [FK_TeamMembers_Projects_ProjectID]
GO
ALTER TABLE [dbo].[TeamMembers]  WITH CHECK ADD  CONSTRAINT [FK_TeamMembers_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeamMembers] CHECK CONSTRAINT [FK_TeamMembers_Users_UserID]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles_RoleID] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([ID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles_RoleID]
GO
