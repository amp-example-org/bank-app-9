@echo off
echo Starting AcmeBankApp on http://localhost:8080
echo.
cd /d "AcmeBankApp.Web"
"C:\Program Files\IIS Express\iisexpress.exe" /path:"%CD%" /port:8080
pause
