# Techdrive App Build & Run Script
# This script compiles the C# WinForms solution using MSBuild and launches the executable.

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "   Techdrive Car Rentals - Fleet Management   " -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

$msbuildPath = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"

if (-not (Test-Path $msbuildPath)) {
    Write-Host "Locating MSBuild.exe on the system..." -ForegroundColor Yellow
    # Look for msbuild.exe dynamically if the standard path is missing
    $found = Get-ChildItem -Path "C:\Program Files\Microsoft Visual Studio", "C:\Program Files (x86)\Microsoft Visual Studio" -Filter "msbuild.exe" -Recurse -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName -First 1
    if ($found) {
        $msbuildPath = $found
        Write-Host "Found MSBuild at: $msbuildPath" -ForegroundColor Green
    }
}

if (-not (Test-Path $msbuildPath)) {
    Write-Host "ERROR: MSBuild.exe was not found. Please install Visual Studio Build Tools or Community Edition." -ForegroundColor Red
    Exit 1
}

Write-Host "Building TechdriveLogin.slnx..." -ForegroundColor Yellow
& $msbuildPath "$PSScriptRoot\TechdriveLogin.slnx" /t:Rebuild /p:Configuration=Debug

if ($LASTEXITCODE -eq 0) {
    Write-Host "SUCCESS: Build completed successfully!" -ForegroundColor Green
    $exePath = "$PSScriptRoot\TechdriveLogin\bin\Debug\TechdriveLogin.exe"
    if (Test-Path $exePath) {
        Write-Host "Launching Techdrive application..." -ForegroundColor Green
        Start-Process -FilePath $exePath
    } else {
        Write-Host "ERROR: Executable not found at $exePath" -ForegroundColor Red
    }
} else {
    Write-Host "ERROR: Build failed. Please check the compilation errors above." -ForegroundColor Red
}
