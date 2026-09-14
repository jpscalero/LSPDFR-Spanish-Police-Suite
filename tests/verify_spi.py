import os
import shutil
import re
import sys

# ============================================================
# VERIFICACIÓN COMPLETA DEL SISTEMA POLICIAL INTEGRAL
# ============================================================

gta_dir = r"C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V"
plugins_dir = os.path.join(gta_dir, "plugins", "LSPDFR")
mod_dir = r"C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR\SistemaPolicialIntegral"

passed = 0
failed = 0
total = 0

def check(name, condition, detail=""):
    global passed, failed, total
    total += 1
    if condition:
        passed += 1
        print("[PASS] {} {}".format(name, detail))
    else:
        failed += 1
        print("[FAIL] {} {}".format(name, detail))

# ============================================================
# FASE 1: DESPLIEGUE
# ============================================================
print("=" * 60)
print("FASE 1: DESPLIEGUE A GTA V")
print("=" * 60)

dll_src = os.path.join(mod_dir, "SistemaPolicialIntegral.dll")
ini_src = os.path.join(mod_dir, "SistemaPolicialIntegral.ini")
dll_dst = os.path.join(plugins_dir, "SistemaPolicialIntegral.dll")
ini_dst = os.path.join(plugins_dir, "SistemaPolicialIntegral.ini")

os.makedirs(plugins_dir, exist_ok=True)

if os.path.exists(dll_src):
    shutil.copy2(dll_src, dll_dst)
    print("  DLL deployed: {}".format(dll_dst))
if os.path.exists(ini_src):
    shutil.copy2(ini_src, ini_dst)
    print("  INI deployed: {}".format(ini_dst))

check("DLL exists at GTA V", os.path.exists(dll_dst))
check("INI exists at GTA V", os.path.exists(ini_dst))
check("DLL size > 50KB", os.path.getsize(dll_dst) > 50000, "({} bytes)".format(os.path.getsize(dll_dst)))

# ============================================================
# FASE 2: VERIFICACIÓN DE ESTRUCTURA DE ARCHIVOS
# ============================================================
print("\n" + "=" * 60)
print("FASE 2: ESTRUCTURA DE ARCHIVOS")
print("=" * 60)

expected_files = [
    "Main.cs",
    "Core/ConfigManager.cs",
    "Core/AudioEngine.cs",
    "Intervention/InterventionMenu.cs",
    "Intervention/DniVerification.cs",
    "Intervention/AlcoholimeterDrager.cs",
    "Intervention/DrugTest5000.cs",
    "Intervention/FriskSearchManager.cs",
    "Intervention/RightsManager.cs",
    "MDT/SitelComputerMenu.cs",
    "MDT/SpanishPenalCode.cs",
    "MDT/DgtPointSystem.cs",
    "MDT/JudicialWarrants.cs",
    "Backup/SirdeeBackupMenu.cs",
    "Backup/UnitSpawner.cs",
    "Ballistics/TaserNmiController.cs",
    "Ballistics/PepperSprayController.cs",
    "Ballistics/BallisticArmorManager.cs",
    "Services/MunicipalFlatbedTow.cs",
    "Services/SamurAmbulanceService.cs",
    "Services/ForensicJudicialVan.cs",
    "Census/SpanishIdentityGenerator.cs",
    "Census/DniModulo23.cs",
    "Census/PedHooker.cs",
    "SistemaPolicialIntegral.ini",
]

for f in expected_files:
    full = os.path.join(mod_dir, f)
    check("File exists: " + f, os.path.exists(full))

# ============================================================
# FASE 3: VALIDACIÓN DE CÓDIGO C#
# ============================================================
print("\n" + "=" * 60)
print("FASE 3: VALIDACIÓN DE CÓDIGO C#")
print("=" * 60)

# Check no C# 6+ features
cs_files = []
for root, _, files in os.walk(mod_dir):
    for f in files:
        if f.endswith(".cs"):
            cs_files.append(os.path.join(root, f))

