-- =============================================
-- Script to Configure Linked Server
-- Run this on the BHVEDPSNET server (192.168.100.50)
-- to connect to the Exam database server (192.168.100.146)
-- =============================================

USE [master]
GO

-- Check if linked server already exists
IF EXISTS (SELECT * FROM sys.servers WHERE name = '192.168.100.146')
BEGIN
    PRINT 'Linked server already exists. Dropping existing linked server...'
    EXEC sp_dropserver '192.168.100.146', 'droplogins'
END
GO

-- Add the linked server
EXEC sp_addlinkedserver 
    @server = '192.168.100.146',
    @srvproduct = 'SQL Server';
GO

-- Configure authentication
-- Option 1: SQL Server Authentication (uncomment and modify if needed)
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = '192.168.100.146',
    @useself = 'false',
    @locallogin = NULL,
    @rmtuser = 'admin',  -- Change to your remote server username
    @rmtpassword = 'universal1!';  -- Change to your remote server password
GO

-- Option 2: Windows Authentication (uncomment if you want to use Windows Auth instead)
-- EXEC sp_addlinkedsrvlogin 
--     @rmtsrvname = '192.168.100.146',
--     @useself = 'true';
-- GO

-- Configure RPC (Remote Procedure Call) options for better performance
EXEC sp_serveroption 
    @server = '192.168.100.146',
    @optname = 'rpc',
    @optvalue = 'true';
GO

EXEC sp_serveroption 
    @server = '192.168.100.146',
    @optname = 'rpc out',
    @optvalue = 'true';
GO

-- Test the linked server connection
PRINT 'Testing linked server connection...'
BEGIN TRY
    SELECT TOP 1 'Connection Successful!' AS Status
    FROM [192.168.100.146].[Corno.Bharati.OnlineExam].[dbo].[Exam] 
    WHERE 1=0;
    PRINT 'Linked server configured successfully!'
END TRY
BEGIN CATCH
    PRINT 'Error testing linked server:'
    PRINT ERROR_MESSAGE()
END CATCH
GO


