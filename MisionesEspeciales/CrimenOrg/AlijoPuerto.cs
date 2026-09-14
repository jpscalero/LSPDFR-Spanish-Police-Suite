using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.CrimenOrg
{
    [CalloutInfo("Alijo de Droga en el Puerto", CalloutProbability.Medium)]
    public class AlijoPuerto : BaseMisionCallout
    {
        private bool ambushTriggered;

        public AlijoPuerto()
        {
            ambushTriggered = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Operación Puerto: Interceptación de contenedor marítimo sospechoso de transportar cocaína.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[VIGILANCIA ADUANERA]~w~ Acérquese al contenedor señalado para proceder a su apertura e inspección.", 6000);
            SpawnCalloutVehicle(SpawnPoint, "hauler", 45f);
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            if (!ambushTriggered && Game.LocalPlayer.Character.DistanceTo(SpawnPoint) < 15f)
            {
                ambushTriggered = true;
                Game.DisplayNotification("~r~¡EMBOSCADA!~w~ Sicarios armados abren fuego para proteger el cargamento.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);

                // Spawneo de sicarios
                SpawnSuspect(SpawnPoint.Around(12f), "g_m_m_armgoon_01", "WEAPON_ASSAULTRIFLE", 150, 100);
                SpawnSuspect(SpawnPoint.Around(14f), "g_m_m_armgoon_01", "WEAPON_MICROSMG", 120, 50);
                SpawnSuspect(SpawnPoint.Around(16f), "g_m_m_armgoon_01", "WEAPON_PUMPSHOTGUN", 130, 50);
                SpawnSuspect(SpawnPoint.Around(18f), "g_m_y_armgoon_02", "WEAPON_PISTOL50", 100, 25);
                SpawnSuspect(SpawnPoint.Around(20f), "g_m_y_armgoon_02", "WEAPON_ASSAULTRIFLE", 140, 75);

                foreach (Ped s in Suspects)
                {
                    if (s != null && s.Exists() && s.IsAlive)
                    {
                        s.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
                return;
            }

            if (ambushTriggered)
            {
                bool anyAlive = false;
                foreach (Ped suspect in Suspects)
                {
                    if (suspect != null && suspect.Exists() && suspect.IsAlive && !Functions.IsPedArrested(suspect))
                    {
                        anyAlive = true;
                        break;
                    }
                }

                if (!anyAlive)
                {
                    CompleteMission("Alijo Portuario Asegurado", "Se han intervenido 500 kg de clorhidrato de cocaína con un valor de 15 millones de euros.");
                }
            }
        }
    }
}
