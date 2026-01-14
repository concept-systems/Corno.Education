# Question Bank V2 - Complete Testing Guide

## 📋 Prerequisites

### 1. Database Setup
- Run the SQL script: `Database_Scripts/Question_Bank_V2_CreateTables.sql`
- Verify all tables are created successfully
- Ensure master data exists:
  - `QB_QuestionType` (MCQ, Short Answer, Long Answer, etc.)
  - `QB_DifficultyLevel` (Easy, Medium, Hard, Very Hard)
  - `QB_TaxonomyLevel` (Remember, Understand, Apply, Analyze, Evaluate, Create)
  - `AspNetRoles` (Question Setter, Question Checker, Moderator roles should exist)

### 2. Master Data Setup
Ensure the following master data exists:
- **Instance** (from Masters)
- **Faculty** (from Masters)
- **Course** (from Masters)
- **CoursePart** (from Masters)
- **Subject** (from Masters)
- **Staff** (from Masters) - with mobile numbers
- **Structure** (from Question Bank area) - for paper generation

### 3. User Setup
- Create users in `AspNetUsers` table
- Assign roles: "Question Setter", "Question Checker", "Moderator"
- Link staff to users (Staff.Mobile should match AspNetUser.UserName)

---

## 🧪 Testing Flow

### Phase 1: Setup and Configuration

#### Test 1.1: Verify Area Registration
1. **Action**: Build and run the application
2. **Expected**: Application starts without errors
3. **Verify**: Check browser console for any JavaScript errors
4. **URL Test**: Navigate to `/Question Bank V2/QB_Dashboard`
5. **Expected**: Dashboard page loads (may require login)

#### Test 1.2: Verify Menu Navigation
1. **Action**: Login as admin/user with Question Bank access
2. **Expected**: Menu shows "Question Bank V2" dropdown
3. **Verify**: Menu items visible:
   - Dashboard
   - Questions
   - Appointments
   - Paper Generation

---

### Phase 2: Question Management (Question Setter Role)

#### Test 2.1: Create Question
1. **Navigate**: Question Bank V2 > Questions > Create
2. **Fill Form**:
   - Select Instance, Faculty, Course, CoursePart, Subject
   - Enter Question Code (auto-generated)
   - Select Question Type (MCQ, Short Answer, etc.)
   - Select Unit (from SubjectChapterDetail)
   - Select Difficulty Level
   - Select Taxonomy Level
   - Enter Marks
   - Enter Question Text (using CKEditor with MathType)
   - Enter Model Answer (using CKEditor)
3. **Action**: Click "Create"
4. **Expected**:
   - Question saved successfully
   - Question text and model answer encrypted in database
   - Status = "Draft"
   - Question appears in grid

#### Test 2.2: View Question
1. **Navigate**: Question Bank V2 > Questions
2. **Action**: Click "View" on any question
3. **Expected**:
   - Question displays correctly
   - HTML content renders properly
   - Math formulas display (if MathType used)
   - Model answer visible

#### Test 2.3: Edit Question
1. **Navigate**: Question Bank V2 > Questions
2. **Action**: Click "Edit" on a Draft question
3. **Modify**: Change question text or model answer
4. **Action**: Click "Save"
5. **Expected**:
   - Changes saved
   - Change logged in `QB_QuestionChangeLog`
   - Updated content encrypted

#### Test 2.4: Submit Question
1. **Navigate**: Question Bank V2 > Questions
2. **Action**: Edit a Draft question
3. **Change Status**: Set Status = "Submitted"
4. **Action**: Save
5. **Expected**:
   - Status changed to "Submitted"
   - Question visible to Checker

---

### Phase 3: Question Review (Question Checker Role)

#### Test 3.1: Review Submitted Questions
1. **Login**: As user with "Question Checker" role
2. **Navigate**: Question Bank V2 > Dashboard
3. **Expected**: Checker Dashboard shows:
   - Pending Review count
   - Reviewed Today count
   - Total Reviewed count
4. **Navigate**: Question Bank V2 > Questions
5. **Filter**: Status = "Submitted"
6. **Expected**: See questions assigned to checker

#### Test 3.2: Approve/Reject Question
1. **Navigate**: Question Bank V2 > Questions
2. **Action**: Edit a Submitted question
3. **Review**: Check question content
4. **Action**: 
   - Set Status = "Approved" (if good)
   - Set Status = "Rejected" (if needs changes)
5. **Action**: Save
6. **Expected**:
   - Status updated
   - Workflow logged in `QB_QuestionWorkflow`
   - If Approved: Question available for paper generation

---

### Phase 4: Appointment Management (Admin/Moderator)

#### Test 4.1: Create Appointment
1. **Navigate**: Question Bank V2 > Appointments > Create
2. **Fill Form**:
   - Select Faculty, Course, Subject
   - Select Structure
   - Enter Number of Papers
   - Set Appointment Date and Time
   - Enter Instructions (optional)
