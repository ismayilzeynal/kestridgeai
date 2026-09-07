#Requires -Version 5.1
<#
.SYNOPSIS
  One-time local setup for the Kestridge AI backend database.

.DESCRIPTION
  Prompts for the MySQL root password, generates a password for each of the five
  service accounts, provisions the databases and grants, applies the migration,
  writes the application connection string and the DSR pepper into dotnet
  user-secrets, and prints the remaining passwords once so they can be stored in
  the team password manager.

  No password is written to a file in this repository, and none is passed on a
  command line where it would show up in the process list: mysql.exe is driven
  through a temporary defaults file that is deleted afterwards.

  Re-running is safe. Provisioning uses CREATE ... IF NOT EXISTS plus an
  explicit ALTER USER, and ops/migrate.sql is idempotent.

.EXAMPLE
  cd backend
  powershell -ExecutionPolicy Bypass -File ops/setup-dev-db.ps1
#>
[CmdletBinding()]
param(
    [string] $MySqlExe = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe",
    [string] $MySqlHost = "127.0.0.1",
    [int]    $Port = 3306
)

$ErrorActionPreference = "Stop"

$backendRoot = Split-Path -Parent $PSScriptRoot
$apiProject  = Join-Path $backendRoot "src\Kestridge.Api\Kestridge.Api.csproj"
$provision   = Join-Path $PSScriptRoot "01-provision.sql"
$verify      = Join-Path $PSScriptRoot "03-verify-grants.sql"
$migrate     = Join-Path $PSScriptRoot "migrate.sql"

foreach ($required in @($MySqlExe, $apiProject, $provision, $verify, $migrate)) {
    if (-not (Test-Path $required)) {
        throw "Not found: $required"
    }
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function New-StrongPassword {
    # 24 bytes of CSPRNG output, base64, with the characters that would need
    # escaping in a MySQL string literal or a connection string mapped away.
    $bytes = New-Object byte[] 24
    $rng = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
    try { $rng.GetBytes($bytes) } finally { $rng.Dispose() }
    return ([Convert]::ToBase64String($bytes) -replace '[+/=]', 'x')
}

function Invoke-MySqlAsRoot {
    param([string] $Sql, [string] $DefaultsFile, [string] $Database = "")

    $sqlFile = [System.IO.Path]::GetTempFileName()
    try {
        [System.IO.File]::WriteAllText($sqlFile, $Sql, $utf8NoBom)

        $arguments = @("--defaults-extra-file=$DefaultsFile", "--batch", "--table")
        if ($Database) { $arguments += $Database }

        # PowerShell 5.1 has no stdin redirection operator, so pipe the file in.
        $output = Get-Content -LiteralPath $sqlFile -Raw | & $MySqlExe @arguments 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "mysql.exe exited with $LASTEXITCODE`n$output"
        }
        return $output
    }
    finally {
        Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue
    }
}

Write-Host ""
Write-Host "Kestridge AI backend - local database setup" -ForegroundColor Cyan
Write-Host "Server: $MySqlHost port $Port"
Write-Host ""

