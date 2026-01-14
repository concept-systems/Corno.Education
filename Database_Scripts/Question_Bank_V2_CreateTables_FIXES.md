# Question Bank V2 SQL Script - Fixes Applied

## Errors Found and Fixed

### 1. **Inconsistent Foreign Key Constraint Naming** (Line 211)
   - **Error**: `FK_QuestionBank_Course` didn't follow naming convention
   - **Fixed**: Changed to `FK_QB_QuestionBank_Course` to match other constraints
   - **Impact**: Consistency in naming convention

### 2. **VARBINARY Comparison in Trigger** (Lines 645, 671)
   - **Error**: Direct comparison `!=` with VARBINARY(MAX) may not work correctly
   - **Fixed**: Added DATALENGTH check first, then content comparison
   - **Impact**: Trigger will correctly detect changes to encrypted QuestionText and ModelAnswer fields
   - **Code Change**:
     ```sql
     -- Before:
     WHERE i.QuestionText != d.QuestionText
     
     -- After:
     WHERE (i.QuestionText IS NULL AND d.QuestionText IS NOT NULL)
        OR (i.QuestionText IS NOT NULL AND d.QuestionText IS NULL)
        OR (i.QuestionText IS NOT NULL AND d.QuestionText IS NOT NULL 
            AND (DATALENGTH(i.QuestionText) != DATALENGTH(d.QuestionText)
                 OR i.QuestionText != d.QuestionText))
     ```

### 3. **Missing Foreign Key Constraints in QB_PaperDetail** (Lines 471-476)
   - **Error**: QuestionTypeId, DifficultyLevelId, and TaxonomyLevelId had no foreign key constraints
   - **Fixed**: Added three foreign key constraints:
     - `FK_QB_PaperDetail_QuestionType`
     - `FK_QB_PaperDetail_DifficultyLevel`
     - `FK_QB_PaperDetail_TaxonomyLevel`
   - **Impact**: Ensures referential integrity for paper details

### 4. **Filtered Index Comment** (Line 291)
   - **Added**: Comment clarifying filtered index requirement (SQL Server 2008+)
   - **Impact**: Better documentation

## Verification Checklist

After running the script, verify:

- [ ] All tables created successfully
- [ ] All foreign key constraints created
- [ ] All indexes created
- [ ] Trigger created and enabled
- [ ] Master data inserted (Question Types, Difficulty Levels, Taxonomy Levels)
- [ ] No syntax errors in SQL Server Management Studio

## Notes

1. **Filtered Index**: The unique index on `QB_RoleAssignment` uses a WHERE clause (filtered index), which requires SQL Server 2008 or later.

2. **VARBINARY Comparison**: The trigger uses DATALENGTH check for better performance when comparing large binary data.

3. **Foreign Key References**: All foreign keys reference existing tables:
   - `Instance`, `Faculty`, `Course`, `CoursePart`, `Branch`, `Subject` (from Masters)
   - `MiscMaster` (for PaperCategory)
   - `SubjectChapterDetail` (for UnitId)
   - `Structure`, `StructureDetail` (from Question Bank)
   - `AspNetUsers`, `AspNetRoles` (from ASP.NET Identity)

4. **Encryption**: QuestionText and ModelAnswer are stored as VARBINARY(MAX) for encrypted storage. The trigger correctly logs changes to these fields.

## Testing the Script

1. **Backup Database**: Always backup before running schema changes
2. **Run Script**: Execute the entire script in SQL Server Management Studio
3. **Check Errors**: Review Messages tab for any errors
4. **Verify Tables**: Run `SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE 'QB_%'`
5. **Verify Constraints**: Run `SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME LIKE 'QB_%'`
6. **Verify Trigger**: Run `SELECT * FROM sys.triggers WHERE name = 'trg_QB_QuestionBank_Update'`
