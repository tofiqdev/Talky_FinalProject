@echo off
echo ========================================
echo   Talky Backend - ngrok Deployment
echo ========================================
echo.

REM Check if ngrok is installed
where ngrok >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] ngrok bulunamadi!
    echo.
    echo Lutfen ngrok'u yukleyin:
    echo 1. https://ngrok.com/download adresine gidin
    echo 2. Windows icin ZIP dosyasini indirin
    echo 3. ngrok.exe'yi bu klasore kopyalayin
    echo.
    pause
    exit /b 1
)

echo [1/3] Backend'i baslatiyor...
echo.

REM Start backend in a new window
start "Talky Backend" cmd /k "cd BackNtier\Talky_API && dotnet run"

echo Backend baslatildi! (Yeni pencerede)
echo Lutfen "Now listening on: http://localhost:5135" mesajini bekleyin...
echo.
timeout /t 10 /nobreak

echo [2/3] ngrok'u baslatiyor...
echo.

REM Start ngrok
echo ngrok URL'inizi kopyalayin ve .env.production dosyasina ekleyin!
echo.
ngrok http 5135

pause
