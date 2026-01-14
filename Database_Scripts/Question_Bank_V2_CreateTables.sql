-- =============================================
-- QUESTION BANK V2 - DATABASE TABLES
-- =============================================
-- Created: 2024
-- Description: Complete database schema for Question Bank V2 module
-- =============================================

USE [Corno.Bharati.OnlineExam]
GO

-- =============================================
-- 1. QB_QuestionType (Master Table)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_QuestionType]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_QuestionType] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL UNIQUE,
        [Code] NVARCHAR(20) NOT NULL UNIQUE,
        [Description] NVARCHAR(500) NULL,
        [HasOptions] BIT DEFAULT 0,
        [HasSubQuestions] BIT DEFAULT 0,
        [AllowPartialMarks] BIT DEFAULT 1,
        [DefaultMarks] DECIMAL(5,2) NULL,
        [Icon] NVARCHAR(100) NULL,
        [DisplayOrder] INT DEFAULT 0,
        [IsActive] BIT DEFAULT 1,
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL
    );

    -- Insert Default Question Types
    INSERT INTO [QB_QuestionType] ([Name], [Code], [HasOptions], [HasSubQuestions], [DisplayOrder]) VALUES
    ('Multiple Choice (Single Answer)', 'MCQ_SINGLE', 1, 0, 1),
    ('Multiple Choice (Multiple Answers)', 'MCQ_MULTIPLE', 1, 0, 2),
    ('True/False', 'TRUE_FALSE', 1, 0, 3),
    ('Short Answer', 'SHORT_ANSWER', 0, 0, 4),
    ('Long Answer', 'LONG_ANSWER', 0, 0, 5),
    ('Fill in the Blanks', 'FILL_BLANK', 0, 0, 6),
    ('Match the Following', 'MATCH_FOLLOWING', 0, 0, 7),
    ('Case Study', 'CASE_STUDY', 0, 1, 8),
    ('Passage Based', 'PASSAGE_BASED', 0, 1, 9),
    ('Numerical', 'NUMERICAL', 0, 0, 10),
    ('Diagram Based', 'DIAGRAM_BASED', 0, 0, 11);

    CREATE INDEX [IX_QB_QuestionType_Code] ON [QB_QuestionType]([Code]);
    CREATE INDEX [IX_QB_QuestionType_IsActive] ON [QB_QuestionType]([IsActive]);
END
GO

-- =============================================
-- 2. QB_DifficultyLevel (Master Table)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_DifficultyLevel]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_DifficultyLevel] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL UNIQUE,
        [Code] NVARCHAR(20) NOT NULL UNIQUE,
        [Level] INT NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [ColorCode] NVARCHAR(7) NULL,
        [DisplayOrder] INT NOT NULL,
        [IsActive] BIT DEFAULT 1,
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE()
    );

    INSERT INTO [QB_DifficultyLevel] ([Name], [Code], [Level], [ColorCode], [DisplayOrder]) VALUES
    ('Easy', 'EASY', 1, '#4CAF50', 1),
    ('Medium', 'MEDIUM', 2, '#FF9800', 2),
    ('Hard', 'HARD', 3, '#F44336', 3),
    ('Very Hard', 'VERY_HARD', 4, '#9C27B0', 4);

    CREATE INDEX [IX_QB_DifficultyLevel_Code] ON [QB_DifficultyLevel]([Code]);
END
GO

