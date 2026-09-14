using System;
using System.IO;
using Rage;

namespace PeligroExtremoCallouts.Core
{
    /// <summary>
    /// Administrador de lectura y cache de parametros de configuracion INI para PeligroExtremoCallouts.
    /// </summary>
    public static class ConfigManager
    {
        private static InitializationFile _ini;
        public static readonly string IniPath = @"plugins\LSPDFR\PeligroExtremoCallouts.ini";

        public static bool ModEnabled { get; private set; }
        public static float GlobalCalloutChanceMultiplier { get; private set; }
        public static bool AggressiveDrivingAI { get; private set; }
        public static bool EnableDriveByShooters { get; private set; }
        public static bool AutoBackup { get; private set; }
        public static int SuspectAccuracy { get; private set; }

        static ConfigManager()
        {
            ModEnabled = true;
            GlobalCalloutChanceMultiplier = 1.0f;
            AggressiveDrivingAI = true;
            EnableDriveByShooters = true;
            AutoBackup = true;
            SuspectAccuracy = 60;
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(IniPath))
                {
                    CreateDefaultIni();
                }

                _ini = new InitializationFile(IniPath);
                _ini.Create();

                ModEnabled = _ini.ReadBoolean("General", "ModEnabled", true);
                GlobalCalloutChanceMultiplier = (float)_ini.ReadDouble("General", "GlobalCalloutChanceMultiplier", 1.0);
                AggressiveDrivingAI = _ini.ReadBoolean("General", "AggressiveDrivingAI", true);
                EnableDriveByShooters = _ini.ReadBoolean("General", "EnableDriveByShooters", true);
                AutoBackup = _ini.ReadBoolean("General", "AutoBackup", true);
                SuspectAccuracy = _ini.ReadInt32("General", "SuspectAccuracy", 60);

                Game.LogTrivial("PeligroExtremoCallouts: Archivo INI cargado correctamente.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts: Error cargando INI: " + ex.Message);
            }
        }

        public static bool IsCalloutEnabled(string calloutIdentifier)
        {
            if (_ini == null) return true;
            return _ini.ReadBoolean(calloutIdentifier, "Enabled", true);
        }

        public static int GetCalloutProbability(string calloutIdentifier, int defaultWeight = 10)
        {
            if (_ini == null) return defaultWeight;
            return _ini.ReadInt32(calloutIdentifier, "ProbabilityWeight", defaultWeight);
        }

        public static int GetMinPoliceUnits(string calloutIdentifier, int defaultValue = 2)
        {
            if (_ini == null) return defaultValue;
            return _ini.ReadInt32(calloutIdentifier, "MinPoliceUnitsRequired", defaultValue);
        }

        public static float GetMinDistance(string calloutIdentifier, float defaultValue = 150f)
        {
            if (_ini == null) return defaultValue;
            return (float)_ini.ReadDouble(calloutIdentifier, "MinDistanceFromPlayer", (double)defaultValue);
        }

        public static int GetCooldownMinutes(string calloutIdentifier, int defaultValue = 15)
        {
            if (_ini == null) return defaultValue;
            return _ini.ReadInt32(calloutIdentifier, "CooldownMinutesBetweenTriggers", defaultValue);
        }

        private static void CreateDefaultIni()
        {
            try
            {
                string dir = Path.GetDirectoryName(IniPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch { }
        }
    }
}
