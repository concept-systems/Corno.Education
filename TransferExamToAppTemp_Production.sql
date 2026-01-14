-- =============================================
-- Stored Procedure: TransferExamToAppTemp
-- Purpose: Transfer Exam entries and ExamSubjects from Exam table to App_Temp tables
-- Database: BHVEDPSNET (on server 192.168.100.50)
-- Source Database: Corno.Bharati.OnlineExam (on server 192.168.100.146)
-- Parameters: @InstanceId, @CollegeId, @CoursePartId
-- 
-- NOTE: This procedure requires a Linked Server to be configured
--       from 192.168.100.50 to 192.168.100.146
--       Linked Server Name: [192.168.100.146] or configure with a friendly name
-- =============================================

USE [BHVEDPSNET]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransferExamToAppTemp]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[TransferExamToAppTemp]
GO

CREATE PROCEDURE [dbo].[TransferExamToAppTemp]
    @InstanceId INT,
    @CollegeId INT,
    @CoursePartId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    DECLARE @AppTempCount INT = 0;
    DECLARE @AppTempSubCount INT = 0;
    DECLARE @LinkedServerName NVARCHAR(128) = '[192.168.100.146]'; -- Change this if you have a different linked server name
    
    -- Temporary table to store Exam data with ExamId before inserting
    DECLARE @TempExamData TABLE (
        ExamId INT,
        PrnNo NVARCHAR(50),
        FormNo NVARCHAR(50),
        CoursePartId INT,
        BranchId INT,
        CentreId INT,
        Bundle NVARCHAR(50),
        AadharNo NVARCHAR(50),
        ExamFee FLOAT,
        CapFee FLOAT,
        StatementOfMarksFee FLOAT,
        LateFee FLOAT,
        SuperLateFee FLOAT,
        OthersFee FLOAT,
        CertificateOfPassingFee FLOAT,
        DissertationFee FLOAT,
        BacklogFee FLOAT,
        Total FLOAT,
        TransactionId NVARCHAR(50),
        PaidAmount FLOAT,
        PaymentDate DATETIME,
        CreatedDate DATETIME,
        ModifiedDate DATETIME,
        FeeId INT,
        RegularFee45 FLOAT,
        BackLogFee45 FLOAT,
        CapFee45 FLOAT,
        StatementOfMarksFee45 FLOAT,
        TotalFee45 FLOAT
    );
    
    -- Temporary table to store newly inserted App_Temp IDs with PRN (to join back to get ExamId)
    DECLARE @NewAppTempWithPRN TABLE (
        Num_PK_ENTRY_ID INT,
        PrnNo NVARCHAR(50)
    );
    
    -- Temporary table to store App_Temp IDs with ExamId
    DECLARE @NewAppTemp TABLE (
        Num_PK_ENTRY_ID INT,
        ExamId INT
    );
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Step 1: Get Exam data into temp table from remote server
        -- Using 4-part name: [LinkedServer].[Database].[Schema].[Table]
        INSERT INTO @TempExamData (
            ExamId, PrnNo, FormNo, CoursePartId, BranchId, CentreId, Bundle, AadharNo,
            ExamFee, CapFee, StatementOfMarksFee, LateFee, SuperLateFee, OthersFee,
            CertificateOfPassingFee, DissertationFee, BacklogFee, Total,
            TransactionId, PaidAmount, PaymentDate, CreatedDate, ModifiedDate,
            FeeId, RegularFee45, BackLogFee45, CapFee45, StatementOfMarksFee45, TotalFee45
        )
        SELECT 
            e.Id AS ExamId,
            e.PrnNo,
            e.FormNo,
            e.CoursePartId,
            e.BranchId,
            e.CentreId,
            e.Bundle,
            e.AadharNo,
            e.ExamFee,
            e.CapFee,
            e.StatementOfMarksFee,
            e.LateFee,
            e.SuperLateFee,
            e.OthersFee,
            e.CertificateOfPassingFee,
            e.DissertationFee,
            e.BacklogFee,
            e.Total,
            e.TransactionId,
            e.PaidAmount,
            e.PaymentDate,
            e.CreatedDate,
            e.ModifiedDate,
            e.FeeId,
            e.RegularFee45,
            e.BackLogFee45,
            e.CapFee45,
            e.StatementOfMarksFee45,
            e.TotalFee45
        FROM [192.168.100.146].[Corno.Bharati.OnlineExam].[dbo].[Exam] e
        WHERE e.InstanceId = @InstanceId
            AND e.CollegeId = @CollegeId
            AND e.CoursePartId = @CoursePartId
            AND e.Status = 'Paid'
            AND e.PrnNo IS NOT NULL
            AND NOT EXISTS (
                SELECT 1 
                FROM BHVEDPSNET.dbo.Tbl_APP_TEMP at
                WHERE at.Num_FK_INST_NO = @InstanceId
                    AND at.Num_FK_COLLEGE_CD = @CollegeId
                    AND at.Num_FK_COPRT_NO = @CoursePartId
                    AND at.Chr_APP_PRN_NO = e.PrnNo
            );
        
        -- Step 2: Insert into Tbl_APP_TEMP using temp table data
        -- Use OUTPUT to capture inserted IDs and PRN
        INSERT INTO Tbl_APP_TEMP (
            Num_FORM_ID,
            Chr_APP_VALID_FLG,
            DELETE_FLG,
            Chr_APP_PRN_NO,
            Num_FK_COPRT_NO,
            Num_FK_INST_NO,
            Num_FK_BR_CD,
            Num_FK_COLLEGE_CD,
            Num_FK_CENTER_CD,
            Num_FK_DistCenter_ID,
            Chr_BUNDAL_NO,
            AadharNo,
            Num_FK_STACTV_CD,
            Num_FK_STUDCAT_CD,
            Var_USR_NM,
            Dtm_DTE_CR,
            Dtm_DTE_UP,
            Chr_REPEATER_FLG,
            Chr_IMPROVEMENT_FLG,
            Num_ExamFee,
            Num_CAPFee,
            Num_StatementFee,
            Num_LateFee,
            Num_SuperLateFee,
            Num_Fine,
            Num_PassingCertificateFee,
            Num_DissertationFee,
            Num_BacklogFee,
            Num_TotalFee,
            Num_Transaction_Id,
            PaidAmount,
            PaymentDate,
            FeeId,
            Num_RegularFee45,
            Num_BacklogFee45,
            Num_CapFee45,
            Num_StatementFee45,
            Num_TotalFee45
        )
        OUTPUT 
            INSERTED.Num_PK_ENTRY_ID,
            INSERTED.Chr_APP_PRN_NO
        INTO @NewAppTempWithPRN (Num_PK_ENTRY_ID, PrnNo)
        SELECT 
            CAST(ISNULL(ted.FormNo, '0') AS INT) AS Num_FORM_ID,
            'A' AS Chr_APP_VALID_FLG,
            'N' AS DELETE_FLG,
            ted.PrnNo AS Chr_APP_PRN_NO,
            CAST(ISNULL(ted.CoursePartId, 0) AS SMALLINT) AS Num_FK_COPRT_NO,
            CAST(@InstanceId AS SMALLINT) AS Num_FK_INST_NO,
            CAST(ISNULL(ted.BranchId, 0) AS SMALLINT) AS Num_FK_BR_CD,
            CAST(@CollegeId AS SMALLINT) AS Num_FK_COLLEGE_CD,
            CAST(0 AS SMALLINT) AS Num_FK_CENTER_CD,
            CAST(ISNULL(ted.CentreId, 0) AS SMALLINT) AS Num_FK_DistCenter_ID,
            ISNULL(ted.Bundle, '') AS Chr_BUNDAL_NO,
            ted.AadharNo AS AadharNo,
            CAST(0 AS SMALLINT) AS Num_FK_STACTV_CD,
            CAST(0 AS SMALLINT) AS Num_FK_STUDCAT_CD,
            SYSTEM_USER AS Var_USR_NM,
            ISNULL(ted.CreatedDate, GETDATE()) AS Dtm_DTE_CR,
            ISNULL(ted.ModifiedDate, GETDATE()) AS Dtm_DTE_UP,
            'N' AS Chr_REPEATER_FLG,
            'N' AS Chr_IMPROVEMENT_FLG,
            ISNULL(ted.ExamFee, 0) AS Num_ExamFee,
            ISNULL(ted.CapFee, 0) AS Num_CAPFee,
            ISNULL(ted.StatementOfMarksFee, 0) AS Num_StatementFee,
            ISNULL(ted.LateFee, 0) AS Num_LateFee,
            ISNULL(ted.SuperLateFee, 0) AS Num_SuperLateFee,
            ISNULL(ted.OthersFee, 0) AS Num_Fine,
            ISNULL(ted.CertificateOfPassingFee, 0) AS Num_PassingCertificateFee,
            ISNULL(ted.DissertationFee, 0) AS Num_DissertationFee,
            ISNULL(ted.BacklogFee, 0) AS Num_BacklogFee,
            ISNULL(ted.Total, 0) AS Num_TotalFee,
            ted.TransactionId AS Num_Transaction_Id,
            ISNULL(ted.PaidAmount, 0) AS PaidAmount,
            ted.PaymentDate AS PaymentDate,
            ted.FeeId AS FeeId,
            ISNULL(ted.RegularFee45, 0) AS Num_RegularFee45,
            ISNULL(ted.BackLogFee45, 0) AS Num_BacklogFee45,
            ISNULL(ted.CapFee45, 0) AS Num_CapFee45,
            ISNULL(ted.StatementOfMarksFee45, 0) AS Num_StatementFee45,
            ISNULL(ted.TotalFee45, 0) AS Num_TotalFee45
        FROM @TempExamData ted;
        
        SET @AppTempCount = @@ROWCOUNT;
        
        -- Step 3: Map App_Temp IDs to ExamIds by joining on PRN
        INSERT INTO @NewAppTemp (Num_PK_ENTRY_ID, ExamId)
        SELECT nat.Num_PK_ENTRY_ID, ted.ExamId
        FROM @NewAppTempWithPRN nat
        INNER JOIN @TempExamData ted ON nat.PrnNo = ted.PrnNo;
        
        -- Step 4: Insert into Tbl_APP_TEMP_SUB for ExamSubject records from remote server
        INSERT INTO Tbl_APP_TEMP_SUB (
            Num_FK_ENTRY_ID,
            Num_FK_INST_NO,
            Num_FK_COPRT_NO,
            Num_FK_SUB_CD,
            Chr_DELETE_FLG,
            Chr_REPH_FLG
        )
        SELECT 
            nat.Num_PK_ENTRY_ID AS Num_FK_ENTRY_ID,
            CAST(@InstanceId AS SMALLINT) AS Num_FK_INST_NO,
            CAST(ISNULL(es.CoursePartId, 0) AS SMALLINT) AS Num_FK_COPRT_NO,
            CAST(es.SubjectCode AS SMALLINT) AS Num_FK_SUB_CD,
            'N' AS Chr_DELETE_FLG,
            CASE 
                WHEN es.SubjectType = 'BackLog' THEN 'R'
                ELSE NULL
            END AS Chr_REPH_FLG
        FROM @NewAppTemp nat
        INNER JOIN [192.168.100.146].[Corno.Bharati.OnlineExam].[dbo].[ExamSubject] es 
            ON nat.ExamId = es.ExamId
        WHERE es.SubjectCode IS NOT NULL
            AND es.SubjectCode > 0
            AND NOT EXISTS (
                SELECT 1 
                FROM BHVEDPSNET.dbo.Tbl_APP_TEMP_SUB ats
                WHERE ats.Num_FK_ENTRY_ID = nat.Num_PK_ENTRY_ID
                    AND ats.Num_FK_SUB_CD = es.SubjectCode
                    AND (ats.Num_FK_COPRT_NO = ISNULL(es.CoursePartId, 0) OR es.CoursePartId IS NULL)
            );
        
        SET @AppTempSubCount = @@ROWCOUNT;
        
        -- Commit transaction if everything succeeded
        COMMIT TRANSACTION;
        
        -- Return success message with counts
        SELECT 
            'Success' AS Status,
            @AppTempCount AS AppTempRecordsInserted,
            @AppTempSubCount AS AppTempSubRecordsInserted,
            @InstanceId AS InstanceId,
            @CollegeId AS CollegeId,
            @CoursePartId AS CoursePartId,
            GETDATE() AS TransferDate;
        
    END TRY
    BEGIN CATCH
        -- Rollback transaction on error
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Capture error information
        SELECT 
            'Error' AS Status,
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_SEVERITY() AS ErrorSeverity,
            ERROR_STATE() AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE() AS ErrorLine,
            ERROR_MESSAGE() AS ErrorMessage;
        
        -- Re-raise the error
        SET @ErrorMessage = ERROR_MESSAGE();
        SET @ErrorSeverity = ERROR_SEVERITY();
        SET @ErrorState = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
GO

PRINT 'Stored Procedure [dbo].[TransferExamToAppTemp] created successfully.'
GO

-- =============================================
-- IMPORTANT: Before using this procedure, you need to configure a Linked Server
-- =============================================
/*
-- To configure a Linked Server from 192.168.100.50 to 192.168.100.146, run this on 192.168.100.50:

EXEC sp_addlinkedserver 
    @server = '192.168.100.146',
    @srvproduct = 'SQL Server';

-- Configure the linked server to use SQL Server authentication (if needed)
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = '192.168.100.146',
    @useself = 'false',
    @locallogin = NULL,
    @rmtuser = 'admin',  -- Change to your remote server username
    @rmtpassword = 'universal1!';  -- Change to your remote server password

-- Or if you want to use Windows Authentication:
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = '192.168.100.146',
    @useself = 'true';

-- Test the linked server connection:
SELECT * FROM [192.168.100.146].[Corno.Bharati.OnlineExam].[dbo].[Exam] WHERE 1=0;
*/


