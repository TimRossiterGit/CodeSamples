USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[WebsiteSettings_GetByWebsiteSlug_Query]    Script Date: 3/17/2017 9:11:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[WebsiteSettings_GetByWebsiteSlug_Query]

        @Slug nvarchar(50)
       ,@CurrentPage int = 1
	   ,@ItemsPerPage int = 10
	   ,@Query nvarchar(50) = null
	   ,@QueryCategory int = null 
	   ,@QuerySettingSection int = null
	   ,@QuerySettingType int = null

AS
/* ----TEST CODE
	Declare @CurrentPage int = 1
    ,@ItemsPerpage int = 10
    ,@Query nvarchar(50) = 'login'
	,@QueryCategory int = 4
	,@QuerySettingSection int = 1
	,@QuerySettingType int = 1


		Execute WebsiteSettings_GetByWebsiteSlug_Query 'bringpro'
		 ,@CurrentPage
	     ,@ItemsPerPage
	     ,@Query 
	     ,@QueryCategory 
	     ,@QuerySettingSection 
	     ,@QuerySettingType 

		
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

		Where (w.Slug = @Slug)
		AND (@QueryCategory IS NULL OR s.Category = @QueryCategory)
	    AND ( @QuerySettingSection IS NULL OR s.SettingSection = @QuerySettingSection)
	    AND ( @QuerySettingType IS NULL OR s.SettingType = @QuerySettingType)
	    AND ( @Query IS NULL OR 
		([s].[Name] LIKE '%'+@Query+'%') OR
		([s].[Description] LIKE '%'+@Query+'%') OR
		([s].[SettingSlug] LIKE '%'+@Query+'%'))

		ORDER BY s.Id
	    OFFSET ((@CurrentPage - 1) * @ItemsPerPage) ROWS
        FETCH NEXT  @ItemsPerPage ROWS ONLY 

		SELECT COUNT('s.Id')
		

		From dbo.WebsiteSettings as ws
		Left join dbo.Website as w
		on ws.WebsiteId = w.Id
		Left join dbo.Settings as s		
		on ws.SettingsId = s.Id
		Left Join dbo.Media as m
		on ws.MediaId = m.Id

		Where (w.Slug = @Slug)
		AND (@QueryCategory IS NULL OR s.Category = @QueryCategory)
	    AND ( @QuerySettingSection IS NULL OR s.SettingSection = @QuerySettingSection)
	    AND ( @QuerySettingType IS NULL OR s.SettingType = @QuerySettingType)
	    AND ( @Query IS NULL OR 
		([s].[Name] LIKE '%'+@Query+'%') OR
		([s].[Description] LIKE '%'+@Query+'%') OR
		([s].[SettingSlug] LIKE '%'+@Query+'%'))
END