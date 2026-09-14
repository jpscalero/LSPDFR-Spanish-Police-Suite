using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;

namespace PoliceLifeSystem.Stats
{
    public static class CrimeHeatMap
    {
        private class ClusterCell
        {
            public float SumX;
            public float SumY;
            public float SumZ;
            public int Count;

            public ClusterCell()
            {
                SumX = 0f;
                SumY = 0f;
                SumZ = 0f;
                Count = 0;
            }
        }

        private static List<Blip> _activeBlips;
        private static bool _isHeatMapActive;

        static CrimeHeatMap()
        {
            _activeBlips = new List<Blip>();
            _isHeatMapActive = false;
        }

        public static void ShowHeatMap()
        {
            if (_isHeatMapActive)
            {
                Game.DisplayNotification("~y~El mapa de calor de delincuencia ya se encuentra activo en el radar.");
                return;
            }

            // Limpiar blips residuales si hubiera
            LimpiarBlips();

            List<CrimeTracker.CrimeRecord> crimes = CrimeTracker.RecentCrimes;

            // Si no hay crímenes registrados todavía, crear algunos puntos basados en la patrulla actual
            if (crimes.Count == 0 && Game.LocalPlayer.Character != null && Game.LocalPlayer.Character.Exists())
            {
                Vector3 p = Game.LocalPlayer.Character.Position;
                crimes.Add(new CrimeTracker.CrimeRecord("Atraco a mano armada", new Vector3(p.X + 80f, p.Y + 40f, p.Z), "Detenido", DateTime.Now));
                crimes.Add(new CrimeTracker.CrimeRecord("Tráfico de estupefacientes", new Vector3(p.X + 75f, p.Y + 45f, p.Z), "Detenido", DateTime.Now));
                crimes.Add(new CrimeTracker.CrimeRecord("Robo con fuerza en vehículo", new Vector3(p.X + 90f, p.Y + 30f, p.Z), "Abatido", DateTime.Now));
                crimes.Add(new CrimeTracker.CrimeRecord("Control de alcoholemia positivo", new Vector3(p.X - 120f, p.Y + 80f, p.Z), "Sanción", DateTime.Now));
                crimes.Add(new CrimeTracker.CrimeRecord("Conducción temeraria", new Vector3(p.X - 110f, p.Y + 95f, p.Z), "Sanción", DateTime.Now));
                crimes.Add(new CrimeTracker.CrimeRecord("Infracción de ordenanza municipal", new Vector3(p.X + 200f, p.Y - 150f, p.Z), "Notificado", DateTime.Now));
            }

            // Agrupar en celdas espaciales de 150 metros
            const float cellSize = 150f;
            Dictionary<string, ClusterCell> grid = new Dictionary<string, ClusterCell>();

            for (int i = 0; i < crimes.Count; i++)
            {
                CrimeTracker.CrimeRecord c = crimes[i];
                int cellX = (int)Math.Floor(c.Posicion.X / cellSize);
                int cellY = (int)Math.Floor(c.Posicion.Y / cellSize);
                string key = cellX + ":" + cellY;

                ClusterCell cell;
                if (!grid.TryGetValue(key, out cell))
                {
                    cell = new ClusterCell();
                    grid[key] = cell;
                }

                cell.SumX += c.Posicion.X;
                cell.SumY += c.Posicion.Y;
                cell.SumZ += c.Posicion.Z;
                cell.Count++;
            }

            // Generar blips coloreados según conflictividad
            foreach (KeyValuePair<string, ClusterCell> kvp in grid)
            {
                ClusterCell cell = kvp.Value;
                if (cell.Count <= 0) continue;

                Vector3 centerPos = new Vector3(cell.SumX / cell.Count, cell.SumY / cell.Count, cell.SumZ / cell.Count);

                try
                {
                    Blip blip = new Blip(centerPos);
                    blip.Scale = 1.25f;

                    if (cell.Count >= 5)
                    {
                        blip.Color = Color.Red;
                        blip.Name = string.Format("Zona Roja: Alta Delincuencia ({0} delitos)", cell.Count);
                    }
                    else if (cell.Count >= 2)
                    {
                        blip.Color = Color.Yellow;
                        blip.Name = string.Format("Zona Amarilla: Delincuencia Media ({0} delitos)", cell.Count);
                    }
                    else
                    {
                        blip.Color = Color.LimeGreen;
                        blip.Name = string.Format("Zona Verde: Baja Conflictividad ({0} delito)", cell.Count);
                    }

                    _activeBlips.Add(blip);
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PoliceLifeSystem: Error al crear blip de calor: " + ex.Message);
                }
            }

            _isHeatMapActive = true;

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~MAPA DE CALOR (CNP)", "~y~Delincuencia Activa (30s)", "~r~Rojo~w~: Alta conflictividad~n~~y~Amarillo~w~: Media~n~~g~Verde~w~: Baja conflictividad");
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Radar_Trace_Centres", "DLC_IE_Extraction_Ambience_Sounds", false);

            // Temporizador de 30 segundos para retirar los blips
            GameFiber.StartNew(delegate
            {
                int elapsed = 0;
                while (elapsed < 30000 && _isHeatMapActive)
                {
                    GameFiber.Sleep(1000);
                    elapsed += 1000;
                }

                LimpiarBlips();
                _isHeatMapActive = false;
                Game.DisplayNotification("~b~Mapa de calor de delincuencia desactivado del radar.");
            });
        }

        private static void LimpiarBlips()
        {
            for (int i = 0; i < _activeBlips.Count; i++)
            {
                Blip b = _activeBlips[i];
                if (b != null && b.Exists())
                {
                    b.Delete();
                }
            }
            _activeBlips.Clear();
        }
    }
}
