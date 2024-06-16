USE [dbAutenticacaoDeUsuarios]
GO

/****** Object:  Table [dbo].[Usuario]    Script Date: 16/06/2024 15:55:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Usuario](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Nome] [nvarchar](max) NOT NULL,
	[Senha] [nvarchar](max) NOT NULL,
	[Imagem] [nvarchar](max) NULL,
	[DataCadastro] [datetime] NOT NULL,
	[Ativo] [bit] NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


