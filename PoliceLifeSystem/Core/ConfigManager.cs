using System;
using System.IO;
using System.Windows.Forms;
using Rage;

namespace PoliceLifeSystem.Core
{
    public static class ConfigManager
    {
        public static bool ModEnabled { get; set; }
        public static Keys MenuKey { get; set; }
        public static Keys DocsKey { get; set; }
        public static int SalaryIntervalMinutes { get; set; }
        public static float XPMultiplier { get; set; }
        public static int CelebrationIntervalDays { get; set; }
        public static int RentCost { get; set; }
        public static int FuelCost { get; set; }

        static ConfigManager()
        {
            ModEnabled = true;
            MenuKey = Keys.Add;
            DocsKey = Keys.Divide;
            SalaryIntervalMinutes = 48;
            XPMultiplier = 1.0f;
            CelebrationIntervalDays = 7;
            RentCost = 600;
            FuelCost = 50;
        }

        public static void Load()
        {
            string filePath = "Plugins/PoliceLifeSystem.ini";

            try
            {
                if (!File.Exists(filePath))
                {
                    CreateDefaultConfig(filePath);
                }

                InitializationFile ini = new InitializationFile(filePath);

                ModEnabled = ini.ReadBoolean("General", "ModEnabled", true);
                SalaryIntervalMinutes = ini.ReadInt32("General", "SalaryIntervalMinutes", 48);

                MenuKey = ini.ReadEnum<Keys>("Teclas", "MenuKey", Keys.Add);
                DocsKey = ini.ReadEnum<Keys>("Teclas", "DocsKey", Keys.Divide);

                XPMultiplier = (float)ini.ReadDouble("XP", "XPMultiplier", 1.0);
                CelebrationIntervalDays = ini.ReadInt32("XP", "CelebrationIntervalDays", 7);
                if (CelebrationIntervalDays < 1) CelebrationIntervalDays = 1;
                if (CelebrationIntervalDays > 365) CelebrationIntervalDays = 365;
                RentCost = ini.ReadInt32("Salarios", "RentCost", 600);
                FuelCost = ini.ReadInt32("Salarios", "FuelCost", 50);

                Game.LogTrivial("PoliceLifeSystem: Configuración cargada correctamente desde " + filePath);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error al cargar configuración: " + ex.Message);
            }
        }

        public static void CreateDefaultConfig(string path)
        {
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("; ============================================================================");
                    sw.WriteLine(";                     POLICE LIFE SYSTEM - CONFIGURACIÓN                      ");
                    sw.WriteLine(";             Mod de Carrera, Economía y Documentación Policial (CNP)         ");
                    sw.WriteLine("; ============================================================================");
                    sw.WriteLine("");
                    sw.WriteLine("[General]");
                    sw.WriteLine("; Habilitar o deshabilitar el plugin");
                    sw.WriteLine("ModEnabled=true");
                    sw.WriteLine("; Intervalo de cobro de nómina en minutos de juego (48 minutos = 1 día en GTA V)");
                    sw.WriteLine("SalaryIntervalMinutes=48");
                    sw.WriteLine("");
                    sw.WriteLine("[Teclas]");
                    sw.WriteLine("; Tecla para abrir el Menú General de Carrera, Tienda y Estadísticas (Por defecto: Add / NumPad +)");
                    sw.WriteLine("MenuKey=Add");
                    sw.WriteLine("; Tecla para abrir el Menú de Documentación Policial y Atestados (Por defecto: Divide / NumPad /)");
                    sw.WriteLine("DocsKey=Divide");
                    sw.WriteLine("");
                    sw.WriteLine("[Salarios]");
                    sw.WriteLine("; Sueldos base mensuales según rango oficial de la Policía Nacional española");
                    sw.WriteLine("BaseSalaryAlumno=1800");
                    sw.WriteLine("BaseSalaryPolicia=2100");
                    sw.WriteLine("BaseSalaryOficial=2400");
                    sw.WriteLine("BaseSalarySubinspector=2800");
                    sw.WriteLine("BaseSalaryInspector=3300");
                    sw.WriteLine("BaseSalaryInspectorJefe=3900");
                    sw.WriteLine("BaseSalaryComisario=4500");
                    sw.WriteLine("; Porcentaje de bonus por nocturnidad (22:00 a 06:00)");
                    sw.WriteLine("NightBonusPercent=30");
                    sw.WriteLine("; Coste del alquiler deducido periódicamente");
                    sw.WriteLine("RentCost=600");
                    sw.WriteLine("; Coste de gasolina por turno de patrulla");
                    sw.WriteLine("FuelCost=50");
                    sw.WriteLine("");
                    sw.WriteLine("[XP]");
                    sw.WriteLine("; Multiplicador de experiencia para ascensos");
                    sw.WriteLine("XPMultiplier=1.0");
                    sw.WriteLine("; Días de juego entre homenajes en la Comisaría Central (1-365)");
                    sw.WriteLine("CelebrationIntervalDays=7");
                }

                Game.LogTrivial("PoliceLifeSystem: Archivo INI creado con éxito en " + path);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error al crear INI por defecto: " + ex.Message);
            }
        }
    }
}
