-- =============================================
-- Quick Performance Report Script
-- Purpose: Generate a quick overview of database performance metrics
-- Run this script to get a snapshot of current database health
-- =============================================

-- =============================================
-- CORNO.BHARATI.ONLINEEXAM DATABASE REPORT
-- =============================================

PRINT '========================================'
PRINT 'CORNO.BHARATI.ONLINEEXAM DATABASE REPORT'
PRINT '========================================'
GO

USE [Corno.Bharati.OnlineExam]
GO

-- Database Size
SELECT 
    'Database Size' AS Metric,
    CAST(SUM(size) * 8.0 / 1024 AS DECIMAL(10,2)) AS Value_MB,
    CAST(SUM(size) * 8.0 / 1024 / 1024 AS DECIMAL(10,2)) AS Value_GB
FROM sys.database_files
WHERE type = 0  -- Data files only
GO

-- Total Indexes Count
SELECT 
    'Total Indexes' AS Metric,
    COUNT(*) AS Count
FROM sys.indexes
WHERE object_id > 100
    AND name IS NOT NULL
GO

-- Fragmented Indexes Count
SELECT 
    'Fragmented Indexes (>10%)' AS Metric,
    COUNT(*) AS Count
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10
    AND ips.page_count > 100
    AND i.name IS NOT NULL
GO

-- Missing Indexes Count
SELECT 
    'Missing Index Recommendations' AS Metric,
    COUNT(*) AS Count
FROM sys.dm_db_missing_index_groups mig
INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
GO

-- Top 5 Largest Tables
SELECT TOP 5
    'Top 5 Largest Tables' AS Metric,
    t.NAME AS TableName,
    CAST(SUM(a.total_pages) * 8.0 / 1024 AS DECIMAL(10,2)) AS Size_MB,
    p.rows AS RowCount
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255
GROUP BY t.Name, p.Rows
ORDER BY SUM(a.total_pages) DESC
GO

-- =============================================
-- BHVEDPSNET DATABASE REPORT
-- =============================================

PRINT '========================================'
PRINT 'BHVEDPSNET DATABASE REPORT'
PRINT '========================================'
GO

USE [BHVEDPSNET]
GO

-- Database Size
SELECT 
    'Database Size' AS Metric,
    CAST(SUM(size) * 8.0 / 1024 AS DECIMAL(10,2)) AS Value_MB,
    CAST(SUM(size) * 8.0 / 1024 / 1024 AS DECIMAL(10,2)) AS Value_GB
FROM sys.database_files
WHERE type = 0
GO

-- Total Indexes Count
SELECT 
    'Total Indexes' AS Metric,
    COUNT(*) AS Count
FROM sys.indexes
WHERE object_id > 100
    AND name IS NOT NULL
GO

-- Fragmented Indexes Count
SELECT 
    'Fragmented Indexes (>10%)' AS Metric,
    COUNT(*) AS Count
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10
    AND ips.page_count > 100
    AND i.name IS NOT NULL
GO

-- Missing Indexes Count
SELECT 
    'Missing Index Recommendations' AS Metric,
    COUNT(*) AS Count
FROM sys.dm_db_missing_index_groups mig
INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
GO

-- Top 5 Largest Tables
SELECT TOP 5
    'Top 5 Largest Tables' AS Metric,
    t.NAME AS TableName,
    CAST(SUM(a.total_pages) * 8.0 / 1024 AS DECIMAL(10,2)) AS Size_MB,
    p.rows AS RowCount
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255
GROUP BY t.Name, p.Rows
ORDER BY SUM(a.total_pages) DESC
GO

PRINT '========================================'
PRINT 'REPORT COMPLETED'
PRINT '========================================'
GO






