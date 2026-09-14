using System;
using System.Collections.Generic;
using System.IO;
using Rage;

namespace RadioYJusticia.Justice
{
    public class ArrestRecord
    {
        public string Nombre { get; set; }
        public string Dni { get; set; }
        public string Delito { get; set; }
        public string Fecha { get; set; }
        public string ResolucionJudicial { get; set; }

        public ArrestRecord(string nombre, string dni, string delito, string fecha, string resolucionJudicial)
        {
            Nombre = nombre;
            Dni = dni;
            Delito = delito;
            Fecha = fecha;
            ResolucionJudicial = resolucionJudicial;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}", Nombre, Dni, Delito, Fecha, ResolucionJudicial);
        }

        public static ArrestRecord FromString(string line)
        {
            if (string.IsNullOrEmpty(line)) return null;
            string[] parts = line.Split('|');
            if (parts.Length >= 5)
            {
                return new ArrestRecord(parts[0], parts[1], parts[2], parts[3], parts[4]);
            }
            return null;
        }
    }

    public static class ArrestHistoryDB
    {
        private static readonly string DbPath = "plugins/LSPDFR/SPI_Data/historial_arrestos.txt";
        private static readonly List<ArrestRecord> records = new List<ArrestRecord>();
        private static readonly object dbLock = new object();

        public static void Load()
        {
            lock (dbLock)
            {
                records.Clear();
                try
                {
                    string dir = Path.GetDirectoryName(DbPath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    if (!File.Exists(DbPath))
                    {
                        // Sembrar algunos antecedentes policiales iniciales realistas para variedad
                        records.Add(new ArrestRecord("Manuel Heredia Jimenez", "48291045K", "Robo con violencia (Art. 242 CP)", "12/03/2025", "Prision provisional comunicada"));
                        records.Add(new ArrestRecord("Antonio Navarro Cortes", "75918234B", "Trafico de drogas (Art. 368 CP)", "05/06/2025", "Libertad provisional con cargos"));
                        records.Add(new ArrestRecord("David Martinez Silva", "33819402P", "Atentado a la autoridad (Art. 550 CP)", "19/08/2025", "Juicio rapido condenatorio"));
                        records.Add(new ArrestRecord("Javier Gonzalez Martin", "50192847Z", "Alcoholemia positiva penal (Art. 379.2 CP)", "01/11/2025", "Retirada de carnet 1 año y multa"));
                        SaveInternal();
                        Game.LogTrivial("RadioYJusticia: Archivo historial_arrestos.txt inicializado con antecedentes base.");
                        return;
                    }

                    string[] lines = File.ReadAllLines(DbPath);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (!string.IsNullOrEmpty(line.Trim()))
                        {
                            ArrestRecord rec = ArrestRecord.FromString(line);
                            if (rec != null)
                            {
                                records.Add(rec);
                            }
                        }
                    }

                    Game.LogTrivial("RadioYJusticia: Base de datos cargada (" + records.Count.ToString() + " registros).");
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia: Error al cargar historial_arrestos.txt: " + ex.Message);
                }
            }
        }

        public static void Save()
        {
            lock (dbLock)
            {
                SaveInternal();
            }
        }

        private static void SaveInternal()
        {
            try
            {
                string dir = Path.GetDirectoryName(DbPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (StreamWriter sw = new StreamWriter(DbPath, false))
                {
                    for (int i = 0; i < records.Count; i++)
                    {
                        sw.WriteLine(records[i].ToString());
                    }
                }

                Game.LogTrivial("RadioYJusticia: Base de datos guardada correctamente (" + records.Count.ToString() + " registros).");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error al guardar historial_arrestos.txt: " + ex.Message);
            }
        }

        public static void AddRecord(string nombre, string dni, string delito, string resolucionJudicial)
        {
            lock (dbLock)
            {
                string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                ArrestRecord newRec = new ArrestRecord(nombre, dni, delito, fecha, resolucionJudicial);
                records.Add(newRec);
                SaveInternal();
                Game.LogTrivial("RadioYJusticia: Nuevo registro anadido al historial judicial: " + dni + " - " + delito);
            }
        }

        public static bool IsReoffender(string dni)
        {
            lock (dbLock)
            {
                if (string.IsNullOrEmpty(dni)) return false;
                for (int i = 0; i < records.Count; i++)
                {
                    if (string.Equals(records[i].Dni, dni, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static List<ArrestRecord> GetHistory(string dni)
        {
            lock (dbLock)
            {
                List<ArrestRecord> matches = new List<ArrestRecord>();
                if (string.IsNullOrEmpty(dni)) return matches;
                for (int i = 0; i < records.Count; i++)
                {
                    if (string.Equals(records[i].Dni, dni, StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(records[i]);
                    }
                }
                return matches;
            }
        }

        public static List<ArrestRecord> GetAllRecords()
        {
            lock (dbLock)
            {
                return new List<ArrestRecord>(records);
            }
        }
    }
}
