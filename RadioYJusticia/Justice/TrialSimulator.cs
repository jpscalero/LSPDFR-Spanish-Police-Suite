using System;
using Rage;
using Rage.Native;
using RadioYJusticia.Core;

namespace RadioYJusticia.Justice
{
    public static class TrialSimulator
    {
        private static readonly Random random = new Random();

        public static void SimulateOralTrial(DetaineeRecord record, string motivoPrevio)
        {
            if (record == null) return;

            GameFiber.StartNew(delegate
            {
                // Espera de deliberación judicial y vista oral
                GameFiber.Sleep(20000);

                bool esReincidente = ArrestHistoryDB.IsReoffender(record.Dni);

                // Determinación del fallo: 80% condena si hay atestado policial completo, 20% absolución
                int probCondena = esReincidente ? 88 : 78;
                int tirada = random.Next(100);

                // Sonido de resolución judicial
                try
                {
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.TrialSimulator.ExecuteFastTrial (Audio): " + ex.Message);
                }

                if (tirada < probCondena)
                {
                    // Fallo Condenatorio
                    string pena = SentenceManager.CalculateSentence(record.Crime, esReincidente);
                    string tribunal = record.Crime.IndexOf("Homicidio", StringComparison.OrdinalIgnoreCase) >= 0 ? "Audiencia Provincial - Seccion Penal" : "Juzgado de lo Penal nº " + random.Next(1, 15).ToString();

                    string resolucionTexto = "CONDENADO: " + pena;
                    ArrestHistoryDB.AddRecord(record.Name, record.Dni, record.Crime, resolucionTexto);

                    string titulo = "~g~SENTENCIA FIRME (" + tribunal + ")";
                    string subtitulo = "~r~FALLO: CONDENATORIO";
                    string detalle = string.Format("~w~Acusado: ~y~{0}~w~ (DNI {1})~n~Delito: {2}~n~~r~Pena impuesta:~w~ {3}",
                        record.Name, record.Dni, record.Crime, pena);

                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", titulo, subtitulo, detalle);
                    Game.LogTrivial("RadioYJusticia [Juicio Oral]: " + record.Name + " CONDENADO a " + pena);
                }
                else
                {
                    // Fallo Absolutorio
                    string tribunal = "Juzgado de lo Penal nº " + random.Next(1, 15).ToString();
                    string resolucionTexto = "ABSUELTO: Principio in dubio pro reo / falta de conviccion probatoria";
                    ArrestHistoryDB.AddRecord(record.Name, record.Dni, record.Crime, resolucionTexto);

                    string titulo = "~g~SENTENCIA FIRME (" + tribunal + ")";
                    string subtitulo = "~g~FALLO: ABSOLUTORIO";
                    string detalle = string.Format("~w~Acusado: ~y~{0}~w~ (DNI {1})~n~Delito juzgado: {2}~n~~g~Motivo:~w~ Aplicacion del principio 'In Dubio Pro Reo' e insuficiencia de prueba de cargo.",
                        record.Name, record.Dni, record.Crime);

                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", titulo, subtitulo, detalle);
                    Game.LogTrivial("RadioYJusticia [Juicio Oral]: " + record.Name + " ABSUELTO.");
                }
            });
        }
    }
}
