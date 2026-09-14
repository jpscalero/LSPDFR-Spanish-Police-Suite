using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.SEPRONA
{
    [CalloutInfo("SEPRONA: Vertido Tóxico en Cuenca Hidrográfica", CalloutProbability.Medium)]
    public class VertidoToxico : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "SEPRONA: Camión cisterna realizando vertidos contaminantes a cauce fluvial.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~g~[SEPRONA]~w~ Alto al vertido. Intercepte a los operarios e identifique las sustancias tóxicas.", 6000);

            SpawnCalloutVehicle(SpawnPoint, "tanker2", 30f);

            // Conductor y operario
            SpawnSuspect(SpawnPoint.Around(3f), "s_m_y_dockwork_01", "WEAPON_WRENCH", 100, 0);
            SpawnSuspect(SpawnPoint.Around(5f), "s_m_m_trucker_01", "", 100, 0);

            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive)
                {
                    if (s.Inventory.Weapons.Count > 0)
                    {
                        s.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                    else
                    {
                        s.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                    }
                }
            }
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            bool anyAlive = false;
            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive)
            {
                CompleteMission("Vertido Ilegal Detenido", "Muestras químicas tomadas para análisis judicial y camión precintado por delito contra el medio ambiente.");
            }
        }
    }
}
