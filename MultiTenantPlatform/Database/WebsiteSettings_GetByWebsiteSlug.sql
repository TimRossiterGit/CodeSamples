USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[WebsiteSettings_GetByWebsiteSlug]    Script Date: 3/26/2017 4:06:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[WebsiteSettings_GetByWebsiteSlug]

@Slug nvarchar(50) = 'bringpro'

AS
/* ----TEST CODE
		Execute WebsiteSettings_GetByWebsiteSlug 'bringpro'

		
*/


BEGIN

		SELECT [ws].[Id]
			  ,[ws].[SettingsId]
			  ,[ws].[WebsiteId]
			  ,[ws].[SettingsValue]
			  ,[ws].[UserId]
			  ,[ws].[MediaId]
			  ,[ws].[DateAdded]
			  ,[ws].[DateModified]
			  ,[w].[Id]
			  ,[w].[Name]
			  ,[w].[Slug]
			  ,[w].[Description]
			  ,[w].[Url]
			  ,[w].[MediaId]
			  ,[w].[DateCreated]
			  ,[w].[DateModified]
			  ,[w].[Phone]
			  --,[w].[BackgroundColor]
			  --,[w].[HeaderColor]
			  ,[s].[Id]
			  ,[s].[Category]
			  ,[s].[Name]
			  ,[s].[DateCreated]
			  ,[s].[DateModified]
			  ,[s].[SettingType]
			  ,[s].[Description]
			  ,[s].[SettingSlug]
			  ,[s].[SettingSection]
			  ,[m].[Id]
			  ,[m].[Url]
			  ,[m].[MediaType]
			  ,[m].[UserId]
			  ,[m].[Title]
			  ,[m].[Description]
			  ,[m].[ExternalMediaId]
			  ,[m].[FileType]
			  ,[m].[Created]
			  ,[m].[Modified]

		From dbo.WebsiteSettings as ws
		Left join dbo.Website as w
		on ws.WebsiteId = w.Id
		Left join dbo.Settings as s		
		on ws.SettingsId = s.Id
		Left Join dbo.Media as m
		on ws.MediaId = m.Id

		Where w.Slug = @Slug
END