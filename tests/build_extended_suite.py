import os
import subprocess
import shutil
import sys

csc_path = r"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
gta_dir = r"C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V"
rph_ref = r"C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR\rph_sdk\lib\net472\RagePluginHook.dll"
lspdfr_ref = os.path.join(gta_dir, "plugins", "LSPD First Response.dll")
rageui_ref = os.path.join(gta_dir, "RAGENativeUI.dll")
plugins_dir = os.path.join(gta_dir, "plugins", "LSPDFR")

mods_root = r"C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR"

mods = [
    {
        "name": "PoliceLifeSystem",
        "dir": os.path.join(mods_root, "PoliceLifeSystem"),
        "dll": "PoliceLifeSystem.dll",
        "ini": "PoliceLifeSystem.ini",
    },
    {
        "name": "TacticalOps",
        "dir": os.path.join(mods_root, "TacticalOps"),
        "dll": "TacticalOps.dll",
        "ini": "TacticalOps.ini",
    },
    {
        "name": "RadioYJusticia",
        "dir": os.path.join(mods_root, "RadioYJusticia"),
        "dll": "RadioYJusticia.dll",
        "ini": "RadioYJusticia.ini",
    },
    {
        "name": "MisionesEspeciales",
        "dir": os.path.join(mods_root, "MisionesEspeciales"),
        "dll": "MisionesEspeciales.dll",
        "ini": "MisionesEspeciales.ini",
    },
]

def collect_cs(folder):
    res = []
    for root, _, files in os.walk(folder):
        for f in files:
            if f.endswith(".cs"):
                res.append(os.path.join(root, f))
    return res

all_success = True

print("=" * 70)
print("COMPILACIÓN DEL ECOSISTEMA EXTENDIDO DE 4 DLLs LSPDFR")
print("=" * 70)

for m in mods:
    print("\n>>> COMPILANDO MOD: " + m["name"])
    cs_files = collect_cs(m["dir"])
    print("  Archivos .cs encontrados: {}".format(len(cs_files)))
    if len(cs_files) == 0:
        print("  [ERROR] No se encontraron archivos .cs en " + m["dir"])
        all_success = False
        continue

    out_dll = os.path.join(m["dir"], m["dll"])
    cmd = [
        csc_path, "/target:library",
        "/out:" + out_dll,
        "/reference:" + rph_ref,
        "/reference:" + lspdfr_ref,
        "/reference:" + rageui_ref,
        "/reference:System.dll",
        "/reference:System.Core.dll",
        "/reference:System.Drawing.dll",
        "/reference:System.Windows.Forms.dll",
        "/platform:x64", "/optimize+",
    ] + cs_files

    res = subprocess.run(cmd, capture_output=True, text=True)
    if res.returncode == 0:
        dll_size = os.path.getsize(out_dll)
        print("  [OK] Compilación exitosa: {} ({} bytes)".format(m["dll"], dll_size))
        
        # Despliegue automático a plugins/LSPDFR
        dst_dll = os.path.join(plugins_dir, m["dll"])
        shutil.copy2(out_dll, dst_dll)
        print("  [DESPLIEGUE] Copiado a: " + dst_dll)

        ini_path = os.path.join(m["dir"], m["ini"])
        if os.path.exists(ini_path):
            dst_ini = os.path.join(plugins_dir, m["ini"])
            shutil.copy2(ini_path, dst_ini)
            print("  [DESPLIEGUE] INI copiado a: " + dst_ini)
    else:
        all_success = False
        print("  [FALLO] Código de retorno: {}".format(res.returncode))
        lines = (res.stdout + res.stderr).split('\n')
        errors = [l for l in lines if ': error ' in l]
        print("  Errores detectados ({}):".format(len(errors)))
        for err in errors[:10]:
            print("    " + err)

print("\n" + "=" * 70)
if all_success:
    print("*** TODAS LAS 4 DLLs SE COMPILARON Y DESPLEGARON CON ÉXITO ***")
    sys.exit(0)
else:
    print("*** AL MENOS UN MOD TUVO ERRORES DE COMPILACIÓN ***")
    sys.exit(1)
