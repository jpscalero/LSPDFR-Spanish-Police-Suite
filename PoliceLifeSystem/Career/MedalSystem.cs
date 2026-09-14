using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using PoliceLifeSystem.Core;
using PoliceLifeSystem.Stats;

namespace PoliceLifeSystem.Career
{
    public static class MedalSystem
    {
        public class MedalEntry
        {
            private string _nombre;
            private string _descripcion;
            private bool _desbloqueada;

            public string Nombre
            {
                get { return _nombre; }
                set { _nombre = value; }
            }

            public string Descripcion
            {
                get { return _descripcion; }
                set { _descripcion = value; }
            }

            public bool Desbloqueada
            {
                get { return _desbloqueada; }
                set { _desbloqueada = value; }
            }

            public MedalEntry(string nombre, string descripcion, bool desbloqueada)
            {
                this._nombre = nombre;
                this._descripcion = descripcion;
                this._desbloqueada = desbloqueada;
            }
        }

        private static List<MedalEntry> _medals;

        public static List<MedalEntry> Medals
        {
            get { return _medals; }
        }

        static MedalSystem()
        {
            _medals = new List<MedalEntry>();
            _medals.Add(new MedalEntry("Cruz al Mérito Policial con Distintivo Rojo", "Concedida por neutralizar amenazas armadas críticas arriesgando la propia vida.", false));
            _medals.Add(new MedalEntry("Cruz con Distintivo Blanco", "Otorgada por una trayectoria profesional intachable y ascenso al cuerpo de mando.", false));
            _medals.Add(new MedalEntry("Medalla al Valor", "Reconocimiento a intervenciones policiales de alto riesgo en defensa ciudadana.", false));
            _medals.Add(new MedalEntry("Servicio Distinguido", "Otorgada por completar con éxito múltiples turnos de patrulla ejemplares.", false));
            _medals.Add(new MedalEntry("Escudo de Honor de Tráfico", "Distinción por actuaciones destacadas en seguridad vial y control del tráfico rodado.", false));
            _medals.Add(new MedalEntry("Medalla de la Policía Judicial", "Concedida por resolución eficaz de delitos y diligencias penales con detenciones.", false));
        }

        public static void CheckMedals()
        {
            if (PerformanceStats.HostilesNeutralized >= 10)
            {
                UnlockMedal("Cruz al Mérito Policial con Distintivo Rojo");
            }

            if (RankManager.CurrentRank >= 3)
            {
                UnlockMedal("Cruz con Distintivo Blanco");
            }

            if (PerformanceStats.HostilesNeutralized >= 3)
            {
                UnlockMedal("Medalla al Valor");
            }

            if (PerformanceStats.ShiftsCompleted >= 5)
            {
                UnlockMedal("Servicio Distinguido");
            }

            if (PerformanceStats.TotalCitations >= 15)
            {
                UnlockMedal("Escudo de Honor de Tráfico");
            }

            if (PerformanceStats.TotalArrests >= 20)
            {
                UnlockMedal("Medalla de la Policía Judicial");
            }
        }

        public static void UnlockMedal(string name)
        {
            MedalEntry entry = null;
            for (int i = 0; i < _medals.Count; i++)
            {
                if (_medals[i].Nombre.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    entry = _medals[i];
                    break;
                }
            }

            if (entry != null && !entry.Desbloqueada)
            {
                entry.Desbloqueada = true;

                string notification = string.Format("~y~¡CONDECORACIÓN POLICIAL!~n~~w~Has recibido la: ~b~{0}", entry.Nombre);
                Game.DisplayNotification(notification);

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "MEDAL_UP", "HUD_AWARDS", false);

                Game.LogTrivial("PoliceLifeSystem: Condecoración desbloqueada -> " + entry.Nombre);

                SaveManager.Save();
            }
        }

        public static string GetSerializedMedals()
        {
            List<string> unlocked = new List<string>();
            for (int i = 0; i < _medals.Count; i++)
            {
                if (_medals[i].Desbloqueada)
                {
                    unlocked.Add(_medals[i].Nombre);
                }
            }
            return string.Join(";", unlocked.ToArray());
        }

        public static void LoadSerializedMedals(string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            string[] names = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < names.Length; i++)
            {
                string n = names[i].Trim();
                for (int j = 0; j < _medals.Count; j++)
                {
                    if (_medals[j].Nombre.Equals(n, StringComparison.OrdinalIgnoreCase))
                    {
                        _medals[j].Desbloqueada = true;
                    }
                }
            }
        }
    }
}
