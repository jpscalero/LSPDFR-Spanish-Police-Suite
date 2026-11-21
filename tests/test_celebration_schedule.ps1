$ErrorActionPreference = "Stop"
$rulesPath = Join-Path $PSScriptRoot "../PoliceLifeSystem/Career/CelebrationScheduleRules.cs"
Add-Type -Path $rulesPath

function Assert-Equal($name, $expected, $actual) {
    if ($expected -ne $actual) {
        throw "$name`: se esperaba '$expected' y se obtuvo '$actual'."
    }
    Write-Output "[OK] $name"
}

$start = [DateTime]::SpecifyKind([DateTime]::new(2025, 1, 1, 12, 0, 0), [DateTimeKind]::Unspecified)
Assert-Equal "No vence un instante antes de siete días" $false ([PoliceLifeSystem.Career.CelebrationScheduleRules]::IsDue($start, $start.AddDays(7).AddTicks(-1), 7))
Assert-Equal "Vence exactamente a los siete días" $true ([PoliceLifeSystem.Career.CelebrationScheduleRules]::IsDue($start, $start.AddDays(7), 7))
Assert-Equal "El intervalo mínimo se limita a un día" $true ([PoliceLifeSystem.Career.CelebrationScheduleRules]::IsDue($start, $start.AddDays(1), 0))
Assert-Equal "El intervalo máximo se limita a 365 días" 365 ([PoliceLifeSystem.Career.CelebrationScheduleRules]::ClampIntervalDays(999))
Assert-Equal "El máximo no vence un día antes" $false ([PoliceLifeSystem.Career.CelebrationScheduleRules]::IsDue($start, $start.AddDays(364), 999))
Assert-Equal "El máximo vence al cumplir 365 días" $true ([PoliceLifeSystem.Career.CelebrationScheduleRules]::IsDue($start, $start.AddDays(365), 999))
Assert-Equal "Fecha de juego retrocedida no vence" $false ([PoliceLifeSystem.Career.CelebrationScheduleRules]::IsDue($start, $start.AddDays(-1), 7))
Assert-Equal "Fecha inicial vacía no vence" $false ([PoliceLifeSystem.Career.CelebrationScheduleRules]::IsDue([DateTime]::MinValue, $start, 7))
Assert-Equal "Fecha guardada extrema no desborda" $true ([PoliceLifeSystem.Career.CelebrationScheduleRules]::IsDue([DateTime]::MinValue.AddTicks(1), [DateTime]::MaxValue, 365))
