using System;
using Rage;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Intervention
{
    public static class DrugTest5000
    {
        public static void PerformTest(Ped suspect)
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    if (suspect == null || !suspect.Exists()) return;
                
                PoliceSuite.Common.SpanishVoiceDispatchEngine.AnnounceDrugTest();
                Game.DisplaySubtitle("~b~Agente:~w~ Vamos a tomar una muestra de saliva. Pase el hisopo por la boca.", 4000);
                GameFiber.Sleep(3000);
                
                Game.DisplaySubtitle("~y~Analizando muestra en DrugTest 5000...", 5000);
                GameFiber.Sleep(5000);
                
                Random rand = new Random();
                bool thc = rand.Next(100) < ConfigManager.DrugProbability;
                bool cocaina = rand.Next(100) < (ConfigManager.DrugProbability / 2);
                bool opiaceos = rand.Next(100) < (ConfigManager.DrugProbability / 3);
                bool anfetaminas = rand.Next(100) < (ConfigManager.DrugProbability / 3);
                
                string resThc = thc ? "~r~POSITIVO" : "~g~NEGATIVO";
                string resCocaina = cocaina ? "~r~POSITIVO" : "~g~NEGATIVO";
                string resOpiaceos = opiaceos ? "~r~POSITIVO" : "~g~NEGATIVO";
                string resAnfetaminas = anfetaminas ? "~r~POSITIVO" : "~g~NEGATIVO";
                
                string msg = string.Format("~b~DRÄGER DRUGTEST~n~~w~THC: {0}~n~~w~COCAÍNA: {1}~n~~w~OPIÁCEOS: {2}~n~~w~ANFETAMINAS: {3}", 
                    resThc, resCocaina, resOpiaceos, resAnfetaminas);
                
                Game.DisplayNotification(msg);
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.DrugTest5000 error: " + ex.Message);
                }
            });
        }
    }
}
