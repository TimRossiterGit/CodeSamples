USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[Settings_SelectAll_Search]    Script Date: 3/17/2017 9:07:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER proc [dbo].[Settings_SelectAll_Search]
		@CurrentPage int = 1
	   ,@ItemsPerPage int = 10
	   ,@Query nvarchar(50) = null
	   ,@QueryCategory int = null 
	   ,@QuerySettingSection int = null
	   ,@QuerySettingType int = null
	   --,@QueryName nvarchar(50) = null
	   --,@QueryDescription nvarchar(500)  = null
	   --,@QuerySettingSlug nvarchar(50)  = null
	
As
	/* ------------TEST CODE----------------
	
	Declare @CurrentPage int = 1
,@ItemsPerpage int = 10
,@QueryName nvarchar(50) = 'login'
		execute dbo.Settings_SelectAll_Search

	*/
BEGIN


	SELECT [Id]
	  ,[Category]
      ,[Name]
      ,[DateCreated]
      ,[DateModified]
      ,[SettingType]
      ,[Description]
	  ,[SettingSlug]
	  ,[SettingSection]

  FROM [dbo].[Settings]

	WHERE (@QueryCategory IS NULL OR Category = @QueryCategory)
	AND ( @QuerySettingSection IS NULL OR SettingSection = @QuerySettingSection)
	AND ( @QuerySettingType IS NULL OR SettingType = @QuerySettingType)
	AND ( @Query IS NULL OR 
		([Name] LIKE '%'+@Query+'%') OR
		([Description] LIKE '%'+@Query+'%') OR
		([SettingSlug] LIKE '%'+@Query+'%'))

	
	  ORDER BY Id
	OFFSET ((@CurrentPage - 1) * @ItemsPerPage) ROWS
             FETCH NEXT  @ItemsPerPage ROWS ONLY 
	
	
	SELECT COUNT('Id')

		FROM [dbo].[Settings]

		WHERE (@QueryCategory IS NULL OR Category = @QueryCategory)
	           AND ( @QuerySettingSection IS NULL OR SettingSection = @QuerySettingSection)
	           AND ( @QuerySettingType IS NULL OR SettingType = @QuerySettingType)
	           AND (@Query IS NULL OR 
		       ([Name] LIKE '%'+@Query+'%') OR
		       ([Description] LIKE '%'+@Query+'%') OR
		       ([SettingSlug] LIKE '%'+@Query+'%'))


END