-- -- Create a new stored procedure called 'spuiUser' in schema 'dbo'
-- -- Drop the stored procedure if it already exists
-- IF EXISTS (
-- SELECT *
--   FROM INFORMATION_SCHEMA.ROUTINES
-- WHERE SPECIFIC_SCHEMA = N'dbo'
--   AND SPECIFIC_NAME = N'spuiUser'
-- )
-- DROP PROCEDURE dbo.spuiUser
-- GO
-- -- Create the stored procedure in the specified schema
-- CREATE PROCEDURE dbo.spuiUser
--   @User NVARCHAR(MAX)   
  
-- AS
-- BEGIN
--       DECLARE @Result BIT = 0;
--       BEGIN TRANSACTION 
--         BEGIN TRY
   
--             INSERT INTO   [dbo].[User] 
--             (Name,
--              Email, 
--              PasswordHash
--              )
--             SELECT [Name], Email, PasswordHash
--             FROM OPENJSON(@User)
--             WITH(
--                [Name] NVARCHAR(100) '$."Name"',
--                 Email NVARCHAR(100) '$."Email"',
--                 PasswordHash NVARCHAR(255) '$."PasswordHash"'
--             )
--             SELECT @Result = 1
--             SELECT @Result AS Result;
         
--           ROLLBACK TRANSACTION
--         END TRY
--         BEGIN CATCH
--         SELECT @Result  AS Result;
--         SELECT
--         (
--              SELECT 
--             ERROR_NUMBER() AS ErrorNumber,
--             ERROR_MESSAGE() AS ErrorMessage,
--             ERROR_SEVERITY() AS ErrorSeverity,
--             ERROR_STATE() AS ErrorState,
--             ERROR_LINE() AS ErrorLine,
--             ERROR_PROCEDURE() AS ErrorProcedure
--              FOR JSON PATH ,WITHOUT_ARRAY_WRAPPER

--         ) AS ErrorDetails;
      
--         ROLLBACK TRANSACTION

--         END CATCH
-- END

-- GO
-- -- example to execute the stored procedure we just created
-- DECLARE @Data NVARCHAR(MAX) = N'[
--     {
--         "Name": "John Doe",
--         "Email": "" 
--         "PasswordHash": "hashed_password"
--         ]'
-- EXECUTE dbo.spuiUser  @Data
-- GO
