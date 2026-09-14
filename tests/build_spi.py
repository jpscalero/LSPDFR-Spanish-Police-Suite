import os
import subprocess
import shutil

csc_path = r"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
gta_dir = r"C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V"
rph_ref = r"C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR\rph_sdk\lib\net472\RagePluginHook.dll"
lspdfr_ref = os.path.join(gta_dir, "plugins", "LSPD First Response.dll")
rageui_ref = os.path.join(gta_dir, "RAGENativeUI.dll")
plugins_dir = os.path.join(gta_dir, "plugins", "LSPDFR")

mod_dir = r"C:\Users\jpsca\Desktop\antigravity\mods\LSPDFR\SistemaPolicialIntegral"

def collect_cs_files(folder):
    cs_files = []
    for root, _, files in os.walk(folder):
        for f in files:
            if f.endswith(".cs"):
                cs_files.append(os.path.join(root, f))
    return cs_files

cs_files = collect_cs_files(mod_dir)
mod_out = os.path.join(mod_dir, "SistemaPolicialIntegral.dll")

cmd = [
    csc_path, "/target:library",
    "/out:" + mod_out,
    "/reference:" + rph_ref,
    "/reference:" + lspdfr_ref,
    "/reference:" + rageui_ref,
    "/reference:System.dll",
    "/reference:System.Core.dll",
    "/reference:System.Drawing.dll",
    "/reference:System.Windows.Forms.dll",
    "/platform:x64", "/optimize+",
] + cs_files

print("Building SistemaPolicialIntegral with {} .cs files...".format(len(cs_files)))
print("Files:")
for f in cs_files:
    print("  " + os.path.basename(f))

res = subprocess.run(cmd, capture_output=True, text=True)
print("\n--- COMPILER OUTPUT ---")
print(res.stdout)
if res.stderr:
    print(res.stderr)

if res.returncode == 0:
    print("\nSUCCESS: Compiled to {}".format(mod_out))
    dll_size = os.path.getsize(mod_out)
    print("DLL size: {} bytes ({:.1f} KB)".format(dll_size, dll_size / 1024.0))
    target_dest = os.path.join(plugins_dir, "SistemaPolicialIntegral.dll")
    shutil.copy2(mod_out, target_dest)
    print("DEPLOYED: Copied to {}".format(target_dest))
else:
    print("\nFAILED: Return code {}".format(res.returncode))
    # Count errors vs warnings
    lines = (res.stdout + res.stderr).split('\n')
    errors = [l for l in lines if ': error ' in l]
    warnings = [l for l in lines if ': warning ' in l]
    print("Errors: {}, Warnings: {}".format(len(errors), len(warnings)))
