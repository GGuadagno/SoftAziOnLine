USE [05Scadenze]
GO

/****** Object:  Table [dbo].[EmailInviateTOggetto]    Script Date: 07/02/2023 10.32.57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmailInviateTOggetto](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Stato] [int] NULL,
	[DataInvio] [datetime] NULL,
	[Note] [ntext] NULL,
	[OLDStato] [int] NULL,
	[Destinatario] [nvarchar](100) NULL,
	[Codice_CoGe] [nvarchar](16) NULL,
	[Rag_Soc] [nvarchar](50) NULL,
	[Denominazione] [nvarchar](50) NULL,
	[Riferimento] [nvarchar](150) NULL,
	[Email] [nvarchar](100) NULL,
	[EmailInvioScad] [nvarchar](100) NULL,
	[EmailInvioFatt] [nvarchar](100) NULL,
	[PECEmail] [nvarchar](310) NULL,
	[Indirizzo] [nvarchar](50) NULL,
	[CAP] [nvarchar](5) NULL,
	[Localita] [nvarchar](50) NULL,
	[Provincia] [nvarchar](2) NULL,
	[Codice_Fiscale] [nvarchar](16) NULL,
	[Partita_IVA] [nvarchar](20) NULL,
	[Telefono1] [nvarchar](30) NULL,
	[Telefono2] [nvarchar](30) NULL,
	[Categorie_desc] [nvarchar](50) NULL,
	[Agenti_desc] [nvarchar](50) NULL
 CONSTRAINT [PK_EmailInviateTOggetto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[EmailInviateTOggetto]  WITH CHECK ADD  CONSTRAINT [FK_EmailInviateT_EmailInviateTOggetto] FOREIGN KEY([Id])
REFERENCES [dbo].[EmailInviateTOggetto] ([Id])
GO

ALTER TABLE [dbo].[EmailInviateTOggetto] CHECK CONSTRAINT [FK_EmailInviateT_EmailInviateTOggetto]
GO

/****** Object:  Index [IX_Stato]    Script Date: 07/02/2023 10.53.46 ******/
CREATE NONCLUSTERED INDEX [IX_Stato] ON [dbo].[EmailInviateTOggetto]
(
	[Stato] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
