#Requires -Version 5.1
<#
.SYNOPSIS
  Publish the API and install it as a Windows service.

.DESCRIPTION
  Publishes a self-contained-free release build to InstallPath, sets the
  machine-scoped environment variables the process reads, registers the service
  with automatic start and restart-on-failure, and starts it.

  Both maintenance loops depend on the process surviving a reboot, which is why
  this is a service and not a scheduled task.

  Secrets are prompted for and written to an appsettings.Production.json inside
  InstallPath, which is then ACLed to Administrators and SYSTEM only. They are
  deliberately NOT machine environment variables: those live in
  HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment, which grants
  Read to BUILTIN\Users and to ALL APPLICATION PACKAGES, so any unprivileged
  local account could read the database credential, the SMTP password and the
  DSR pepper. Only ASPNETCORE_ENVIRONMENT and ASPNETCORE_URLS go in the
  environment. Nothing secret is ever written into the repository.

  Run from an elevated prompt.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File ops/install-service.ps1
#>
[CmdletBinding()]
param(
    [string] $ServiceName = "KestridgeApi",
    [string] $DisplayName = "Kestridge AI API",
    [string] $InstallPath = "C:\Kestridge\api",
    [string] $Url = "http://127.0.0.1:5199"
)

$ErrorActionPreference = "Stop"

$identity = [Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()
if (-not $identity.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw "Run this from an elevated PowerShell prompt."
}

$backendRoot = Split-Path -Parent $PSScriptRoot
$apiProject = Join-Path $backendRoot "src\Kestridge.Api\Kestridge.Api.csproj"
if (-not (Test-Path $apiProject)) { throw "Not found: $apiProject" }

function Read-Secret {
    param([string] $Prompt)
    $secure = Read-Host -Prompt $Prompt -AsSecureString
    $bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
    try { return [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr) }
    finally { [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr) }
}

Write-Host ""
Write-Host "Kestridge AI API - service install" -ForegroundColor Cyan
Write-Host ""

$connection  = Read-Secret "ConnectionStrings__Default (kestridge_app)"
$smtpHost    = Read-Host   "Kestridge__Smtp__Host"
$smtpUser    = Read-Host   "Kestridge__Smtp__User (blank for none)"
$smtpPass    = Read-Secret "Kestridge__Smtp__Password"
$toAddress   = Read-Host   "Kestridge__Contact__ToAddress (where notifications go)"
$fromAddress = Read-Host   "Kestridge__Contact__FromAddress (on the sending domain)"
$pepper      = Read-Secret "Kestridge__Dsr__EmailHashPepper"

Write-Host ""
Write-Host "Publishing to $InstallPath ..."
& dotnet publish $apiProject -c Release -o $InstallPath --nologo
if ($LASTEXITCODE -ne 0) { throw "dotnet publish exited with $LASTEXITCODE" }

# Non-secret, and needed before configuration is read.
Write-Host "Setting machine-scoped environment variables (non-secret only)..."
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production", [EnvironmentVariableTarget]::Machine)
[Environment]::SetEnvironmentVariable("ASPNETCORE_URLS", $Url, [EnvironmentVariableTarget]::Machine)

# Everything below is secret or security-relevant and goes in an ACLed file.
# UseWindowsService sets ContentRoot to the executable directory, so
# appsettings.Production.json here is picked up automatically.
$productionSettings = [ordered]@{
    ConnectionStrings = [ordered]@{ Default = $connection }
    Kestridge = [ordered]@{
        Smtp = [ordered]@{
            Host     = $smtpHost
            User     = $smtpUser
            Password = $smtpPass
        }
        Contact = [ordered]@{
            ToAddress   = $toAddress
            FromAddress = $fromAddress
            # Previews must stay off: any Vercel user can create a project named
            # kestridgeai-something under their own scope.
            RequireOrigin = $true
        }
        Cors = [ordered]@{ AllowVercelPreviews = $false }
        Dsr  = [ordered]@{ EmailHashPepper = $pepper }
    }
}

$settingsPath = Join-Path $InstallPath "appsettings.Production.json"
$json = $productionSettings | ConvertTo-Json -Depth 6
[System.IO.File]::WriteAllText($settingsPath, $json, (New-Object System.Text.UTF8Encoding($false)))

Write-Host "Restricting $settingsPath to Administrators and SYSTEM..."
& icacls $settingsPath /inheritance:r /grant:r "*S-1-5-32-544:(R,W)" "*S-1-5-18:(R,W)" | Out-Null
if ($LASTEXITCODE -ne 0) { throw "icacls failed with $LASTEXITCODE on $settingsPath" }

# The service runs as LocalSystem (sc.exe create default), which is S-1-5-18 and
# is already granted above. Grant the account explicitly if that ever changes.

# Any earlier install of this service may have left secrets in the machine
# environment. Remove them so they stop being world-readable.
foreach ($stale in @(
    "ConnectionStrings__Default",
    "Kestridge__Smtp__Password",
    "Kestridge__Smtp__Host",
    "Kestridge__Smtp__User",
    "Kestridge__Contact__ToAddress",
    "Kestridge__Contact__FromAddress",
    "Kestridge__Dsr__EmailHashPepper",
    "Kestridge__Cors__AllowVercelPreviews",
    "Kestridge__Contact__RequireOrigin")) {
    if ([Environment]::GetEnvironmentVariable($stale, [EnvironmentVariableTarget]::Machine)) {
        [Environment]::SetEnvironmentVariable($stale, $null, [EnvironmentVariableTarget]::Machine)
        Write-Host "  removed stale machine variable $stale"
    }
}

$exe = Join-Path $InstallPath "Kestridge.Api.exe"
if (-not (Test-Path $exe)) { throw "Publish produced no $exe" }

$existing = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existing) {
    Write-Host "Stopping and removing the existing service..."
    if ($existing.Status -ne "Stopped") { Stop-Service -Name $ServiceName -Force }
    & sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Seconds 2
}

