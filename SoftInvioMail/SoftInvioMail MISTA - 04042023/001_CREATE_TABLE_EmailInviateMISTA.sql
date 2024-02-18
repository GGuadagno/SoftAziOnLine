USE [05Scadenze]
GO

/****** Object:  Table [dbo].[EmailInviateMISTA]    Script Date: 07/02/2023 10.32.57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmailInviateMISTA](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Stato] [int] NULL,
	[DataInvio] [datetime] NULL,
	[Note] [ntext] NULL,
	[Destinatario] [nvarchar](255) NULL
 CONSTRAINT [PK_EmailInviateMISTA] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO