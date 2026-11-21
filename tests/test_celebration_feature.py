from pathlib import Path
import unittest


ROOT = Path(__file__).resolve().parents[1]


class CelebrationFeatureTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.manager = (ROOT / "PoliceLifeSystem/Career/CelebrationManager.cs").read_text(encoding="utf-8")
        cls.rules = (ROOT / "PoliceLifeSystem/Career/CelebrationScheduleRules.cs").read_text(encoding="utf-8")
        cls.config = (ROOT / "PoliceLifeSystem/Core/ConfigManager.cs").read_text(encoding="utf-8")
        cls.save = (ROOT / "PoliceLifeSystem/Core/SaveManager.cs").read_text(encoding="utf-8")
        cls.build = (ROOT / "build.ps1").read_text(encoding="utf-8")

    def test_interval_is_configurable_and_bounded(self):
        self.assertIn('ini.ReadInt32("XP", "CelebrationIntervalDays", 7)', self.config)
        self.assertIn("CelebrationIntervalDays < 1", self.config)
        self.assertIn("CelebrationIntervalDays > 365", self.config)
        for ini in (
            ROOT / "PoliceLifeSystem/PoliceLifeSystem.ini",
            ROOT / "plugins/LSPDFR/PoliceLifeSystem.ini",
        ):
            self.assertIn("CelebrationIntervalDays=7", ini.read_text(encoding="utf-8"))

    def test_schedule_uses_game_dates_and_handles_clock_changes(self):
        self.assertIn("World.DateTime", self.manager)
        self.assertIn("TimeSpan.FromDays(ClampIntervalDays(intervalDays))", self.rules)
        self.assertIn("currentGameDate < lastCelebration", self.rules)

    def test_invitation_uses_radio_voice_and_notification(self):
        self.assertIn("SpanishVoiceDispatchEngine.SpeakDispatch(radioMessage)", self.manager)
        self.assertIn("Game.DisplayNotification(", self.manager)
        self.assertIn("Game.DisplaySubtitle(", self.manager)

    def test_five_live_officers_are_required_before_zone_or_scene(self):
        self.assertIn("private const int MinimumOfficers = 5", self.manager)
        self.assertIn("CountActiveOfficers() < MinimumOfficers", self.manager)
        self.assertIn("officer.IsAlive", self.manager)
        self.assertIn("officer.BlockPermanentEvents = true", self.manager)
        self.assertIn("officer.IsPersistent = true", self.manager)
        self.assertIn("officerModel.LoadAndWait()", self.manager)

    def test_blue_marker_requires_interaction_and_scene_camera(self):
        self.assertIn("0, 125, 255, 155", self.manager)
        self.assertIn("Game.IsKeyDown(Keys.E)", self.manager)
        self.assertIn('CREATE_CAM<int>("DEFAULT_SCRIPTED_CAMERA", true)', self.manager)
        self.assertIn("RENDER_SCRIPT_CAMS(true", self.manager)
        self.assertIn("RENDER_SCRIPT_CAMS(false", self.manager)
        self.assertIn('"WORLD_HUMAN_CHEERING"', self.manager)

    def test_pending_invitation_and_last_date_are_persisted(self):
        self.assertIn('key.Equals("CelebrationLastDate"', self.save)
        self.assertIn('key.Equals("CelebrationPending"', self.save)
        self.assertIn('sw.WriteLine("CelebrationLastDate="', self.save)
        self.assertIn('sw.WriteLine("CelebrationPending="', self.save)

    def test_module_build_includes_existing_shared_radio_engine(self):
        self.assertRegex(
            self.build,
            r'Name\s*=\s*"PoliceLifeSystem";\s*Dir\s*=\s*"PoliceLifeSystem";\s*NeedsShared\s*=\s*\$true',
        )


if __name__ == "__main__":
    unittest.main(verbosity=2)
