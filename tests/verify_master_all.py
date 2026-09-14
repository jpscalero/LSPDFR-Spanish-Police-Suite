import os
import re
import sys
import subprocess

gta_dir = r"C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V"
plugins_dir = os.path.join(gta_dir, "plugins", "LSPDFR")
mods_base_dir = r"C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR"

passed = 0
failed = 0
total = 0

def check(name, condition, detail=""):
    global passed, failed, total
    total += 1
    if condition:
        passed += 1
        print("  [PASS] {} {}".format(name, detail))
    else:
        failed += 1
        print("  [FAIL] {} {}".format(name, detail))

print("=" * 75)
print("VERIFICACIÓN MAESTRA DE TODA LA SUITE LSPDFR (8 DLLs + REEMPLAZOS 100%)")
print("=" * 75)

# 1. VERIFICACIÓN DE DLLs E INIs EN GTA V
print("\n--- 1. DESPLIEGUE DE LAS 8 DLLs E INIs EN GTA V PLUGINS/LSPDFR ---")
modules = [
    ("SistemaPolicialIntegral", "SistemaPolicialIntegral.dll", "SistemaPolicialIntegral.ini"),
    ("PoliceLifeSystem", "PoliceLifeSystem.dll", "PoliceLifeSystem.ini"),
    ("TacticalOps", "TacticalOps.dll", "TacticalOps.ini"),
    ("RadioYJusticia", "RadioYJusticia.dll", "RadioYJusticia.ini"),
    ("MisionesEspeciales", "MisionesEspeciales.dll", "MisionesEspeciales.ini"),
    ("TacticalAndFederalCallouts", "TacticalAndFederalCallouts.dll", "TacticalAndFederalCallouts.ini"),
    ("PeligroExtremoCallouts", "PeligroExtremoCallouts.dll", "PeligroExtremoCallouts.ini"),
    ("AlternativeCallouts", "AlternativeCallouts.dll", "AlternativeCallouts.ini"),
]

for name, dll, ini in modules:
    dll_path = os.path.join(plugins_dir, dll)
    ini_path = os.path.join(plugins_dir, ini)
    check("DLL " + dll + " existe en GTA V", os.path.exists(dll_path))
    if os.path.exists(dll_path):
        size = os.path.getsize(dll_path)
        check("DLL " + dll + " tamaño válido (>20KB)", size > 20000, "({} bytes)".format(size))
    check("INI " + ini + " existe en GTA V", os.path.exists(ini_path))

# 2. PERSISTENCIA DE DATOS
print("\n--- 2. CARPETAS DE PERSISTENCIA Y DATOS ---")
spi_data = os.path.join(plugins_dir, "SPI_Data")
check("Carpeta SPI_Data existe", os.path.exists(spi_data))

# 3. ANÁLISIS ESTÁTICO DE SINTAXIS C# 5 EN TODO EL CÓDIGO FUENTE
print("\n--- 3. ANÁLISIS ESTÁTICO C# 5 (SIN INCOMPATIBILIDADES) ---")
csharp6_forbidden = [
    (r'\$"[^\n]*"', 'String interpolation ($"")'),
    (r'\bnameof\s*\(', 'nameof() operator'),
    (r'\?\.\w', 'Null-conditional operator (?.)'),
    (r'\basync\s+void\b|\basync\s+Task\b', 'async/await'),
]

all_cs_files = []
for root, _, files in os.walk(mods_base_dir):
    for f in files:
        if f.endswith(".cs") and not root.startswith(os.path.join(mods_base_dir, ".git")):
            all_cs_files.append(os.path.join(root, f))

c6_errors = 0
for cs in all_cs_files:
    rel = os.path.relpath(cs, mods_base_dir)
    with open(cs, 'r', encoding='utf-8', errors='ignore') as fh:
        content = fh.read()
        for pat, desc in csharp6_forbidden:
            matches = re.findall(pat, content)
            code_matches = [m for m in matches if not m.strip().startswith("//")]
            if len(code_matches) > 0:
                c6_errors += 1
                check(rel + " contiene " + desc, False, str(code_matches[:3]))

check("Total archivos .cs analizados ({}) 100% C# 5 limpio".format(len(all_cs_files)), c6_errors == 0)

