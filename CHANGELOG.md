# Changelog

Todos los cambios notables en este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto se adhiere a [Semantic Versioning](https://semver.org/lang/es/).

---

## [1.0.0] - 2026-09-14

### Añadido
- **8 Módulos Policiales Independientes (.NET Framework 4.8 / C# 5.0 x64)**:
  - `SistemaPolicialIntegral.dll`: Sustituto integral nativo de StopThePed y UltimateBackup (menú de intervención `F9`, terminal SITEL `F10`, menú de apoyo SIRDEE `Insert`, maniobras PIT y de caja, tiras de clavos, bloqueos de carretera en V con GEO, y balística no letal TASER NMI).
  - `PoliceLifeSystem.dll`: Progresión de carrera policial (CNP), régimen retributivo, economía de agente, y gestor de atestados judiciales (`NumPad +`, `NumPad /`).
  - `TacticalOps.dll`: Operaciones especiales, balizamiento perimetral cinemático de escenas de crimen (`Home`), botiquín táctico IFAK (`End`), y asalto táctico coordinado CQB (`PageDown`).
  - `RadioYJusticia.dll`: Emisora de radiocomunicaciones policiales SIRDEE (`NumPad *`), botón de pánico de emergencia (`NumPad -`), y tramitación judicial penal.
  - `MisionesEspeciales.dll`: 27 avisos de élite categorizados (Crimen Organizado, SEPRONA, Policía Aeroportuaria, UFAM y Eventos Masivos).
  - `TacticalAndFederalCallouts.dll`: Avisos tácticos de intervención rápida (SWAT/NOOSE, FIB y unidades de investigación).
  - `PeligroExtremoCallouts.dll`: Avisos de alto riesgo con tiroteos masivos, emboscadas y persecuciones a gran velocidad.
  - `AlternativeCallouts.dll`: Avisos de patrulla cotidiana con 5 desenlaces dinámicos (mediación pacífica, resistencia armada, fuga a pie, fuga en vehículo y giro crítico de alto riesgo).
- **Módulo de Código Compartido (`SharedCore/`)**:
  - `CalloutSpawner.cs`: Spawning seguro de peds, vehículos, props y operadores armados con cálculo adaptativo de elevación de terreno y agua (`GetGroundZ`).
  - `CalloutBlips.cs`: Gestión reactiva de marcadores en el mapa con descarte automático de entidades muertas o eliminadas.
  - `CalloutEffects.cs`: Control de combate avanzado, precisión balística, blindaje táctico, detonaciones y evacuación de rehenes.
- **Pipeline de Compilación Unificado (`build.ps1`)**:
  - Compilación automatizada de las 8 DLLs con `csc.exe` v4.8 bajo flags `/langversion:5 /codepage:65001 /platform:x64 /optimize+ /nologo`.
  - Soporte de archivos de respuesta `.rsp` para rutas de archivos con espacios y mitigación del límite de argumentos en la consola de comandos de Windows.
  - Opciones de limpieza (`-Clean`), compilación por módulo (`-Project <nombre>`) y despliegue directo a la carpeta del juego (`-Deploy`).
  - Archivo de configuración parametrizado `build.config.json` con plantilla versionada `build.config.json.example`.
- **Metadatos Formales de Ensamblado (`AssemblyInfo.cs`)**:
  - Incorporados GUIDs únicos de ensamblado, metadatos corporativos y versionado `1.0.0.0` en los 8 proyectos.

### Modificado
- **Saneamiento Integral de Excepciones**:
  - Eliminados el 100% de los bloques `catch { }` vacíos en los 257 archivos `.cs` del repositorio.
  - Sustituidos por captura explícita `catch (Exception ex)` y registro en el log de RagePluginHook mediante `Game.LogTrivial` con indicación contextual del módulo, clase y método causante.
- **Robustecimiento del Ciclo de Vida de los Plugins**:
  - Blindados los métodos `Initialize()`, `Finally()` y el manejador del evento `OnOnDutyStateChanged` en los 8 archivos `Main.cs`, evitando cierres inesperados del hilo del juego ante fallos de subsistemas aislados.
- **Refactorización de Clases Base (`BaseCallout.cs`)**:
  - Reducido el tamaño de las clases base de llamadas en los 3 paquetes (`TacticalAndFederalCallouts`, `PeligroExtremoCallouts`, `AlternativeCallouts`) a menos de 300 líneas por archivo.
  - Extraídas las responsabilidades de spawning, blips y combate al módulo `SharedCore`, preservando intacta la API pública y protegida para compatibilidad retrospectiva con todas las llamadas hijas.
- **Control de Versiones y Limpieza de Binarios**:
  - Actualizado `.gitignore` para excluir compilados (`*.dll`, `*.pdb`, `*.log`, `build.config.json`).
  - Desindexados los binarios commiteados del árbol de git (`git rm --cached`), dejando los Releases de GitHub como vía de distribución oficial de binarios.
- **Informe de Verificación Técnica (`INFORME_DE_VERIFICACION.md`)**:
  - Actualizado con total honestidad y rigor metodológico, detallando las comprobaciones estáticas, pruebas unitarias matemáticas y el entorno de compilación local frente a las limitaciones legales de runners en la nube.
