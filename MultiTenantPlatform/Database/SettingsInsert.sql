USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[Settings_Insert]    Script Date: 3/17/2017 9:04:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER Proc [dbo].[Settings_Insert]
				 @Id int Output
				,@Category int
				,@Name nvarchar(50)				
				,@SettingType int
				,@Description nvarchar(500)
				,@SettingSlug nvarchar(50) = null
				,@SettingSection int = null
			

AS

/*-----------TEST CODE--------------

		Declare @Id int 
				,@Category int = 1
				,@Name nvarchar(50) = 'Anna'
				,@SettingType int = 1
				,@Description nvarchar(500) = 'Testing Description 456'
				,@SettingSlug nvarchar(50) = 'test'
				,@SettingSection int = 1

		Execute dbo.Settings_Insert
				 @Id Output
				,@Category 
				,@Name
				,@SettingType
				,@Description 
				,@SettingSlug
				,@SettingSection

		Select * 
		From dbo.Settings
				
*/

BEGIN

		INSERT INTO [dbo].[Settings]
           ([Category]
           ,[Name]
           ,[DateCreated]
           ,[DateModified]
           ,[SettingType]
           ,[Description]
		   ,[SettingSlug]
		   ,[SettingSection])

     VALUES
           ( @Category
           ,@Name
           ,GETUTCDATE()
           ,GETUTCDATE()
           ,@SettingType
           ,@Description
		   ,@SettingSlug
		   ,@SettingSection )

		   Set @Id = SCOPE_IDENTITY()

END