-- =============================================
-- 3. QB_TaxonomyLevel (Master Table - Bloom's Taxonomy)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_TaxonomyLevel]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_TaxonomyLevel] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL UNIQUE,
        [Code] NVARCHAR(20) NOT NULL UNIQUE,
        [Level] INT NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Keywords] NVARCHAR(500) NULL,
        [ColorCode] NVARCHAR(7) NULL,
        [DisplayOrder] INT NOT NULL,
        [IsActive] BIT DEFAULT 1,
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE()
    );

    INSERT INTO [QB_TaxonomyLevel] ([Name], [Code], [Level], [Description], [Keywords], [ColorCode], [DisplayOrder]) VALUES
    ('Remember', 'REMEMBER', 1, 'Recall facts and basic concepts', 'define, list, name, recall, recognize', '#2196F3', 1),
    ('Understand', 'UNDERSTAND', 2, 'Explain ideas or concepts', 'explain, describe, interpret, summarize', '#4CAF50', 2),
    ('Apply', 'APPLY', 3, 'Use information in new situations', 'solve, use, demonstrate, calculate', '#FF9800', 3),
    ('Analyze', 'ANALYZE', 4, 'Draw connections among ideas', 'compare, contrast, examine, analyze', '#FF5722', 4),
    ('Evaluate', 'EVALUATE', 5, 'Justify a stand or decision', 'evaluate, judge, critique, justify', '#9C27B0', 5),
    ('Create', 'CREATE', 6, 'Produce new or original work', 'create, design, construct, develop', '#E91E63', 6);

    CREATE INDEX [IX_QB_TaxonomyLevel_Code] ON [QB_TaxonomyLevel]([Code]);
END
GO

-- =============================================
-- 4. QB_QuestionBank (Main Question Table)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_QuestionBank]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_QuestionBank] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [QuestionCode] NVARCHAR(50) NOT NULL UNIQUE,
        [InstanceId] INT NOT NULL,
        [FacultyId] INT NOT NULL,
        [CourseId] INT NOT NULL,
        [CoursePartId] INT NOT NULL,
        [BranchId] INT NULL,
        [SubjectId] INT NOT NULL,
        [PaperCategoryId] INT NOT NULL,
        
        -- Question Content (Encrypted)
        [QuestionText] VARBINARY(MAX) NOT NULL, -- Encrypted storage
        [QuestionTypeId] INT NOT NULL,
        [UnitId] INT NULL, -- Maps to SubjectChapterDetail.Id (displayed as Unit)
        [TopicId] INT NULL,
        [SubTopicId] INT NULL,
        
        -- Classification
        [DifficultyLevelId] INT NOT NULL,
        [TaxonomyLevelId] INT NOT NULL,
        [LearningOutcomeId] INT NULL,
        [CognitiveLevelId] INT NULL,
        
        -- Assessment Details
        [Marks] DECIMAL(5,2) NOT NULL,
        [TimeAllotted] INT NULL,
        [NegativeMarks] DECIMAL(5,2) NULL DEFAULT 0,
        [PartialMarksAllowed] BIT DEFAULT 0,
        
        -- Answer Details (Encrypted)
        [ModelAnswer] VARBINARY(MAX) NULL, -- Encrypted storage
        [AnswerExplanation] VARBINARY(MAX) NULL,
        [Hints] VARBINARY(MAX) NULL,
        [SolutionSteps] VARBINARY(MAX) NULL,
        
        -- Media & Attachments
        [HasImage] BIT DEFAULT 0,
        [HasAudio] BIT DEFAULT 0,
        [HasVideo] BIT DEFAULT 0,
        [HasFormula] BIT DEFAULT 0,
        
        -- Metadata
        [LanguageId] INT NULL DEFAULT 1,
        [Source] NVARCHAR(200) NULL,
        [Reference] NVARCHAR(500) NULL,
        [Tags] NVARCHAR(500) NULL,
        [Keywords] NVARCHAR(500) NULL,
        
        -- Workflow Status
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Draft',
        
        -- Role-based Assignment (Using AspNetUser Ids)
        [SetterUserId] NVARCHAR(128) NOT NULL,
        [CheckerUserId] NVARCHAR(128) NULL,
        [ModeratorUserId] NVARCHAR(128) NULL,
        
        -- Review & Approval
        [CheckerComments] NVARCHAR(MAX) NULL,
        [CheckerApprovedDate] DATETIME NULL,
        [ModeratorComments] NVARCHAR(MAX) NULL,
        [ModeratorApprovedDate] DATETIME NULL,
        [RejectionReason] NVARCHAR(MAX) NULL,
        [RejectedByRole] NVARCHAR(20) NULL,
        [RejectedByUserId] NVARCHAR(128) NULL,
        
        -- Quality & Review
        [QualityScore] DECIMAL(3,2) NULL,
        [UsageCount] INT DEFAULT 0,
        [LastUsedDate] DATETIME NULL,
        
        -- Version Control
        [Version] INT DEFAULT 1,
        [ParentQuestionId] INT NULL,
        [IsLatestVersion] BIT DEFAULT 1,
        
        -- Statistics
        [AverageTimeTaken] INT NULL,
        [SuccessRate] DECIMAL(5,2) NULL,
        [DiscriminationIndex] DECIMAL(5,2) NULL,
        
        -- Audit Fields
        [CompanyId] INT NULL,
        [SerialNo] INT NULL,
        [Code] NVARCHAR(50) NULL,
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        [DeletedBy] NVARCHAR(100) NULL,
        [DeletedDate] DATETIME NULL,
        
        CONSTRAINT [FK_QB_QuestionBank_Instance] FOREIGN KEY ([InstanceId]) REFERENCES [Instance]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_Faculty] FOREIGN KEY ([FacultyId]) REFERENCES [Faculty]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_Course] FOREIGN KEY ([CourseId]) REFERENCES [Course]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_CoursePart] FOREIGN KEY ([CoursePartId]) REFERENCES [CoursePart]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [Subject]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_PaperCategory] FOREIGN KEY ([PaperCategoryId]) REFERENCES [MiscMaster]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_QuestionType] FOREIGN KEY ([QuestionTypeId]) REFERENCES [QB_QuestionType]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_Unit] FOREIGN KEY ([UnitId]) REFERENCES [SubjectChapterDetail]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_DifficultyLevel] FOREIGN KEY ([DifficultyLevelId]) REFERENCES [QB_DifficultyLevel]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_TaxonomyLevel] FOREIGN KEY ([TaxonomyLevelId]) REFERENCES [QB_TaxonomyLevel]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_Setter] FOREIGN KEY ([SetterUserId]) REFERENCES [AspNetUsers]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_Checker] FOREIGN KEY ([CheckerUserId]) REFERENCES [AspNetUsers]([Id]),
        CONSTRAINT [FK_QB_QuestionBank_Moderator] FOREIGN KEY ([ModeratorUserId]) REFERENCES [AspNetUsers]([Id])
    );

    CREATE INDEX [IX_QB_QuestionBank_InstanceId] ON [QB_QuestionBank]([InstanceId]);
    CREATE INDEX [IX_QB_QuestionBank_SubjectId] ON [QB_QuestionBank]([SubjectId]);
    CREATE INDEX [IX_QB_QuestionBank_Status] ON [QB_QuestionBank]([Status]);
    CREATE INDEX [IX_QB_QuestionBank_QuestionTypeId] ON [QB_QuestionBank]([QuestionTypeId]);
    CREATE INDEX [IX_QB_QuestionBank_DifficultyLevelId] ON [QB_QuestionBank]([DifficultyLevelId]);
    CREATE INDEX [IX_QB_QuestionBank_TaxonomyLevelId] ON [QB_QuestionBank]([TaxonomyLevelId]);
    CREATE INDEX [IX_QB_QuestionBank_QuestionCode] ON [QB_QuestionBank]([QuestionCode]);
    CREATE INDEX [IX_QB_QuestionBank_SetterUserId] ON [QB_QuestionBank]([SetterUserId]);
