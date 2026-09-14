using System;
using Rage;
using Rage.Native;
using PoliceLifeSystem.Core;

namespace PoliceLifeSystem.Career
{
    public static class RankManager
    {
        private static int _currentRank;

        public static int CurrentRank
        {
            get { return _currentRank; }
            set { _currentRank = value; }
        }

        static RankManager()
        {
            _currentRank = 0;
        }

        public static string GetRankTitle(int rank)
        {
            switch (rank)
            {
                case 0:
                    return "Policía Alumno";
                case 1:
                    return "Policía";
                case 2:
                    return "Oficial de Policía";
                case 3:
                    return "Subinspector";
                case 4:
                    return "Inspector";
                case 5:
                    return "Inspector Jefe";
                case 6:
                    return "Comisario";
                default:
                    if (rank < 0) return "Policía Alumno";
                    return "Comisario";
            }
        }

        public static int GetRequiredXP(int rank)
        {
            switch (rank)
            {
                case 0:
                    return 0;
                case 1:
                    return 500;
                case 2:
                    return 1500;
                case 3:
                    return 3500;
                case 4:
                    return 7000;
                case 5:
                    return 12000;
                case 6:
                    return 20000;
                default:
                    if (rank < 0) return 0;
                    return 20000;
            }
        }

        public static int GetCurrentRank()
        {
            return _currentRank;
        }

        public static void CheckRankUp()
        {
            int currentXp = XPSystem.CurrentXP;
            int eligibleRank = _currentRank;

            for (int r = 6; r >= 0; r--)
            {
                if (currentXp >= GetRequiredXP(r))
                {
                    eligibleRank = r;
                    break;
                }
            }

            if (eligibleRank > _currentRank)
            {
                _currentRank = eligibleRank;
                string rankTitle = GetRankTitle(_currentRank);

                string message = string.Format("~g~¡ASCENSO POLICIAL!~n~~w~Has sido ascendido a ~b~{0}~w~.", rankTitle);
                Game.DisplayNotification(message);

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "RANK_UP", "HUD_AWARDS", false);

                SaveManager.Save();

                Game.LogTrivial("PoliceLifeSystem: Ascenso concedido -> " + rankTitle);
            }
        }
    }
}