$secureRoot = Read-Host -Prompt "MySQL root password" -AsSecureString
$rootBstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureRoot)
try {
    $rootPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($rootBstr)
}
finally {
    [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($rootBstr)
}

$defaultsFile = [System.IO.Path]::GetTempFileName()

try {
    $defaults = "[client]`nhost=$MySqlHost`nport=$Port`nuser=root`npassword=`"$rootPassword`"`n"
    [System.IO.File]::WriteAllText($defaultsFile, $defaults, $utf8NoBom)

    Invoke-MySqlAsRoot -Sql "SELECT VERSION();" -DefaultsFile $defaultsFile | Out-Null
    Write-Host "Connected." -ForegroundColor Green

    $passwords = [ordered]@{
        APP      = New-StrongPassword
        MIGRATOR = New-StrongPassword
        OPS      = New-StrongPassword
        BACKUP   = New-StrongPassword
        TEST     = New-StrongPassword
    }

    $sql = Get-Content -LiteralPath $provision -Raw
    foreach ($key in $passwords.Keys) {
        $sql = $sql.Replace("<$($key)_PASSWORD>", $passwords[$key])
    }

    # CREATE USER IF NOT EXISTS leaves an existing password alone, so set it
    # explicitly. That is what makes a second run produce a working credential.
    $alters = @"
ALTER USER 'kestridge_app'@'127.0.0.1'      IDENTIFIED WITH caching_sha2_password BY '$($passwords.APP)';
ALTER USER 'kestridge_migrator'@'127.0.0.1' IDENTIFIED WITH caching_sha2_password BY '$($passwords.MIGRATOR)';
ALTER USER 'kestridge_ops'@'127.0.0.1'      IDENTIFIED WITH caching_sha2_password BY '$($passwords.OPS)';
ALTER USER 'kestridge_backup'@'127.0.0.1'   IDENTIFIED WITH caching_sha2_password BY '$($passwords.BACKUP)';
ALTER USER 'kestridge_test'@'127.0.0.1'     IDENTIFIED WITH caching_sha2_password BY '$($passwords.TEST)';
FLUSH PRIVILEGES;
"@

    Write-Host "Provisioning databases, users and grants..."
    Invoke-MySqlAsRoot -Sql ($sql + "`n" + $alters) -DefaultsFile $defaultsFile | Out-Null

    Write-Host "Applying ops/migrate.sql to kestridge and kestridge_test..."
    $migrationSql = Get-Content -LiteralPath $migrate -Raw
    Invoke-MySqlAsRoot -Sql $migrationSql -DefaultsFile $defaultsFile -Database "kestridge"      | Out-Null
    Invoke-MySqlAsRoot -Sql $migrationSql -DefaultsFile $defaultsFile -Database "kestridge_test" | Out-Null

    function New-ConnectionString {
        param([string] $Database, [string] $User, [string] $Password)
        # SslMode=Preferred and AllowPublicKeyRetrieval=True are acceptable here
        # and only here: MySQL is bound to 127.0.0.1 on this machine. Across a
        # network hop, production uses VerifyFull with retrieval off.
        return "Server=$MySqlHost;Port=$Port;Database=$Database;User ID=$User;Password=$Password;" +
               "SslMode=Preferred;AllowPublicKeyRetrieval=True;DateTimeKind=Utc;" +
               "DefaultCommandTimeout=15;Pooling=true;MaximumPoolSize=20"
    }

    $appConnection  = New-ConnectionString -Database "kestridge"      -User "kestridge_app"  -Password $passwords.APP
    $testConnection = New-ConnectionString -Database "kestridge_test" -User "kestridge_test" -Password $passwords.TEST
    $pepper = New-StrongPassword

    # Piped as JSON on stdin, not passed as arguments. An argument vector is
    # readable by any local process through Win32_Process.CommandLine and is
    # captured permanently by command-line auditing, Sysmon and most EDR agents.
    Write-Host "Writing the connection string and the DSR pepper to dotnet user-secrets..."
    $secrets = [ordered]@{
        "ConnectionStrings:Default"       = $appConnection
        "Kestridge:Dsr:EmailHashPepper"   = $pepper
    }
    $secrets | ConvertTo-Json | & dotnet user-secrets set --project $apiProject | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "dotnet user-secrets set exited with $LASTEXITCODE" }

    Write-Host ""
    Write-Host "Verifying grants..." -ForegroundColor Cyan
    Invoke-MySqlAsRoot -Sql (Get-Content -LiteralPath $verify -Raw) -DefaultsFile $defaultsFile -Database "kestridge"

    Write-Host ""
    Write-Host "=========================================================" -ForegroundColor Yellow
    Write-Host " Store these in the team password manager now." -ForegroundColor Yellow
    Write-Host " They are printed once and are not written to any file." -ForegroundColor Yellow
    Write-Host "=========================================================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  kestridge_migrator  $($passwords.MIGRATOR)"
    Write-Host "  kestridge_ops       $($passwords.OPS)"
    Write-Host "  kestridge_backup    $($passwords.BACKUP)"
    Write-Host "  kestridge_test      $($passwords.TEST)"
    Write-Host ""
    Write-Host "  DSR email hash pepper (also in user-secrets):"
    Write-Host "  $pepper"
    Write-Host ""
    Write-Host "kestridge_app is already in user-secrets and is not printed."
    Write-Host ""
    Write-Host "To run the database-backed tests, set this for the session:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  `$env:KESTRIDGE_TEST_CONNECTION = '$testConnection'"
    Write-Host "  dotnet test"
    Write-Host ""
    Write-Host "Done." -ForegroundColor Green
}
finally {
    Remove-Item $defaultsFile -Force -ErrorAction SilentlyContinue
}
