@echo off
echo ========================================
echo   Talky Backend - Starting Server
echo ========================================
echo.

REM Check if .NET 8 is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET 8 Runtime is not installed!
    echo Please install from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo .NET Runtime: OK
echo.

REM Set environment to Production
set ASPNETCORE_ENVIRONMENT=Production

echo Starting Talky API...
echo.
echo Server will be available at:
echo - HTTP:  http://localhost:5135
echo - HTTPS: https://localhost:7001
echo.
echo Press Ctrl+C to stop the server
echo.

REM Start the application
Talky_API.exe

pause