END
GO

-- =============================================
-- 5. QB_QuestionOptions (For MCQ)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_QuestionOptions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_QuestionOptions] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [QuestionBankId] INT NOT NULL,
        [OptionText] NVARCHAR(MAX) NOT NULL,
        [OptionOrder] INT NOT NULL,
        [IsCorrect] BIT DEFAULT 0,
        [IsPartialCorrect] BIT DEFAULT 0,
        [Marks] DECIMAL(5,2) NULL,
        [Explanation] NVARCHAR(MAX) NULL,
        [HasImage] BIT DEFAULT 0,
        [ImagePath] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        
        CONSTRAINT [FK_QB_QuestionOptions_QuestionBank] FOREIGN KEY ([QuestionBankId]) 
            REFERENCES [QB_QuestionBank]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_QB_QuestionOptions_QuestionBankId] ON [QB_QuestionOptions]([QuestionBankId]);
END
GO

-- =============================================
-- 6. QB_RoleAssignment (Role assignments per Instance and Subject)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_RoleAssignment]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_RoleAssignment] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [InstanceId] INT NOT NULL,
        [SubjectId] INT NOT NULL,
        [UserId] NVARCHAR(128) NOT NULL,
        [RoleId] NVARCHAR(128) NOT NULL,
        [RoleName] NVARCHAR(50) NOT NULL,
        [IsActive] BIT DEFAULT 1,
        [AssignedDate] DATETIME DEFAULT GETDATE(),
        [AssignedBy] NVARCHAR(100) NULL,
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        
        CONSTRAINT [FK_QB_RoleAssignment_Instance] FOREIGN KEY ([InstanceId]) REFERENCES [Instance]([Id]),
        CONSTRAINT [FK_QB_RoleAssignment_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [Subject]([Id]),
        CONSTRAINT [FK_QB_RoleAssignment_User] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
        CONSTRAINT [FK_QB_RoleAssignment_Role] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id])
    );

    -- Filtered unique index (SQL Server 2008+)
    CREATE UNIQUE INDEX [IX_QB_RoleAssignment_Unique] ON [QB_RoleAssignment]([InstanceId], [SubjectId], [UserId], [RoleId]) 
        WHERE [IsActive] = 1;
    CREATE INDEX [IX_QB_RoleAssignment_InstanceSubject] ON [QB_RoleAssignment]([InstanceId], [SubjectId]);
    CREATE INDEX [IX_QB_RoleAssignment_UserId] ON [QB_RoleAssignment]([UserId]);
