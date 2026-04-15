param(
    [string]$Runtime = "win-x64",
    [string]$TargetTriple = "x86_64-pc-windows-msvc",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$publishDir = "./src-tauri/binaries"

if (!(Test-Path $publishDir)) {
    New-Item -ItemType Directory -Path $publishDir | Out-Null
}

dotnet publish ../backend/WeatherStyler/WeatherStyler.csproj `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    /p:PublishSingleFile=true `
    /p:IncludeNativeLibrariesForSelfExtract=true `
    /p:PublishTrimmed=false `
    -o $publishDir

$sourceExe = Join-Path $publishDir "WeatherStyler.exe"
$targetExe = Join-Path $publishDir "backend-$TargetTriple.exe"

if (Test-Path $sourceExe) {
    Copy-Item $sourceExe $targetExe -Force
}

Write-Host "Backend sidecar gotowy: $targetExe"
