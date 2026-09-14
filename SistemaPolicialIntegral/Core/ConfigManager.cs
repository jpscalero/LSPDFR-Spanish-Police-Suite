using System;
using System.IO;
using Rage;

namespace SistemaPolicialIntegral.Core
{
    public static class ConfigManager
    {
        public static bool ModEnabled { get; set; }
        public static System.Windows.Forms.Keys InterventionKey { get; set; }
        public static System.Windows.Forms.Keys MdtKey { get; set; }
        public static System.Windows.Forms.Keys BackupKey { get; set; }
        public static int AlcoholProbability { get; set; }
        public static int DrugProbability { get; set; }
        public static int TaserNmiDuration { get; set; }
        public static int ArmorPlateHP { get; set; }
        public static int TowResponseTime { get; set; }

        public static bool AmbientServicesEnabled { get; set; }
        public static int AmbientIntervalMin { get; set; }
        public static int AmbientIntervalMax { get; set; }
        public static bool AmbientPoliceEnabled { get; set; }
        public static bool AmbientAmbulanceEnabled { get; set; }
        public static bool AmbientHearseEnabled { get; set; }
        public static bool AmbientFireEnabled { get; set; }
        public static bool AmbientTowEnabled { get; set; }

        public static void Load()
        {
            string filePath = "Plugins/SistemaPolicialIntegral.ini";

            if (!File.Exists(filePath))
            {
                CreateDefaultConfig(filePath);
            }

            InitializationFile ini = new InitializationFile(filePath);

            ModEnabled = ini.ReadBoolean("General", "ModEnabled", true);
            
            InterventionKey = ini.ReadEnum<System.Windows.Forms.Keys>("Teclas", "InterventionKey", System.Windows.Forms.Keys.F9);
            MdtKey = ini.ReadEnum<System.Windows.Forms.Keys>("Teclas", "MdtKey", System.Windows.Forms.Keys.F10);
            BackupKey = ini.ReadEnum<System.Windows.Forms.Keys>("Teclas", "BackupKey", System.Windows.Forms.Keys.Insert);

            AlcoholProbability = ini.ReadInt32("Alcoholimetro", "AlcoholProbability", 40);
            DrugProbability = ini.ReadInt32("Drogas", "DrugProbability", 30);
            
            TaserNmiDuration = ini.ReadInt32("General", "TaserNmiDuration", 5);
            ArmorPlateHP = ini.ReadInt32("Balistica", "ArmorPlateHP", 50);
            TowResponseTime = ini.ReadInt32("Grua", "TowResponseTime", 30);

            AmbientServicesEnabled = ini.ReadBoolean("ServiciosAmbientales", "AmbientServicesEnabled", true);
            AmbientIntervalMin = ini.ReadInt32("ServiciosAmbientales", "IntervaloMinSegundos", 45);
            AmbientIntervalMax = ini.ReadInt32("ServiciosAmbientales", "IntervaloMaxSegundos", 75);
            AmbientPoliceEnabled = ini.ReadBoolean("ServiciosAmbientales", "HabilitarPatrullasPolicia", true);
            AmbientAmbulanceEnabled = ini.ReadBoolean("ServiciosAmbientales", "HabilitarAmbulanciasSAMUR", true);
            AmbientHearseEnabled = ini.ReadBoolean("ServiciosAmbientales", "HabilitarCochesFunebres", true);
            AmbientFireEnabled = ini.ReadBoolean("ServiciosAmbientales", "HabilitarBomberos", true);
            AmbientTowEnabled = ini.ReadBoolean("ServiciosAmbientales", "HabilitarGruasMunicipales", true);

            Game.LogTrivial("SistemaPolicialIntegral: Configuración cargada.");
        }

        private static void CreateDefaultConfig(string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("[General]");
                    sw.WriteLine("ModEnabled=true");
                    sw.WriteLine("TaserNmiDuration=5");
                    sw.WriteLine("");
                    sw.WriteLine("[Teclas]");
                    sw.WriteLine("InterventionKey=F9");
                    sw.WriteLine("MdtKey=F10");
                    sw.WriteLine("BackupKey=Insert");
                    sw.WriteLine("");
                    sw.WriteLine("[Alcoholimetro]");
                    sw.WriteLine("AlcoholProbability=40");
                    sw.WriteLine("");
                    sw.WriteLine("[Drogas]");
                    sw.WriteLine("DrugProbability=30");
                    sw.WriteLine("");
                    sw.WriteLine("[Balistica]");
                    sw.WriteLine("ArmorPlateHP=50");
                    sw.WriteLine("");
                    sw.WriteLine("[Grua]");
                    sw.WriteLine("TowResponseTime=30");
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("SistemaPolicialIntegral: Error creando INI por defecto: " + ex.Message);
            }
        }
    }
}