END
GO

-- =============================================
-- 7. QB_Appointment (Appointments per Instance)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_Appointment]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_Appointment] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [AppointmentCode] NVARCHAR(50) NOT NULL UNIQUE,
        [InstanceId] INT NOT NULL,
        [FacultyId] INT NOT NULL,
        [CourseId] INT NOT NULL,
        [CoursePartId] INT NOT NULL,
        [BranchId] INT NULL,
        [SubjectId] INT NOT NULL,
        [PaperCategoryId] INT NOT NULL,
        [StructureId] INT NOT NULL,
        
        [NoOfPapers] INT NOT NULL DEFAULT 1,
        [AppointmentDate] DATETIME NOT NULL,
        [AppointmentTime] TIME NULL,
        [Instructions] NVARCHAR(MAX) NULL,
        
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Created',
        
        [EmailSent] BIT DEFAULT 0,
        [SmsSent] BIT DEFAULT 0,
        [WhatsAppSent] BIT DEFAULT 0,
        [EmailSentDate] DATETIME NULL,
        [SmsSentDate] DATETIME NULL,
        [WhatsAppSentDate] DATETIME NULL,
        
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        
        CONSTRAINT [FK_QB_Appointment_Instance] FOREIGN KEY ([InstanceId]) REFERENCES [Instance]([Id]),
        CONSTRAINT [FK_QB_Appointment_Faculty] FOREIGN KEY ([FacultyId]) REFERENCES [Faculty]([Id]),
        CONSTRAINT [FK_QB_Appointment_Course] FOREIGN KEY ([CourseId]) REFERENCES [Course]([Id]),
        CONSTRAINT [FK_QB_Appointment_CoursePart] FOREIGN KEY ([CoursePartId]) REFERENCES [CoursePart]([Id]),
        CONSTRAINT [FK_QB_Appointment_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [Subject]([Id]),
        CONSTRAINT [FK_QB_Appointment_PaperCategory] FOREIGN KEY ([PaperCategoryId]) REFERENCES [MiscMaster]([Id]),
        CONSTRAINT [FK_QB_Appointment_Structure] FOREIGN KEY ([StructureId]) REFERENCES [Structure]([Id])
    );

    CREATE INDEX [IX_QB_Appointment_InstanceId] ON [QB_Appointment]([InstanceId]);
    CREATE INDEX [IX_QB_Appointment_SubjectId] ON [QB_Appointment]([SubjectId]);
    CREATE INDEX [IX_QB_Appointment_Status] ON [QB_Appointment]([Status]);
END
GO

