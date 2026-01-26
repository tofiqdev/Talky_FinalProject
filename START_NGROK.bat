@echo off
echo ========================================
echo Starting ngrok...
echo ========================================
echo.
echo Make sure backend is running first!
echo.
echo Copy the HTTPS URL from ngrok output
echo Example: https://abc123.ngrok-free.app
echo.
echo ========================================
echo.

C:\ngrok\ngrok.exe http 5282

pause
