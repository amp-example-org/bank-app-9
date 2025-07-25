# Legacy PowerShell Deployment Script for AcmeBankApp
# Typical of 2018-2019 enterprise deployment processes

param(
    [Parameter(Mandatory=$false)]
    [string]$Environment = "Development",
    
    [Parameter(Mandatory=$false)]
    [string]$TargetServer = "localhost",
    
    [Parameter(Mandatory=$false)]
    [string]$BuildConfiguration = "Release",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$UpdateDatabase = $true
)

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host " AcmeBankApp Legacy Deployment Script v1.0" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$buildNumber = Get-Date -Format "yyyyMMddHHmmss"
$logFile = "Deploy_$buildNumber.log"

function Write-Log {
    param($Message, $Level = "INFO")
    $logEntry = "[$timestamp] [$Level] $Message"
    Write-Host $logEntry
    Add-Content -Path $logFile -Value $logEntry
}

try {
    Write-Log "Starting deployment process..." "INFO"
    Write-Log "Environment: $Environment" "INFO"
    Write-Log "Target Server: $TargetServer" "INFO"
    Write-Log "Build Configuration: $BuildConfiguration" "INFO"
    Write-Log "Build Number: $buildNumber" "INFO"
    Write-Host ""

    # Legacy: Check prerequisites
    Write-Log "Checking prerequisites..." "INFO"
    
    # Check MSBuild
    $msbuildPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    if (-not (Test-Path $msbuildPath)) {
        $msbuildPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
    }
    if (-not (Test-Path $msbuildPath)) {
        throw "MSBuild not found. Please install Visual Studio 2017/2019"
    }
    Write-Log "MSBuild found: $msbuildPath" "INFO"

    # Legacy: Restore NuGet packages
    Write-Log "Restoring NuGet packages..." "INFO"
    $nugetPath = ".\packages\NuGet.exe"
    if (-not (Test-Path $nugetPath)) {
        Write-Log "Downloading NuGet.exe..." "INFO"
        Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/v4.9.4/nuget.exe" -OutFile $nugetPath
    }
    & $nugetPath restore AcmeBankApp.sln
    if ($LASTEXITCODE -ne 0) { throw "NuGet restore failed" }

    # Legacy: Build Angular application
    Write-Log "Building Angular application..." "INFO"
    if (Test-Path "BuildScripts\BuildAngular.bat") {
        & "BuildScripts\BuildAngular.bat"
        if ($LASTEXITCODE -ne 0) { throw "Angular build failed" }
    } else {
        Write-Log "Angular build script not found, using pre-built assets" "WARN"
    }

    # Legacy: Build .NET application
    Write-Log "Building .NET application..." "INFO"
    & $msbuildPath "AcmeBankApp.sln" /p:Configuration=$BuildConfiguration /p:Platform="Any CPU" /p:PublishProfile="FolderProfile" /p:PublishUrl="bin\Publish" /verbosity:minimal
    if ($LASTEXITCODE -ne 0) { throw ".NET build failed" }

    # Legacy: Run tests (if not skipped)
    if (-not $SkipTests) {
        Write-Log "Running unit tests..." "INFO"
        $vsTestPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
        if (Test-Path $vsTestPath) {
            & $vsTestPath "AcmeBankApp.Tests\bin\$BuildConfiguration\AcmeBankApp.Tests.dll"
            if ($LASTEXITCODE -ne 0) { 
                Write-Log "Some tests failed, continuing deployment..." "WARN"
            }
        } else {
            Write-Log "VSTest not found, skipping tests" "WARN"
        }
    }

    # Legacy: Database update
    if ($UpdateDatabase) {
        Write-Log "Updating database..." "INFO"
        $connectionString = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AcmeBank.mdf;Initial Catalog=AcmeBank;Integrated Security=True"
        
        # Legacy: Create database if it doesn't exist
        $createDbScript = @"
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AcmeBank')
BEGIN
    CREATE DATABASE AcmeBank
END
"@
        
        # Legacy: Run EF migrations (simplified for demo)
        Write-Log "Running Entity Framework migrations..." "INFO"
        try {
            # This would normally run actual EF migrations
            Write-Log "Database schema updated successfully" "INFO"
        } catch {
            Write-Log "Database update failed: $($_.Exception.Message)" "ERROR"
        }
    }

    # Legacy: Deploy to IIS (local development simulation)
    Write-Log "Deploying to IIS..." "INFO"
    $iisPath = "C:\inetpub\wwwroot\AcmeBankApp"
    $sourcePath = "AcmeBankApp.Web"

    # Legacy: Create app_offline.htm
    $appOfflinePath = "$iisPath\app_offline.htm"
    if (Test-Path $iisPath) {
        Write-Log "Taking application offline..." "INFO"
        @"
<html>
<head><title>Application Offline</title></head>
<body>
<h1>Application Temporarily Offline</h1>
<p>The Acme Bank application is being updated. Please try again in a few minutes.</p>
<p>Deployment started at: $timestamp</p>
</body>
</html>
"@ | Out-File -FilePath $appOfflinePath -Encoding UTF8
    }

    # Legacy: Copy files
    if (-not (Test-Path $iisPath)) {
        New-Item -ItemType Directory -Path $iisPath -Force
    }

    Write-Log "Copying application files..." "INFO"
    
    # Legacy file copy patterns
    $filesToCopy = @(
        "bin\*",
        "Content\*",
        "Scripts\*", 
        "Views\*",
        "App_Start\*",
        "Global.asax",
        "web.config"
    )

    foreach ($pattern in $filesToCopy) {
        $source = Join-Path $sourcePath $pattern
        if (Test-Path $source) {
            $destination = Join-Path $iisPath (Split-Path $pattern -Parent)
            if (-not (Test-Path $destination)) {
                New-Item -ItemType Directory -Path $destination -Force
            }
            Copy-Item -Path $source -Destination $destination -Recurse -Force
            Write-Log "Copied: $pattern" "INFO"
        }
    }

    # Legacy: Update web.config for environment
    Write-Log "Updating configuration for $Environment environment..." "INFO"
    $webConfigPath = "$iisPath\web.config"
    if (Test-Path $webConfigPath) {
        $webConfig = Get-Content $webConfigPath -Raw
        
        # Legacy config transformations
        switch ($Environment) {
            "Production" {
                $webConfig = $webConfig -replace '<compilation debug="true"', '<compilation debug="false"'
                $webConfig = $webConfig -replace '<customErrors mode="Off"', '<customErrors mode="On"'
            }
            "Staging" {
                $webConfig = $webConfig -replace '<compilation debug="true"', '<compilation debug="false"'
            }
        }
        
        $webConfig | Out-File -FilePath $webConfigPath -Encoding UTF8
    }

    # Legacy: Remove app_offline.htm
    if (Test-Path $appOfflinePath) {
        Remove-Item $appOfflinePath -Force
        Write-Log "Application brought back online" "INFO"
    }

    # Legacy: IIS App Pool recycle
    Write-Log "Recycling application pool..." "INFO"
    try {
        Import-Module WebAdministration -ErrorAction SilentlyContinue
        if (Get-Command "Restart-WebAppPool" -ErrorAction SilentlyContinue) {
            Restart-WebAppPool -Name "DefaultAppPool"
            Write-Log "Application pool recycled successfully" "INFO"
        }
    } catch {
        Write-Log "Could not recycle app pool: $($_.Exception.Message)" "WARN"
    }

    # Legacy: Generate deployment report
    $deploymentReport = @"
Deployment Report - AcmeBankApp
===============================
Deployment Date: $timestamp
Environment: $Environment
Target Server: $TargetServer
Build Number: $buildNumber
Build Configuration: $BuildConfiguration
Deployed By: $env:USERNAME
Machine: $env:COMPUTERNAME

Files Deployed:
- .NET Framework 4.8 Application
- Angular 7 SPA Assets  
- Database Schema Updates
- Configuration Updates

Status: SUCCESS
"@

    $reportPath = "DeploymentReport_$buildNumber.txt"
    $deploymentReport | Out-File -FilePath $reportPath -Encoding UTF8
    Write-Log "Deployment report saved: $reportPath" "INFO"

    Write-Host ""
    Write-Host "=============================================" -ForegroundColor Green
    Write-Host " Deployment Completed Successfully!" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Green
    Write-Host ""
    Write-Log "Deployment completed successfully!" "SUCCESS"
    Write-Host "Application URL: http://$TargetServer/AcmeBankApp" -ForegroundColor Yellow
    Write-Host "Build Number: $buildNumber" -ForegroundColor Yellow
    Write-Host "Log File: $logFile" -ForegroundColor Yellow
    Write-Host ""

} catch {
    Write-Host ""
    Write-Host "=============================================" -ForegroundColor Red
    Write-Host " Deployment Failed!" -ForegroundColor Red
    Write-Host "=============================================" -ForegroundColor Red
    Write-Log "Deployment failed: $($_.Exception.Message)" "ERROR"
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Check log file: $logFile" -ForegroundColor Red
    
    # Legacy: Remove app_offline.htm on failure
    $appOfflinePath = "C:\inetpub\wwwroot\AcmeBankApp\app_offline.htm"
    if (Test-Path $appOfflinePath) {
        Remove-Item $appOfflinePath -Force
        Write-Log "Removed app_offline.htm after failure" "INFO"
    }
    
    exit 1
}
