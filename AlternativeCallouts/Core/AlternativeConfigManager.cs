using System;
using System.IO;
using Rage;

namespace AlternativeCallouts.Core
{
    public static class AlternativeConfigManager
    {
        private static InitializationFile _ini;
        private static readonly string IniPath = @"plugins/LSPDFR/AlternativeCallouts.ini";

        static AlternativeConfigManager()
        {
            LoadConfig();
        }

        public static void LoadConfig()
        {
            try
            {
                if (!File.Exists(IniPath))
                {
                    CreateDefaultIni();
                }
                _ini = new InitializationFile(IniPath);
                _ini.Create();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts: Error al cargar INI: " + ex.Message);
            }
        }

        public static bool IsCalloutEnabled(string calloutName)
        {
            try
            {
                if (_ini == null) return true;
                return _ini.ReadBoolean(calloutName, "Enabled", true);
            }
            catch { return true; }
        }

        public static int GetForcedOutcome(string calloutName)
        {
            try
            {
                if (_ini == null) return -1;
                return _ini.ReadInt32(calloutName, "ForceOutcome", -1);
            }
            catch { return -1; }
        }

        private static void CreateDefaultIni()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(IniPath))
                {
                    sw.WriteLine("// AlternativeCallouts - Configuracion de Llamadas Alternativas Multiruta");
                    sw.WriteLine("// Cada llamada posee minimo 5 alternativas aleatorias (0=Pacifico, 1=Pelea, 2=FugaPie, 3=FugaVehiculo, 4=GiroCritico)");
                    sw.WriteLine("// ForceOutcome = -1 (Aleatorio 5 ramas), o fijar 0 a 4 para probar una rama especifica.");
                    sw.WriteLine();
                }
            }
            catch { }
        }
    }
}