3. **Assign Roles**:
   - Select Question Setters (MultiSelect)
   - Select Question Checkers (MultiSelect)
   - Select Moderators (MultiSelect)
4. **Action**: Click "Create Appointment"
5. **Expected**:
   - Appointment created
   - Appointment code generated (APT-YYYY-#####)
   - Login credentials generated for each assigned staff
   - Status = "Created"

#### Test 4.2: Send Notifications
1. **Navigate**: Question Bank V2 > Appointments
2. **Action**: Click "View" on an appointment
3. **Action**: Click "Send Email" button
4. **Expected**:
   - Email sent to all assigned staff
   - Email contains:
     - Appointment details
     - Temporary username
     - Temporary password
     - Deadline information
5. **Verify**: Check `QB_AppointmentDetail` table:
   - `EmailSent = true`
   - `EmailSentDate` populated
   - `EmailSentCount` incremented

6. **Repeat** for SMS and WhatsApp notifications

#### Test 4.3: View Appointment Details
1. **Navigate**: Question Bank V2 > Appointments
2. **Action**: Click "View" on an appointment
3. **Expected**: See:
   - Appointment information
   - Assigned staff list
   - Login credentials for each staff
   - Notification status (Email/SMS/WhatsApp sent)

---

### Phase 5: Paper Generation (Moderator Role)

#### Test 5.1: Automatic Paper Generation
1. **Prerequisites**:
   - At least one approved question exists
   - Structure is defined
   - Appointment is created
2. **Navigate**: Question Bank V2 > Paper Generation
3. **Action**: Click "Generate Auto" (or navigate from appointment)
4. **Fill Form**:
   - Select Appointment
   - Enter Number of Sets (e.g., 3)
5. **Action**: Click "Generate Papers"
6. **Expected**:
   - Papers generated successfully
   - Each paper has unique code (PAP-YYYY-#####-SET#)
   - Questions selected based on:
     - Structure requirements
     - Difficulty distribution
     - Taxonomy distribution
     - Usage history
   - Paper Status = "Generated"

#### Test 5.2: Manual Paper Generation
1. **Navigate**: Question Bank V2 > Paper Generation
2. **Action**: Click "Generate Manual"
3. **Select**: Appointment
4. **For each Structure Detail**:
   - Select a question from available approved questions
5. **Action**: Click "Generate Paper"
6. **Expected**:
   - Paper created with selected questions
   - Paper Status = "Generated"

#### Test 5.3: View Paper
1. **Navigate**: Question Bank V2 > Paper Generation
2. **Action**: Click "View" on a generated paper
3. **Expected**: See:
   - Paper information (Code, Set Number, Type, Max Marks)
   - List of questions with:
     - Section number
     - Question number
     - Question text (preview)
     - Marks
     - Difficulty level
     - Taxonomy level

#### Test 5.4: Draw Paper
1. **Navigate**: Question Bank V2 > Paper Generation
2. **Action**: Click "View" on a Generated paper
3. **Action**: Click "Draw Paper" button
4. **Confirm**: Click "OK" on confirmation dialog
5. **Expected**:
   - Paper Status = "Drawn"
   - Word document generated
   - Paper locked (cannot be modified)
   - `DrawnDate` populated

#### Test 5.5: Download Word Document
1. **Navigate**: Question Bank V2 > Paper Generation
2. **Action**: Click "View" on a Drawn paper
3. **Action**: Click "Download Word"
4. **Expected**:
   - Word document (.docx) downloads
   - Document contains:
     - Header with paper information
     - Instructions
     - Sections and questions
     - Proper formatting

---

### Phase 6: Dashboard Testing

#### Test 6.1: Admin Dashboard
1. **Login**: As admin user
2. **Navigate**: Question Bank V2 > Dashboard
3. **Expected**: See statistics cards:
   - Total Questions
   - Approved Questions
   - Total Appointments
   - Total Papers
4. **Action**: Click on each card
5. **Expected**: Navigate to respective module

#### Test 6.2: Setter Dashboard
1. **Login**: As user with "Question Setter" role
2. **Navigate**: Question Bank V2 > Dashboard
3. **Expected**: See:
   - My Questions count
   - Draft Questions count
   - Submitted Questions count
   - Approved Questions count
   - Recent Appointments list

#### Test 6.3: Checker Dashboard
1. **Login**: As user with "Question Checker" role
2. **Navigate**: Question Bank V2 > Dashboard
3. **Expected**: See:
   - Pending Review count
   - Reviewed Today count
   - Total Reviewed count
   - Recent Appointments list

#### Test 6.4: Moderator Dashboard
1. **Login**: As user with "Moderator" role
2. **Navigate**: Question Bank V2 > Dashboard
3. **Expected**: See:
   - Total Papers count
   - Generated Papers count
   - Drawn Papers count
   - Recent Appointments with action buttons

---

## 🔍 Verification Checklist

### Database Verification
- [ ] All tables created (`QB_QuestionBank`, `QB_Appointment`, `QB_Paper`, etc.)
- [ ] Foreign key constraints working
- [ ] Encryption working (QuestionText and ModelAnswer are VARBINARY)
- [ ] Audit logs working (`QB_QuestionChangeLog`, `QB_QuestionWorkflow`)

### Security Verification
- [ ] Questions encrypted in database
- [ ] Questions decrypted when retrieved
- [ ] Role-based access control working
- [ ] Users can only access their assigned questions/appointments

### Functionality Verification
- [ ] Question CRUD operations working
- [ ] Appointment creation and notification sending working
- [ ] Automatic paper generation algorithm working
- [ ] Manual paper generation working
- [ ] Word document generation working
- [ ] Paper drawing and locking working

### UI/UX Verification
- [ ] All pages load without errors
- [ ] Telerik Grids display data correctly
- [ ] CKEditor with MathType working
- [ ] Dropdowns populate correctly
- [ ] Forms validate correctly
- [ ] Error messages display properly
- [ ] Success messages display properly

---

## 🐛 Common Issues and Solutions

### Issue 1: Area Not Found
**Error**: "The view 'Index' or its master was not found"
**Solution**: 
- Verify area registration file exists
- Check namespace matches area name
- Rebuild solution

### Issue 2: Service Not Registered
**Error**: "The type X is not registered"
**Solution**:
- Check `Bootstrapper.cs` has all service registrations
- Verify service interfaces match implementations

### Issue 3: Encryption/Decryption Errors
**Error**: "Padding is invalid and cannot be removed"
**Solution**:
- Verify encryption key hasn't changed
- Check key version in encrypted data
- Ensure `QuestionEncryptionService` is registered

### Issue 4: Questions Not Appearing
**Error**: Questions not showing in grid
**Solution**:
- Check `GetQuestions` action returns data
- Verify decryption is working
- Check user permissions
- Verify InstanceId matches

### Issue 5: Paper Generation Fails
**Error**: "No approved questions available"
**Solution**:
- Ensure questions are approved (Status = "Approved")
- Verify questions match structure requirements
- Check subject and instance match

---

## 📊 Test Data Setup Script

```sql
-- Insert Master Data for Testing

-- Question Types
INSERT INTO QB_QuestionType (Name, Code, Status) VALUES
('Multiple Choice', 'MCQ', 'Active'),
('Short Answer', 'SA', 'Active'),
('Long Answer', 'LA', 'Active');

-- Difficulty Levels
INSERT INTO QB_DifficultyLevel (Name, Code, Status) VALUES
('Easy', 'EASY', 'Active'),
('Medium', 'MED', 'Active'),
('Hard', 'HARD', 'Active'),
('Very Hard', 'VHARD', 'Active');

-- Taxonomy Levels
INSERT INTO QB_TaxonomyLevel (Name, Code, Level, Status) VALUES
('Remember', 'REM', 1, 'Active'),
('Understand', 'UND', 2, 'Active'),
('Apply', 'APP', 3, 'Active'),
('Analyze', 'ANA', 4, 'Active'),
('Evaluate', 'EVA', 5, 'Active'),
('Create', 'CRE', 6, 'Active');

-- Create Roles (if not exists)
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Question Setter')
    INSERT INTO AspNetRoles (Id, Name) VALUES (NEWID(), 'Question Setter');

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Question Checker')
    INSERT INTO AspNetRoles (Id, Name) VALUES (NEWID(), 'Question Checker');

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Moderator')
    INSERT INTO AspNetRoles (Id, Name) VALUES (NEWID(), 'Moderator');
```

---

## 🎯 Quick Test Scenarios

### Scenario 1: Complete Workflow
1. Admin creates appointment
2. Admin sends notifications
3. Setter creates questions
4. Setter submits questions
5. Checker reviews and approves
6. Moderator generates paper (auto)
7. Moderator draws paper
8. Moderator downloads Word document

### Scenario 2: Multiple Sets
1. Create appointment with 3 papers
2. Generate 3 sets automatically
3. Verify each set has different questions
4. Draw all 3 sets
5. Download all Word documents

### Scenario 3: Manual Selection
1. Create appointment
2. Generate paper manually
3. Select specific questions
4. Verify paper contains selected questions
5. Draw and download

---

## ✅ Success Criteria

The module is working correctly if:
1. ✅ All pages load without errors
2. ✅ Questions can be created, edited, and viewed
3. ✅ Appointments can be created and notifications sent
4. ✅ Papers can be generated (auto and manual)
5. ✅ Papers can be drawn and Word documents downloaded
6. ✅ Dashboards show correct statistics
7. ✅ Role-based access control works
8. ✅ Data encryption/decryption works
9. ✅ All Telerik controls function properly
10. ✅ No console errors in browser

---

**Note**: This is a comprehensive testing guide. Start with Phase 1 and proceed sequentially. If any phase fails, fix issues before proceeding to the next phase.
