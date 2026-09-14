using System;
using System.IO;
using Rage;
using PoliceLifeSystem.Career;
using PoliceLifeSystem.Economy;
using PoliceLifeSystem.Stats;

namespace PoliceLifeSystem.Core
{
    public static class SaveManager
    {
        private const string DataDirectory = "plugins/LSPDFR/SPI_Data";
        private const string CareerFile = "plugins/LSPDFR/SPI_Data/carrera.txt";
        private const string EconomyFile = "plugins/LSPDFR/SPI_Data/economia.txt";

        public static void Load()
        {
            try
            {
                if (!Directory.Exists(DataDirectory))
                {
                    Directory.CreateDirectory(DataDirectory);
                }

                // Cargar carrera y estadísticas
                if (File.Exists(CareerFile))
                {
                    string[] lines = File.ReadAllLines(CareerFile);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith(";")) continue;

                        int eqIdx = line.IndexOf('=');
                        if (eqIdx > 0)
                        {
                            string key = line.Substring(0, eqIdx).Trim();
                            string val = line.Substring(eqIdx + 1).Trim();

                            if (key.Equals("CurrentRank", StringComparison.OrdinalIgnoreCase))
                            {
                                int r;
                                if (int.TryParse(val, out r)) RankManager.CurrentRank = r;
                            }
                            else if (key.Equals("CurrentXP", StringComparison.OrdinalIgnoreCase))
                            {
                                int xp;
                                if (int.TryParse(val, out xp)) XPSystem.CurrentXP = xp;
                            }
                            else if (key.Equals("MedalsUnlocked", StringComparison.OrdinalIgnoreCase))
                            {
                                MedalSystem.LoadSerializedMedals(val);
                            }
                            else if (key.Equals("TotalArrests", StringComparison.OrdinalIgnoreCase))
                            {
                                int arr;
                                if (int.TryParse(val, out arr)) PerformanceStats.TotalArrests = arr;
                            }
                            else if (key.Equals("TotalCitations", StringComparison.OrdinalIgnoreCase))
                            {
                                int cit;
                                if (int.TryParse(val, out cit)) PerformanceStats.TotalCitations = cit;
                            }
                            else if (key.Equals("HostilesNeutralized", StringComparison.OrdinalIgnoreCase))
                            {
                                int neu;
                                if (int.TryParse(val, out neu)) PerformanceStats.HostilesNeutralized = neu;
                            }
                            else if (key.Equals("ShiftsCompleted", StringComparison.OrdinalIgnoreCase))
                            {
                                int sh;
                                if (int.TryParse(val, out sh)) PerformanceStats.ShiftsCompleted = sh;
                            }
                        }
                    }
                    Game.LogTrivial("PoliceLifeSystem: Datos de carrera cargados correctamente.");
                }

                // Cargar economía
                if (File.Exists(EconomyFile))
                {
                    string[] lines = File.ReadAllLines(EconomyFile);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith(";")) continue;

                        int eqIdx = line.IndexOf('=');
                        if (eqIdx > 0)
                        {
                            string key = line.Substring(0, eqIdx).Trim();
                            string val = line.Substring(eqIdx + 1).Trim();

                            if (key.Equals("CurrentBalance", StringComparison.OrdinalIgnoreCase))
                            {
                                int bal;
                                if (int.TryParse(val, out bal)) SalaryManager.CurrentBalance = bal;
                            }
                        }
                    }
                    Game.LogTrivial("PoliceLifeSystem: Datos de economía cargados correctamente.");
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error al cargar datos guardados: " + ex.Message);
            }
        }

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(DataDirectory))
                {
                    Directory.CreateDirectory(DataDirectory);
                }

                // Guardar carrera
                using (StreamWriter sw = new StreamWriter(CareerFile, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("; ==============================================");
                    sw.WriteLine(";      POLICE LIFE SYSTEM - DATOS DE CARRERA    ");
                    sw.WriteLine("; ==============================================");
                    sw.WriteLine("CurrentRank=" + RankManager.CurrentRank);
                    sw.WriteLine("CurrentXP=" + XPSystem.CurrentXP);
                    sw.WriteLine("MedalsUnlocked=" + MedalSystem.GetSerializedMedals());
                    sw.WriteLine("TotalArrests=" + PerformanceStats.TotalArrests);
                    sw.WriteLine("TotalCitations=" + PerformanceStats.TotalCitations);
                    sw.WriteLine("HostilesNeutralized=" + PerformanceStats.HostilesNeutralized);
                    sw.WriteLine("ShiftsCompleted=" + PerformanceStats.ShiftsCompleted);
                }

                // Guardar economía
                using (StreamWriter sw = new StreamWriter(EconomyFile, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("; ==============================================");
                    sw.WriteLine(";      POLICE LIFE SYSTEM - DATOS ECONÓMICOS    ");
                    sw.WriteLine("; ==============================================");
                    sw.WriteLine("CurrentBalance=" + SalaryManager.CurrentBalance);
                }

                Game.LogTrivial("PoliceLifeSystem: Datos guardados en disco con éxito.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error al guardar datos: " + ex.Message);
            }
        }
    }
}