-- =============================================
-- 8. QB_AppointmentDetail (Staff assigned to appointments)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_AppointmentDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_AppointmentDetail] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [AppointmentId] INT NOT NULL,
        [UserId] NVARCHAR(128) NOT NULL,
        [RoleId] NVARCHAR(128) NOT NULL,
        [RoleName] NVARCHAR(50) NOT NULL,
        
        [TemporaryUsername] NVARCHAR(100) NOT NULL UNIQUE,
        [TemporaryPassword] NVARCHAR(255) NOT NULL,
        [PasswordSalt] NVARCHAR(255) NULL,
        [OtpEnabled] BIT DEFAULT 0,
        [OtpSecret] NVARCHAR(100) NULL,
        
        [EmailSent] BIT DEFAULT 0,
        [SmsSent] BIT DEFAULT 0,
        [WhatsAppSent] BIT DEFAULT 0,
        [EmailSentDate] DATETIME NULL,
        [SmsSentDate] DATETIME NULL,
        [WhatsAppSentDate] DATETIME NULL,
        [EmailSentCount] INT DEFAULT 0,
        [SmsSentCount] INT DEFAULT 0,
        [WhatsAppSentCount] INT DEFAULT 0,
        
        [IsAccepted] BIT DEFAULT 0,
        [AcceptedDate] DATETIME NULL,
        [LoginCount] INT DEFAULT 0,
        [LastLoginDate] DATETIME NULL,
        
        [QuestionsAssigned] INT DEFAULT 0,
        [QuestionsCompleted] INT DEFAULT 0,
        [QuestionsApproved] INT DEFAULT 0,
        [QuestionsRejected] INT DEFAULT 0,
        
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        
        CONSTRAINT [FK_QB_AppointmentDetail_Appointment] FOREIGN KEY ([AppointmentId]) 
            REFERENCES [QB_Appointment]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QB_AppointmentDetail_User] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
        CONSTRAINT [FK_QB_AppointmentDetail_Role] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id])
    );

    CREATE INDEX [IX_QB_AppointmentDetail_AppointmentId] ON [QB_AppointmentDetail]([AppointmentId]);
    CREATE INDEX [IX_QB_AppointmentDetail_UserId] ON [QB_AppointmentDetail]([UserId]);
    CREATE INDEX [IX_QB_AppointmentDetail_Username] ON [QB_AppointmentDetail]([TemporaryUsername]);
END
GO

-- =============================================
-- 9. QB_Paper (Paper Generation)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_Paper]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_Paper] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [PaperCode] NVARCHAR(50) NOT NULL UNIQUE,
        [AppointmentId] INT NOT NULL,
        [StructureId] INT NOT NULL,
        [InstanceId] INT NOT NULL,
        [FacultyId] INT NOT NULL,
        [CourseId] INT NOT NULL,
        [CoursePartId] INT NOT NULL,
        [BranchId] INT NULL,
        [SubjectId] INT NOT NULL,
        [PaperCategoryId] INT NOT NULL,
        
        [PaperType] NVARCHAR(20) NOT NULL,
        [SetNumber] INT NOT NULL,
        [MaxMarks] INT NOT NULL,
        [NoOfSections] INT NULL,
        [TimeDuration] INT NULL,
        
        [GeneratedBy] NVARCHAR(100) NULL,
        [GeneratedDate] DATETIME DEFAULT GETDATE(),
        [ModeratorUserId] NVARCHAR(128) NOT NULL,
        [ModeratorComments] NVARCHAR(MAX) NULL,
        
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Draft',
        [DrawnDate] DATETIME NULL,
        [DrawnBy] NVARCHAR(100) NULL,
        
        [WordDocumentContent] VARBINARY(MAX) NULL,
        [WordDocumentFileName] NVARCHAR(255) NULL,
        [WordDocumentGeneratedDate] DATETIME NULL,
        
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        [ModifiedBy] NVARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        
        CONSTRAINT [FK_QB_Paper_Appointment] FOREIGN KEY ([AppointmentId]) REFERENCES [QB_Appointment]([Id]),
        CONSTRAINT [FK_QB_Paper_Structure] FOREIGN KEY ([StructureId]) REFERENCES [Structure]([Id]),
        CONSTRAINT [FK_QB_Paper_Moderator] FOREIGN KEY ([ModeratorUserId]) REFERENCES [AspNetUsers]([Id])
    );

    CREATE INDEX [IX_QB_Paper_AppointmentId] ON [QB_Paper]([AppointmentId]);
    CREATE INDEX [IX_QB_Paper_StructureId] ON [QB_Paper]([StructureId]);
    CREATE INDEX [IX_QB_Paper_Status] ON [QB_Paper]([Status]);
