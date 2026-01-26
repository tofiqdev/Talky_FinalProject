# ngrok Setup Script

Write-Host "=== ngrok Setup ===" -ForegroundColor Cyan
Write-Host ""

# Check if ngrok exists
$ngrokPath = "C:\ngrok\ngrok.exe"

if (Test-Path $ngrokPath) {
    Write-Host "✓ ngrok found at: $ngrokPath" -ForegroundColor Green
    
    # Set auth token
    Write-Host ""
    Write-Host "Setting auth token..." -ForegroundColor Yellow
    & $ngrokPath config add-authtoken usr_2onHEGLfCGQnE2lgS1HzdmqkzMa
    
    Write-Host ""
    Write-Host "✓ Auth token configured!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Start backend: cd back/TalkyAPI && dotnet run"
    Write-Host "2. Start ngrok: C:\ngrok\ngrok.exe http 5282"
    Write-Host ""
} else {
    Write-Host "✗ ngrok not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please download ngrok:" -ForegroundColor Yellow
    Write-Host "1. Visit: https://ngrok.com/download"
    Write-Host "2. Download Windows (64-bit) version"
    Write-Host "3. Extract ngrok.exe to C:\ngrok\"
    Write-Host "4. Run this script again"
    Write-Host ""
    
    # Try to open download page
    Start-Process "https://ngrok.com/download"
}
