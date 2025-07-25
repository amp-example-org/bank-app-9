@echo off
REM Legacy Angular Build Script for Deployment
REM This represents the type of build process used in 2018-2019

echo ==========================================
echo  Acme Bank - Angular 7 Build Script
echo ==========================================
echo.

set ANGULAR_SOURCE=AcmeBankApp.Web\Scripts\angular-src
set OUTPUT_DIR=AcmeBankApp.Web\Scripts\dist
set NODE_MODULES=node_modules

echo [INFO] Starting Angular build process...
echo [INFO] Timestamp: %date% %time%
echo.

REM Legacy check for Node.js
node --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Node.js not found! Please install Node.js 10.x or higher
    echo [ERROR] Download from: https://nodejs.org/
    pause
    exit /b 1
)

echo [INFO] Node.js version:
node --version

REM Legacy check for npm
npm --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] npm not found!
    pause
    exit /b 1
)

echo [INFO] npm version:
npm --version
echo.

REM Legacy: Check if Angular source exists
if not exist "%ANGULAR_SOURCE%" (
    echo [WARN] Angular source directory not found: %ANGULAR_SOURCE%
    echo [WARN] Using pre-built assets from dist folder
    echo [WARN] For production builds, please ensure Angular source is available
    echo.
    goto :use_prebuilt
)

echo [INFO] Changing to Angular source directory...
cd /d "%ANGULAR_SOURCE%"

REM Legacy: Install dependencies if node_modules doesn't exist
if not exist "%NODE_MODULES%" (
    echo [INFO] Installing Angular dependencies...
    echo [INFO] This may take several minutes on first run...
    npm install --legacy-peer-deps
    if errorlevel 1 (
        echo [ERROR] npm install failed!
        pause
        exit /b 1
    )
) else (
    echo [INFO] Dependencies already installed, skipping npm install
)

echo.
echo [INFO] Building Angular application for production...
echo [INFO] Output directory: %OUTPUT_DIR%

REM Legacy Angular CLI build with specific version
npx ng build --configuration=production --output-path="../../%OUTPUT_DIR%" --base-href="/app/"

if errorlevel 1 (
    echo [ERROR] Angular build failed!
    echo [ERROR] Check the error messages above for details
    pause
    exit /b 1
)

echo [INFO] Angular build completed successfully!
echo.

REM Legacy: Copy additional assets
echo [INFO] Copying additional assets...
if exist "src\assets\*" (
    xcopy /s /y "src\assets\*" "..\..\%OUTPUT_DIR%\assets\"
)

REM Legacy: Generate build info file
echo [INFO] Generating build information...
cd /d "..\..\"
echo Build Date: %date% %time% > "%OUTPUT_DIR%\build-info.txt"
echo Build Machine: %computername% >> "%OUTPUT_DIR%\build-info.txt"
echo Build User: %username% >> "%OUTPUT_DIR%\build-info.txt"
echo Angular Version: 7.2.15 >> "%OUTPUT_DIR%\build-info.txt"

goto :build_complete

:use_prebuilt
echo [INFO] Using pre-built Angular assets
echo [INFO] These assets were committed to source control for immediate deployment
echo [INFO] Files location: %OUTPUT_DIR%
echo.

:build_complete
echo ==========================================
echo  Build Process Complete!
echo ==========================================
echo.
echo [SUCCESS] Angular application is ready for deployment
echo [INFO] Built files are in: %OUTPUT_DIR%
echo [INFO] The application can now be deployed to IIS
echo.

REM Legacy: Optional file size report
if exist "%OUTPUT_DIR%\main.js" (
    echo [INFO] Build size information:
    for %%I in ("%OUTPUT_DIR%\main.js") do echo [INFO] main.js: %%~zI bytes
    for %%I in ("%OUTPUT_DIR%\runtime.js") do echo [INFO] runtime.js: %%~zI bytes
    for %%I in ("%OUTPUT_DIR%\polyfills.js") do echo [INFO] polyfills.js: %%~zI bytes
    for %%I in ("%OUTPUT_DIR%\styles.css") do echo [INFO] styles.css: %%~zI bytes
)

echo.
echo [INFO] To rebuild for development, run: npm run build:dev
echo [INFO] To serve locally, run: npm start
echo [INFO] Build log can be found in build.log
echo.

pause
