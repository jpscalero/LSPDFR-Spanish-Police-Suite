using System;
using System.IO;
using System.Windows.Forms;
using Rage;

namespace TacticalOps.Core
{
    public static class ConfigManager
    {
        public static bool ModEnabled { get; set; }
        public static Keys SceneKey { get; set; }
        public static Keys MedicalKey { get; set; }
        public static Keys CqbKey { get; set; }
        public static int BleedDamageRate { get; set; }
        public static int MaxProps { get; set; }

        static ConfigManager()
        {
            ModEnabled = true;
            SceneKey = Keys.Home;
            MedicalKey = Keys.End;
            CqbKey = Keys.PageDown;
            BleedDamageRate = 3;
            MaxProps = 30;
        }

        public static void Load()
        {
            string iniPath = "Plugins/LSPDFR/TacticalOps.ini";
            if (!File.Exists(iniPath))
            {
                if (File.Exists("Plugins/TacticalOps.ini"))
                {
                    iniPath = "Plugins/TacticalOps.ini";
                }
                else
                {
                    CreateDefaultConfig(iniPath);
                }
            }

            try
            {
                InitializationFile ini = new InitializationFile(iniPath);
                ini.Create();

                ModEnabled = ini.ReadBoolean("General", "ModEnabled", true);
                BleedDamageRate = ini.ReadInt32("General", "BleedDamageRate", 3);
                MaxProps = ini.ReadInt32("General", "MaxProps", 30);

                SceneKey = ini.ReadEnum<Keys>("Teclas", "SceneKey", Keys.Home);
                MedicalKey = ini.ReadEnum<Keys>("Teclas", "MedicalKey", Keys.End);
                CqbKey = ini.ReadEnum<Keys>("Teclas", "CqbKey", Keys.PageDown);

                Game.LogTrivial("TacticalOps: Configuracion cargada correctamente desde " + iniPath);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps: Error al leer archivo INI: " + ex.Message);
                ModEnabled = true;
                SceneKey = Keys.Home;
                MedicalKey = Keys.End;
                CqbKey = Keys.PageDown;
                BleedDamageRate = 3;
                MaxProps = 30;
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

                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.WriteLine("[General]");
                    sw.WriteLine("// Habilitar o deshabilitar el mod TacticalOps");
                    sw.WriteLine("ModEnabled=true");
                    sw.WriteLine("// Intervalo en segundos para el dano por desangrado (1 HP cada X segundos)");
                    sw.WriteLine("BleedDamageRate=3");
                    sw.WriteLine("// Maximo de objetos de escena desplegables simultaneamente");
                    sw.WriteLine("MaxProps=30");
                    sw.WriteLine("");
                    sw.WriteLine("[Teclas]");
                    sw.WriteLine("// Tecla para abrir el Menu de Gestion de Escenas (Default: Home)");
                    sw.WriteLine("SceneKey=Home");
                    sw.WriteLine("// Tecla para abrir el Menu de Primeros Auxilios IFAK (Default: End)");
                    sw.WriteLine("MedicalKey=End");
                    sw.WriteLine("// Tecla para abrir el Menu de Entrada Tactica CQB (Default: PageDown)");
                    sw.WriteLine("CqbKey=PageDown");
                }
                Game.LogTrivial("TacticalOps: Archivo INI creado por defecto en " + path);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps: Error creando INI por defecto: " + ex.Message);
            }
        }
    }
}
