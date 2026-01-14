-- =============================================
-- Database Maintenance and Statistics Update Script
-- Purpose: Update statistics and perform maintenance tasks
-- Databases: Corno.Bharati.OnlineExam, BHVEDPSNET
-- =============================================

-- =============================================
-- SECTION 1: UPDATE STATISTICS
-- Statistics help SQL Server create optimal query plans
-- =============================================

PRINT 'Starting statistics update for Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

-- Update statistics with full scan for better accuracy
EXEC sp_updatestats
PRINT 'Statistics updated for Corno.Bharati.OnlineExam'
GO

-- Update statistics for specific large tables
UPDATE STATISTICS [dbo].[Exam] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Registration] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Student] WITH FULLSCAN
UPDATE STATISTICS [dbo].[ExamSubject] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Revalution] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Convocation] WITH FULLSCAN
UPDATE STATISTICS [dbo].[EnvironmentStudy] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Appointment] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Question] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Paper] WITH FULLSCAN
UPDATE STATISTICS [dbo].[TimeTable] WITH FULLSCAN
UPDATE STATISTICS [dbo].[AnswerSheet] WITH FULLSCAN
PRINT 'Large table statistics updated for Corno.Bharati.OnlineExam'
GO

PRINT 'Starting statistics update for BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

-- Update statistics with full scan
EXEC sp_updatestats
PRINT 'Statistics updated for BHVEDPSNET'
GO

-- Update statistics for specific large tables
UPDATE STATISTICS [dbo].[TBL_STUDENT_INFO] WITH FULLSCAN
UPDATE STATISTICS [dbo].[TBL_STUDENT_EXAMS] WITH FULLSCAN
UPDATE STATISTICS [dbo].[TBL_STUDENT_SUBJECT] WITH FULLSCAN
UPDATE STATISTICS [dbo].[TBL_STUDENT_PAP_MARKS] WITH FULLSCAN
UPDATE STATISTICS [dbo].[TBL_STUDENT_CAT_MARKS] WITH FULLSCAN
UPDATE STATISTICS [dbo].[TBL_STUDENT_COURSE] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Tbl_COURSE_MSTR] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Tbl_COURSE_PART_MSTR] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Tbl_COLLEGE_MSTR] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Tbl_SUBJECT_MSTR] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Tbl_TIMETABLE_TRX] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Tbl_REG_TEMP] WITH FULLSCAN
PRINT 'Large table statistics updated for BHVEDPSNET'
GO

-- =============================================
-- SECTION 2: REBUILD/REORGANIZE INDEXES
-- This reduces fragmentation and improves performance
-- =============================================

PRINT 'Starting index maintenance for Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

-- Reorganize indexes with fragmentation between 10% and 30%
-- Rebuild indexes with fragmentation above 30%
DECLARE @TableName NVARCHAR(128)
DECLARE @IndexName NVARCHAR(128)
DECLARE @Fragmentation FLOAT
DECLARE @SQL NVARCHAR(MAX)

DECLARE index_cursor CURSOR FOR
SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.avg_fragmentation_in_percent AS Fragmentation
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10
    AND ips.page_count > 1000  -- Only process indexes with more than 1000 pages
    AND i.name IS NOT NULL

OPEN index_cursor
FETCH NEXT FROM index_cursor INTO @TableName, @IndexName, @Fragmentation

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @Fragmentation > 30
    BEGIN
        SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [dbo].[' + @TableName + '] REBUILD WITH (ONLINE = ON, FILLFACTOR = 90)'
        PRINT 'Rebuilding index: ' + @IndexName + ' on table: ' + @TableName + ' (Fragmentation: ' + CAST(@Fragmentation AS VARCHAR(10)) + '%)'
        EXEC sp_executesql @SQL
    END
    ELSE
    BEGIN
        SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [dbo].[' + @TableName + '] REORGANIZE'
        PRINT 'Reorganizing index: ' + @IndexName + ' on table: ' + @TableName + ' (Fragmentation: ' + CAST(@Fragmentation AS VARCHAR(10)) + '%)'
        EXEC sp_executesql @SQL
    END
    
    FETCH NEXT FROM index_cursor INTO @TableName, @IndexName, @Fragmentation
END

CLOSE index_cursor
DEALLOCATE index_cursor

PRINT 'Index maintenance completed for Corno.Bharati.OnlineExam'
GO

PRINT 'Starting index maintenance for BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

DECLARE index_cursor2 CURSOR FOR
SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.avg_fragmentation_in_percent AS Fragmentation
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10
    AND ips.page_count > 1000
    AND i.name IS NOT NULL

OPEN index_cursor2
FETCH NEXT FROM index_cursor2 INTO @TableName, @IndexName, @Fragmentation

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @Fragmentation > 30
    BEGIN
        SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [dbo].[' + @TableName + '] REBUILD WITH (ONLINE = ON, FILLFACTOR = 90)'
        PRINT 'Rebuilding index: ' + @IndexName + ' on table: ' + @TableName + ' (Fragmentation: ' + CAST(@Fragmentation AS VARCHAR(10)) + '%)'
        EXEC sp_executesql @SQL
    END
    ELSE
    BEGIN
        SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [dbo].[' + @TableName + '] REORGANIZE'
        PRINT 'Reorganizing index: ' + @IndexName + ' on table: ' + @TableName + ' (Fragmentation: ' + CAST(@Fragmentation AS VARCHAR(10)) + '%)'
        EXEC sp_executesql @SQL
    END
    
    FETCH NEXT FROM index_cursor2 INTO @TableName, @IndexName, @Fragmentation
END

CLOSE index_cursor2
DEALLOCATE index_cursor2

PRINT 'Index maintenance completed for BHVEDPSNET'
GO

-- =============================================
-- SECTION 3: CLEAR PROCEDURE CACHE
-- Use with caution - only in maintenance windows
-- =============================================

-- Uncomment the following lines during maintenance windows
-- DBCC FREEPROCCACHE
-- PRINT 'Procedure cache cleared'

-- =============================================
-- SECTION 4: UPDATE DATABASE OPTIONS
-- =============================================

USE [Corno.Bharati.OnlineExam]
GO

-- Set auto update statistics to async for better performance
ALTER DATABASE [Corno.Bharati.OnlineExam] SET AUTO_UPDATE_STATISTICS_ASYNC ON
PRINT 'Auto update statistics async enabled for Corno.Bharati.OnlineExam'
GO

USE [BHVEDPSNET]
GO

ALTER DATABASE [BHVEDPSNET] SET AUTO_UPDATE_STATISTICS_ASYNC ON
PRINT 'Auto update statistics async enabled for BHVEDPSNET'
GO

-- =============================================
-- SECTION 5: SET COMPATIBILITY LEVEL (if needed)
-- =============================================

-- Check current compatibility level
-- ALTER DATABASE [Corno.Bharati.OnlineExam] SET COMPATIBILITY_LEVEL = 150  -- SQL Server 2019
-- ALTER DATABASE [BHVEDPSNET] SET COMPATIBILITY_LEVEL = 150

PRINT 'Database maintenance and statistics update completed!'
GO






