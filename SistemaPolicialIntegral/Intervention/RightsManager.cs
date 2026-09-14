using System;
using Rage;

namespace SistemaPolicialIntegral.Intervention
{
    public static class RightsManager
    {
        public static void ReadRights(Ped suspect)
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    if (suspect == null || !suspect.Exists()) return;

                string[] derechos = new string[]
                {
                    "Tiene derecho a guardar silencio, no declarando si no desea...",
                    "Tiene derecho a no declarar contra sí mismo...",
                    "Tiene derecho a designar abogado...",
                    "Tiene derecho a ser asistido por un intérprete...",
                    "Tiene derecho a reconocimiento médico...",
                    "Tiene derecho a comunicar su detención a familiar o persona..."
                };

                Game.DisplayNotification("~b~INFORMACIÓN DE DERECHOS~n~~w~Iniciando lectura (Art. 520 LECrim)");
                PoliceSuite.Common.SpanishVoiceDispatchEngine.AnnounceRights();

                foreach (string derecho in derechos)
                {
                    Game.DisplaySubtitle("~b~Agente:~w~ " + derecho, 4000);
                    GameFiber.Sleep(4000);
                }
                
                Game.DisplaySubtitle("~b~Agente:~w~ ¿Ha entendido usted sus derechos?", 3000);
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.RightsManager error: " + ex.Message);
                }
            });
        }
    }
}
