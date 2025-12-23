# Script to create GitHub repository and push code
param(
    [string]$Token = "",
    [string]$RepoName = "Corno.Education",
    [string]$OrgName = "concept-systems",
    [string]$Description = "Online Exam System - Corno Education Platform"
)

$ErrorActionPreference = "Stop"

# If no token provided, try to push directly first (uses stored git credentials)
if ([string]::IsNullOrEmpty($Token)) {
    Write-Host "No token provided. Attempting to push directly using stored git credentials..." -ForegroundColor Cyan
    Write-Host "If repository doesn't exist, this will fail and we'll need to create it first." -ForegroundColor Yellow
    Write-Host ""
    
    # Try to push - if repo exists, this will work with stored credentials
    git remote remove origin 2>$null
    git remote add origin https://github.com/$OrgName/$RepoName.git
    git branch -M main
    
    $pushResult = git push -u origin main 2>&1
    $pushExitCode = $LASTEXITCODE
    
    if ($pushExitCode -eq 0) {
        Write-Host ""
        Write-Host "Success! Code has been pushed to GitHub using stored credentials." -ForegroundColor Green
        Write-Host "Repository: https://github.com/$OrgName/$RepoName" -ForegroundColor Green
        exit 0
    } else {
        Write-Host "Push failed. Error: $pushResult" -ForegroundColor Red
        Write-Host ""
        Write-Host "The repository doesn't exist yet. To create it automatically, you need a Personal Access Token." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Please run this script with a token:" -ForegroundColor Cyan
        Write-Host "  .\create-github-repo.ps1 -Token 'YOUR_GITHUB_TOKEN'" -ForegroundColor White
        Write-Host ""
        Write-Host "Or create the repository manually at:" -ForegroundColor Cyan
        Write-Host "  https://github.com/organizations/$OrgName/repositories/new" -ForegroundColor White
        Write-Host "  Then run: git push -u origin main" -ForegroundColor White
        exit 1
    }
}

# Create repository via GitHub API
Write-Host "Creating repository '$RepoName' under organization '$OrgName'..." -ForegroundColor Cyan

$headers = @{
    "Authorization" = "token $Token"
    "Accept" = "application/vnd.github.v3+json"
}

$body = @{
    name = $RepoName
    description = $Description
    private = $false
    auto_init = $false
} | ConvertTo-Json

try {
    $url = "https://api.github.com/orgs/$OrgName/repos"
    $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body -ContentType "application/json"
    
    Write-Host "Repository created successfully!" -ForegroundColor Green
    Write-Host "Repository URL: $($response.html_url)" -ForegroundColor Green
    
    # Now push the code
    Write-Host ""
    Write-Host "Pushing code to repository..." -ForegroundColor Cyan
    
    $repoUrl = "https://$Token@github.com/$OrgName/$RepoName.git"
    
    # Remove existing remote if it exists
    git remote remove origin 2>$null
    
    # Add remote with token
    git remote add origin $repoUrl
    git branch -M main
    
    # Push code
    git push -u origin main
    
    Write-Host ""
    Write-Host "Success! Code has been pushed to GitHub." -ForegroundColor Green
    Write-Host "Repository: https://github.com/$OrgName/$RepoName" -ForegroundColor Green
    
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
    exit 1
}

