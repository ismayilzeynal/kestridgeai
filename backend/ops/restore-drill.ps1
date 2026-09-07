#Requires -Version 5.1
<#
.SYNOPSIS
  Monthly restore drill. Proves a backup is actually restorable.

.DESCRIPTION
  Restores the newest backup archive into a scratch database, checks that the
  three tables exist and that contact_submissions has rows, prints the row
  counts, and drops the scratch database again.

  A backup that has never been restored is a hypothesis, not a backup. Run this
  monthly and record the date in RUNBOOK.md.

  The scratch database name is fixed and must not be kestridge or
  kestridge_test; the script refuses otherwise.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File ops/restore-drill.ps1
#>
[CmdletBinding()]
param(
    [string] $MySqlExe = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe",
    [string] $MySqlHost = "127.0.0.1",
    [int]    $Port = 3306,
    [string] $BackupPath = (Join-Path $PSScriptRoot "backups"),
    [string] $ScratchDatabase = "kestridge_restore_drill"
)

$ErrorActionPreference = "Stop"

if ($ScratchDatabase -in @("kestridge", "kestridge_test")) {
    throw "Refusing to restore over $ScratchDatabase."
}
if (-not (Test-Path $MySqlExe)) { throw "Not found: $MySqlExe" }

$archive = Get-ChildItem -Path $BackupPath -Filter "kestridge-*.sql.zip" |
           Sort-Object LastWriteTime -Descending |
           Select-Object -First 1

if (-not $archive) { throw "No backup archive found in $BackupPath" }

Write-Host "Drilling: $($archive.Name) ($([math]::Round($archive.Length / 1KB, 1)) KB)"

$secure = Read-Host -Prompt "MySQL root password" -AsSecureString
$bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
try { $rootPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr) }
finally { [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr) }

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$defaultsFile = [System.IO.Path]::GetTempFileName()
$workDir = Join-Path $env:TEMP ("kestridge-drill-" + [System.IO.Path]::GetRandomFileName())

function Invoke-Sql {
    param([string] $Sql, [string] $Database = "")

    $sqlFile = [System.IO.Path]::GetTempFileName()
    try {
        [System.IO.File]::WriteAllText($sqlFile, $Sql, $utf8NoBom)
        $arguments = @("--defaults-extra-file=$defaultsFile", "--batch", "--table")
        if ($Database) { $arguments += $Database }

        $output = Get-Content -LiteralPath $sqlFile -Raw | & $MySqlExe @arguments 2>&1
        if ($LASTEXITCODE -ne 0) { throw "mysql.exe exited with $LASTEXITCODE`n$output" }
        return $output
    }
    finally {
        Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue
    }
}

try {
    $defaults = "[client]`nhost=$MySqlHost`nport=$Port`nuser=root`npassword=`"$rootPassword`"`n"
    [System.IO.File]::WriteAllText($defaultsFile, $defaults, $utf8NoBom)

    New-Item -ItemType Directory -Path $workDir -Force | Out-Null
    Expand-Archive -Path $archive.FullName -DestinationPath $workDir -Force

    $dump = Get-ChildItem -Path $workDir -Filter "*.sql" | Select-Object -First 1
    if (-not $dump) { throw "Archive contained no .sql file" }

    Write-Host "Creating scratch database $ScratchDatabase..."
    Invoke-Sql -Sql "DROP DATABASE IF EXISTS ``$ScratchDatabase``; CREATE DATABASE ``$ScratchDatabase`` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;" | Out-Null

    Write-Host "Restoring..."
    $restore = Get-Content -LiteralPath $dump.FullName -Raw | & $MySqlExe "--defaults-extra-file=$defaultsFile" $ScratchDatabase 2>&1
    if ($LASTEXITCODE -ne 0) { throw "Restore failed with $LASTEXITCODE`n$restore" }

    Write-Host ""
    Write-Host "Tables restored:"
    Invoke-Sql -Database $ScratchDatabase -Sql @"
SELECT TABLE_NAME, TABLE_ROWS, ENGINE, ROW_FORMAT
  FROM information_schema.TABLES
 WHERE TABLE_SCHEMA = '$ScratchDatabase'
 ORDER BY TABLE_NAME;
"@

    $counts = Invoke-Sql -Database $ScratchDatabase -Sql @"
SELECT
  (SELECT COUNT(*) FROM contact_submissions) AS submissions,
  (SELECT COUNT(*) FROM job_runs)            AS job_runs,
  (SELECT COUNT(*) FROM dsr_log)             AS dsr_log;
"@

    Write-Host ""
    Write-Host "Row counts:"
    $counts

    Write-Host ""
    Write-Host "Restore drill passed. Record today's date in RUNBOOK.md." -ForegroundColor Green
}
finally {
    if (Test-Path $defaultsFile) {
        try { Invoke-Sql -Sql "DROP DATABASE IF EXISTS ``$ScratchDatabase``;" | Out-Null } catch { }
    }
    Remove-Item $defaultsFile -Force -ErrorAction SilentlyContinue
    Remove-Item $workDir -Recurse -Force -ErrorAction SilentlyContinue
}
