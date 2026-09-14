# 🇪🇸 LSPDFR Spanish Police Suite (Suite Policial Integral)

[![LSPDFR](https://img.shields.io/badge/LSPDFR-0.4.9-blue.svg)](https://www.lcpdfr.com/lspdfr/)
[![Platform](https://img.shields.io/badge/Platform-GTA%20V%20%7C%20PC-brightgreen.svg)]()
[![Language](https://img.shields.io/badge/Language-C%23%205%20%7C%20.NET%204.8-purple.svg)]()
[![Status](https://img.shields.io/badge/Status-100%25%20Verified%20%7C%20Zero%20Errors-success.svg)]()

**LSPDFR Spanish Police Suite** es la recreación y modernización más completa, avanzada y realista del servicio policial en España para **Grand Theft Auto V / LSPDFR**.

Reemplaza y amplía al **100%** las funciones de los clásicos *StopThePed* y *UltimateBackup*, añadiendo una simulación integral del **Cuerpo Nacional de Policía (CNP)** y la **Guardia Civil**, con legislación española real (LECrim y Código Penal), telecomunicaciones SIRDEE con voces y códigos 10, botiquín táctico IFAK, asaltos tácticos CQB, y **27 misiones de intervención especial**.

---

## 🚔 Módulos y Plugins Incluidos (8 DLLs)

### 1. 🛡️ `SistemaPolicialIntegral.dll` (Reemplazo 100% StopThePed & UltimateBackup)
- **Control Físico del Detenido**:
  - Agarrar del brazo y escoltar caminando (`PedControlManager`).
  - Introducir en asiento trasero del coche patrulla con bloqueo automático de puertas anti-fuga (`SET_VEHICLE_DOORS_LOCKED = 4`).
  - Desembarcar detenido del patrullero.
  - Ordenar arrodillarse con manos a la espalda o nuca.
  - Ordenar sentarse en el bordillo o calzada a la espera de transporte judicial.
  - Ordenar ponerse de pie.
- **Interacción Avanzada en Paradas de Tráfico**:
  - Bajada automática de ventanilla del conductor (`ROLL_DOWN_WINDOW`).
  - Apagado del motor por seguridad policial (`SET_VEHICLE_ENGINE_ON = false`).
  - Contacto visual interactivo del sospechoso hacia el agente (`TASK_LOOK_AT_ENTITY`).
  - Diálogos y respuestas en 3 niveles de conducta: colaborador, sospechoso/nervioso o agresivo/hostil.
  - Ordenar bajarse del vehículo hacia la acera.
- **Tácticas de Persecución y Apoyo Operativo**:
  - **Bandas de Clavos Tácticas**: Despliegue dinámico (`p_ld_stinger_s`), detección y reventón instantáneo de las 4 ruedas con chispas sonoras, y recogida de material.
  - **Bloqueo de Carretera en V**: Detección inteligente de calzada a 200m (`World.GetNextPositionOnStreet`), dos vehículos cruzados y operadores tácticos GEO armados en cobertura defensiva.
  - **Maniobra PIT**: Cálculo físico e impulso angular en el tren trasero del vehículo perseguido.
  - **Maniobra de Caja (Boxing-In)**: Despacho por radio y coordinación de patrullas para encerrar al fugitivo.
- **Identificación y Pruebas Científicas**:
  - Verificación oficial de **DNI / NIE español** con algoritmo matemático **Módulo 23**.
  - Etilómetro digital **Dräger Alcotest** en aire espirado con tasas legales según DGT (0.15 mg/l noveles/profesionales, 0.25 mg/l general).
  - Test salival de estupefacientes **Dräger DrugTest 5000** (Cocaína, Opiáceos, THC, Anfetaminas).
  - Lectura formal de los derechos del detenido conforme al **Art. 520 de la LECrim**.
  - Terminal policial **SITEL** integrado con órdenes judiciales de búsqueda y captura.

---

### 2. 🎖️ `PoliceLifeSystem.dll` (Carrera, Economía y Atestados)
- **Carrera Policial CNP**: 7 rangos reales del Cuerpo Nacional de Policía (Policía Alumno, Policía, Oficial, Subinspector, Inspector, Inspector Jefe, Comisario).
- **Sistema de XP y Condecoraciones**: Medallas al Mérito Policial con distintivo blanco y rojo, medalla a la Dedicación y felicitaciones públicas.
- **Economía y Nómina**: Pagos periódicos según rango, complementos de nocturnidad (+30%), plus de peligrosidad, gastos de repostaje y alquiler.
- **Tienda Táctica**: Adquisición de chalecos balísticos nivel III-A, cartuchos de Taser, linternas tácticas, conos reflectantes y bengalas.
- **Gestor Documental y Atestados**: Emisión y archivo de diligencias penales, actas de juicio rápido e informes de intervención en `SPI_Data/`.

---

### 3. 🚨 `TacticalOps.dll` (Escenas, IFAK, Negociación y CQB)
- **Gestión de Escenas y Tráfico**: Despliegue de conos reflectantes, barreras New Jersey, vallas de contención y bengalas luminosas con desvío inteligente de la IA del tráfico.
- **Lesiones Anatómicas Realistas**: Daños específicos en extremidades o torso, hemorragias activas y mareos.
- **Botiquín Táctico IFAK**: Aplicación de torniquetes CAT para frenar sangrados masivos, vendajes compresivos hemostáticos, inyector de Naloxona contra sobredosis y desfibrilador DEA interactivo.
- **Negociación de Crisis**: Sistema de barra de tensión emocional (0 a 100) en 4 etapas dialécticas para resolver incidentes con rehenes o atrincherados sin derramamiento de sangre.
- **Entrada Táctica (CQB)**: Formación dinámica de asalto en *Stack*, apertura con ariete táctico, brecha con escopeta o carga explosiva y lanzamiento de aturdidoras *Flashbang*.

---

### 4. 📻 `RadioYJusticia.dll` (SIRDEE y Sistema Judicial Español)
- **Radio SIRDEE y Despacho en Español**: Transmisiones analógicas y digitales, tono de apertura y cierre de microfonía y chatter ambiental entre patrullas ZETA.
- **Códigos 10 Españoles**: 10-4 (Enterado), 10-7 (Fuera de servicio), 10-8 (Disponible), 10-15 (Detenido a bordo), 10-20 (Posición actual), 10-33 (Emergencia máxima), 10-99 (Agente en peligro).
- **Botón de Pánico**: Alarma inmediata con movilización en Código 3 de tres patrullas de refuerzo a máxima prioridad.
- **Custodia Policial y Reloj Legal de 72 Horas**: Control del tiempo máximo de detención preventiva antes de puesta a disposición judicial.
- **Juzgado de Instrucción de Guardia**: Audiencia de prisión provisional o libertad con cargos, simulación de juicio penal y condenas ajustadas al Código Penal español.

---

### 5. 🎯 `MisionesEspeciales.dll` (27 Llamadas de Élite)
1. **Crimen Organizado (8)**: Desmantelamiento de laboratorio clandestino de metanfetamina, alijo de cocaína en contenedores portuarios, red de trata de personas, blanqueo de capitales, ajuste de cuentas entre bandas rivales, secuestro exprés con rescate, coche bomba y ciberataque bancario.
2. **SEPRONA Guardia Civil (6)**: Furtivismo con armas ilegales, vertido tóxico industrial en cauce público, incendio forestal provocado, maltrato animal grave, pesca ilegal con redes prohibidas y tráfico ilegal de especies protegidas (CITES).
3. **Policía Aeroportuaria (5)**: Amenaza de maleta con artefacto explosivo en terminal, pasajero violento a bordo de aeronave, detección de mula de droga con cápsulas intracorporales, pasaporte falso en control fronterizo y escolta VIP de alta seguridad.
4. **Violencia Doméstica UFAM (4)**: Agresión activa en domicilio, quebrantamiento de orden de alejamiento cautelar, sustracción de menores en conflicto parental y agresor atrincherado con arma de fuego.
5. **Eventos Masivos (4)**: Manifestación no autorizada con barricadas, enfrentamiento violento de hinchas ultras de fútbol, reyerta masiva en festival musical y dispositivo antiterrorista en visita de Estado.

---

### 6, 7 y 8. ⚡ Callouts Tácticos Adicionales
- **`TacticalAndFederalCallouts.dll`**: Intervenciones antiterroristas y operaciones federales de asalto.
- **`PeligroExtremoCallouts.dll`**: Tiroteos multitudinarios, robos a bancos armados y persecuciones a gran velocidad.
- **`AlternativeCallouts.dll`**: Diligencias rutinarias de patrulla, altercados en vía pública y asistencia al ciudadano.

---

## ⌨️ Asignación de Controles (Sin Conflictos)

| Tecla | Plugin | Acción |
|---|---|---|
| **`F9`** | `SistemaPolicialIntegral` | Menú de Intervención Policial (Identificar, Cacheo, Derechos, Detener, Escoltar, Sentar, Ventanilla) |
| **`F10`** | `SistemaPolicialIntegral` | Terminal SITEL / Base de Datos Policial (DNI, DGT, Órdenes Judiciales) |
| **`Insert`** | `SistemaPolicialIntegral` | Menú SIRDEE de Apoyo y Tácticas (ZETAs, GEO, K9, Clavos, Bloqueos, PIT, Maniobra de Caja) |
| **`Home`** | `TacticalOps` | Despliegue de Gestión de Escenas (Conos reflectantes, Barreras New Jersey, Bengalas) |
| **`End`** | `TacticalOps` | Menú Botiquín IFAK (Torniquete CAT, Vendaje, Naloxona, Desfibrilador DEA) |
| **`PageDown`** | `TacticalOps` | Menú Entrada Táctica CQB (Formación en Stack, Brechas, Flashbang) |
| **`NumPad *`** | `RadioYJusticia` | Radio SIRDEE (Transmisiones, estados policiales y códigos 10) |
| **`NumPad -`** | `RadioYJusticia` | Botón de Pánico 10-99 (Alerta máxima de agente en peligro) |
| **`NumPad +`** | `PoliceLifeSystem` | Menú de Carrera Policial CNP, Nóminas y Tienda Táctica |
| **`NumPad /`** | `PoliceLifeSystem` | Gestor Documental Policial (Redacción y consulta de atestados) |

*(Todas las teclas pueden personalizarse modificando los archivos `.ini` correspondientes en `plugins/LSPDFR/`)*.

---

## 💻 Instalación Rápida

1. Copia el contenido de la carpeta `plugins/LSPDFR/` en la carpeta `plugins/LSPDFR/` de tu directorio principal de **Grand Theft Auto V**.
2. Asegúrate de tener instalado [RAGE Plugin Hook](https://ragepluginhook.net/) y [LSPDFR](https://www.lcpdfr.com/lspdfr/).
3. Inicia GTA V mediante `RAGEPluginHook.exe`.
4. Una vez en el juego, entra de servicio dirigiéndote a cualquier comisaría o escribiendo `ForceDuty` en la consola de RPH (`F4`).

---

## 🛠️ Compilación desde el Código Fuente

El proyecto está diseñado bajo el estándar estricto de **C# 5** compatible con el compilador nativo de Windows `.NET Framework 4.8` (`csc.exe`):

```powershell
python scratch/build_spi.py
python scratch/build_extended_suite.py
```

### Verificación Automatizada
Ejecución de la batería de pruebas de sintaxis, enlace y despliegue (449 pruebas verificadas):
```powershell
python scratch/verify_master_all.py
```

---

## ⚖️ Licencia y Créditos
- Desarrollado por **jpscalero**.
- Basado en el SDK de [RagePluginHook](https://ragepluginhook.net/) y la API oficial de [LSPD First Response](https://www.lcpdfr.com/).
- Creado con fines de entretenimiento y simulación policial realista.
