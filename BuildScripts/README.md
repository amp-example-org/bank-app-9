# AcmeBankApp Build Scripts

This folder contains legacy build and deployment scripts typical of 2018-2019 enterprise .NET development environments.

## Scripts Overview

### BuildAngular.bat
Legacy batch script for building the Angular 7 application. This represents the manual build processes common before modern CI/CD pipelines.

**Usage:**
```cmd
cd BuildScripts
BuildAngular.bat
```

**Features:**
- Checks for Node.js and npm installation
- Installs Angular dependencies with `--legacy-peer-deps`
- Builds Angular app for production
- Copies assets to the .NET application
- Generates build information file
- Uses pre-built assets as fallback

### Deploy.ps1
PowerShell deployment script for deploying to IIS. Simulates enterprise deployment practices of the era.

**Usage:**
```powershell
cd BuildScripts
.\Deploy.ps1 -Environment "Production" -TargetServer "web-server-01"
```

**Parameters:**
- `-Environment`: Development, Staging, Production (default: Development)
- `-TargetServer`: Target server name (default: localhost)
- `-BuildConfiguration`: Debug or Release (default: Release)
- `-SkipTests`: Skip unit tests (default: false)
- `-UpdateDatabase`: Run database updates (default: true)

**Features:**
- NuGet package restoration
- MSBuild compilation
- Angular application building
- Unit test execution
- Database migration (simulated)
- IIS deployment with app_offline.htm
- Web.config transformations
- Application pool recycling
- Deployment reporting

## Legacy Development Workflow

### Daily Development (2018-2019 style)
1. Developer makes changes to .NET or Angular code
2. Runs `BuildAngular.bat` to compile Angular app
3. Tests locally with IIS Express
4. Commits both source code AND built assets to source control
5. CI system deploys using `Deploy.ps1`

### Production Deployment
1. Release manager runs build scripts on build server
2. Manual testing in staging environment
3. Production deployment using PowerShell script
4. Manual verification and rollback procedures

## Pre-built Assets

The `AcmeBankApp.Web\Scripts\dist\` folder contains pre-built Angular 7 assets that are committed to source control. This was a common practice in 2018-2019 for:

- **Immediate Development**: Developers could run the app without needing Node.js locally
- **Build Consistency**: Avoided "works on my machine" issues with different Node/npm versions
- **Deployment Reliability**: Build server didn't need to compile Angular during deployment
- **Legacy Infrastructure**: Many enterprises couldn't easily install Node.js on production servers

## Technical Debt Indicators

These scripts demonstrate typical technical debt of the era:

### Build Process Issues
- Manual batch files instead of modern CI/CD
- Mixed PowerShell and batch scripts
- Hard-coded paths and assumptions
- No containerization or environment parity
- Verbose logging without structured data

### Deployment Anti-patterns
- Direct file copying instead of proper deployment packages
- `app_offline.htm` for zero-downtime deployments
- Manual IIS configuration management
- Environment-specific config file transformations
- No automated rollback procedures

### Security Concerns
- Scripts run with elevated privileges
- Passwords and connection strings in config files
- No secret management
- Deployment logs contain sensitive information

## Modern Equivalent

Today, this would be replaced by:
- GitHub Actions / Azure DevOps Pipelines
- Docker containers
- Infrastructure as Code (Terraform/ARM)
- Kubernetes deployments
- Azure Key Vault for secrets
- Blue-green or canary deployments

## Files Structure

```
BuildScripts/
├── BuildAngular.bat     # Angular build script
├── Deploy.ps1           # PowerShell deployment script
└── README.md           # This documentation

AcmeBankApp.Web/Scripts/dist/  # Pre-built Angular assets
├── main.js             # Compiled Angular application
├── runtime.js          # Angular runtime and polyfills
├── polyfills.js        # Legacy browser support
├── styles.css          # Compiled Angular styles
└── build-info.txt      # Build metadata (generated)
```

This represents an authentic snapshot of enterprise .NET development practices from 2018-2019, complete with the technical debt and manual processes that drove the industry toward modern DevOps practices.
