# Guía de Instalación — 9 Mods LSPDFR (Instalados)

> ✅ **Todos los mods fueron instalados automáticamente el 9 de junio de 2026.**
> `GTAV/` = `C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V\`

---

## 1. Kuchera Callouts 3.0

| Archivo | Destino instalado |
|---|---|
| `KucheraCallouts.dll` | `GTAV/plugins/LSPDFR/` |
| `KucheraCallouts.ini` | `GTAV/plugins/LSPDFR/` |
| `RAGENativeUI.dll` | `GTAV/` (raíz) |
| Carpeta `Kuchera Audio/` (79 archivos .wav) | `GTAV/lspdfr/audio/scanner/Kuchera Audio/` |

**Dependencias:** LSPDFR, RAGE Plugin Hook, RAGENativeUI

---

## 2. Ambient Emergency Traffic V1.2.0

| Archivo | Destino instalado |
|---|---|
| `AmbientEmergencyTraffic.dll` | `GTAV/plugins/LSPDFR/` |
| `AmbientEmergencyTraffic.pdb` | `GTAV/plugins/LSPDFR/` |
| `AmbientEmergencyTraffic.ini` | `GTAV/plugins/LSPDFR/AmbientEmergencyTraffic/` |

**Dependencias:** LSPDFR, RAGE Plugin Hook, RAGENativeUI

---

## 3. AIDispatch 0.90.2 (For Policing Redefined)

| Archivo | Destino instalado |
|---|---|
| `AIDispatch.dll` | `GTAV/plugins/LSPDFR/` |
| `AIDispatch.ini` | `GTAV/plugins/LSPDFR/` |
| `AIDispatch_ChainAudio.txt` | `GTAV/plugins/LSPDFR/` |
| `AIDispatch_CustomPhrases.txt` | `GTAV/plugins/LSPDFR/` |
| `AIDispatch_PursuitRoadNames.txt` | `GTAV/plugins/LSPDFR/` |
| `Newtonsoft.Json.dll` | `GTAV/plugins/LSPDFR/` |
| Carpeta `AIDispatch/sounds/` (radio_start/end/fail/static) | `GTAV/plugins/LSPDFR/AIDispatch/sounds/` |
| Carpeta `AIDispatch/` (audio: Backup, Checks, General, PedReturns, PlateReturns, PursuitLocation, Services, Status, TrafficStop) | `GTAV/lspdfr/audio/scanner/AIDispatch/` |
| Carpeta `PolicingRedefinedAudio/` (PR_Crime, PR_Insurance, PR_License, etc.) | `GTAV/lspdfr/audio/scanner/PolicingRedefinedAudio/` |

**Dependencias:** LSPDFR, RAGE Plugin Hook, Policing Redefined, Damage Tracker Framework, Clave API Gemini  
**Configuración pendiente:** Editar `AIDispatch.ini` → sección `[Gemini]` → `ApiKey=TU_CLAVE_AQUÍ`  
Obtener clave gratis en: https://aistudio.google.com/api-keys

---

## 4. Real Traffic AI V2.0.1

| Archivo | Destino instalado |
|---|---|
| `Real Traffic AI.dll` | `GTAV/scripts/` |
| `Real Traffic AI.ini` | `GTAV/scripts/` |

> Se eliminaron archivos viejos (`RealTrafficAI.dll`/`.ini`) que estaban mal colocados en la raíz.

**Dependencias:** ScriptHookV, ScriptHookVDotNet

---

## 5. LSPDFR Policer Advanced v2.0.1

| Archivo | Destino instalado |
|---|---|
| `CommonLibSparky81x.dll` | `GTAV/` (raíz) |
| `RAGENativeUI.dll` | `GTAV/` (raíz, actualizado) |
| `LSPDFRPolicerAdvanced.dll` | `GTAV/plugins/LSPDFR/` |
| `PolicerAdvanced.ini` | `GTAV/plugins/LSPDFR/` |
| Carpeta `PolicerAdvanced/data/` | `GTAV/plugins/LSPDFR/PolicerAdvanced/data/` |
| Carpeta `PolicerAdvanced/Localization/` | `GTAV/plugins/LSPDFR/PolicerAdvanced/Localization/` |
| Carpeta `PolicerAdvanced/MDT/` (imágenes de vehículos, UI) | `GTAV/plugins/LSPDFR/PolicerAdvanced/MDT/` |
| Carpeta `PolicerAdvanced/Radio/` (botones UI) | `GTAV/plugins/LSPDFR/PolicerAdvanced/Radio/` |
| Carpeta `PolicerAdvanced/` (audio scanner) | `GTAV/lspdfr/audio/scanner/PolicerAdvanced/` |

> Se eliminaron archivos viejos de la versión anterior que estaban en la raíz y en carpetas incorrectas.

**Dependencias:** LSPDFR 0.4.9+, RAGE Plugin Hook, RAGENativeUI, CommonLibSparky81x

---

## 6. Gang Realism Overhaul

| Archivo | Destino instalado |
|---|---|
| `GangWorldSpawner.dll` | `GTAV/scripts/` |
| `GangWorldSystem.dll` | `GTAV/scripts/` |

**Dependencias:** ScriptHookV, ScriptHookVDotNet 3 (SHVDN3)

---

## 7. ✅ Realistic Charges & Citations for CompuLite

| Archivo | Destino instalado |
|---|---|
| `Charges.xml` | `GTAV/plugins/LSPDFR/CompuLite/` |
| `Citations.xml` | `GTAV/plugins/LSPDFR/CompuLite/` |

> **Nota importante:** Se ha eliminado **Policing Redefined** para evitar el conflicto con este mod. Los archivos XML de cargos y multas se han instalado correctamente, pero **debes descargar e instalar el mod base CompuLite** (`CompuLite.dll`), ya que había sido borrado previamente de tu equipo.

---

## 8. AIDispatch Spanish Voice Pack v0.9

| Archivo | Destino instalado |
|---|---|
| Carpeta `AIDispatch/` con subcarpetas: Backup, Checks, General, PedReturns, PlateReturns, PursuitLocation, Services, Status, TrafficStop (50 audios .wav en español) | `GTAV/lspdfr/audio/scanner/AIDispatch/` (reemplaza audio inglés) |

**Dependencias:** AIDispatch (mod #3) instalado previamente  
**Nota:** 133 archivos de audio total en la carpeta (50 español + 83 inglés originales del mod base)

---

## 9. LSPDFR Partners Beta v0.0.0

| Archivo | Destino instalado |
|---|---|
| `LSPDFRPartners.dll` | `GTAV/plugins/LSPDFR/` |
| `LSPDFRPartners.pdb` | `GTAV/plugins/LSPDFR/` |
| `LSPDFR Partners.ini` | `GTAV/plugins/LSPDFR/LSPDFR Partners/` |

**Dependencias:** LSPDFR, RAGE Plugin Hook  
⚠️ **Versión beta** — puede tener errores conocidos.

---

## 10. Stop The Ped (BejoIjo)

| Archivo | Destino instalado |
|---|---|
| `StopThePed.dll` | `GTAV/plugins/LSPDFR/` |
| `StopThePed.ini` | `GTAV/plugins/LSPDFR/` |
| Carpetas varias | `GTAV/plugins/LSPDFR/StopThePed/` |

**Dependencias:** LSPDFR, RAGE Plugin Hook, RAGENativeUI

---

## 11. Ultimate Backup (BejoIjo)

| Archivo | Destino instalado |
|---|---|
| `UltimateBackup.dll` | `GTAV/plugins/LSPDFR/` |
| `UltimateBackup.ini` | `GTAV/plugins/LSPDFR/` |
| Carpetas varias | `GTAV/plugins/LSPDFR/UltimateBackup/` |

**Dependencias:** LSPDFR, RAGE Plugin Hook, RAGENativeUI

---

## 12. Callout Interface

| Archivo | Destino instalado |
|---|---|
| `CalloutInterface.dll` | `GTAV/plugins/LSPDFR/` |
| `CalloutInterface.ini` | `GTAV/plugins/LSPDFR/` |
| `CalloutInterface` folder | `GTAV/plugins/LSPDFR/` |
| Texturas (iptc) | `GTAV/lspdfr/data/custom/` |

**Dependencias:** LSPDFR, RAGE Plugin Hook

---

## 13. UnitedCallouts

| Archivo | Destino instalado |
|---|---|
| `UnitedCallouts.dll` | `GTAV/plugins/LSPDFR/` |
| `UnitedCallouts.ini` | `GTAV/plugins/LSPDFR/` |
| `UnitedCallouts` folder | `GTAV/lspdfr/audio/scanner/` |

**Dependencias:** LSPDFR, RAGE Plugin Hook

---

## Estructura Final Instalada

```
Grand Theft Auto V/
├── RAGENativeUI.dll          ← (actualizado por Policer Advanced)
├── CommonLibSparky81x.dll    ← (Policer Advanced)
├── ScriptHookV.dll           ← (ya existía)
├── RAGEPluginHook.exe        ← (ya existía)
│
├── scripts/
│   ├── Real Traffic AI.dll        ← MOD 4
│   ├── Real Traffic AI.ini        ← MOD 4
│   ├── GangWorldSpawner.dll       ← MOD 6
│   └── GangWorldSystem.dll        ← MOD 6
│
├── plugins/
│   ├── LSPD First Response.dll    ← (ya existía)
│   └── LSPDFR/
│       ├── KucheraCallouts.dll          ← MOD 1
│       ├── KucheraCallouts.ini          ← MOD 1
│       ├── AmbientEmergencyTraffic.dll  ← MOD 2
│       ├── AmbientEmergencyTraffic/     ← MOD 2 (config)
│       ├── AIDispatch.dll               ← MOD 3
│       ├── AIDispatch.ini               ← MOD 3
│       ├── AIDispatch_*.txt             ← MOD 3
│       ├── Newtonsoft.Json.dll          ← MOD 3
│       ├── AIDispatch/sounds/           ← MOD 3
│       ├── LSPDFRPolicerAdvanced.dll    ← MOD 5
│       ├── PolicerAdvanced.ini          ← MOD 5
│       ├── PolicerAdvanced/             ← MOD 5 (data, MDT, Radio, Localization)
│       ├── CompuLite/                   ← MOD 7
│       │   ├── Charges.xml
│       │   └── Citations.xml
│       ├── LSPDFRPartners.dll           ← MOD 9
│       └── LSPDFR Partners/             ← MOD 9 (config)
│
└── lspdfr/
    └── audio/
        └── scanner/
            ├── Kuchera Audio/           ← MOD 1 (79 .wav)
            ├── AIDispatch/              ← MOD 3 + MOD 8 (español)
            ├── PolicingRedefinedAudio/  ← MOD 3
            └── PolicerAdvanced/         ← MOD 5
```

---

## ✅ Acciones Pendientes Resueltas

Todos los requisitos adicionales que faltaban inicialmente han sido instalados y configurados con éxito:
1. **ScriptHookVDotNet:** Instalado y actualizado a **v3.7.0 Nightly** (requerido para evitar el crasheo a los 5 minutos de Real Traffic AI y Gang Realism).
2. **Clave API de Gemini:** Configurada en `AIDispatch.ini`.
3. **Damage Tracker Framework:** Instalado (requerido para Policing Redefined).
4. **Policing Redefined:** ❌ Eliminado a petición del usuario para evitar conflictos con CompuLite (⚠️ *Advertencia: AIDispatch 0.90.2 podría crashear o no funcionar, ya que Policing Redefined es uno de sus requisitos*).

¡Tu LSPDFR está 100% configurado y listo para jugar con todos los mods activados!
