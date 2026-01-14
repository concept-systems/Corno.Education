# Question Bank V2 - DocumentFormat.OpenXml Package Fix

## Issue
Build error: `The type or namespace name 'DocumentFormat' could not be found`

## Root Cause
The `WordDocumentService.cs` uses `DocumentFormat.OpenXml` namespace, but the NuGet package `DocumentFormat.OpenXml` was not installed/referenced in the `Corno.Services` project.

## Fix Applied

### 1. Added Package Reference to Project File
**File**: `Libraries/Corno.Services/Corno.Services.csproj`

Added the reference:
```xml
<Reference Include="DocumentFormat.OpenXml, Version=2.20.0.0, Culture=neutral, PublicKeyToken=8fb06cb64d019a17, processorArchitecture=MSIL">
  <HintPath>..\..\packages\DocumentFormat.OpenXml.2.20.0\lib\net46\DocumentFormat.OpenXml.dll</HintPath>
</Reference>
```

### 2. Added Package to packages.config
**File**: `Libraries/Corno.Services/packages.config`

Added:
```xml
<package id="DocumentFormat.OpenXml" version="2.20.0" targetFramework="net48" />
```

## Next Steps Required

### Install the NuGet Package

You need to install the `DocumentFormat.OpenXml` NuGet package. You can do this in one of the following ways:

#### Option 1: Using NuGet Package Manager Console
1. Open Visual Studio
2. Go to **Tools** → **NuGet Package Manager** → **Package Manager Console**
3. Select the `Corno.Services` project
4. Run:
   ```
   Install-Package DocumentFormat.OpenXml -Version 2.20.0
   ```

#### Option 2: Using NuGet Package Manager UI
1. Right-click on the `Corno.Services` project
2. Select **Manage NuGet Packages...**
3. Go to **Browse** tab
4. Search for `DocumentFormat.OpenXml`
5. Select version `2.20.0`
6. Click **Install**

#### Option 3: Restore Packages
If the package is already in `packages.config`, you can restore:
1. Right-click on the solution
2. Select **Restore NuGet Packages**

## Package Details

- **Package Name**: DocumentFormat.OpenXml
- **Version**: 2.20.0
- **Target Framework**: .NET Framework 4.6+
- **Purpose**: Generate Word documents (.docx) using OpenXML SDK

## What This Package Does

The `DocumentFormat.OpenXml` package provides:
- `DocumentFormat.OpenXml` namespace - Core OpenXML types
- `DocumentFormat.OpenXml.Packaging` namespace - Document packaging
- `DocumentFormat.OpenXml.Wordprocessing` namespace - Word document elements

This is used by `WordDocumentService.cs` to generate Word documents for papers.

## Verification

After installing the package, verify:
1. The package appears in `packages` folder
2. The reference is resolved in Visual Studio
3. The build succeeds without errors
4. The `using DocumentFormat.OpenXml;` statements work

## Alternative Solution

If you prefer not to use OpenXML, you could:
1. Use Telerik Document Processing (already in project)
2. Use NPOI (already in project, but primarily for Excel)
3. Use DevExpress RichEdit (already in project)

However, OpenXML is the standard Microsoft SDK for Word document generation and is recommended.
