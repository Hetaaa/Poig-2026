# Skrypt automatyzujący publikację backendu .NET 10 dla Tauri Sidecar
$ErrorActionPreference = "Stop"

# --- KONFIGURACJA ŚCIEŻEK ---
$RootPath = "C:\Git\Studia\Poig-2026"
$ProjectFolder = "$RootPath\backend\WeatherStyler"
$TargetBinariesFolder = "$RootPath\tauri\src-tauri\binaries"
$FinalExeName = "backend-x86_64-pc-windows-msvc.exe"

Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host " Uruchamianie automatycznej publikacji backendu .NET 10  " -ForegroundColor Cyan
Write-Host "=========================================================" -ForegroundColor Cyan

# 1. Czyszczenie starego pliku exe w folderze docelowym, aby uniknąć konfliktów uprawnień
if (Test-Path "$TargetBinariesFolder\$FinalExeName") {
    Write-Host "[1/4] Usuwanie starego pliku sidecara..." -ForegroundColor Yellow
    Remove-Item "$TargetBinariesFolder\$FinalExeName" -Force
} else {
    Write-Host "[1/4] Brak starego pliku sidecara do usunięcia. Pomijam." -ForegroundColor Green
}

# 2. Przejście do folderu projektu Web API
Write-Host "[2/4] Wchodzenie do katalogu projektu..." -ForegroundColor Yellow
Set-Location -Path $ProjectFolder

# 3. Wywołanie komendy dotnet publish bezpośrednio do folderu binaries w Tauri
Write-Host "[3/4] Kompilacja i publikacja projektu .NET (Release, win-x64)..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 -o $TargetBinariesFolder

# 4. Zmiana nazwy pliku wykonywalnego na format akceptowany przez Tauri
Write-Host "[4/4] Formatowanie nazwy pliku na Tauri Sidecar..." -ForegroundColor Yellow
$SourceExe = "$TargetBinariesFolder\WeatherStyler.exe"

if (Test-Path $SourceExe) {
    Move-Item -Path $SourceExe -Destination "$TargetBinariesFolder\$FinalExeName" -Force
    Write-Host "`n✔ Sukces! Backend został pomyślnie przygotowany dla Tauri." -ForegroundColor Green
    Write-Host "Plik: $TargetBinariesFolder\$FinalExeName" -ForegroundColor Gray
} else {
    # Zabezpieczenie na wypadek, gdyby projekt nazywał się WeatherStyler.API.exe
    $AlternativeSourceExe = "$TargetBinariesFolder\WeatherStyler.API.exe"
    if (Test-Path $AlternativeSourceExe) {
        Move-Item -Path $AlternativeSourceExe -Destination "$TargetBinariesFolder\$FinalExeName" -Force
        Write-Host "`n✔ Sukces! Backend został pomyślnie przygotowany dla Tauri." -ForegroundColor Green
        Write-Host "Plik: $TargetBinariesFolder\$FinalExeName" -ForegroundColor Gray
    } else {
        Write-Host "`n❌ Błąd: Nie znaleziono skompilowanego pliku .exe w folderze binaries!" -ForegroundColor Red
        Exit 1
    }
}

Write-Host "`nMożesz teraz uruchomić: cargo tauri dev" -ForegroundColor Cyan
Write-Host "=========================================================" -ForegroundColor Cyan