$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$outDll = Join-Path $scriptDir "PoliceLifeSystem.dll"

$rphDll = "C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR\rph_sdk\lib\net472\RagePluginHook.dll"
$lspdfrDll = "C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR\PoliceAcademyMod\LSPD_First_Response.dll"
$rnuiDll = "C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR\PoliceAcademyMod\RAGENativeUI.dll"

Write-Host "Compilando PoliceLifeSystem..." -ForegroundColor Cyan

& $csc /target:library /optimize+ `
    /out:"$outDll" `
    /r:"$rphDll" `
    /r:"$lspdfrDll" `
    /r:"$rnuiDll" `
    /r:System.Windows.Forms.dll `
    /r:System.Drawing.dll `
    /recurse:"$scriptDir\*.cs"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Compilación exitosa -> $outDll" -ForegroundColor Green
} else {
    Write-Host "Error en la compilación." -ForegroundColor Red
}
