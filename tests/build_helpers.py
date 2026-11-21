import shutil
import subprocess
import sys
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parents[1]


def run_build(project):
    shell = shutil.which("pwsh") or shutil.which("powershell.exe") or shutil.which("powershell")
    if not shell:
        print("ERROR: PowerShell is required to run the unified build pipeline.", file=sys.stderr)
        return 2

    command = [
        shell,
        "-NoProfile",
        "-ExecutionPolicy",
        "Bypass",
        "-File",
        str(REPO_ROOT / "build.ps1"),
        "-Project",
        project,
        "-NoDeploy",
    ]
    return subprocess.run(command, cwd=REPO_ROOT, check=False).returncode
