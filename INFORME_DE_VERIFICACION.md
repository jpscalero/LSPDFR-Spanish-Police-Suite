# Informe de validación

Este informe describe las comprobaciones que se pueden repetir sin instalar GTA V, RAGE Plugin Hook ni LSPDFR.

## Pruebas disponibles

| Comprobación | Resultado |
| --- | --- |
| `python -m unittest discover -s tests -p "test_*.py"` | 8 pruebas pasaron |
| `python -m compileall -q tests` | Pasó |
| `tests/validate_powershell.ps1` | Analizó 4 scripts PowerShell sin errores de sintaxis |
| `tests/test_algorithms.ps1` | Pasaron casos válidos e inválidos del DNI y 100 identificadores generados |
| `build.ps1 -ValidateOnly` | Se encontró `csc.exe`; no pudo validar referencias porque falta `RagePluginHook.dll` |

## Límites

La compilación completa de las ocho DLL requiere las referencias oficiales `RagePluginHook.dll`, `LSPD First Response.dll` y `RAGENativeUI.dll`. No están incluidas en este repositorio ni disponibles en el entorno revisado. Por tanto, aquí no se afirma que los módulos compilen ni que sean compatibles en ejecución con versiones concretas del juego o de sus plugins.

Las pruebas Python verifican configuración, rutas de compilación, ausencia de rutas personales y estructura básica. La prueba del DNI compila e invoca directamente `DniModulo23.cs` en PowerShell; incluye una regresión para rechazar espacios y dígitos Unicode que `int.TryParse` aceptaba antes.

Los antiguos scripts de verificación dependían de rutas personales y de DLL ya desplegadas en la instalación local de GTA V; algunos copiaban archivos al juego y otros comprobaban artefactos externos. Sus contadores históricos de cientos de comprobaciones no eran una validación reproducible del árbol actual. Se sustituyeron por pruebas sin despliegue automático.