END
GO

-- =============================================
-- 10. QB_PaperDetail (Questions in Paper)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_PaperDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_PaperDetail] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [PaperId] INT NOT NULL,
        [QuestionBankId] INT NULL,
        [StructureDetailId] INT NULL,
        
        [SectionNo] INT NOT NULL,
        [QuestionNo] INT NOT NULL,
        [QuestionTypeId] INT NOT NULL,
        [UnitId] INT NULL,
        [TopicId] INT NULL,
        [DifficultyLevelId] INT NOT NULL,
        [TaxonomyLevelId] INT NOT NULL,
        [Marks] DECIMAL(5,2) NOT NULL,
        
        [QuestionText] NVARCHAR(MAX) NOT NULL,
        [ModelAnswer] NVARCHAR(MAX) NULL,
        
        [SelectionMethod] NVARCHAR(20) NULL,
        [SelectionCriteria] NVARCHAR(MAX) NULL,
        
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        
        CONSTRAINT [FK_QB_PaperDetail_Paper] FOREIGN KEY ([PaperId]) 
            REFERENCES [QB_Paper]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QB_PaperDetail_QuestionBank] FOREIGN KEY ([QuestionBankId]) 
            REFERENCES [QB_QuestionBank]([Id]),
        CONSTRAINT [FK_QB_PaperDetail_StructureDetail] FOREIGN KEY ([StructureDetailId]) 
            REFERENCES [StructureDetail]([Id]),
        CONSTRAINT [FK_QB_PaperDetail_QuestionType] FOREIGN KEY ([QuestionTypeId]) 
            REFERENCES [QB_QuestionType]([Id]),
        CONSTRAINT [FK_QB_PaperDetail_DifficultyLevel] FOREIGN KEY ([DifficultyLevelId]) 
            REFERENCES [QB_DifficultyLevel]([Id]),
        CONSTRAINT [FK_QB_PaperDetail_TaxonomyLevel] FOREIGN KEY ([TaxonomyLevelId]) 
            REFERENCES [QB_TaxonomyLevel]([Id])
    );

    CREATE INDEX [IX_QB_PaperDetail_PaperId] ON [QB_PaperDetail]([PaperId]);
    CREATE INDEX [IX_QB_PaperDetail_QuestionBankId] ON [QB_PaperDetail]([QuestionBankId]);
END
GO

-- =============================================
-- 11. QB_QuestionWorkflow (Workflow History)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_QuestionWorkflow]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_QuestionWorkflow] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [QuestionBankId] INT NOT NULL,
        [FromStatus] NVARCHAR(20) NULL,
        [ToStatus] NVARCHAR(20) NOT NULL,
        [RoleName] NVARCHAR(50) NOT NULL,
        [UserId] NVARCHAR(128) NOT NULL,
        [Comments] NVARCHAR(MAX) NULL,
        [ActionDate] DATETIME DEFAULT GETDATE(),
        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        
        CONSTRAINT [FK_QB_QuestionWorkflow_QuestionBank] FOREIGN KEY ([QuestionBankId]) 
            REFERENCES [QB_QuestionBank]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QB_QuestionWorkflow_User] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
    );

    CREATE INDEX [IX_QB_QuestionWorkflow_QuestionBankId] ON [QB_QuestionWorkflow]([QuestionBankId]);
    CREATE INDEX [IX_QB_QuestionWorkflow_UserId] ON [QB_QuestionWorkflow]([UserId]);
END
GO

