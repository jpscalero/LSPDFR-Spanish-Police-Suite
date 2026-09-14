using System;
using Rage;
using PoliceLifeSystem.Core;

namespace PoliceLifeSystem.Career
{
    public static class XPSystem
    {
        private static int _currentXP;

        public static int CurrentXP
        {
            get { return _currentXP; }
            set { _currentXP = Math.Max(0, value); }
        }

        static XPSystem()
        {
            _currentXP = 0;
        }

        public static void AddXP(int amount, string reason)
        {
            if (amount <= 0) return;

            float mult = ConfigManager.XPMultiplier;
            if (mult <= 0f) mult = 1.0f;

            int effectiveAmount = (int)(amount * mult);
            if (effectiveAmount < 1) effectiveAmount = 1;

            _currentXP += effectiveAmount;

            string subtitle = string.Format("~g~+{0} XP~w~: {1}", effectiveAmount, reason);
            Game.DisplaySubtitle(subtitle, 4000);

            RankManager.CheckRankUp();
            MedalSystem.CheckMedals();
            SaveManager.Save();

            Game.LogTrivial("PoliceLifeSystem: XP otorgada: " + effectiveAmount + " (" + reason + ")");
        }

        public static void RemoveXP(int amount, string reason)
        {
            if (amount <= 0) return;

            _currentXP = Math.Max(0, _currentXP - amount);

            string subtitle = string.Format("~r~-{0} XP~w~: {1}", amount, reason);
            Game.DisplaySubtitle(subtitle, 4000);

            SaveManager.Save();

            Game.LogTrivial("PoliceLifeSystem: XP restada: " + amount + " (" + reason + ")");
        }
    }
}
