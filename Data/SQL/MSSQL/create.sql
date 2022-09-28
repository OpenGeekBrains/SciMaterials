USE SciMaterials;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[comments_files]') AND type in (N'U'))
	DROP TABLE comments_files
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[files_tags]') AND type in (N'U'))
	DROP TABLE files_tags
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[comments_files]') AND type in (N'U'))
	DROP TABLE comments_files
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[files]') AND type in (N'U'))
	DROP TABLE [dbo].files
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tags]') AND type in (N'U'))
	DROP TABLE tags
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[content_types]') AND type in (N'U'))
	DROP TABLE content_types
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[categories]') AND type in (N'U'))
	DROP TABLE categories
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[comments]') AND type in (N'U'))
	DROP TABLE comments
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[users]') AND type in (N'U'))
	DROP TABLE [dbo].users
GO

CREATE TABLE users
(
    id uniqueidentifier NOT NULL PRIMARY KEY,
    name nvarchar(255) NOT NULL UNIQUE,
    email nvarchar(255) NOT NULL
);

CREATE TABLE files
(
    id uniqueidentifier NOT NULL PRIMARY KEY,
    name nvarchar(255) NOT NULL UNIQUE,
    title nvarchar(255) NOT NULL,
    description nvarchar(MAX),
    size [bigint] NOT NULL,
	owner_id uniqueidentifier NOT NULL,
	created_at datetime NOT NULL,
    content_type_id uniqueidentifier,
	category_id uniqueidentifier NOT NULL
);

CREATE TABLE content_types
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	name nvarchar(100) NOT NULL UNIQUE
);

CREATE TABLE tags
(
    id uniqueidentifier NOT NULL PRIMARY KEY,
	name nvarchar(255) NOT NULL UNIQUE
);

CREATE TABLE files_tags
(
    file_id uniqueidentifier NOT NULL,
	tag_id uniqueidentifier NOT NULL,
	PRIMARY KEY (file_id, tag_id)
);

CREATE TABLE comments
(
    id uniqueidentifier NOT NULL PRIMARY KEY,
	parent_id uniqueidentifier,
	owner_id uniqueidentifier NOT NULL,
	text nvarchar(max),
	created_at datetime NOT NULL
);

CREATE TABLE comments_files
(
    comment_id uniqueidentifier NOT NULL,
	file_id uniqueidentifier NOT NULL,
	PRIMARY KEY (comment_id, file_id)
);

CREATE TABLE categories
(
    id uniqueidentifier NOT NULL PRIMARY KEY,
	parent_id uniqueidentifier,
	name nvarchar(200),
	description nvarchar(max),
	created_at datetime NOT NULL
);

ALTER TABLE files
	ADD CONSTRAINT files_content_type_id_fk
	FOREIGN KEY (content_type_id)
	REFERENCES content_types(id);

ALTER TABLE files
	ADD CONSTRAINT files_category_id_fk
	FOREIGN KEY (category_id)
	REFERENCES categories(id);

ALTER TABLE files_tags
	ADD CONSTRAINT files_tags_tag_id_fk
	FOREIGN KEY (tag_id)
	REFERENCES tags(id);

ALTER TABLE files_tags
	ADD CONSTRAINT files_tags_file_id_fk
	FOREIGN KEY (file_id)
	REFERENCES files(id);

ALTER TABLE comments_files
	ADD CONSTRAINT comments_files_comment_id_fk
	FOREIGN KEY (comment_id)
	REFERENCES comments(id);

ALTER TABLE comments_files
	ADD CONSTRAINT comments_files_file_id_fk
	FOREIGN KEY (file_id)
	REFERENCES files(id);

ALTER TABLE files
	ADD CONSTRAINT files_owner_id_fk
	FOREIGN KEY(owner_id)
	REFERENCES users(id);

ALTER TABLE comments
	ADD CONSTRAINT comments_owner_id_fk
	FOREIGN KEY(owner_id)
	REFERENCES users(id);