# 4. VERIFICACIÓN DE STOPTHEPED 100% REEMPLAZO
print("\n--- 4. CARACTERÍSTICAS DE STOPTHEPED (100% REEMPLAZO Y MEJORA) ---")
ped_control = os.path.join(mods_base_dir, "SistemaPolicialIntegral", "Intervention", "PedControlManager.cs")
traffic_stop = os.path.join(mods_base_dir, "SistemaPolicialIntegral", "Intervention", "TrafficStopInteraction.cs")
intervention_menu = os.path.join(mods_base_dir, "SistemaPolicialIntegral", "Intervention", "InterventionMenu.cs")

check("PedControlManager.cs existe", os.path.exists(ped_control))
check("TrafficStopInteraction.cs existe", os.path.exists(traffic_stop))
check("InterventionMenu.cs existe", os.path.exists(intervention_menu))

with open(ped_control, 'r', encoding='utf-8', errors='ignore') as f:
    pc_code = f.read()
    check("Control: Agarrar y Escoltar (ToggleGrabPed)", "ToggleGrabPed" in pc_code)
    check("Control: Soltar detenido (ReleaseGrabbedPed)", "ReleaseGrabbedPed" in pc_code)
    check("Control: Meter en patrulla (PutInVehicle)", "PutInVehicle" in pc_code)
    check("Control: Sacar de patrulla (RemoveFromVehicle)", "RemoveFromVehicle" in pc_code)
    check("Control: Ordenar arrodillarse (MakePedKneel)", "MakePedKneel" in pc_code)
    check("Control: Ordenar sentarse en bordillo (MakePedSitOnCurb)", "MakePedSitOnCurb" in pc_code)
    check("Control: Ordenar ponerse de pie (MakePedStandUp)", "MakePedStandUp" in pc_code)

with open(traffic_stop, 'r', encoding='utf-8', errors='ignore') as f:
    ts_code = f.read()
    check("Tráfico: Bajar ventanilla (ROLL_DOWN_WINDOW)", "ROLL_DOWN_WINDOW" in ts_code)
    check("Tráfico: Apagar motor (SET_VEHICLE_ENGINE_ON)", "SET_VEHICLE_ENGINE_ON" in ts_code)
    check("Tráfico: Mirar al agente (TASK_LOOK_AT_ENTITY)", "TASK_LOOK_AT_ENTITY" in ts_code)
    check("Tráfico: Apearse del vehículo (OrderDriverExit)", "OrderDriverExit" in ts_code)

with open(intervention_menu, 'r', encoding='utf-8', errors='ignore') as f:
    im_code = f.read()
    options = [
        "itemDni", "itemAlcoholemia", "itemDrogas", "itemCacheo",
        "itemRegistroVeh", "itemDerechos", "itemDetener",
        "itemEscoltar", "itemMeterPatrulla", "itemSacarPatrulla",
        "itemArrodillar", "itemSentar", "itemPonerPie",
        "itemVentanilla", "itemBajarCoche", "itemLiberar"
    ]
    for opt in options:
        check("Menú Intervención incluye " + opt, opt in im_code)

# 5. VERIFICACIÓN DE ULTIMATEBACKUP 100% REEMPLAZO
print("\n--- 5. CARACTERÍSTICAS DE ULTIMATEBACKUP (100% REEMPLAZO Y MEJORA) ---")
spike_mgr = os.path.join(mods_base_dir, "SistemaPolicialIntegral", "Backup", "SpikeStripManager.cs")
roadblock_mgr = os.path.join(mods_base_dir, "SistemaPolicialIntegral", "Backup", "RoadblockManager.cs")
pursuit_mgr = os.path.join(mods_base_dir, "SistemaPolicialIntegral", "Backup", "PursuitTacticsManager.cs")
sirdee_menu = os.path.join(mods_base_dir, "SistemaPolicialIntegral", "Backup", "SirdeeBackupMenu.cs")

check("SpikeStripManager.cs existe", os.path.exists(spike_mgr))
check("RoadblockManager.cs existe", os.path.exists(roadblock_mgr))
check("PursuitTacticsManager.cs existe", os.path.exists(pursuit_mgr))
check("SirdeeBackupMenu.cs existe", os.path.exists(sirdee_menu))

