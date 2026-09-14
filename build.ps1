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
    [switch]$Clean
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
    $Config = Get-Content $ResolvedConfigPath -Raw | ConvertFrom-Json
} elseif (Test-Path $ExampleConfigPath) {
    Write-Host "[CONFIG] '$ResolvedConfigPath' no encontrado. Usando plantilla: $ExampleConfigPath" -ForegroundColor Yellow
    $Config = Get-Content $ExampleConfigPath -Raw | ConvertFrom-Json
} else {
    Write-Host "[CONFIG] Configuración no encontrada. Aplicando valores predeterminados de fábrica." -ForegroundColor Yellow
    $Config = [PSComObject]@{
        gtaPath      = "C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V"
        rphSdkPath   = ".\rph_sdk\lib\net472\RagePluginHook.dll"
        cscPath      = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
        outputDir    = ".\bin"
        deployToGame = $false
    }
}

# 2. Resolver y Validar Rutas de Herramientas y Dependencias
$CscPath = $Config.cscPath
if (-not (Test-Path $CscPath)) {
    $FallbackCsc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
    if (Test-Path $FallbackCsc) {
        $CscPath = $FallbackCsc
    } else {
        Write-Error "CRÍTICO: No se encontró el compilador C# (csc.exe) en '$CscPath' ni en '$FallbackCsc'."
    }
}

$GtaDir = $Config.gtaPath
$OutputDir = if ([System.IO.Path]::IsPathRooted($Config.outputDir)) { $Config.outputDir } else { Join-Path $ScriptDir $Config.outputDir }
$ShouldDeploy = $Deploy.IsPresent -or ($Config.deployToGame -eq $true)

$RphRef = if ([System.IO.Path]::IsPathRooted($Config.rphSdkPath)) { $Config.rphSdkPath } else { Join-Path $ScriptDir $Config.rphSdkPath }
if (-not (Test-Path $RphRef)) {
    $RphRef = Join-Path $ScriptDir "rph_sdk\lib\net472\RagePluginHook.dll"
}

$LspdfrRef = Join-Path $GtaDir "plugins\LSPD First Response.dll"
$RageuiRef = Join-Path $GtaDir "RAGENativeUI.dll"
$PluginsDir = Join-Path $GtaDir "plugins\LSPDFR"

Write-Host " Verificaciones del Entorno:" -ForegroundColor Gray
Write-Host "   Compilador CSC   : $CscPath" -ForegroundColor DarkGray
Write-Host "   RPH SDK          : $RphRef $(if (Test-Path $RphRef) {'[OK]'} else {'[NO ENCONTRADO]'})" -ForegroundColor $(if (Test-Path $RphRef) {'DarkGray'} else {'Red'})
Write-Host "   LSPDFR DLL       : $LspdfrRef $(if (Test-Path $LspdfrRef) {'[OK]'} else {'[NO ENCONTRADO]'})" -ForegroundColor $(if (Test-Path $LspdfrRef) {'DarkGray'} else {'Red'})
Write-Host "   RAGENativeUI     : $RageuiRef $(if (Test-Path $RageuiRef) {'[OK]'} else {'[NO ENCONTRADO]'})" -ForegroundColor $(if (Test-Path $RageuiRef) {'DarkGray'} else {'Red'})
Write-Host "   Directorio Salida: $OutputDir" -ForegroundColor DarkGray
Write-Host "   Despliegue GTA   : $(if ($ShouldDeploy) {'ACTIVADO -> ' + $PluginsDir} else {'DESACTIVADO'})" -ForegroundColor DarkGray
Write-Host ""

if (-not (Test-Path $RphRef)) {
    Write-Error "Falta referencia requerida: RagePluginHook.dll en '$RphRef'."
}
if (-not (Test-Path $LspdfrRef)) {
    Write-Error "Falta referencia requerida: LSPD First Response.dll en '$LspdfrRef'."
}
if (-not (Test-Path $RageuiRef)) {
    Write-Error "Falta referencia requerida: RAGENativeUI.dll en '$RageuiRef'."
}

# 3. Preparar Carpeta de Salida
if ($Clean.IsPresent -and (Test-Path $OutputDir)) {
    Write-Host "[CLEAN] Limpiando carpeta de salida: $OutputDir" -ForegroundColor Yellow
    Remove-Item -Path $OutputDir -Recurse -Force
}
if (-not (Test-Path $OutputDir)) {
    New-Item -Path $OutputDir -ItemType Directory -Force | Out-Null
}

# 4. Definición de los 8 Módulos de la Suite
$AllModules = @(
    @{ Name = "SistemaPolicialIntegral";    Dir = "SistemaPolicialIntegral";    NeedsShared = $false },
    @{ Name = "PoliceLifeSystem";           Dir = "PoliceLifeSystem";           NeedsShared = $false },
    @{ Name = "TacticalOps";                Dir = "TacticalOps";                NeedsShared = $false },
    @{ Name = "RadioYJusticia";             Dir = "RadioYJusticia";             NeedsShared = $false },
    @{ Name = "MisionesEspeciales";         Dir = "MisionesEspeciales";         NeedsShared = $false },
    @{ Name = "TacticalAndFederalCallouts"; Dir = "TacticalAndFederalCallouts"; NeedsShared = $true  },
    @{ Name = "PeligroExtremoCallouts";     Dir = "PeligroExtremoCallouts";     NeedsShared = $true  },
    @{ Name = "AlternativeCallouts";        Dir = "AlternativeCallouts";        NeedsShared = $true  }
)

$ModulesToBuild = if ($Project -eq "All") {
    $AllModules
} else {
    $AllModules | Where-Object { $_.Name -like "*$Project*" }
}

if ($ModulesToBuild.Count -eq 0) {
    Write-Error "No se encontró ningún módulo que coincida con el nombre '$Project'."
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
    $rspFile = Join-Path $env:TEMP "csc_$modName.rsp"
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

    foreach ($s in $sources) {
        $rspLines.Add("`"$s`"")
    }
    [System.IO.File]::WriteAllLines($rspFile, $rspLines, [System.Text.Encoding]::UTF8)

    $proc = Start-Process -FilePath $CscPath -ArgumentList "@`"$rspFile`"" -NoNewWindow -PassThru -Wait -RedirectStandardOutput "$env:TEMP\csc_out.txt" -RedirectStandardError "$env:TEMP\csc_err.txt"
    $modStopwatch.Stop()
    
    $stdout = if (Test-Path "$env:TEMP\csc_out.txt") { Get-Content "$env:TEMP\csc_out.txt" -Raw } else { "" }
    $stderr = if (Test-Path "$env:TEMP\csc_err.txt") { Get-Content "$env:TEMP\csc_err.txt" -Raw } else { "" }

    if (Test-Path $rspFile) { Remove-Item $rspFile -Force }
    
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
