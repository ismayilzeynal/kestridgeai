#Requires -Version 5.1
<#
.SYNOPSIS
  Turn an email address into the MySQL hex literal the DSR scripts expect.

.DESCRIPTION
  The DSR scripts address a data subject by an email address that the subject
  chose. ContactValidator accepts an apostrophe in the local part, so pasting a
  stored address into a single-quoted SQL literal is not safe:

    o'brien@example.com          -> a syntax error, so a real request cannot be
                                    processed with the documented tooling
    a'/**/or/**/1=1/**/'b@c.d    -> parses as LOWER('a' OR 1=1 OR 'b@c.d'),
                                    evaluates to '1', and every statement in the
                                    script then matches zero rows and reports
                                    success. The requester's data is neither
                                    exported nor deleted, and the log records the
                                    request as handled.

  A hex literal has no delimiter to break out of, which removes the hazard
  rather than trying to escape around it.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File ops/dsr-hex.ps1 "o'brien@example.com"

  SET @subject = LOWER(CONVERT(0x6f27627269656e406578616d706c652e636f6d USING utf8mb4));
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)][string] $EmailAddress
)

$ErrorActionPreference = "Stop"

$normalized = $EmailAddress.Trim().ToLowerInvariant()

if ($normalized.Length -eq 0) { throw "Empty address." }
if ($normalized -notmatch '@')  { throw "Not an email address: $EmailAddress" }

$bytes = [System.Text.Encoding]::UTF8.GetBytes($normalized)
$hex = ($bytes | ForEach-Object { $_.ToString("x2") }) -join ""

Write-Host ""
Write-Host "Address:  $normalized"
Write-Host "Bytes:    $($bytes.Length)"
Write-Host ""
Write-Host "Paste this line into ops/dsr-export.sql, ops/dsr-delete.sql or ops/dsr-log.sql:"
Write-Host ""
Write-Host "SET @subject = LOWER(CONVERT(0x$hex USING utf8mb4));"
Write-Host ""
Write-Host "Then run SELECT @subject; first and confirm it prints the address exactly."
Write-Host ""