-- =============================================
-- 12. QB_QuestionChangeLog (Change Audit)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_QuestionChangeLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_QuestionChangeLog] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [QuestionBankId] INT NOT NULL,
        [FieldName] NVARCHAR(50) NOT NULL,
        [OldValue] VARBINARY(MAX) NULL,
        [NewValue] VARBINARY(MAX) NULL,
        [ChangedBy] NVARCHAR(100) NULL,
        [ChangeDate] DATETIME DEFAULT GETDATE(),
        [ChangeType] NVARCHAR(20) NOT NULL,
        [IsDirectDbChange] BIT DEFAULT 0,
        
        CONSTRAINT [FK_QB_QuestionChangeLog_QuestionBank] FOREIGN KEY ([QuestionBankId]) 
            REFERENCES [QB_QuestionBank]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_QB_QuestionChangeLog_QuestionBankId] ON [QB_QuestionChangeLog]([QuestionBankId]);
    CREATE INDEX [IX_QB_QuestionChangeLog_ChangeDate] ON [QB_QuestionChangeLog]([ChangeDate]);
END
GO

-- =============================================
-- 13. QB_AppointmentNotification (Notification History)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_AppointmentNotification]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_AppointmentNotification] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [AppointmentDetailId] INT NOT NULL,
        [NotificationType] NVARCHAR(20) NOT NULL,
        [Recipient] NVARCHAR(255) NOT NULL,
        [Subject] NVARCHAR(500) NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [Status] NVARCHAR(20) NOT NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [SentDate] DATETIME DEFAULT GETDATE(),
        [DeliveredDate] DATETIME NULL,
        [ReadDate] DATETIME NULL,
        
        CONSTRAINT [FK_QB_AppointmentNotification_AppointmentDetail] FOREIGN KEY ([AppointmentDetailId]) 
            REFERENCES [QB_AppointmentDetail]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_QB_AppointmentNotification_AppointmentDetailId] ON [QB_AppointmentNotification]([AppointmentDetailId]);
END
GO

-- =============================================
-- 14. QB_Otp (OTP Management)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_Otp]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QB_Otp] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [AppointmentDetailId] INT NOT NULL,
        [UserId] NVARCHAR(128) NOT NULL,
        [OtpCode] NVARCHAR(10) NOT NULL,
        [OtpType] NVARCHAR(20) NOT NULL,
        [MobileNo] NVARCHAR(20) NULL,
        [Email] NVARCHAR(255) NULL,
        [GeneratedDate] DATETIME DEFAULT GETDATE(),
        [ExpiryDate] DATETIME NOT NULL,
        [VerifiedDate] DATETIME NULL,
        [IsUsed] BIT DEFAULT 0,
        [Attempts] INT DEFAULT 0,
        
        CONSTRAINT [FK_QB_Otp_AppointmentDetail] FOREIGN KEY ([AppointmentDetailId]) 
            REFERENCES [QB_AppointmentDetail]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QB_Otp_User] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
    );

    CREATE INDEX [IX_QB_Otp_AppointmentDetailId] ON [QB_Otp]([AppointmentDetailId]);
    CREATE INDEX [IX_QB_Otp_OtpCode] ON [QB_Otp]([OtpCode]);
END
GO

-- =============================================
-- 15. Add Branch Foreign Keys (Conditional)
-- =============================================
-- NOTE: Branch foreign keys are commented out because the Branch table structure
-- may vary. The BranchId columns are created as nullable INT columns.
-- If you need to add Branch foreign keys, uncomment and modify the code below
-- after verifying your Branch table structure and primary key.

/*
-- Add Branch FK to QB_QuestionBank if Branch table exists and has Id as primary key
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Branch]') AND type in (N'U'))
   AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branch]') AND name = 'Id')
   AND EXISTS (SELECT * FROM sys.key_constraints kc
               INNER JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
               WHERE kc.parent_object_id = OBJECT_ID(N'[dbo].[Branch]') AND ic.column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[Branch]'), 'Id', 'ColumnId'))
   AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_QuestionBank]') AND type in (N'U'))
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_QB_QuestionBank_Branch')
BEGIN
    ALTER TABLE [dbo].[QB_QuestionBank]
    ADD CONSTRAINT [FK_QB_QuestionBank_Branch] FOREIGN KEY ([BranchId]) REFERENCES [Branch]([Id]);
    PRINT 'Added FK_QB_QuestionBank_Branch constraint';
END
GO

-- Add Branch FK to QB_Appointment if Branch table exists and has Id as primary key
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Branch]') AND type in (N'U'))
   AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branch]') AND name = 'Id')
   AND EXISTS (SELECT * FROM sys.key_constraints kc
               INNER JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
               WHERE kc.parent_object_id = OBJECT_ID(N'[dbo].[Branch]') AND ic.column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[Branch]'), 'Id', 'ColumnId'))
   AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_Appointment]') AND type in (N'U'))
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_QB_Appointment_Branch')
BEGIN
    ALTER TABLE [dbo].[QB_Appointment]
    ADD CONSTRAINT [FK_QB_Appointment_Branch] FOREIGN KEY ([BranchId]) REFERENCES [Branch]([Id]);
    PRINT 'Added FK_QB_Appointment_Branch constraint';
END
GO
*/

