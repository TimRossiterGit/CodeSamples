USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[Website_GetAllBySlug]    Script Date: 3/17/2017 9:09:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[Website_GetAllBySlug]
	@Slug nvarchar(50)

AS
/*------------TEST CODE--------------

Declare @Slug nvarchar(50) = 'bringpro'

Execute dbo.Website_GetAllBySlug
	@Slug


-------------------------------------*/


BEGIN

	SELECT [w].[Id]
      ,[w].[Name]
      ,[w].[Slug]
      ,[w].[Description]
      ,[w].[Url]
      ,[w].[MediaId]
      ,[w].[DateCreated]
      ,[w].[DateModified]
      ,[w].[Phone]
	  ,[w].[AddressId]
	  ,[m].[Id]
	  ,[m].[Created]
	  ,[m].[Modified]
	  ,[m].[Url]
	  ,[m].[MediaType]
	  ,[m].[UserId]
	  ,[m].[Title]
	  ,[m].[Description]
	  ,[m].[ExternalMediaId]
	  ,[m].[FileType]
	  ,[ad].[AddressId]
	  ,[ad].[DateCreated]
	  ,[ad].[DateModified]
	  ,[ad].[UserId]
	  ,[ad].[Name]
	  ,[ad].[ExternalPlaceId]
	  ,[ad].[Line1]
	  ,[ad].[Line2]
	  ,[ad].[City]
	  ,[ad].[StateId]
	  ,[ad].[ZipCode]
	  ,[ad].[Latitude]
	  ,[ad].[Longitude]

	From dbo.Website w
	Left Join dbo.Media m
	on w.MediaId = m.Id
	Left Join dbo.Address ad
	on w.AddressId = ad.AddressId

	Where Slug = @Slug

END