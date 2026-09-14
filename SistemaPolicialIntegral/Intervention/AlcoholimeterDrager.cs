using System;
using Rage;
using LSPD_First_Response.Mod.API;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Intervention
{
    public static class AlcoholimeterDrager
    {
        public static void PerformTest(Ped suspect)
        {
            GameFiber.StartNew(delegate
            {
                if (suspect == null || !suspect.Exists()) return;
                
                Game.DisplaySubtitle("~b~Agente:~w~ Por favor, sople continuamente hasta que yo le diga.", 4000);
                GameFiber.Sleep(2000);
                
                // Animación (simulada con un sleep para el test)
                if (Game.LocalPlayer.Character.Tasks != null)
                {
                    // Placeholder para animación
                    // Game.LocalPlayer.Character.Tasks.PlayAnimation("mp_arrest_paired", "cop_p2_back_left", 1f, AnimationFlags.None);
                }
                
                for(int i = 0; i < 5; i++)
                {
                    AudioEngine.PlayDragerBeep();
                    GameFiber.Sleep(1000);
                }
                
                Random rand = new Random();
                int chance = rand.Next(100);
                
                double result = 0.0;
                string status = "";
                string color = "";
                
                if (chance < (100 - ConfigManager.AlcoholProbability))
                {
                    // Negativo
                    result = rand.NextDouble() * 0.24;
                    status = "NEGATIVO";
                    color = "~g~";
                }
                else if (chance < (100 - ConfigManager.AlcoholProbability + 25))
                {
                    // Positivo Administrativo
                    result = 0.25 + (rand.NextDouble() * 0.34);
                    status = "POSITIVO (Sanción)";
                    color = "~y~";
                }
                else
                {
                    // Positivo Penal
                    result = 0.61 + (rand.NextDouble() * 0.50);
                    status = "POSITIVO (Delito)";
                    color = "~r~";
                }
                
                string msg = string.Format("~b~DRÄGER ALCOTEST~n~~w~Resultado: {0}{1:0.00} mg/l~n~~w~Estado: {2}", color, result, status);
                Game.DisplayNotification(msg);
            });
        }
    }
}