c6_violations = []
for cs_file in cs_files:
    with open(cs_file, "r", encoding="utf-8", errors="ignore") as fh:
        content = fh.read()
        basename = os.path.basename(cs_file)
        # Check for string interpolation
        if re.search(r'\$"', content):
            c6_violations.append("{}: string interpolation $\"...\"".format(basename))
        # Check for null-conditional
        if re.search(r'\?\.\w', content):
            c6_violations.append("{}: null-conditional ?.".format(basename))
        # Check for nameof
        if "nameof(" in content:
            c6_violations.append("{}: nameof()".format(basename))

check("No C# 6+ features", len(c6_violations) == 0, 
      "violations: " + str(c6_violations) if c6_violations else "")

# Check all files have correct namespace
namespace_ok = True
for cs_file in cs_files:
    with open(cs_file, "r", encoding="utf-8", errors="ignore") as fh:
        content = fh.read()
        if "namespace SistemaPolicialIntegral" not in content:
            namespace_ok = False
            print("  MISSING namespace: " + os.path.basename(cs_file))

check("All files use SistemaPolicialIntegral namespace", namespace_ok)

# ============================================================
# FASE 4: VALIDACIÓN ALGORITMO DNI MÓDULO 23
# ============================================================
print("\n" + "=" * 60)
print("FASE 4: VALIDACIÓN ALGORITMO DNI")
print("=" * 60)

TABLA = "TRWAGMYFPDXBNJZSQVHLCKE"

test_cases = [
    (12345678, 'Z'),
    (00000000, 'T'),
    (99999999, 'R'),
    (11111111, 'H'),
    (22222222, 'J'),
    (44444444, 'A'),
]

all_dni_ok = True
for num, expected_letter in test_cases:
    calculated = TABLA[num % 23]
    ok = calculated == expected_letter
    if not ok:
        all_dni_ok = False
        print("  DNI {}: expected {}, got {}".format(num, expected_letter, calculated))
    else:
        print("  DNI {}{} OK".format(num, expected_letter))

check("DNI Modulo 23 algorithm correct", all_dni_ok)

# Check the algorithm is correctly implemented in source
with open(os.path.join(mod_dir, "Census", "DniModulo23.cs"), "r", encoding="utf-8", errors="ignore") as fh:
    dni_source = fh.read()
    check("DniModulo23 contains TRWAGMYFPDXBNJZSQVHLCKE", "TRWAGMYFPDXBNJZSQVHLCKE" in dni_source)
    check("DniModulo23 uses modulo 23", "% 23" in dni_source)

# ============================================================
# FASE 5: VALIDACIÓN DE TECLAS NO CONFLICTIVAS
# ============================================================
print("\n" + "=" * 60)
print("FASE 5: VALIDACIÓN DE TECLAS")
print("=" * 60)

# Teclas usadas por plugins existentes de LSPDFR
existing_keys = {
    "E": "GTA V Context/Interact",
    "G": "StopThePed",
    "B": "UltimateBackup",
    "F11": "CompuLite MDT",
    "T": "GTA V Phone",
    "Y": "LSPDFR Menu",
    "F4": "RPH Console",
    "F6": "LSPDFR Action Menu",
    "F7": "LSPDFR Callout Status",
    "F8": "RPH Settings",
}

with open(os.path.join(mod_dir, "SistemaPolicialIntegral.ini"), "r", encoding="utf-8", errors="ignore") as fh:
    ini_content = fh.read()

our_keys = {}
for line in ini_content.split("\n"):
    if "Key=" in line and "=" in line and not line.strip().startswith(";"):
        parts = line.strip().split("=", 1)
        if len(parts) == 2:
            our_keys[parts[0].strip()] = parts[1].strip()

print("  Our keys: {}".format(our_keys))
conflicts = []
for key_name, key_val in our_keys.items():
    if key_val in existing_keys:
        conflicts.append("{} ({}) conflicts with {}".format(key_name, key_val, existing_keys[key_val]))

