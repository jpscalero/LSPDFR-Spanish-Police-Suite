$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$scripts = Get-ChildItem -LiteralPath $repoRoot -Filter "*.ps1" -File -Recurse
$errors = @()
foreach ($scriptFile in $scripts) {
    $tokens = $null
    $parseErrors = $null
    $source = [System.IO.File]::ReadAllText($scriptFile.FullName)
    [System.Management.Automation.Language.Parser]::ParseInput(
        $source,
        [ref]$tokens,
        [ref]$parseErrors
    ) | Out-Null
    foreach ($parseError in $parseErrors) {
        $errors += "{0}:{1}: {2}" -f $scriptFile.FullName, $parseError.Extent.StartLineNumber, $parseError.Message
    }
}
if ($errors.Count -gt 0) {
    $errors | ForEach-Object { [Console]::Error.WriteLine($_) }
    exit 1
}
Write-Host "Parsed $($scripts.Count) PowerShell files successfully."
