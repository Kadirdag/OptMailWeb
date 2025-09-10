Eklenecek olan tüm kullanıcılar bu listede olacaktır.
```
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Mail_Kullanici](
	[SYS_NO] [int] IDENTITY(1,1) NOT NULL,
	[BolumId] [int] NOT NULL,
	[KurumId] [int] NOT NULL,
	[AraciKurumId] [int] NOT NULL,
	[AdSoyad] [nvarchar](100) NOT NULL,
	[Telefon] [nvarchar](50) NULL,
	[Mail] [nvarchar](100) NOT NULL,
	[Adres] [nvarchar](200) NULL,
	[Notlar] [nvarchar](max) NULL,
	[Aktif] [bit] NULL,
	[KAYIT_ANI] [datetime] NULL,
	[GUNCELLEME_ANI] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SYS_NO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Mail_Kullanici] ADD  DEFAULT ((1)) FOR [Aktif]
GO

ALTER TABLE [dbo].[Mail_Kullanici] ADD  DEFAULT (getdate()) FOR [KAYIT_ANI]
GO

```

Filtreleme kısmında 3. ve son filtreme olduğu için bolumıd ve aracı kurum ıd ile filtrelenmiş data gelmesi gerekiyor.
```
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Birimler](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ad] [nvarchar](100) NOT NULL,
	[BolumId] [int] NOT NULL,
	[KurumId] [int] NOT NULL,
	[AraciKurumId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
```
Örnek INSERT kodu
```
SET IDENTITY_INSERT Birimler ON
GO
INSERT INTO Birimler([Id],[Ad],[BolumId],[KurumId],[AraciKurumId])
SELECT 1,N'Operasyon',1,1,1
UNION ALL
SELECT 2,N'Muhasebe',1,2,1
UNION ALL
SELECT 3,N'Takas',2,3,1
UNION ALL
SELECT 4,N'Bilgi İşlem',2,4,1
SET IDENTITY_INSERT Birimler OFF
GO

```



Aracı KURUM listesi bu tabloda tutuluyor

```

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AraciKurum](
	[AraciKurumId] [int] IDENTITY(1,1) NOT NULL,
	[Ad] [nvarchar](100) NOT NULL,
	[BolumId] [int] NOT NULL,
 CONSTRAINT [PK__AraciKur__5B3A125B8F777A47] PRIMARY KEY CLUSTERED 
(
	[AraciKurumId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AraciKurum] ADD  CONSTRAINT [DF_AraciKurum_Bolum]  DEFAULT ((0)) FOR [BolumId]
GO
```


Bölüm listesini tutan tablo
```

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Bolum](
	[BolumId] [int] IDENTITY(1,1) NOT NULL,
	[Ad] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BolumId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
```





"""""""""""""""""""""""""""""""""""" PROSEDÜRLER """"""""""""""""""""""""""""""""""""






Filtrelemeyi sağlatan prosedür.
```

CREATE PROCEDURE [dbo].[SP_GET_FILTERED_USERS]
    @BOLUM_ID INT = 0,
    @ARACI_KURUM_ID INT = 0,
    @KURUM_ID INT = 0
AS
BEGIN
    SELECT 
        k.SYS_NO AS Id,
        k.AdSoyad AS AdSoyad,
        k.Mail,
        k.Telefon
    FROM Mail_Kullanici k
    WHERE k.Aktif = 1
      AND (@BOLUM_ID = 0 OR k.BolumId = @BOLUM_ID)
      AND (@ARACI_KURUM_ID = 0 OR k.AraciKurumId = @ARACI_KURUM_ID)
      AND (@KURUM_ID = 0 OR k.KurumId = @KURUM_ID)
      --AND (@BIRIM_ID = 0 OR k.B= @BIRIM_ID)
END

GO
```