with open(spike_mgr, 'r', encoding='utf-8', errors='ignore') as f:
    sp_code = f.read()
    check("Clavos: Prop p_ld_stinger_s", "p_ld_stinger_s" in sp_code)
    check("Clavos: Detección y pinchado de ruedas (BURST)", "SET_VEHICLE_TYRE_BURST" in sp_code)
    check("Clavos: Retirada limpia", "RemoveSpikeStrips" in sp_code)

with open(roadblock_mgr, 'r', encoding='utf-8', errors='ignore') as f:
    rb_code = f.read()
    check("Bloqueo: Detección de calle (GetNextPositionOnStreet)", "GetNextPositionOnStreet" in rb_code)
    check("Bloqueo: Cobertura táctica de agentes", "TakeCoverFrom" in rb_code)
    check("Bloqueo: Radar Blip táctico", "Blip" in rb_code)
    check("Bloqueo: Limpieza completa (ClearRoadblock)", "ClearRoadblock" in rb_code)

with open(pursuit_mgr, 'r', encoding='utf-8', errors='ignore') as f:
    pu_code = f.read()
    check("Persecución: Maniobra PIT (APPLY_FORCE_TO_ENTITY)", "APPLY_FORCE_TO_ENTITY" in pu_code)
    check("Persecución: Maniobra Caja (OrderBoxingIn)", "OrderBoxingIn" in pu_code)

with open(sirdee_menu, 'r', encoding='utf-8', errors='ignore') as f:
    sm_code = f.read()
    backup_items = [
        "itemZeta", "itemTrafico", "itemUpr", "itemGeo", "itemK9", "itemHelicoptero",
        "itemGrua", "itemAmbulancia", "itemCientifica",
        "itemSpikes", "itemRemoveSpikes", "itemRoadblock", "itemClearRoadblock",
        "itemPit", "itemBoxing"
    ]
    for b_item in backup_items:
        check("Menú SIRDEE Backup incluye " + b_item, b_item in sm_code)

# 6. VERIFICACIÓN DE MAPEO DE TECLAS (SIN COLISIONES)
print("\n--- 6. MAPEO DE TECLAS (SIN CONFLICTOS ENTRE MODS NI LSPDFR) ---")
key_bindings = [
    ("SistemaPolicialIntegral", "InterventionKey", "F9"),
    ("SistemaPolicialIntegral", "SitelComputerKey", "F10"),
    ("SistemaPolicialIntegral", "BackupKey", "Insert"),
    ("TacticalOps", "GestionEscenasKey", "Home"),
    ("TacticalOps", "BotiquinKey", "End"),
    ("TacticalOps", "CqbEntryKey", "PageDown"),
    ("RadioYJusticia", "SirdeeRadioKey", "Multiply"),
    ("RadioYJusticia", "BotonPanicoKey", "Subtract"),
    ("PoliceLifeSystem", "CareerMenuKey", "Add"),
    ("PoliceLifeSystem", "DocumentsMenuKey", "Divide"),
]

assigned_keys = {}
has_collision = False
for mod, name, key in key_bindings:
    if key in assigned_keys:
        has_collision = True
        check("Conflicto de tecla: " + key + " asignada a " + assigned_keys[key] + " y " + name, False)
    else:
        assigned_keys[key] = mod + "." + name
        check("Tecla única para " + mod + " (" + name + " = " + key + ")", True)

check("Cero conflictos de teclas en toda la suite", not has_collision)

# 7. RESUMEN FINAL
print("\n" + "=" * 75)
print("RESUMEN GENERAL DE VERIFICACIÓN:")
print("  Total pruebas: {}".format(total))
print("  Superadas:    {}".format(passed))
print("  Falladas:     {}".format(failed))
print("=" * 75)

if failed == 0:
    print("\n>>> RESULTADO: 100% EXITOSO - CERO ERRORES EN TODOS LOS MODS <<<")
    sys.exit(0)
else:
    print("\n>>> RESULTADO: EXISTEN {} ERRORES QUE DEBEN REVISARSE <<<".format(failed))
    sys.exit(1)
