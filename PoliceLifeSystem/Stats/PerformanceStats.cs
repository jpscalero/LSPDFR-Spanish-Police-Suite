using System;
using Rage;
using PoliceLifeSystem.Career;
using PoliceLifeSystem.Economy;

namespace PoliceLifeSystem.Stats
{
    public static class PerformanceStats
    {
        private static int _totalArrests;
        private static int _totalCitations;
        private static int _hostilesNeutralized;
        private static int _shiftsCompleted;

        public static int TotalArrests
        {
            get { return _totalArrests; }
            set { _totalArrests = Math.Max(0, value); }
        }

        public static int TotalCitations
        {
            get { return _totalCitations; }
            set { _totalCitations = Math.Max(0, value); }
        }

        public static int HostilesNeutralized
        {
            get { return _hostilesNeutralized; }
            set { _hostilesNeutralized = Math.Max(0, value); }
        }

        public static int ShiftsCompleted
        {
            get { return _shiftsCompleted; }
            set { _shiftsCompleted = Math.Max(0, value); }
        }

        static PerformanceStats()
        {
            _totalArrests = 0;
            _totalCitations = 0;
            _hostilesNeutralized = 0;
            _shiftsCompleted = 0;
        }

        public static void AddArrest()
        {
            _totalArrests++;
            MedalSystem.CheckMedals();
        }

        public static void AddCitation()
        {
            _totalCitations++;
            MedalSystem.CheckMedals();
        }

        public static void AddNeutralized()
        {
            _hostilesNeutralized++;
            MedalSystem.CheckMedals();
        }

        public static void AddShift()
        {
            _shiftsCompleted++;
            MedalSystem.CheckMedals();
        }

        public static void ShowStatsNotification()
        {
            string rankTitle = RankManager.GetRankTitle(RankManager.CurrentRank);
            int xp = XPSystem.CurrentXP;
            int balance = SalaryManager.CurrentBalance;

            string content = string.Format(
                "~w~Rango: ~b~{0}~w~~n~XP Actual: ~g~{1}~w~~n~Detenciones: ~y~{2}~w~~n~Denuncias: ~y~{3}~w~~n~Amenazas Neutralizadas: ~r~{4}~w~~n~Turnos Realizados: ~c~{5}~w~~n~Saldo en Cuenta: ~g~{6}€~w~",
                rankTitle,
                xp,
                _totalArrests,
                _totalCitations,
                _hostilesNeutralized,
                _shiftsCompleted,
                balance
            );

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~HOJA DE SERVICIO", "~y~Estadísticas de Carrera", content);
        }

        public static string GetSummaryText()
        {
            return string.Format(
                "Detenciones: {0} | Denuncias: {1} | Neutralizados: {2} | Turnos: {3}",
                _totalArrests,
                _totalCitations,
                _hostilesNeutralized,
                _shiftsCompleted
            );
        }
    }
}
