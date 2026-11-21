# ==============================================================================
# LSPDFR Spanish Police Suite - Unified Build Pipeline
# Compatible with PowerShell 5.1+ / PowerShell Core (pwsh)
# Target: C# 5.0 / .NET Framework 4.8 / x64 Architecture
# ==============================================================================

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$ConfigPath = "build.config.json",

    [Parameter(Position = 1)]
    [string]$Project = "All",

    [switch]$Deploy,
    [switch]$Clean,
    [switch]$NoDeploy,
    [switch]$ValidateOnly
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host " LSPDFR Spanish Police Suite - Pipeline de Compilación Unificado" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan

# 1. Cargar Configuración
$ResolvedConfigPath = if ([System.IO.Path]::IsPathRooted($ConfigPath)) { $ConfigPath } else { Join-Path $ScriptDir $ConfigPath }
$ExampleConfigPath  = Join-Path $ScriptDir "build.config.json.example"

$Config = $null
if (Test-Path $ResolvedConfigPath) {
    Write-Host "[CONFIG] Cargando configuración desde: $ResolvedConfigPath" -ForegroundColor DarkGray
    try {
        $Config = Get-Content $ResolvedConfigPath -Raw | ConvertFrom-Json -ErrorAction Stop
    } catch {
        throw "La configuración '$ResolvedConfigPath' no es JSON válido: $($_.Exception.Message)"
    }
    $ConfigDirectory = Split-Path -Parent $ResolvedConfigPath
} elseif (Test-Path $ExampleConfigPath) {
    Write-Host "[CONFIG] '$ResolvedConfigPath' no encontrado. Usando plantilla: $ExampleConfigPath" -ForegroundColor Yellow
    try {
        $Config = Get-Content $ExampleConfigPath -Raw | ConvertFrom-Json -ErrorAction Stop
    } catch {
        throw "La plantilla '$ExampleConfigPath' no es JSON válido: $($_.Exception.Message)"
    }
    $ConfigDirectory = $ScriptDir
} else {
    throw "No existe la configuración ni la plantilla '$ExampleConfigPath'."
}

function Resolve-BuildPath {
    param([string]$Value, [string]$BaseDirectory)
    if ([string]::IsNullOrWhiteSpace($Value)) { return $null }
    if ([System.IO.Path]::IsPathRooted($Value)) {
        return [System.IO.Path]::GetFullPath($Value)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $BaseDirectory $Value))
}

function Get-FirstExistingFile {
    param([string[]]$Candidates)
    foreach ($candidate in $Candidates) {
        if (-not [string]::IsNullOrWhiteSpace($candidate) -and (Test-Path -LiteralPath $candidate -PathType Leaf)) {
            return [System.IO.Path]::GetFullPath($candidate)
        }
    }
    return $null
}

function Assert-ManagedAssembly {
    param([string]$Path, [string]$Name)
    if ([string]::IsNullOrWhiteSpace($Path)) {
        throw "Falta la ruta de $Name. Configura gtaPath y/o la ruta DLL correspondiente en build.config.json, o establece GTA_V_ROOT."
    }
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "No se encontró $Name en '$Path'. Configura build.config.json o las variables de entorno indicadas en README.md."
    }
    try {
        $assemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($Path)
    } catch {
        throw "La referencia $Name no es un ensamblado .NET válido: '$Path'. $($_.Exception.Message)"
    }
    if ([string]::IsNullOrWhiteSpace($assemblyName.Name)) {
        throw "La referencia $Name no contiene un nombre de ensamblado válido: '$Path'."
    }
    Write-Host ("   {0,-18}: {1} ({2}, {3})" -f $Name, $Path, $assemblyName.Name, $assemblyName.Version) -ForegroundColor DarkGray
}

$GtaDir = Resolve-BuildPath -Value ([string]$Config.gtaPath) -BaseDirectory $ConfigDirectory
if (-not $GtaDir -and $env:GTA_V_ROOT) { $GtaDir = [System.IO.Path]::GetFullPath($env:GTA_V_ROOT) }

$ConfiguredRph = Resolve-BuildPath -Value ([string]$Config.rphSdkPath) -BaseDirectory $ConfigDirectory
$ConfiguredLspdfr = Resolve-BuildPath -Value ([string]$Config.lspdfrDllPath) -BaseDirectory $ConfigDirectory
$ConfiguredRageUi = Resolve-BuildPath -Value ([string]$Config.rageNativeUiPath) -BaseDirectory $ConfigDirectory

