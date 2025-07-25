@echo off
echo Testing AcmeBankApp on IIS...
echo Opening http://localhost/AcmeBankApp in browser
start http://localhost/AcmeBankApp
echo.
echo If browser doesn't open automatically, navigate to:
echo http://localhost/AcmeBankApp
echo.
echo Press any key to check if IIS is responding...
pause > nul
curl -s http://localhost/AcmeBankApp > response.html
if %ERRORLEVEL% EQU 0 (
    echo SUCCESS: IIS is responding!
    echo Response saved to response.html
    type response.html | findstr /i "error\|exception" > nul
    if %ERRORLEVEL% EQU 0 (
        echo WARNING: Found errors in response. Check response.html
    ) else (
        echo Response looks good!
    )
) else (
    echo ERROR: Failed to connect to IIS
)
pause
