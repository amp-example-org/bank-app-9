# AcmeBankApp Development Guide

This is a legacy .NET Framework 4.8 MVC application with an embedded Angular 7 SPA. This guide covers building, running, and testing the application.

## Project Structure

```
AcmeBankApp/
├── AcmeBankApp.Core/          # Business models and interfaces
├── AcmeBankApp.Data/          # Data access layer with Entity Framework
├── AcmeBankApp.Tests/         # Unit tests (MSTest)
├── AcmeBankApp.Web/           # MVC web application + Angular SPA
├── BuildScripts/              # Deployment and build automation
├── packages/                  # NuGet packages (auto-generated)
└── AcmeBankApp.sln           # Visual Studio solution file
```

## Prerequisites

- **.NET Framework 4.8** or higher
- **MSBuild** (via .NET SDK or Visual Studio)
- **IIS Express** (for development)
- **NuGet** package manager

## Build Commands

### Build the entire solution
```bash
dotnet msbuild AcmeBankApp.sln /p:Configuration=Debug
```

### Build individual projects
```bash
# Core library
dotnet msbuild AcmeBankApp.Core/AcmeBankApp.Core.csproj /p:Configuration=Debug

# Data layer
dotnet msbuild AcmeBankApp.Data/AcmeBankApp.Data.csproj /p:Configuration=Debug

# Web application
dotnet msbuild AcmeBankApp.Web/AcmeBankApp.Web.csproj /p:Configuration=Debug
```

### Restore NuGet packages
```bash
nuget.exe restore AcmeBankApp.sln
```

## Run Commands

### Option 1: Using RunApp.bat (Recommended)
```bash
RunApp.bat
```
- Starts IIS Express on http://localhost:8080
- Uses simplified configuration for development

### Option 2: IIS Express directly
```bash
cd AcmeBankApp.Web
"C:\Program Files\IIS Express\iisexpress.exe" /path:"%CD%" /port:8080
```

### Option 3: Full IIS (if deployed)
```bash
# Application deployed to: c:\inetpub\wwwroot\AcmeBankApp
# Access via: http://localhost/AcmeBankApp
```

## Application URLs

After starting the application:

- **Home Page**: http://localhost:8080/
- **Login Page**: http://localhost:8080/Account/Login
- **Angular SPA**: http://localhost:8080/app
- **API Endpoints**: http://localhost:8080/api/

### Demo Credentials
- **Username**: `demo`
- **Password**: `password123`

## Test Commands

### Run all tests
```bash
# Using MSTest (if available)
vstest.console.exe AcmeBankApp.Tests\bin\Debug\AcmeBankApp.Tests.dll

# Or using test runner in Visual Studio
# Test Explorer → Run All Tests
```

### Test projects
- **AcmeBankApp.Tests**: Unit tests for services and data layer
- Test files: `*Tests.cs` in the Tests project

## Development Workflow

### 1. Make code changes
Edit files in Visual Studio or your preferred editor

### 2. Build and deploy locally
```bash
# Build the solution
dotnet msbuild AcmeBankApp.Web/AcmeBankApp.Web.csproj /p:Configuration=Debug

# Copy updated DLLs to IIS (if using full IIS)
copy "AcmeBankApp.Web\bin\AcmeBankApp.Web.dll" "c:\inetpub\wwwroot\AcmeBankApp\bin\"
copy "AcmeBankApp.Core\bin\Debug\AcmeBankApp.Core.dll" "c:\inetpub\wwwroot\AcmeBankApp\bin\"
copy "AcmeBankApp.Data\bin\Debug\AcmeBankApp.Data.dll" "c:\inetpub\wwwroot\AcmeBankApp\bin\"
```

### 3. Test the application
- Navigate to http://localhost:8080
- Test login functionality
- Verify Angular SPA loads at `/app`

## Key Configuration Files

### Web.config
- **Location**: `AcmeBankApp.Web/Web.config`
- **Contains**: Database connections, app settings, NLog configuration
- **Important settings**:
  - `MaxTransferAmount`: 50000
  - `SessionTimeoutMinutes`: 60
  - `BankApiKey`: ACME-BANK-2019-SECRET-KEY-12345

### Route Configuration
- **Location**: `AcmeBankApp.Web/App_Start/RouteConfig.cs`
- **Key routes**:
  - `app/{*catchall}` → App controller (Angular SPA)
  - `api/{action}` → Api controller
  - Default MVC routes

### Dependency Injection
- **Framework**: Autofac
- **Configuration**: `AcmeBankApp.Web/App_Start/AutofacConfig.cs`
- **Services**: UserService, AccountService

## Common Issues & Solutions

### Build Errors

**Error**: `Microsoft.WebApplication.targets not found`
```bash
# Temporarily disable in .csproj file:
<!-- <Import Project="$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets" /> -->
```

**Error**: Missing `System` namespace
```csharp
// Add to Global.asax.cs:
using System;
```

### Runtime Errors

**Error**: NullReferenceException in views
- Ensure controllers pass proper models to views
- Check ViewBag/ViewData properties are set

**Error**: 404 on `/app` route
- Verify base href in `Views/App/Index.cshtml`: `<base href="/app/">`
- Check MVC routing in RouteConfig.cs

**Error**: Autofac dependency injection failures
- Ensure parameterless constructors exist for controllers
- Verify services are registered in AutofacConfig.cs

### IIS Express Issues

**Error**: "Unable to start iisexpress"
- Try running with elevated permissions
- Check if port 8080 is available: `netstat -an | findstr :8080`
- Use simplified path-based hosting instead of config files

## Logging

### NLog Configuration
- **Log Directory**: `c:\logs\acmebank\`
- **Log Files**: `{date}.log` (e.g., `2025-01-24.log`)
- **Configuration**: In Web.config `<nlog>` section

### Debug Output
- Use `LogHelper.Info()`, `LogHelper.Error()` for application logging
- Check Windows Event Viewer → Application logs for system errors

## Architecture Notes

### Legacy Patterns
This is a 2018-era application with some legacy patterns:
- **Service Locator**: Anti-pattern alongside proper DI
- **Static LogHelper**: Should be injected dependency
- **Mixed authentication**: Session-based with some modern elements
- **Inline scripts**: Angular + jQuery mixed approach

### Security Considerations
- **No CSRF protection**: Forms lack `@Html.AntiForgeryToken()`
- **Exposed API keys**: Secrets in ViewBag passed to client-side
- **Plain text configs**: Sensitive data in Web.config
- **Minimal input validation**: Legacy validation patterns

### Database
- **Provider**: Entity Framework 6 with SQL Server LocalDB
- **Connection**: `(LocalDB)\MSSQLLocalDB` 
- **Schema**: Created via `App_Data/CreateDatabase.sql`

## Deployment

### Using BuildScripts/Deploy.ps1
```powershell
powershell -ExecutionPolicy Bypass -File "BuildScripts\Deploy.ps1" -Environment Development
```

### Manual Deployment
1. Build solution in Release mode
2. Copy `AcmeBankApp.Web` contents to IIS directory
3. Update Web.config for target environment
4. Restart application pool

## Version Control

### Commit Guidelines
- Use descriptive commit messages
- Include issue numbers when applicable
- Separate functional changes from formatting/refactoring

### Ignored Files (.gitignore)
- Build outputs (`bin/`, `obj/`)
- Visual Studio files (`.vs/`, `*.user`)
- NuGet packages (`packages/`)
- Log files (`*.log`)
- IIS configurations (`applicationhost.config`)
