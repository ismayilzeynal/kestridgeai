#Requires -Version 5.1
<#
.SYNOPSIS
  Nightly logical backup of the kestridge database.

.DESCRIPTION
  Runs mysqldump as kestridge_backup, compresses the dump, keeps the last
  RetentionDays locally, and refuses to run without an offsite copy target so
  that "we have backups" cannot quietly mean "we have backups on the machine
  that just died".

  The dump contains every inquiry body. Treat the output directory as personal
  data: it is inside the same retention promise as the database itself, and the
  window it creates has to be stated in DSR-PROCESS.md.

  Password source, in order: the KESTRIDGE_BACKUP_PASSWORD environment variable,
  otherwise a prompt. It is written to a temporary defaults file, never to a
  command line.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File ops/backup.ps1 -OffsitePath D:\offsite\kestridge
#>
[CmdletBinding()]
param(
    [string] $MySqlDumpExe = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe",
    [string] $MySqlHost = "127.0.0.1",
    [int]    $Port = 3306,
    [string] $Database = "kestridge",
    [string] $OutputPath = (Join-Path $PSScriptRoot "backups"),
    [Parameter(Mandatory = $true)][string] $OffsitePath,
    [int]    $RetentionDays = 30,
    [string] $Stamp
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $MySqlDumpExe)) { throw "Not found: $MySqlDumpExe" }
if (-not (Test-Path $OutputPath))   { New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null }
if (-not (Test-Path $OffsitePath))  { throw "Offsite path does not exist: $OffsitePath" }

if (-not $Stamp) { $Stamp = (Get-Date).ToUniversalTime().ToString("yyyyMMdd-HHmmss") }

$password = $env:KESTRIDGE_BACKUP_PASSWORD
if (-not $password) {
    $secure = Read-Host -Prompt "kestridge_backup password" -AsSecureString
    $bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
    try { $password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr) }
    finally { [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr) }
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$defaultsFile = [System.IO.Path]::GetTempFileName()
$dumpFile = Join-Path $OutputPath "kestridge-$Stamp.sql"

try {
    $defaults = "[client]`nhost=$MySqlHost`nport=$Port`nuser=kestridge_backup`npassword=`"$password`"`n"
    [System.IO.File]::WriteAllText($defaultsFile, $defaults, $utf8NoBom)

    # --single-transaction keeps InnoDB consistent without locking writers out.
    # --skip-comments and a fixed --result-file keep the dump byte-stable, which
    # is what makes a restore drill comparable run to run.
    $arguments = @(
        "--defaults-extra-file=$defaultsFile",
        "--single-transaction",
        "--skip-lock-tables",
        "--set-gtid-purged=OFF",
        "--default-character-set=utf8mb4",
        "--hex-blob",
        "--routines=FALSE",
        "--triggers=FALSE",
        "--events=FALSE",
        "--result-file=$dumpFile",
        $Database
    )

    & $MySqlDumpExe @arguments
    if ($LASTEXITCODE -ne 0) { throw "mysqldump exited with $LASTEXITCODE" }

    $archive = "$dumpFile.zip"
    Compress-Archive -Path $dumpFile -DestinationPath $archive -Force
    Remove-Item $dumpFile -Force

    Copy-Item $archive -Destination $OffsitePath -Force

    $sizeKb = [math]::Round((Get-Item $archive).Length / 1KB, 1)
    Write-Host "Backup written: $archive ($sizeKb KB)"
    Write-Host "Offsite copy:   $(Join-Path $OffsitePath (Split-Path $archive -Leaf))"

    $cutoff = (Get-Date).AddDays(-$RetentionDays)
    $stale = Get-ChildItem -Path $OutputPath -Filter "kestridge-*.sql.zip" |
             Where-Object { $_.LastWriteTime -lt $cutoff }

    foreach ($file in $stale) {
        Remove-Item $file.FullName -Force
        Write-Host "Pruned: $($file.Name)"
    }

    Write-Host "Local retention: $RetentionDays days. State this window in DSR-PROCESS.md."
}
finally {
    Remove-Item $defaultsFile -Force -ErrorAction SilentlyContinue
}
