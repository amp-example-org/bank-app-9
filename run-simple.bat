@echo off
echo Starting simple web server...
cd AcmeBankApp.Web
python -m http.server 8080 2>nul || echo "Python not available, trying dotnet..."
if errorlevel 1 (
    dotnet run 2>nul || echo "Dotnet run not available"
)
pause
