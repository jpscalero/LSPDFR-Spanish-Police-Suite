using System;
using System.IO;
using System.Windows.Forms;
using Rage;

namespace RadioYJusticia.Core
{
    public static class ConfigManager
    {
        public static bool ModEnabled { get; set; }
        public static Keys RadioKey { get; set; }
        public static Keys PanicKey { get; set; }
        public static int ChatterIntervalSeconds { get; set; }
        public static int JudicialSpeedMinutes { get; set; }

        static ConfigManager()
        {
            ModEnabled = true;
            RadioKey = Keys.Multiply;
            PanicKey = Keys.Subtract;
            ChatterIntervalSeconds = 90;
            JudicialSpeedMinutes = 2;
        }

        public static void Load()
        {
            string primaryPath = "plugins/LSPDFR/RadioYJusticia.ini";
            string fallbackPath1 = "Plugins/RadioYJusticia.ini";
            string fallbackPath2 = "RadioYJusticia.ini";

            string activePath = primaryPath;

            if (File.Exists(primaryPath))
            {
                activePath = primaryPath;
            }
            else if (File.Exists(fallbackPath1))
            {
                activePath = fallbackPath1;
            }
            else if (File.Exists(fallbackPath2))
            {
                activePath = fallbackPath2;
            }
            else
            {
                CreateDefaultConfig(primaryPath);
                activePath = primaryPath;
            }

            try
            {
                InitializationFile ini = new InitializationFile(activePath);

                ModEnabled = ini.ReadBoolean("General", "ModEnabled", true);
                RadioKey = ini.ReadEnum<Keys>("Teclas", "RadioKey", Keys.Multiply);
                PanicKey = ini.ReadEnum<Keys>("Teclas", "PanicKey", Keys.Subtract);
                ChatterIntervalSeconds = ini.ReadInt32("Radio", "ChatterIntervalSeconds", 90);
                JudicialSpeedMinutes = ini.ReadInt32("TiemposJudiciales", "JudicialSpeedMinutes", 2);

                if (ChatterIntervalSeconds < 10) ChatterIntervalSeconds = 90;
                if (JudicialSpeedMinutes < 1) JudicialSpeedMinutes = 2;

                Game.LogTrivial("RadioYJusticia: Archivo de configuracion cargado desde " + activePath);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error leyendo INI, aplicando valores predeterminados: " + ex.Message);
            }
        }

        private static void CreateDefaultConfig(string path)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.WriteLine("; ==================================================================");
                    sw.WriteLine("; RadioYJusticia - Mod Nativo LSPDFR de Comunicaciones y Justicia");
                    sw.WriteLine("; ==================================================================");
                    sw.WriteLine("");
                    sw.WriteLine("[General]");
                    sw.WriteLine("; Habilita o deshabilita la totalidad del mod");
                    sw.WriteLine("ModEnabled=true");
                    sw.WriteLine("");
                    sw.WriteLine("[Teclas]");
                    sw.WriteLine("; Tecla para desplegar el Menu de Radio SIRDEE / TETRA (Multiply = NumPad *)");
                    sw.WriteLine("RadioKey=Multiply");
                    sw.WriteLine("; Tecla para activar el Boton de Panico 10-99 (Subtract = NumPad -)");
                    sw.WriteLine("PanicKey=Subtract");
                    sw.WriteLine("");
                    sw.WriteLine("[Radio]");
                    sw.WriteLine("; Intervalo promedio en segundos entre transmisiones de radio ambiental");
                    sw.WriteLine("ChatterIntervalSeconds=90");
                    sw.WriteLine("");
                    sw.WriteLine("[TiemposJudiciales]");
                    sw.WriteLine("; Minutos de juego reales para completar el ciclo de custodia de 72 horas");
                    sw.WriteLine("JudicialSpeedMinutes=2");
                }

                Game.LogTrivial("RadioYJusticia: Archivo INI creado exitosamente en " + path);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error al crear INI por defecto: " + ex.Message);
            }
        }
    }
}