$RphRef = if ($ConfiguredRph) {
    $ConfiguredRph
} else {
    Get-FirstExistingFile @(
        (Join-Path $ScriptDir "rph_sdk\lib\net472\RagePluginHook.dll"),
        $(if ($GtaDir) { Join-Path $GtaDir "RagePluginHook.dll" })
    )
}
$LspdfrRef = if ($ConfiguredLspdfr) { $ConfiguredLspdfr } elseif ($GtaDir) { Join-Path $GtaDir "plugins\LSPD First Response.dll" } else { $null }
$RageuiRef = if ($ConfiguredRageUi) { $ConfiguredRageUi } elseif ($GtaDir) { Join-Path $GtaDir "RAGENativeUI.dll" } else { $null }
$PluginsDir = if ($GtaDir) { Join-Path $GtaDir "plugins\LSPDFR" } else { $null }

$ConfiguredCsc = Resolve-BuildPath -Value ([string]$Config.cscPath) -BaseDirectory $ConfigDirectory
$CscCandidates = @($ConfiguredCsc, $env:CSC_PATH)
if ($env:WINDIR) {
    $CscCandidates += (Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe")
    $CscCandidates += (Join-Path $env:WINDIR "Microsoft.NET\Framework\v4.0.30319\csc.exe")
}
$CscPath = Get-FirstExistingFile $CscCandidates
if (-not $CscPath) {
    throw "No se encontró csc.exe. Configura cscPath en build.config.json o establece CSC_PATH."
}

$ConfiguredOutput = if ([string]::IsNullOrWhiteSpace([string]$Config.outputDir)) { ".\bin" } else { [string]$Config.outputDir }
$OutputDir = Resolve-BuildPath -Value $ConfiguredOutput -BaseDirectory $ConfigDirectory
$ShouldDeploy = (-not $NoDeploy.IsPresent) -and $Deploy.IsPresent

Write-Host " Verificaciones del Entorno:" -ForegroundColor Gray
Write-Host "   Compilador CSC   : $CscPath" -ForegroundColor DarkGray
Write-Host "   GTA V root       : $(if ($GtaDir) {$GtaDir} else {'not set (direct DLL paths required)'})" -ForegroundColor DarkGray
Write-Host "   Directorio Salida: $OutputDir" -ForegroundColor DarkGray
Write-Host "   Despliegue GTA   : $(if ($ShouldDeploy) {'ACTIVADO -> ' + $PluginsDir} else {'DESACTIVADO'})" -ForegroundColor DarkGray
Write-Host ""

Assert-ManagedAssembly -Path $RphRef -Name "RagePluginHook.dll"
Assert-ManagedAssembly -Path $LspdfrRef -Name "LSPD First Response.dll"
Assert-ManagedAssembly -Path $RageuiRef -Name "RAGENativeUI.dll"

if ($ShouldDeploy -and (-not $GtaDir -or -not (Test-Path -LiteralPath $PluginsDir -PathType Container))) {
    throw "El despliegue está activado, pero el directorio GTA V/plugins/LSPDFR no existe. Revisa gtaPath antes de desplegar."
}
if ($ValidateOnly) {
    Write-Host "[OK] Configuración, compilador y referencias .NET validados." -ForegroundColor Green
    exit 0
}

# 3. Definición de los 8 Módulos de la Suite
$AllModules = @(
    @{ Name = "SistemaPolicialIntegral";    Dir = "SistemaPolicialIntegral";    NeedsShared = $true  },
    @{ Name = "PoliceLifeSystem";           Dir = "PoliceLifeSystem";           NeedsShared = $false },
    @{ Name = "TacticalOps";                Dir = "TacticalOps";                NeedsShared = $false },
    @{ Name = "RadioYJusticia";             Dir = "RadioYJusticia";             NeedsShared = $true  },
    @{ Name = "MisionesEspeciales";         Dir = "MisionesEspeciales";         NeedsShared = $true  },
    @{ Name = "TacticalAndFederalCallouts"; Dir = "TacticalAndFederalCallouts"; NeedsShared = $true  },
    @{ Name = "PeligroExtremoCallouts";     Dir = "PeligroExtremoCallouts";     NeedsShared = $true  },
    @{ Name = "AlternativeCallouts";        Dir = "AlternativeCallouts";        NeedsShared = $true  }
)

if ($Project -eq "All") {
    $ModulesToBuild = @($AllModules)
} else {
    $ModulesToBuild = @($AllModules | Where-Object { $_.Name -like "*$Project*" })
}

if ($ModulesToBuild.Count -eq 0) {
    throw "No se encontró ningún módulo que coincida con el nombre '$Project'."
}

# 4. Preparar Carpeta de Salida. Limpiar solo los artefactos propios de los módulos seleccionados.
if ((Test-Path -LiteralPath $OutputDir) -and -not (Test-Path -LiteralPath $OutputDir -PathType Container)) {
    throw "El destino de salida existe y no es un directorio: '$OutputDir'."
}
if ($Clean.IsPresent -and (Test-Path -LiteralPath $OutputDir -PathType Container)) {
    Write-Host "[CLEAN] Quitando artefactos de los módulos seleccionados en: $OutputDir" -ForegroundColor Yellow
    foreach ($mod in $ModulesToBuild) {
        foreach ($extension in @("dll", "ini")) {
            $artifact = Join-Path $OutputDir ("{0}.{1}" -f $mod.Name, $extension)
            if (Test-Path -LiteralPath $artifact -PathType Leaf) {
                Remove-Item -LiteralPath $artifact -Force
            }
        }
    }
}
if (-not (Test-Path -LiteralPath $OutputDir -PathType Container)) {
    New-Item -Path $OutputDir -ItemType Directory -Force | Out-Null
}

# 5. Obtener Fuentes Compartidas (SharedCore)
$SharedSources = @(Get-ChildItem -Path (Join-Path $ScriptDir "SharedCore") -Filter "*.cs" -Recurse | Select-Object -ExpandProperty FullName)

# 6. Ejecución del Proceso de Compilación
$Results = @()
$OverallSuccess = $true
$TotalStopwatch = [System.Diagnostics.Stopwatch]::StartNew()

foreach ($mod in $ModulesToBuild) {
    $modName = $mod.Name
    $modDir = Join-Path $ScriptDir $mod.Dir
    $outDll = Join-Path $OutputDir "$modName.dll"
    $modIni = Join-Path $modDir "$modName.ini"
    
    Write-Host ">>> Compilando módulo: $modName" -ForegroundColor Cyan
    
    $sources = @(Get-ChildItem -Path $modDir -Filter "*.cs" -Recurse | Select-Object -ExpandProperty FullName)
    if ($mod.NeedsShared) {
        $sources += $SharedSources
    }
    
    $modStopwatch = [System.Diagnostics.Stopwatch]::StartNew()

    # Creación de archivo de respuesta .rsp para evitar problemas con rutas con espacios y límites de CLI
    $tempBase = Join-Path ([System.IO.Path]::GetTempPath()) ("lsp_build_{0}_{1}" -f $modName, [guid]::NewGuid().ToString("N"))
    $rspFile = "$tempBase.rsp"
    $stdoutFile = "$tempBase.stdout.txt"
    $stderrFile = "$tempBase.stderr.txt"
    $rspLines = [System.Collections.Generic.List[string]]::new()
    $rspLines.Add("/target:library")
    $rspLines.Add("/out:`"$outDll`"")
    $rspLines.Add("/langversion:5")
    $rspLines.Add("/codepage:65001")
    $rspLines.Add("/platform:x64")
    $rspLines.Add("/optimize+")
    $rspLines.Add("/nologo")
    $rspLines.Add("/reference:`"$RphRef`"")
    $rspLines.Add("/reference:`"$LspdfrRef`"")
    $rspLines.Add("/reference:`"$RageuiRef`"")
    $rspLines.Add("/reference:System.dll")
    $rspLines.Add("/reference:System.Core.dll")
    $rspLines.Add("/reference:System.Drawing.dll")
    $rspLines.Add("/reference:System.Windows.Forms.dll")
    $SpeechRef = $null
    if ($env:WINDIR) {
        $SpeechGacRoot = Join-Path $env:WINDIR "Microsoft.NET\assembly\GAC_MSIL\System.Speech"
        if (Test-Path -LiteralPath $SpeechGacRoot -PathType Container) {
            $SpeechRef = Get-FirstExistingFile @(
                (Get-ChildItem -LiteralPath $SpeechGacRoot -Filter "System.Speech.dll" -File -Recurse -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName)
            )
        }
    }
    if ($SpeechRef -and (Test-Path -LiteralPath $SpeechRef -PathType Leaf)) {
        $rspLines.Add("/reference:`"$SpeechRef`"")
    } else {
        $rspLines.Add("/reference:System.Speech.dll")
    }

    foreach ($s in $sources) {
        $rspLines.Add("`"$s`"")
    }
    try {
        [System.IO.File]::WriteAllLines($rspFile, $rspLines, [System.Text.Encoding]::UTF8)
        $proc = Start-Process -FilePath $CscPath -ArgumentList "@`"$rspFile`"" -NoNewWindow -PassThru -Wait -RedirectStandardOutput $stdoutFile -RedirectStandardError $stderrFile
        $modStopwatch.Stop()

        $stdout = if (Test-Path -LiteralPath $stdoutFile) { Get-Content $stdoutFile -Raw } else { "" }
        $stderr = if (Test-Path -LiteralPath $stderrFile) { Get-Content $stderrFile -Raw } else { "" }
    } finally {
        foreach ($tempFile in @($rspFile, $stdoutFile, $stderrFile)) {
            if (Test-Path -LiteralPath $tempFile -PathType Leaf) { Remove-Item -LiteralPath $tempFile -Force }
        }
    }
    
    if ($proc.ExitCode -eq 0 -and (Test-Path $outDll)) {
        $size = (Get-Item $outDll).Length
        $sizeKb = [math]::Round($size / 1KB, 1)
        Write-Host "    [OK] Compilado exitosamente: $sizeKb KB ($($modStopwatch.ElapsedMilliseconds) ms)" -ForegroundColor Green

        # Copiar INI si existe
        if (Test-Path $modIni) {
            Copy-Item -Path $modIni -Destination (Join-Path $OutputDir "$modName.ini") -Force
        }

        # Despliegue en GTA V si está habilitado
        if ($ShouldDeploy) {
            if (-not (Test-Path $PluginsDir)) {
                New-Item -Path $PluginsDir -ItemType Directory -Force | Out-Null
            }
            Copy-Item -Path $outDll -Destination (Join-Path $PluginsDir "$modName.dll") -Force
            if (Test-Path $modIni) {
                Copy-Item -Path $modIni -Destination (Join-Path $PluginsDir "$modName.ini") -Force
            }
            Write-Host "    [DESPLIEGUE] Copiado a plugins/LSPDFR/" -ForegroundColor DarkGreen
        }

        $Results += [PSCustomObject]@{
            Modulo   = $modName
            Estado   = "EXITO"
            Fuentes  = $sources.Count
            Tamano   = "$sizeKb KB"
            TiempoMs = $modStopwatch.ElapsedMilliseconds
        }
    } else {
        $OverallSuccess = $false
        Write-Host "    [FALLO] Código de salida: $($proc.ExitCode)" -ForegroundColor Red
        if ($stdout) { Write-Host $stdout -ForegroundColor Yellow }
        if ($stderr) { Write-Host $stderr -ForegroundColor Red }

        $Results += [PSCustomObject]@{
            Modulo   = $modName
            Estado   = "FALLO"
            Fuentes  = $sources.Count
            Tamano   = "0 KB"
            TiempoMs = $modStopwatch.ElapsedMilliseconds
        }
    }
}

$TotalStopwatch.Stop()

# 7. Resumen Final
Write-Host ""
Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host " RESUMEN DE COMPILACIÓN DE LA SUITE POLICIAL ESPAÑOLA" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan
$Results | Format-Table -AutoSize | Out-String | Write-Host

Write-Host "Tiempo total: $([math]::Round($TotalStopwatch.ElapsedMilliseconds / 1000, 2)) segundos" -ForegroundColor Gray
Write-Host "Directorio de artefactos: $OutputDir" -ForegroundColor Gray

if ($OverallSuccess) {
    Write-Host "`n✓ TODOS LOS MÓDULOS FUERON COMPILADOS EXITOSAMENTE CON C# 5 / .NET 4.8" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`n✗ HUBO ERRORES EN LA COMPILACIÓN DE UNO O MÁS MÓDULOS." -ForegroundColor Red
    exit 1
}