-- =============================================
-- 16. Triggers for Change Logging
-- =============================================

-- Trigger to log all changes to QuestionText and ModelAnswer
-- Only create if QB_QuestionBank and QB_QuestionChangeLog tables exist

-- Drop trigger if it exists
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_QB_QuestionBank_Update')
    DROP TRIGGER [trg_QB_QuestionBank_Update]
GO

-- Create trigger if tables exist
-- Note: CREATE TRIGGER must be the first statement in a batch, so we use dynamic SQL
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_QuestionBank]') AND type in (N'U'))
   AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QB_QuestionChangeLog]') AND type in (N'U'))
BEGIN
    DECLARE @sql NVARCHAR(MAX);
    SET @sql = N'
    CREATE TRIGGER [trg_QB_QuestionBank_Update]
    ON [dbo].[QB_QuestionBank]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Log changes to QuestionText
        INSERT INTO [QB_QuestionChangeLog] (
            [QuestionBankId], 
            [FieldName], 
            [OldValue], 
            [NewValue], 
            [ChangedBy], 
            [ChangeDate], 
            [ChangeType],
            [IsDirectDbChange]
        )
        SELECT 
            i.Id,
            ''QuestionText'',
            d.QuestionText,
            i.QuestionText,
            ISNULL(i.ModifiedBy, SYSTEM_USER),
            GETDATE(),
            ''Update'',
            CASE WHEN i.ModifiedBy IS NULL THEN 1 ELSE 0 END
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE (i.QuestionText IS NULL AND d.QuestionText IS NOT NULL)
           OR (i.QuestionText IS NOT NULL AND d.QuestionText IS NULL)
           OR (i.QuestionText IS NOT NULL AND d.QuestionText IS NOT NULL 
               AND (DATALENGTH(i.QuestionText) != DATALENGTH(d.QuestionText)
                    OR i.QuestionText != d.QuestionText));
        
        -- Log changes to ModelAnswer
        INSERT INTO [QB_QuestionChangeLog] (
            [QuestionBankId], 
            [FieldName], 
            [OldValue], 
            [NewValue], 
            [ChangedBy], 
            [ChangeDate], 
            [ChangeType],
            [IsDirectDbChange]
        )
        SELECT 
            i.Id,
            ''ModelAnswer'',
            d.ModelAnswer,
            i.ModelAnswer,
            ISNULL(i.ModifiedBy, SYSTEM_USER),
            GETDATE(),
            ''Update'',
            CASE WHEN i.ModifiedBy IS NULL THEN 1 ELSE 0 END
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        WHERE (i.ModelAnswer IS NULL AND d.ModelAnswer IS NOT NULL)
           OR (i.ModelAnswer IS NOT NULL AND d.ModelAnswer IS NULL)
           OR (i.ModelAnswer IS NOT NULL AND d.ModelAnswer IS NOT NULL 
               AND (DATALENGTH(i.ModelAnswer) != DATALENGTH(d.ModelAnswer)
                    OR i.ModelAnswer != d.ModelAnswer));
    END';
    
    EXEC sp_executesql @sql;
    PRINT 'Created trg_QB_QuestionBank_Update trigger';
END
ELSE
BEGIN
    PRINT 'Warning: QB_QuestionBank or QB_QuestionChangeLog table does not exist. Trigger not created.';
END
GO

PRINT 'Question Bank V2 database tables created successfully!'
GO
