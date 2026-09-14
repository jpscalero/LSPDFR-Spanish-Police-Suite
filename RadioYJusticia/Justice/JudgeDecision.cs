using System;
using Rage;
using Rage.Native;
using RadioYJusticia.Core;

namespace RadioYJusticia.Justice
{
    public static class JudgeDecision
    {
        private static readonly Random random = new Random();

        public static void DecideCase(DetaineeRecord record)
        {
            if (record == null) return;

            GameFiber.StartNew(delegate
            {
                // Breve espera de deliberación del Juez de Instrucción de Guardia
                GameFiber.Sleep(5000);

                int roll = random.Next(100);
                string resolucion = "";
                string tipoAuto = "";
                bool pasaAJuicio = false;

                if (roll < 25)
                {
                    // 25%: Libertad provisional sin cargos por falta de pruebas
                    tipoAuto = "~g~LIBERTAD SIN CARGOS";
                    resolucion = "Auto de sobreseimiento provisional y libertad sin cargos por insuficiencia de indicios probatorios.";
                    pasaAJuicio = false;
                }
                else if (roll < 70)
                {
                    // 45%: Libertad provisional con cargos y medidas cautelares
                    tipoAuto = "~y~LIBERTAD CON CARGOS";
                    resolucion = "Libertad provisional con cargos. Medidas cautelares: retirada de pasaporte, prohibicion de salida del pais y comparecencia apud acta los dias 1 y 15 de cada mes.";
                    pasaAJuicio = false;
                }
                else if (roll < 90)
                {
                    // 20%: Prisión provisional comunicada y sin fianza
                    tipoAuto = "~r~PRISION PROVISIONAL";
                    resolucion = "Auto de prision provisional comunicada y sin fianza (Art. 503 LECrim) por riesgo acreditado de fuga y reiteracion delictiva. Ingreso inmediato en Centro Penitenciario.";
                    pasaAJuicio = true;
                }
                else
                {
                    // 10%: Juicio rápido inmediato
                    tipoAuto = "~b~JUICIO RAPIDO (DILIGENCIAS URGENTES)";
                    resolucion = "Incoacion de Juicio Rapido inmediato (Art. 795 LECrim) ante el Juzgado de Guardia por flagrancia delictiva.";
                    pasaAJuicio = true;
                }

                // Guardar en la base de datos persistente
                ArrestHistoryDB.AddRecord(record.Name, record.Dni, record.Crime, resolucion);

                // Sonido de resolución judicial
                try
                {
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "BASE_JUMP_PASSED", "HUD_AWARDS", false);
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.JudgeDecision.DictateSentence (Audio): " + ex.Message);
                }

                string detalle = string.Format(
                    "~w~Investigado: ~y~{0}~w~ (DNI {1})~n~Delito instruido: {2}~n~~b~Resolucion:~w~ {3}",
                    record.Name, record.Dni, record.Crime, resolucion
                );

                Game.DisplayNotification(
                    "3dtextures",
                    "mpgroundlogo_cops",
                    "~r~AUTO JUDICIAL - JUZGADO DE GUARDIA",
                    tipoAuto,
                    detalle
                );

                Game.LogTrivial("RadioYJusticia [Juez de Guardia]: Auto emitido para " + record.Name + " -> " + resolucion);

                // Si se acordó prisión provisional o juicio rápido, enviar a juicio oral
                if (pasaAJuicio)
                {
                    TrialSimulator.SimulateOralTrial(record, resolucion);
                }
            });
        }
    }
}
