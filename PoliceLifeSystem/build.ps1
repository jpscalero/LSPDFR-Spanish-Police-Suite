[CmdletBinding()]
param(
    [string]$ConfigPath = "..\build.config.json",
    [switch]$Deploy,
    [switch]$Clean
)

$moduleDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $moduleDir
$buildScript = Join-Path $repoRoot "build.ps1"
$resolvedConfig = if ([System.IO.Path]::IsPathRooted($ConfigPath)) {
    $ConfigPath
} else {
    [System.IO.Path]::GetFullPath((Join-Path $moduleDir $ConfigPath))
}

if (-not (Test-Path -LiteralPath $buildScript -PathType Leaf)) {
    throw "No se encontró el pipeline unificado: $buildScript"
}

& $buildScript -ConfigPath $resolvedConfig -Project "PoliceLifeSystem" -Deploy:$Deploy -Clean:$Clean -NoDeploy:(-not $Deploy.IsPresent)
exit $LASTEXITCODE
