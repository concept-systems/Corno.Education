-- =============================================
-- Database Relationship Verification Script
-- Purpose: Verify foreign key relationships and identify missing constraints
-- Databases: Corno.Bharati.OnlineExam, BHVEDPSNET
-- =============================================

-- =============================================
-- SECTION 1: CHECK EXISTING FOREIGN KEY CONSTRAINTS
-- =============================================

PRINT 'Checking foreign key relationships in Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS ParentTable,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ParentColumn,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferencedColumn,
    fk.is_disabled AS IsDisabled,
    fk.is_not_trusted AS IsNotTrusted
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc ON fk.object_id = fc.constraint_object_id
ORDER BY ParentTable, ForeignKeyName
GO

PRINT 'Checking foreign key relationships in BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS ParentTable,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ParentColumn,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferencedColumn,
    fk.is_disabled AS IsDisabled,
    fk.is_not_trusted AS IsNotTrusted
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc ON fk.object_id = fc.constraint_object_id
ORDER BY ParentTable, ForeignKeyName
GO

-- =============================================
-- SECTION 2: IDENTIFY ORPHANED RECORDS
-- These are records that reference non-existent parent records
-- =============================================

PRINT 'Checking for orphaned records in Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

-- Check Exam table for orphaned StudentId references
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Exam' AND schema_id = SCHEMA_ID('dbo'))
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Student' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    SELECT 
        'Exam' AS TableName,
        'StudentId' AS ColumnName,
        COUNT(*) AS OrphanedRecords
    FROM [dbo].[Exam] e
    LEFT JOIN [dbo].[Student] s ON e.StudentId = s.Id
    WHERE s.Id IS NULL AND e.StudentId IS NOT NULL
END
GO

-- Check ExamSubject table for orphaned ExamId references
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ExamSubject' AND schema_id = SCHEMA_ID('dbo'))
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Exam' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    SELECT 
        'ExamSubject' AS TableName,
        'ExamId' AS ColumnName,
        COUNT(*) AS OrphanedRecords
    FROM [dbo].[ExamSubject] es
    LEFT JOIN [dbo].[Exam] e ON es.ExamId = e.Id
    WHERE e.Id IS NULL AND es.ExamId IS NOT NULL
END
GO

-- Check Registration table for orphaned PrnNo references
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Registration' AND schema_id = SCHEMA_ID('dbo'))
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Student' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    SELECT 
        'Registration' AS TableName,
        'PrnNo' AS ColumnName,
        COUNT(*) AS OrphanedRecords
    FROM [dbo].[Registration] r
    LEFT JOIN [dbo].[Student] s ON r.PrnNo = s.Prn
    WHERE s.Prn IS NULL AND r.PrnNo IS NOT NULL
END
GO

PRINT 'Checking for orphaned records in BHVEDPSNET...'
GO

USE [BHVEDPSNET]
GO

-- Check TBL_STUDENT_EXAMS for orphaned PRN references
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TBL_STUDENT_EXAMS' AND schema_id = SCHEMA_ID('dbo'))
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TBL_STUDENT_INFO' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    SELECT 
        'TBL_STUDENT_EXAMS' AS TableName,
        'Chr_FK_PRN_NO' AS ColumnName,
        COUNT(*) AS OrphanedRecords
    FROM [dbo].[TBL_STUDENT_EXAMS] se
    LEFT JOIN [dbo].[TBL_STUDENT_INFO] si ON se.Chr_FK_PRN_NO = si.Chr_PRN_NO
    WHERE si.Chr_PRN_NO IS NULL AND se.Chr_FK_PRN_NO IS NOT NULL
END
GO

-- Check TBL_STUDENT_SUBJECT for orphaned PRN references
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TBL_STUDENT_SUBJECT' AND schema_id = SCHEMA_ID('dbo'))
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TBL_STUDENT_INFO' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    SELECT 
        'TBL_STUDENT_SUBJECT' AS TableName,
        'Chr_FK_PRN_NO' AS ColumnName,
        COUNT(*) AS OrphanedRecords
    FROM [dbo].[TBL_STUDENT_SUBJECT] ss
    LEFT JOIN [dbo].[TBL_STUDENT_INFO] si ON ss.Chr_FK_PRN_NO = si.Chr_PRN_NO
    WHERE si.Chr_PRN_NO IS NULL AND ss.Chr_FK_PRN_NO IS NOT NULL
END
GO

-- Check TBL_STUDENT_SUBJECT for orphaned Subject references
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TBL_STUDENT_SUBJECT' AND schema_id = SCHEMA_ID('dbo'))
AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_SUBJECT_MSTR' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    SELECT 
        'TBL_STUDENT_SUBJECT' AS TableName,
        'NUM_FK_SUB_CD' AS ColumnName,
        COUNT(*) AS OrphanedRecords
    FROM [dbo].[TBL_STUDENT_SUBJECT] ss
    LEFT JOIN [dbo].[Tbl_SUBJECT_MSTR] sm ON ss.NUM_FK_SUB_CD = sm.Num_PK_SUB_CD
    WHERE sm.Num_PK_SUB_CD IS NULL AND ss.NUM_FK_SUB_CD IS NOT NULL
END
GO

-- =============================================
-- SECTION 3: SUGGESTED FOREIGN KEY CONSTRAINTS
-- These are relationships that should exist based on code analysis
-- =============================================

