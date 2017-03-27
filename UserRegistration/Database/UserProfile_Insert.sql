USE [C28]
GO
/****** Object:  StoredProcedure [dbo].[UserProfiles_Insert]    Script Date: 3/26/2017 6:48:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER Proc [dbo].[UserProfiles_Insert]
@UserId nvarchar(128)
,@FirstName nvarchar(100)
,@LastName nvarchar(100)
,@ExternalUserId nvarchar(128) = null
,@TokenHash uniqueidentifier = null


As


/*
Declare @UserProfileId int = 0

Declare @UserId nvarchar(128) = '27'
,@FirstName nvarchar(100) = 'Test'
,@LastName nvarchar(100) = 'McTestPerson'
,@ExternalUserId nvarchar(128) = 'lh904287oihf'
,@TokenHash uniqueidentifier = '3D878240-3775-44F2-A884-3768C4ED6BCF'

INSERT INTO [dbo].[UserProfiles]
           ([UserId]
           ,[FirstName]
           ,[LastName]
           ,[ExternalUserId]
		   ,TokenHash)
     VALUES
           (@UserId
           ,@FirstName
           ,@LastName
           ,@ExternalUserId
		   ,@TokenHash)

					SET @UserProfileId = SCOPE_IDENTITY()

SELECT *
FROM dbo.UserProfiles
WHERE UserProfileId = @UserProfileId
*/

BEGIN

INSERT INTO [dbo].[UserProfiles]
           ([UserId]
           ,[FirstName]
           ,[LastName]
           ,[ExternalUserId]
		   ,TokenHash)
     
	 VALUES
           (@UserId
           ,@FirstName
           ,@LastName
           ,@ExternalUserId
		   ,@TokenHash)



END