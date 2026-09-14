using System;
using System.IO;
using System.Collections.Generic;
using Rage;
using PoliceLifeSystem.Core;
using PoliceLifeSystem.Career;

namespace PoliceLifeSystem.Stats
{
    public static class CrimeTracker
    {
        public class CrimeRecord
        {
            private string _tipo;
            private Vector3 _posicion;
            private string _resultado;
            private DateTime _fecha;

            public string Tipo
            {
                get { return _tipo; }
                set { _tipo = value; }
            }

            public Vector3 Posicion
            {
                get { return _posicion; }
                set { _posicion = value; }
            }

            public string Resultado
            {
                get { return _resultado; }
                set { _resultado = value; }
            }

            public DateTime Fecha
            {
                get { return _fecha; }
                set { _fecha = value; }
            }

            public CrimeRecord(string tipo, Vector3 pos, string resultado, DateTime fecha)
            {
                this._tipo = tipo;
                this._posicion = pos;
                this._resultado = resultado;
                this._fecha = fecha;
            }
        }

        private const string StatsFile = "plugins/LSPDFR/SPI_Data/estadisticas.txt";
        private static List<CrimeRecord> _recentCrimes;

        public static List<CrimeRecord> RecentCrimes
        {
            get { return _recentCrimes; }
        }

        static CrimeTracker()
        {
            _recentCrimes = new List<CrimeRecord>();
        }

        public static void RecordCrime(string tipo, Vector3 pos, string resultado)
        {
            DateTime now = DateTime.Now;
            CrimeRecord record = new CrimeRecord(tipo, pos, resultado, now);
            _recentCrimes.Add(record);

            try
            {
                string dir = Path.GetDirectoryName(StatsFile);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string line = string.Format(
                    "[{0:yyyy-MM-dd HH:mm:ss}] Tipo: {1} | Pos: ({2:F1}, {3:F1}, {4:F1}) | Resultado: {5}",
                    now,
                    tipo,
                    pos.X,
                    pos.Y,
                    pos.Z,
                    resultado
                );

                using (StreamWriter sw = new StreamWriter(StatsFile, true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(line);
                }

                // Actualizar estadísticas según el resultado
                string resLower = resultado.ToLower();
                if (resLower.Contains("deten") || resLower.Contains("arrest"))
                {
                    PerformanceStats.AddArrest();
                }
                else if (resLower.Contains("multa") || resLower.Contains("denuncia") || resLower.Contains("sanci"))
                {
                    PerformanceStats.AddCitation();
                }
                else if (resLower.Contains("abatido") || resLower.Contains("neutralizado"))
                {
                    PerformanceStats.AddNeutralized();
                }

                XPSystem.AddXP(50, "Resolución de intervención criminal");
                SaveManager.Save();

                Game.LogTrivial("PoliceLifeSystem: Delito registrado: " + tipo + " en (" + pos.X + "," + pos.Y + ")");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error al registrar delito en fichero: " + ex.Message);
            }
        }

        public static void LoadHistory()
        {
            try
            {
                if (!File.Exists(StatsFile)) return;

                string[] lines = File.ReadAllLines(StatsFile);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrEmpty(line) || !line.Contains("Pos:")) continue;

                    // Formato: [2026-09-14 10:30:00] Tipo: Robo | Pos: (120.5, -450.2, 35.1) | Resultado: Detenido
                    int pStart = line.IndexOf("Pos: (");
                    if (pStart > 0)
                    {
                        int pEnd = line.IndexOf(')', pStart);
                        if (pEnd > pStart)
                        {
                            string coordsStr = line.Substring(pStart + 6, pEnd - (pStart + 6));
                            string[] parts = coordsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 3)
                            {
                                float x, y, z;
                                if (float.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x) &&
                                    float.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y) &&
                                    float.TryParse(parts[2].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out z))
                                {
                                    Vector3 pos = new Vector3(x, y, z);
                                    _recentCrimes.Add(new CrimeRecord("Incidente histórico", pos, "Registrado", DateTime.Now));
                                }
                            }
                        }
                    }
                }

                Game.LogTrivial("PoliceLifeSystem: Historial de delitos cargado en memoria (" + _recentCrimes.Count + " registros).");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error al cargar historial de delitos: " + ex.Message);
            }
        }
    }
}