PRINT 'Suggested Foreign Key Constraints for Corno.Bharati.OnlineExam:'
PRINT '================================================================'
PRINT '1. Exam.StudentId -> Student.Id'
PRINT '2. Exam.InstanceId -> Instance.Id'
PRINT '3. Exam.CourseId -> Course.Id'
PRINT '4. Exam.CoursePartId -> CoursePart.Id'
PRINT '5. Exam.CollegeId -> College.Id'
PRINT '6. ExamSubject.ExamId -> Exam.Id'
PRINT '7. ExamSubject.SubjectId -> Subject.Id'
PRINT '8. Registration.InstanceId -> Instance.Id'
PRINT '9. Registration.CourseId -> Course.Id'
PRINT '10. Registration.FacultyId -> Faculty.Id'
PRINT '11. Revalution.StudentId -> Student.Id'
PRINT '12. RevalutionSubject.RevalutionId -> Revalution.Id'
PRINT '13. Convocation.StudentId -> Student.Id'
PRINT '14. EnvironmentStudy.StudentId -> Student.Id'
PRINT '15. Appointment.SubjectId -> Subject.Id'
PRINT '16. Appointment.FacultyId -> Faculty.Id'
PRINT '17. Question.SubjectId -> Subject.Id'
PRINT '18. Paper.SubjectId -> Subject.Id'
PRINT '19. AnswerSheet.StudentId -> Student.Id'
PRINT '20. AnswerSheet.ExamId -> Exam.Id'
PRINT '================================================================'
GO

PRINT 'Suggested Foreign Key Constraints for BHVEDPSNET:'
PRINT '================================================================'
PRINT '1. TBL_STUDENT_EXAMS.Chr_FK_PRN_NO -> TBL_STUDENT_INFO.Chr_PRN_NO'
PRINT '2. TBL_STUDENT_EXAMS.Num_FK_INST_NO -> Tbl_SYS_INST.Num_PK_INST_NO'
PRINT '3. TBL_STUDENT_EXAMS.Num_FK_CO_CD -> Tbl_COLLEGE_MSTR.Num_PK_CO_CD'
PRINT '4. TBL_STUDENT_SUBJECT.Chr_FK_PRN_NO -> TBL_STUDENT_INFO.Chr_PRN_NO'
PRINT '5. TBL_STUDENT_SUBJECT.NUM_FK_SUB_CD -> Tbl_SUBJECT_MSTR.Num_PK_SUB_CD'
PRINT '6. TBL_STUDENT_PAP_MARKS.Chr_FK_PRN_NO -> TBL_STUDENT_INFO.Chr_PRN_NO'
PRINT '7. TBL_STUDENT_COURSE.Chr_FK_PRN_NO -> TBL_STUDENT_INFO.Chr_PRN_NO'
PRINT '8. TBL_STUDENT_COURSE.Num_FK_CO_CD -> Tbl_COLLEGE_MSTR.Num_PK_CO_CD'
PRINT '9. TBL_STUDENT_COURSE.Num_FK_COPRT_NO -> Tbl_COURSE_PART_MSTR.Num_PK_COPRT_NO'
PRINT '10. Tbl_COURSE_PART_MSTR.Num_FK_COURSE_CD -> Tbl_COURSE_MSTR.Num_PK_COURSE_CD'
PRINT '11. Tbl_COLLEGE_COURSE_MSTR.Num_FK_CO_CD -> Tbl_COLLEGE_MSTR.Num_PK_CO_CD'
PRINT '12. Tbl_COLLEGE_COURSE_MSTR.Num_FK_COURSE_CD -> Tbl_COURSE_MSTR.Num_PK_COURSE_CD'
PRINT '13. Tbl_SUBJECT_CAT_MSTR.Num_FK_SUB_CD -> Tbl_SUBJECT_MSTR.Num_PK_SUB_CD'
PRINT '14. Tbl_SUB_CATPAP_MSTR.NUM_FK_SUB_CD -> Tbl_SUBJECT_MSTR.Num_PK_SUB_CD'
PRINT '15. Tbl_SUB_CATPAP_MSTR.NUM_FK_CAT_CD -> Tbl_SUBJECT_CAT_MSTR.Num_PK_CAT_CD'
PRINT '================================================================'
GO

-- =============================================
-- SECTION 4: CHECK INDEX ALIGNMENT WITH FOREIGN KEYS
-- =============================================

PRINT 'Checking index alignment with foreign keys in Corno.Bharati.OnlineExam...'
GO

USE [Corno.Bharati.OnlineExam]
GO

SELECT 
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ForeignKeyColumn,
    CASE 
        WHEN i.name IS NULL THEN 'MISSING INDEX'
        ELSE 'INDEX EXISTS: ' + i.name
    END AS IndexStatus
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc ON fk.object_id = fc.constraint_object_id
LEFT JOIN sys.index_columns ic ON fc.parent_object_id = ic.object_id 
    AND fc.parent_column_id = ic.column_id
    AND ic.key_ordinal = 1
LEFT JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
WHERE OBJECT_NAME(fk.parent_object_id) IS NOT NULL
ORDER BY TableName, ForeignKeyColumn
GO

PRINT 'Relationship verification completed!'
GO