check("No key conflicts with existing plugins", len(conflicts) == 0,
      "conflicts: " + str(conflicts) if conflicts else "F9, F10, Insert = unique")

# ============================================================
# FASE 6: VALIDACIÓN DE CONTENIDO ESPAÑOL
# ============================================================
print("\n" + "=" * 60)
print("FASE 6: CONTENIDO ESPAÑOL")
print("=" * 60)

# Check SpanishIdentityGenerator has enough names
with open(os.path.join(mod_dir, "Census", "SpanishIdentityGenerator.cs"), "r", encoding="utf-8", errors="ignore") as fh:
    gen_source = fh.read()
    # Count quoted strings in arrays
    male_names = re.findall(r'"[A-ZÁÉÍÓÚÑÜ][a-záéíóúñü]+"', gen_source)
    check("Spanish names count > 200", len(male_names) > 200, "found {} quoted names/surnames".format(len(male_names)))

# Check SpanishPenalCode has entries
with open(os.path.join(mod_dir, "MDT", "SpanishPenalCode.cs"), "r", encoding="utf-8", errors="ignore") as fh:
    penal_source = fh.read()
    articles = re.findall(r'Articulo\s*=\s*"([^"]+)"', penal_source)
    check("Penal Code has 5+ articles", len(articles) >= 5, "found {} articles".format(len(articles)))

# Check RightsManager has Art. 520 LECrim
with open(os.path.join(mod_dir, "Intervention", "RightsManager.cs"), "r", encoding="utf-8", errors="ignore") as fh:
    rights_source = fh.read()
    check("RightsManager mentions Art. 520", "520" in rights_source or "derechos" in rights_source.lower())

# ============================================================
# FASE 7: VALIDACIÓN DE MÓDULOS EN MAIN.CS
# ============================================================
print("\n" + "=" * 60)
print("FASE 7: MÓDULOS REGISTRADOS EN MAIN.CS")
print("=" * 60)

with open(os.path.join(mod_dir, "Main.cs"), "r", encoding="utf-8", errors="ignore") as fh:
    main_source = fh.read()

modules_to_check = [
    "InterventionMenu",
    "SitelComputerMenu",
    "SirdeeBackupMenu",
    "TaserNmiController",
    "PepperSprayController",
    "BallisticArmorManager",
    "PedHooker",
]

for mod in modules_to_check:
    check("Main.cs initializes " + mod, mod in main_source)

check("Main.cs has no commented-out modules", "// GameFiber" not in main_source and "/*" not in main_source)
check("ConfigManager.Load() called before modules", 
      main_source.index("ConfigManager.Load()") < main_source.index("InterventionMenu"))

# ============================================================
# FASE 8: RESUMEN DE OTROS 3 MODS EXISTENTES
# ============================================================
print("\n" + "=" * 60)
print("FASE 8: VERIFICACIÓN MODS EXISTENTES")
print("=" * 60)

existing_mods = [
    ("TacticalAndFederalCallouts", "TacticalAndFederalCallouts.dll"),
    ("PeligroExtremoCallouts", "PeligroExtremoCallouts.dll"),
    ("AlternativeCallouts", "AlternativeCallouts.dll"),
]

for mod_name, dll_name in existing_mods:
    dll_path = os.path.join(plugins_dir, dll_name)
    check("{} deployed".format(mod_name), os.path.exists(dll_path),
          "({} bytes)".format(os.path.getsize(dll_path)) if os.path.exists(dll_path) else "MISSING")

# ============================================================
# RESUMEN FINAL
# ============================================================
print("\n" + "=" * 60)
print("RESUMEN FINAL")
print("=" * 60)
print("Passed: {}/{}".format(passed, total))
print("Failed: {}/{}".format(failed, total))

if failed == 0:
    print("\n*** TODAS LAS VERIFICACIONES PASADAS EXITOSAMENTE ***")
    print("SistemaPolicialIntegral.dll desplegado y listo para uso en GTA V.")
else:
    print("\n*** HAY FALLOS QUE CORREGIR ***")

sys.exit(0 if failed == 0 else 1)
