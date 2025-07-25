# AcmeBankApp - Legacy .NET Framework 4.8 Banking Application

A realistic recreation of a legacy enterprise banking application from the 2018-2019 era, complete with authentic technical debt, security vulnerabilities, and deprecated practices.

## 🏦 Project Overview

AcmeBankApp demonstrates a typical enterprise .NET Framework 4.8 MVC application with an embedded Angular 7 SPA. This project includes all the hallmarks of legacy development practices:

- **Legacy Architecture**: Multi-project solution with traditional layered architecture
- **Deprecated Packages**: Entity Framework 6.2, System.Data.SqlClient 4.6, Angular 7.2.15
- **Security Vulnerabilities**: SQL injection, XSS, CSRF, exposed secrets
- **Technical Debt**: Service Locator anti-pattern, static helpers, heavy web.config
- **Manual Build Process**: Batch files and PowerShell scripts instead of modern CI/CD

## 🔧 Technology Stack

### Backend (.NET Framework 4.8)
- **MVC**: ASP.NET MVC 5.2.7
- **IoC Container**: Autofac 4.9.2 (with Service Locator anti-pattern)
- **Data Access**: Entity Framework 6.2.0 + Raw ADO.NET
- **Database**: SQL Server LocalDB (legacy connection patterns)
- **Logging**: NLog 4.6.8 (static logging helper)
- **JSON**: Newtonsoft.Json 11.0.2

### Frontend (Angular 7)
- **Framework**: Angular 7.2.15 (pre-built assets committed to source)
- **UI Library**: Bootstrap 3.3.7
- **JavaScript**: jQuery 1.12.4 + Migrate 1.4.1
- **Build Process**: Manual webpack compilation

### Legacy Browsers Support
- Internet Explorer 11
- Chrome 70+ (2018 era)
- Firefox 60+ (2018 era)
- Edge Legacy

## 🏗️ Project Structure

```
AcmeBankApp/
├── AcmeBankApp.sln                 # Visual Studio solution
├── AcmeBankApp.Core/               # Domain models and interfaces
├── AcmeBankApp.Data/               # EF6 + ADO.NET data access layer
├── AcmeBankApp.Web/                # MVC web application
│   ├── Scripts/dist/               # Pre-built Angular 7 assets (committed)
│   ├── Controllers/                # MVC controllers with security issues
│   ├── Views/                      # Razor views with legacy patterns
│   └── web.config                  # Heavy configuration file
├── AcmeBankApp.Tests/              # MSTest unit tests
├── BuildScripts/                   # Legacy build and deployment scripts
└── README.md                       # This file
```

## 🚀 Getting Started

### Prerequisites
- **Visual Studio 2017/2019** (for authentic experience)
- **.NET Framework 4.8** SDK
- **SQL Server LocalDB** (usually included with Visual Studio)
- **IIS Express** (for local development)

### Quick Start
1. **Clone the repository**
   ```cmd
   git clone [repository-url]
   cd AcmeBankApp
   ```

2. **Restore NuGet packages**
   ```cmd
   nuget restore AcmeBankApp.sln
   ```

3. **Setup database**
   ```cmd
   sqlcmd -S "(LocalDB)\MSSQLLocalDB" -i "AcmeBankApp.Web\App_Data\CreateDatabase.sql"
   ```

4. **Build the solution**
   ```cmd
   MSBuild AcmeBankApp.sln /p:Configuration=Debug
   ```

5. **Run the application**
   - Open `AcmeBankApp.sln` in Visual Studio
   - Set `AcmeBankApp.Web` as startup project
   - Press F5 to run with IIS Express

### Default Credentials
- **Username**: `demo`
- **Password**: `password123`

## 🌐 Angular Banking SPA

The application includes a fully functional Angular 7 banking SPA with:

### 📊 Dashboard
- Account summary with balances
- Recent transaction history
- Quick action buttons

### 🏦 Account Management
- View all user accounts
- Account details and transaction history
- Balance inquiries

### 💸 Money Transfer
- Transfer between user accounts
- Real-time balance updates
- Transaction confirmation

### 💳 Bill Payment
- Coming soon placeholder (typical of legacy apps)
- Popular payee list

### 👤 Profile Management
- Update user information
- Security questions (stored in plain text)
- Account preferences

### 🧮 Loan Calculator
- Mortgage payment calculator
- Current rate display
- Interactive calculation form

## ⚠️ Security Vulnerabilities (Educational)