Write-Host "Registering $ServiceName ..."
& sc.exe create $ServiceName binPath= "`"$exe`"" start= auto DisplayName= "`"$DisplayName`"" | Out-Null
if ($LASTEXITCODE -ne 0) { throw "sc.exe create exited with $LASTEXITCODE" }

& sc.exe description $ServiceName "Contact form API for kestridge.com. Stores submissions and notifies the team." | Out-Null

# Restart after 60 seconds on each of the first three failures, reset the
# counter daily. A crash at 03:00 must not leave notifications parked.
& sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000 | Out-Null

# MySQL has to be up first, otherwise the first health probe reports degraded
# for no reason. The app itself tolerates it: it does not connect at startup.
& sc.exe config $ServiceName depend= MySQL80 | Out-Null

Write-Host "Starting..."
Start-Service -Name $ServiceName
Start-Sleep -Seconds 3

$service = Get-Service -Name $ServiceName
Write-Host "Service status: $($service.Status)"

Write-Host ""
Write-Host "Smoke test:" -ForegroundColor Cyan
try {
    $health = Invoke-RestMethod -Uri "$Url/api/health" -TimeoutSec 10
    Write-Host "  GET $Url/api/health -> $($health | ConvertTo-Json -Compress)"
}
catch {
    Write-Host "  GET $Url/api/health failed: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Remaining steps:" -ForegroundColor Cyan
Write-Host "  1. Point the reverse proxy for api.kestridge.com at $Url."
Write-Host "  2. Set NEXT_PUBLIC_FORM_ENDPOINT in Vercel and trigger a REBUILD."
Write-Host "  3. Work through the go-live checklist in RUNBOOK.md."
Write-Host ""
Write-Host "Secrets live in $settingsPath (Administrators and SYSTEM only)."
Write-Host "Verify with:  icacls `"$settingsPath`""
Write-Host ""
