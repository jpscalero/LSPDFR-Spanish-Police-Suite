import json
import re
import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
MODULES = (
    "SistemaPolicialIntegral",
    "PoliceLifeSystem",
    "TacticalOps",
    "RadioYJusticia",
    "MisionesEspeciales",
    "TacticalAndFederalCallouts",
    "PeligroExtremoCallouts",
    "AlternativeCallouts",
)
PERSONAL_PATH = re.compile(r"C:\\Users\\[^\\]+\\Desktop\\antigravity", re.IGNORECASE)


class PortableBuildTests(unittest.TestCase):
    def test_example_configuration_is_valid_and_does_not_assume_a_machine(self):
        config_path = ROOT / "build.config.json.example"
        config = json.loads(config_path.read_text(encoding="utf-8"))
        self.assertEqual(config["outputDir"], "./bin")
        self.assertEqual(config["gtaPath"], "")
        for key in ("rphSdkPath", "lspdfrDllPath", "rageNativeUiPath", "cscPath"):
            self.assertIn(key, config)

    def test_no_personal_absolute_paths_remain_in_build_or_test_files(self):
        candidates = [
            path for path in ROOT.rglob("*")
            if path.is_file() and ".git" not in path.parts
            and path.suffix.lower() in {".ps1", ".py", ".json", ".md", ".yml", ".yaml"}
        ]
        offenders = []
        for path in candidates:
            if PERSONAL_PATH.search(path.read_text(encoding="utf-8", errors="replace")):
                offenders.append(path.relative_to(ROOT).as_posix())
        self.assertEqual(offenders, [])

    def test_build_pipeline_has_portable_resolution_and_safe_validation_modes(self):
        source = (ROOT / "build.ps1").read_text(encoding="utf-8")
        for required in (
            "GTA_V_ROOT",
            "CSC_PATH",
            "Resolve-BuildPath",
            "Get-FirstExistingFile",
            "Assert-ManagedAssembly",
            "AssemblyName]::GetAssemblyName",
            "ValidateOnly",
            "NoDeploy",
            "$Deploy.IsPresent",
            "GetTempPath()",
            "Remove-Item -LiteralPath $artifact -Force",
        ):
            self.assertIn(required, source)
        self.assertNotIn("Remove-Item -Path $OutputDir -Recurse", source)

    def test_all_modules_have_source_files(self):
        for module in MODULES:
            with self.subTest(module=module):
                source_files = list((ROOT / module).rglob("*.cs"))
                self.assertTrue(source_files, f"No C# source files found under {module}")

    def test_module_build_entrypoint_uses_the_shared_pipeline(self):
        wrapper = (ROOT / "PoliceLifeSystem" / "build.ps1").read_text(encoding="utf-8")
        self.assertIn('Join-Path $repoRoot "build.ps1"', wrapper)
        self.assertIn('-Project "PoliceLifeSystem"', wrapper)
        self.assertIn("NoDeploy", wrapper)

    def test_legacy_build_helpers_never_deploy_to_the_game_folder(self):
        for name in ("build_spi.py", "build_extended_suite.py"):
            with self.subTest(script=name):
                source = (ROOT / "tests" / name).read_text(encoding="utf-8")
                self.assertIn("run_build", source)
                self.assertNotIn("copy2", source)
                self.assertNotIn("C:\\", source)

    def test_safe_build_wrappers_do_not_write_game_files(self):
        for name in ("verify_spi.py", "verify_extended_suite.py", "verify_master_all.py"):
            with self.subTest(script=name):
                source = (ROOT / "tests" / name).read_text(encoding="utf-8")
                self.assertNotIn("shutil.copy", source)
                self.assertNotIn("os.makedirs", source)
                self.assertNotIn("C:\\", source)

    def test_spi_dni_validation_rejects_non_ascii_digits(self):
        source = (ROOT / "SistemaPolicialIntegral" / "Census" / "DniModulo23.cs").read_text(encoding="utf-8")
        self.assertIn("TRWAGMYFPDXBNJZSQVHLCKE", source)
        self.assertRegex(source, r"%\s*23")
        self.assertIn("numPart[i] < '0' || numPart[i] > '9'", source)
        self.assertIn("NumberStyles.None, CultureInfo.InvariantCulture", source)


if __name__ == "__main__":
    unittest.main()
