$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$source = Join-Path $repoRoot "SistemaPolicialIntegral\Census\DniModulo23.cs"
Add-Type -Path $source

$validCases = @(
    @{ Dni = "12345678Z"; Expected = $true },
    @{ Dni = "00000000T"; Expected = $true },
    @{ Dni = "99999999R"; Expected = $true },
    @{ Dni = "12345678z"; Expected = $true }
)
foreach ($case in $validCases) {
    $actual = [SistemaPolicialIntegral.Census.DniModulo23]::ValidarDni($case.Dni)
    if ($actual -ne $case.Expected) { throw "Unexpected DNI validation result for '$($case.Dni)'." }
}

$fullWidthDigits = [string]::Concat([char]0xFF11, [char]0xFF12, [char]0xFF13, [char]0xFF14, [char]0xFF15, [char]0xFF16, [char]0xFF17, [char]0xFF18, "Z")
$invalidCases = @("", "1234567Z", "12345678A", " 1234567Z", $fullWidthDigits, "1234567-Z")
foreach ($dni in $invalidCases) {
    if ([SistemaPolicialIntegral.Census.DniModulo23]::ValidarDni($dni)) {
        throw "Invalid DNI was accepted: '$dni'."
    }
}

for ($i = 0; $i -lt 100; $i++) {
    $generated = [SistemaPolicialIntegral.Census.DniModulo23]::GenerarDniAleatorio()
    if (-not [SistemaPolicialIntegral.Census.DniModulo23]::ValidarDni($generated)) {
        throw "Generated DNI failed validation: '$generated'."
    }
}

Write-Host "DNI modulo 23 behavior passed (known cases, malformed input, generated values)."
