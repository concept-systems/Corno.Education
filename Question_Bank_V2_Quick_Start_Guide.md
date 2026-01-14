# Question Bank V2 - Quick Start Guide

## Prerequisites

1. Run the database script: `Database_Scripts/Question_Bank_V2_CreateTables.sql`
2. Ensure Telerik controls are properly configured in the project
3. Ensure MathType plugin is available (if needed for formulas)

## Setup Steps

### 1. Database Setup
```sql
-- Execute the SQL script
USE [Corno_Bharati_OnlineExam]
GO
-- Run: Database_Scripts/Question_Bank_V2_CreateTables.sql
```

### 2. Register Area (if not auto-registered)
Add to `Global.asax.cs`:
```csharp
AreaRegistration.RegisterAllAreas();
```

### 3. Add Menu Item (Optional)
Add to your navigation menu:
```html
<li>
    <a href="@Url.Action("Index", "QB_QuestionBank", new { area = "Question Bank V2" })">
        <i class="fa fa-book"></i> Question Bank V2
    </a>
</li>
```

### 4. Configure Telerik Editor for MathType (If Required)
In the Create/Edit views, you may need to add MathType plugin configuration:
```javascript
// Add to Telerik Editor configuration
.Tools(tools => tools
    // ... existing tools ...
    .CustomButton("MathType")
    .CustomButton("ChemType")
)
```

## Usage

### Creating a Question
1. Navigate to: `/Question Bank V2/QB_QuestionBank/Create`
2. Select context (Instance, Faculty, Course, Subject)
3. Enter question text using Telerik Editor
4. Select Unit, Difficulty, Taxonomy
5. Enter marks and other details
6. Enter model answer (optional)
7. Click "Save as Draft" or "Submit for Check"

### Viewing Questions
1. Navigate to: `/Question Bank V2/QB_QuestionBank/Index`
2. Use Telerik Grid filters to find questions
3. Click "View" to see full question
4. Click "Edit" to modify (if allowed)

### Workflow
- **Setter**: Creates questions → Submits for check
- **Checker**: Reviews → Approves/Rejects/Requests revision
- **Moderator**: Generates papers from approved questions

## Encryption

- QuestionText and ModelAnswer are automatically encrypted
- Encryption uses hardcoded standard keys (AES-256)
- Keys are in: `QuestionEncryptionService.cs`
- **DO NOT CHANGE** encryption keys without re-encrypting all data

## Important Notes

1. **Unit vs Chapter**: Database uses `UnitId` (maps to `SubjectChapterDetail.Id`), displayed as "Unit" in UI

2. **Status Flow**:
   - Draft → Submitted for Check → Approved by Checker → Approved
   - Can be rejected at any stage
   - Can request revision

3. **Paper Status**:
   - Draft/Generated: Can modify
   - Drawn: LOCKED (cannot modify)

4. **Telerik Editor**: Currently configured with basic tools. For MathType, additional plugin configuration may be needed.

## Troubleshooting

### Issue: Telerik Grid not loading
- Check Telerik scripts are included in layout
- Verify area registration
- Check controller action returns proper JSON

### Issue: Editor not saving content
- Ensure form submission gets editor value via JavaScript
- Check `ValidateInput(false)` attribute on controller action

### Issue: Encryption errors
- Verify encryption service is registered in Bootstrapper
- Check encryption keys are not modified

## Next Implementation Steps

1. **Appointment Management**
   - Create QB_AppointmentController
   - Implement notification system (Email/SMS/WhatsApp)
   - Generate login credentials

2. **Paper Generation**
   - Implement auto-generation algorithm
   - Create manual selection interface
   - Word document generation (OpenXML)

3. **Dashboard**
   - Role-based dashboards
   - Statistics and analytics
   - Progress tracking

4. **Review Interface**
   - Checker review page
   - Moderator review page
   - Approval/rejection workflow

## Support

For issues or questions, refer to:
- `Question_Bank_V2_Implementation_Summary.md` - Complete implementation details
- Database script comments
- Code comments in service files