This application intentionally includes common security issues from the 2018-2019 era:

### SQL Injection
- `DataHelper.ValidateUserLegacy()` - String concatenation
- `ApiController.SearchTransactions()` - Direct query building
- Various ADO.NET methods without parameterization

### Cross-Site Scripting (XSS)
- `@Html.Raw()` usage in views
- Unescaped user input in error messages
- Client-side JavaScript with user data

### Cross-Site Request Forgery (CSRF)
- Missing `[ValidateAntiForgeryToken]` attributes
- No CSRF protection on API endpoints
- Angular forms without CSRF tokens

### Information Disclosure
- Detailed error pages in production
- API keys exposed in HTML/JavaScript
- Database connection strings in web.config
- System information endpoints

### Authentication Issues
- Plain text password storage
- Weak session management
- No proper logout mechanism
- Security questions in plain text

## 🔨 Build Process (Legacy)

### Angular Build
```cmd
cd BuildScripts
BuildAngular.bat
```

### Full Deployment
```powershell
cd BuildScripts
.\Deploy.ps1 -Environment "Production" -TargetServer "web-server-01"
```

### Manual IIS Deployment
1. Build solution in Release mode
2. Copy `AcmeBankApp.Web` files to IIS directory
3. Create `app_offline.htm` during deployment
4. Update `web.config` for environment
5. Remove `app_offline.htm` to bring online

## 🧪 Testing

### Run Unit Tests
```cmd
vstest.console.exe AcmeBankApp.Tests\bin\Debug\AcmeBankApp.Tests.dll
```

### Test Categories
- **SecurityVulnerability**: Tests demonstrating security issues
- **Performance**: Basic performance tests (flawed)
- **Integration**: Database integration tests

## 📁 Pre-built Assets

The `Scripts/dist/` folder contains committed Angular build output:
- `main.js` - Application bundle (250KB)
- `runtime.js` - Angular runtime and polyfills
- `polyfills.js` - IE11 and legacy browser support
- `styles.css` - Compiled application styles

This was a common practice in 2018-2019 for teams that:
- Couldn't install Node.js on production servers
- Wanted immediate development without Angular CLI setup
- Needed consistent builds across environments
- Had limited CI/CD capabilities

## 🏛️ Legacy Architecture Patterns

### Service Locator Anti-pattern
```csharp
// Bad: Static service access
var userService = ServiceLocator.GetService<IUserService>();
```

### Static Logging Helper
```csharp
// Common in 2018: Global static logger
LogHelper.Info("User logged in");
LogHelper.LogUserActivity(userName, "Login", details);
```

### Heavy Web.config
- 200+ appSettings keys
- Hardcoded connection strings
- Manual assembly binding redirects
- Environment-specific transformations

### Mixed Data Access Patterns
- Entity Framework 6 Code First
- Raw ADO.NET with string concatenation
- DataTable-based operations
- Repository pattern mixed with Active Record

## 🚧 Technical Debt Indicators

### Code Smells
- `#region` overuse for organization
- Static helper classes everywhere
- Global exception handling that swallows errors
- Mixed naming conventions (PascalCase vs camelCase)
- Inline styles and JavaScript in views

### Configuration Issues
- Secrets in plain text configuration files
- Environment-specific config scattered throughout
- Manual SSL certificate management
- IIS-specific deployment assumptions

### Build & Deployment
- Manual batch file builds
- PowerShell scripts with hardcoded paths
- No containerization or environment parity
- Manual database migration scripts
- File-based deployment with downtime

## 🔄 Migration Path (Modern Equivalent)

To modernize this application today, you would:

1. **Framework**: Upgrade to .NET 6+ and ASP.NET Core
2. **Frontend**: Angular 15+ with Angular CLI
3. **Data**: EF Core with proper async patterns
4. **Security**: Identity Server, proper authentication
5. **Configuration**: Azure Key Vault, environment variables
6. **Build**: GitHub Actions, containerized builds
7. **Deployment**: Kubernetes, blue-green deployments
8. **Monitoring**: Application Insights, structured logging

## 📜 License

This project is for educational purposes only. It demonstrates legacy patterns and security vulnerabilities that should **NOT** be used in production applications.

## ⚠️ Disclaimer

This application contains intentional security vulnerabilities and outdated practices for educational purposes. Do not use this code in production environments or as a template for real applications.

---

*"Legacy code is code without tests... and proper security."* - With apologies to Michael Feathers
