-- =============================================
-- Database Performance Optimization Script
-- Database: Corno.Bharati.OnlineExam
-- Purpose: Create indexes for improved query performance
-- =============================================

USE [Corno.Bharati.OnlineExam]
GO

PRINT 'Starting index optimization for Corno.Bharati.OnlineExam database...'
GO

-- =============================================
-- SECTION 1: FOREIGN KEY INDEXES
-- These indexes improve JOIN performance
-- =============================================

-- Exam Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_StudentId' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_StudentId] ON [dbo].[Exam] ([StudentId])
    INCLUDE ([Id], [InstanceId], [CollegeId], [CourseId], [CoursePartId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Exam_StudentId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_InstanceId' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_InstanceId] ON [dbo].[Exam] ([InstanceId])
    INCLUDE ([Id], [StudentId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Exam_InstanceId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_CourseId' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_CourseId] ON [dbo].[Exam] ([CourseId])
    INCLUDE ([Id], [StudentId], [Status])
    PRINT 'Created index: IX_Exam_CourseId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_CoursePartId' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_CoursePartId] ON [dbo].[Exam] ([CoursePartId])
    INCLUDE ([Id], [StudentId], [Status])
    PRINT 'Created index: IX_Exam_CoursePartId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_CollegeId' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_CollegeId] ON [dbo].[Exam] ([CollegeId])
    INCLUDE ([Id], [StudentId], [Status])
    PRINT 'Created index: IX_Exam_CollegeId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_Status' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_Status] ON [dbo].[Exam] ([Status])
    INCLUDE ([Id], [StudentId], [CreatedDate])
    PRINT 'Created index: IX_Exam_Status'
END
GO

-- Registration Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registration_PrnNo' AND object_id = OBJECT_ID('Registration'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Registration_PrnNo] ON [dbo].[Registration] ([PrnNo])
    INCLUDE ([Id], [InstanceId], [FacultyId], [CourseId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Registration_PrnNo'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registration_InstanceId' AND object_id = OBJECT_ID('Registration'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Registration_InstanceId] ON [dbo].[Registration] ([InstanceId])
    INCLUDE ([Id], [PrnNo], [Status], [CreatedDate])
    PRINT 'Created index: IX_Registration_InstanceId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registration_CourseId' AND object_id = OBJECT_ID('Registration'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Registration_CourseId] ON [dbo].[Registration] ([CourseId])
    INCLUDE ([Id], [PrnNo], [Status])
    PRINT 'Created index: IX_Registration_CourseId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registration_Status' AND object_id = OBJECT_ID('Registration'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Registration_Status] ON [dbo].[Registration] ([Status])
    INCLUDE ([Id], [PrnNo], [CreatedDate])
    PRINT 'Created index: IX_Registration_Status'
END
GO

-- Student Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Student_Prn' AND object_id = OBJECT_ID('Student'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Student_Prn] ON [dbo].[Student] ([Prn])
    INCLUDE ([Id], [FacultyId], [SubjectId], [RegistrationStatus])
    PRINT 'Created index: IX_Student_Prn'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Student_FacultyId' AND object_id = OBJECT_ID('Student'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Student_FacultyId] ON [dbo].[Student] ([FacultyId])
    INCLUDE ([Id], [Prn], [RegistrationStatus])
    PRINT 'Created index: IX_Student_FacultyId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Student_SubjectId' AND object_id = OBJECT_ID('Student'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Student_SubjectId] ON [dbo].[Student] ([SubjectId])
    INCLUDE ([Id], [Prn])
    PRINT 'Created index: IX_Student_SubjectId'
END
GO

-- ExamSubject Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ExamSubject_ExamId' AND object_id = OBJECT_ID('ExamSubject'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ExamSubject_ExamId] ON [dbo].[ExamSubject] ([ExamId])
    INCLUDE ([Id], [SubjectId])
    PRINT 'Created index: IX_ExamSubject_ExamId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ExamSubject_SubjectId' AND object_id = OBJECT_ID('ExamSubject'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ExamSubject_SubjectId] ON [dbo].[ExamSubject] ([SubjectId])
    INCLUDE ([Id], [ExamId])
    PRINT 'Created index: IX_ExamSubject_SubjectId'
END
GO

-- Revalution Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Revalution_StudentId' AND object_id = OBJECT_ID('Revalution'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Revalution_StudentId] ON [dbo].[Revalution] ([StudentId])
    INCLUDE ([Id], [InstanceId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Revalution_StudentId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Revalution_InstanceId' AND object_id = OBJECT_ID('Revalution'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Revalution_InstanceId] ON [dbo].[Revalution] ([InstanceId])
    INCLUDE ([Id], [StudentId], [Status])
    PRINT 'Created index: IX_Revalution_InstanceId'
END
GO

-- RevalutionSubject Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RevalutionSubject_RevalutionId' AND object_id = OBJECT_ID('RevalutionSubject'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RevalutionSubject_RevalutionId] ON [dbo].[RevalutionSubject] ([RevalutionId])
    INCLUDE ([Id], [SubjectId])
    PRINT 'Created index: IX_RevalutionSubject_RevalutionId'
END
GO

-- Convocation Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Convocation_StudentId' AND object_id = OBJECT_ID('Convocation'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Convocation_StudentId] ON [dbo].[Convocation] ([StudentId])
    INCLUDE ([Id], [InstanceId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Convocation_StudentId'
END
GO

-- EnvironmentStudy Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EnvironmentStudy_StudentId' AND object_id = OBJECT_ID('EnvironmentStudy'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_EnvironmentStudy_StudentId] ON [dbo].[EnvironmentStudy] ([StudentId])
    INCLUDE ([Id], [InstanceId], [Status], [CreatedDate])
    PRINT 'Created index: IX_EnvironmentStudy_StudentId'
END
GO

-- =============================================
-- SECTION 2: DATE RANGE INDEXES
-- These indexes improve date-based queries
-- =============================================

-- Exam Date Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_CreatedDate' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_CreatedDate] ON [dbo].[Exam] ([CreatedDate])
    INCLUDE ([Id], [StudentId], [Status])
    PRINT 'Created index: IX_Exam_CreatedDate'
END
GO

-- Registration Date Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registration_CreatedDate' AND object_id = OBJECT_ID('Registration'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Registration_CreatedDate] ON [dbo].[Registration] ([CreatedDate])
    INCLUDE ([Id], [PrnNo], [Status])
    PRINT 'Created index: IX_Registration_CreatedDate'
END
GO

-- =============================================
-- SECTION 3: COMPOSITE INDEXES
-- These indexes improve multi-column queries
-- =============================================

-- Exam Composite Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_InstanceId_Status' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_InstanceId_Status] ON [dbo].[Exam] ([InstanceId], [Status])
    INCLUDE ([Id], [StudentId], [CreatedDate])
    PRINT 'Created index: IX_Exam_InstanceId_Status'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exam_StudentId_Status' AND object_id = OBJECT_ID('Exam'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Exam_StudentId_Status] ON [dbo].[Exam] ([StudentId], [Status])
    INCLUDE ([Id], [InstanceId], [CreatedDate])
    PRINT 'Created index: IX_Exam_StudentId_Status'
END
GO

-- Registration Composite Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registration_InstanceId_Status' AND object_id = OBJECT_ID('Registration'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Registration_InstanceId_Status] ON [dbo].[Registration] ([InstanceId], [Status])
    INCLUDE ([Id], [PrnNo], [CreatedDate])
    PRINT 'Created index: IX_Registration_InstanceId_Status'
END
GO

-- =============================================
-- SECTION 4: PAPER SETTING MODULE INDEXES
-- =============================================

-- Appointment Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_InstanceId' AND object_id = OBJECT_ID('Appointment'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Appointment_InstanceId] ON [dbo].[Appointment] ([InstanceId])
    INCLUDE ([Id], [SubjectId], [FacultyId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Appointment_InstanceId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_SubjectId' AND object_id = OBJECT_ID('Appointment'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Appointment_SubjectId] ON [dbo].[Appointment] ([SubjectId])
    INCLUDE ([Id], [InstanceId], [FacultyId], [Status])
    PRINT 'Created index: IX_Appointment_SubjectId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointment_FacultyId' AND object_id = OBJECT_ID('Appointment'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Appointment_FacultyId] ON [dbo].[Appointment] ([FacultyId])
    INCLUDE ([Id], [SubjectId], [Status])
    PRINT 'Created index: IX_Appointment_FacultyId'
END
GO

-- AppointmentDetail Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AppointmentDetail_AppointmentId' AND object_id = OBJECT_ID('AppointmentDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AppointmentDetail_AppointmentId] ON [dbo].[AppointmentDetail] ([AppointmentId])
    INCLUDE ([Id])
    PRINT 'Created index: IX_AppointmentDetail_AppointmentId'
END
GO

-- Schedule Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Schedule_InstanceId' AND object_id = OBJECT_ID('Schedule'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Schedule_InstanceId] ON [dbo].[Schedule] ([InstanceId])
    INCLUDE ([Id], [SubjectId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Schedule_InstanceId'
END
GO

-- =============================================
-- SECTION 5: QUESTION BANK MODULE INDEXES
-- =============================================

-- Question Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Question_SubjectId' AND object_id = OBJECT_ID('Question'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Question_SubjectId] ON [dbo].[Question] ([SubjectId])
    INCLUDE ([Id], [StructureId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Question_SubjectId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Question_StructureId' AND object_id = OBJECT_ID('Question'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Question_StructureId] ON [dbo].[Question] ([StructureId])
    INCLUDE ([Id], [SubjectId], [Status])
    PRINT 'Created index: IX_Question_StructureId'
END
GO

-- QuestionAppointment Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_QuestionAppointment_SubjectId' AND object_id = OBJECT_ID('QuestionAppointment'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_QuestionAppointment_SubjectId] ON [dbo].[QuestionAppointment] ([SubjectId])
    INCLUDE ([Id], [InstanceId], [FacultyId], [Status])
    PRINT 'Created index: IX_QuestionAppointment_SubjectId'
END
GO

-- Paper Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Paper_SubjectId' AND object_id = OBJECT_ID('Paper'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Paper_SubjectId] ON [dbo].[Paper] ([SubjectId])
    INCLUDE ([Id], [InstanceId], [Status], [CreatedDate])
    PRINT 'Created index: IX_Paper_SubjectId'
END
GO

-- Structure Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Structure_SubjectId' AND object_id = OBJECT_ID('Structure'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Structure_SubjectId] ON [dbo].[Structure] ([SubjectId])
    INCLUDE ([Id], [Status])
    PRINT 'Created index: IX_Structure_SubjectId'
END
GO

-- =============================================
-- SECTION 6: TIME TABLE INDEXES
-- =============================================

-- TimeTable Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TimeTable_InstanceId' AND object_id = OBJECT_ID('TimeTable'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_TimeTable_InstanceId] ON [dbo].[TimeTable] ([InstanceId])
    INCLUDE ([Id], [Status], [CreatedDate])
    PRINT 'Created index: IX_TimeTable_InstanceId'
END
GO

-- =============================================
-- SECTION 7: ANSWER SHEET INDEXES
-- =============================================

-- AnswerSheet Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AnswerSheet_StudentId' AND object_id = OBJECT_ID('AnswerSheet'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AnswerSheet_StudentId] ON [dbo].[AnswerSheet] ([StudentId])
    INCLUDE ([Id], [ExamId], [Status])
    PRINT 'Created index: IX_AnswerSheet_StudentId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AnswerSheet_ExamId' AND object_id = OBJECT_ID('AnswerSheet'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AnswerSheet_ExamId] ON [dbo].[AnswerSheet] ([ExamId])
    INCLUDE ([Id], [StudentId])
    PRINT 'Created index: IX_AnswerSheet_ExamId'
END
GO

-- =============================================
-- SECTION 8: LINK/ENROLLMENT INDEXES
-- =============================================

-- Link Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Link_InstanceId' AND object_id = OBJECT_ID('Link'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Link_InstanceId] ON [dbo].[Link] ([InstanceId])
    INCLUDE ([Id], [Status], [CreatedDate])
    PRINT 'Created index: IX_Link_InstanceId'
END
GO

-- LinkDetail Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LinkDetail_LinkId' AND object_id = OBJECT_ID('LinkDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LinkDetail_LinkId] ON [dbo].[LinkDetail] ([LinkId])
    INCLUDE ([Id])
    PRINT 'Created index: IX_LinkDetail_LinkId'
END
GO

-- =============================================
-- SECTION 9: TRANSACTION OTP INDEXES
-- =============================================

-- TransactionOtp Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TransactionOtp_PrnNo' AND object_id = OBJECT_ID('TransactionOtp'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_TransactionOtp_PrnNo] ON [dbo].[TransactionOtp] ([PrnNo])
    INCLUDE ([Id], [Otp], [CreatedDate], [ExpiryDate])
    PRINT 'Created index: IX_TransactionOtp_PrnNo'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TransactionOtp_CreatedDate' AND object_id = OBJECT_ID('TransactionOtp'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_TransactionOtp_CreatedDate] ON [dbo].[TransactionOtp] ([CreatedDate])
    INCLUDE ([Id], [PrnNo], [ExpiryDate])
    PRINT 'Created index: IX_TransactionOtp_CreatedDate'
END
GO

-- =============================================
-- SECTION 10: MASTER TABLE INDEXES
-- =============================================

-- Faculty Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Faculty_Code' AND object_id = OBJECT_ID('Faculty'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Faculty_Code] ON [dbo].[Faculty] ([Code])
    INCLUDE ([Id], [Name])
    PRINT 'Created index: IX_Faculty_Code'
END
GO

-- College Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_College_Code' AND object_id = OBJECT_ID('College'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_College_Code] ON [dbo].[College] ([Code])
    INCLUDE ([Id], [Name])
    PRINT 'Created index: IX_College_Code'
END
GO

-- Course Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Course_Code' AND object_id = OBJECT_ID('Course'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Course_Code] ON [dbo].[Course] ([Code])
    INCLUDE ([Id], [Name])
    PRINT 'Created index: IX_Course_Code'
END
GO

-- Subject Table Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subject_Code' AND object_id = OBJECT_ID('Subject'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Subject_Code] ON [dbo].[Subject] ([Code])
    INCLUDE ([Id], [Name])
    PRINT 'Created index: IX_Subject_Code'
END
GO

PRINT 'Index optimization completed for Corno.Bharati.OnlineExam database!'
GO






