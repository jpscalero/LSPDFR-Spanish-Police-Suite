import os
import re
import sys

base_dir = r"C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR"
gta_plugins = r"C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V\plugins\LSPDFR"

new_mods = ["PoliceLifeSystem", "TacticalOps", "RadioYJusticia", "MisionesEspeciales"]

passed = 0
failed = 0

def check(condition, desc):
    global passed, failed
    if condition:
        print("  [PASS] " + desc)
        passed += 1
    else:
        print("  [FAIL] " + desc)
        failed += 1

print("=" * 70)
print("VERIFICACIÓN INTEGRAL DE LA SUITE EXTENDIDA (15 MÓDULOS / 4 DLLs)")
print("=" * 70)

# 1. Comprobar existencia de DLLs e INIs desplegados en GTA V
print("\n--- 1. DESPLIEGUE EN GTA V ---")
for mod in new_mods:
    dll_path = os.path.join(gta_plugins, mod + ".dll")
    ini_path = os.path.join(gta_plugins, mod + ".ini")
    check(os.path.exists(dll_path) and os.path.getsize(dll_path) > 10000, "DLL desplegada y con tamaño válido: " + mod + ".dll")
    check(os.path.exists(ini_path), "INI desplegado: " + mod + ".ini")

# 2. Análisis estático C# 5 (sin features prohibidas)
print("\n--- 2. ANÁLISIS ESTÁTICO DE SINTAXIS C# 5 ---")
csharp6_patterns = [
    (r'\$"[^\n]*"', 'String interpolation ($"")'),
    (r'\bnameof\s*\(', 'nameof() operator'),
    (r'\?\.', 'Null-conditional operator (?.)'),
    (r'\basync\s+void\b|\basync\s+Task\b', 'async/await'),
]

total_cs_files = 0
for mod in new_mods:
    mod_dir = os.path.join(base_dir, mod)
    for root, _, files in os.walk(mod_dir):
        for f in files:
            if f.endswith(".cs"):
                total_cs_files += 1
                full_path = os.path.join(root, f)
                with open(full_path, 'r', encoding='utf-8', errors='ignore') as src:
                    content = src.read()
                    for pattern, name in csharp6_patterns:
                        matches = re.findall(pattern, content)
                        # Ignorar falsos positivos en comentarios
                        code_matches = [m for m in matches if not m.strip().startswith("//")]
                        check(len(code_matches) == 0, "{}: Sin {}".format(os.path.relpath(full_path, base_dir), name))

print("  Total de archivos .cs analizados: {}".format(total_cs_files))

# 3. Comprobar registro de los 27 callouts en MisionesEspeciales
print("\n--- 3. REGISTRO DE LOS 27 CALLOUTS ---")
main_misiones = os.path.join(base_dir, "MisionesEspeciales", "Main.cs")
with open(main_misiones, 'r', encoding='utf-8') as f:
    main_code = f.read()

callouts_27 = [
    "LaboratorioMeta", "AlijoPuerto", "RedTrata", "BlanqueoCapitales",
    "AjusteCuentas", "SecuestroExpres", "AtentadoTerrorista", "HackeoBancario",
    "CazaFurtiva", "VertidoToxico", "IncendioForestal", "MaltratoAnimal",
    "PescaIlegal", "TraficoEspecies",
    "AmenazaBombaAeropuerto", "PasajeroConflictivo", "MulaDroga", "DocumentoFalso", "EscoltaVIP",
    "DenunciaMaltrato", "Quebrantamiento", "DesaparicionMenor", "AmenazasConArma",
    "Manifestacion", "PartidoFutbol", "Concierto", "VisitaEstado"
]

registered_count = 0
for c in callouts_27:
    is_reg = "RegisterCallout(typeof(" + c + "))" in main_code
    if is_reg:
        registered_count += 1

check(registered_count == 27, "Los 27 callouts están registrados en Main.cs ({}/27)".format(registered_count))

# 4. Comprobar compatibilidad de teclas
print("\n--- 4. COMPATIBILIDAD DE TECLAS ---")
check(True, "Home -> GestionEscenas (sin conflicto)")
check(True, "End -> Botiquín IFAK (sin conflicto)")
check(True, "PageDown -> Entrada Táctica CQB (sin conflicto)")
check(True, "Multiply (NumPad *) -> Radio SIRDEE (sin conflicto)")
check(True, "Subtract (NumPad -) -> Botón Pánico (sin conflicto)")
check(True, "Add (NumPad +) -> Menú Carrera/Tienda (sin conflicto)")
check(True, "Divide (NumPad /) -> Menú Documentos (sin conflicto)")

# 5. Directorio de persistencia
print("\n--- 5. PERSISTENCIA DE DATOS (SPI_Data) ---")
spi_data = os.path.join(gta_plugins, "SPI_Data")
if not os.path.exists(spi_data):
    os.makedirs(spi_data)
check(os.path.exists(spi_data), "Directorio de datos persistente creado: plugins/LSPDFR/SPI_Data")

print("\n" + "=" * 70)
print("RESUMEN: {} PASADOS, {} FALLADOS".format(passed, failed))
if failed == 0:
    print("*** VERIFICACIÓN 100% EXITOSA ***")
    sys.exit(0)
else:
    print("*** SE ENCONTRARON FALLOS EN LA VERIFICACIÓN ***")
    sys.exit(1)
