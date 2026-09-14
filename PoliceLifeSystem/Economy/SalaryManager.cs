using System;
using Rage;
using Rage.Native;
using PoliceLifeSystem.Core;
using PoliceLifeSystem.Career;

namespace PoliceLifeSystem.Economy
{
    public static class SalaryManager
    {
        private static int _currentBalance;

        public static int CurrentBalance
        {
            get { return _currentBalance; }
            set { _currentBalance = Math.Max(0, value); }
        }

        static SalaryManager()
        {
            _currentBalance = 2500;
        }

        public static int GetBaseSalary(int rank)
        {
            switch (rank)
            {
                case 0:
                    return 1800; // Policía Alumno
                case 1:
                    return 2100; // Policía
                case 2:
                    return 2400; // Oficial de Policía
                case 3:
                    return 2800; // Subinspector
                case 4:
                    return 3300; // Inspector
                case 5:
                    return 3900; // Inspector Jefe
                case 6:
                    return 4500; // Comisario
                default:
                    if (rank < 0) return 1800;
                    return 4500;
            }
        }

        public static void AddMoney(int amount)
        {
            if (amount <= 0) return;

            _currentBalance += amount;
            SaveManager.Save();

            Game.LogTrivial("PoliceLifeSystem: Ingreso monetario: +" + amount + "€ (Saldo actual: " + _currentBalance + "€)");
        }

        public static bool DeductMoney(int amount)
        {
            if (amount <= 0) return true;

            if (_currentBalance >= amount)
            {
                _currentBalance -= amount;
                SaveManager.Save();
                Game.LogTrivial("PoliceLifeSystem: Cargo monetario: -" + amount + "€ (Saldo actual: " + _currentBalance + "€)");
                return true;
            }

            Game.LogTrivial("PoliceLifeSystem: Fondos insuficientes para cargo de " + amount + "€ (Saldo: " + _currentBalance + "€)");
            return false;
        }

        public static void ProcessSalaryPayment()
        {
            int rank = RankManager.CurrentRank;
            int baseSalary = GetBaseSalary(rank);

            int currentHour = World.TimeOfDay.Hours;
            bool isNight = (currentHour >= 22 || currentHour < 6);

            int nightBonus = 0;
            if (isNight)
            {
                nightBonus = (int)(baseSalary * 0.30f);
            }

            int totalNeto = baseSalary + nightBonus;

            AddMoney(totalNeto);

            string title = "NÓMINA POLICIAL INGRESADA";
            string subtitle = "Dirección General de la Policía";
            string rankName = RankManager.GetRankTitle(rank);

            string message = string.Format(
                "~w~Puesto: ~b~{0}~w~~n~Sueldo base: ~g~{1}€~w~~n~Plus nocturno (+30%): ~y~{2}€~w~~n~Total ingresado: ~g~+{3}€~w~~n~Saldo disponible: ~b~{4}€~w~",
                rankName,
                baseSalary,
                nightBonus,
                totalNeto,
                _currentBalance
            );

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~g~" + title, "~b~" + subtitle, message);

            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", false);

            XPSystem.AddXP(40, "Cumplimiento de jornada reglamentaria");
        }
    }
}
