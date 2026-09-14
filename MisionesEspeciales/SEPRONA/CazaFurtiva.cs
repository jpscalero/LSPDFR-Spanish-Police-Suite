using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.SEPRONA
{
    [CalloutInfo("SEPRONA: Caza Furtiva en Espacio Natural Protegido", CalloutProbability.Medium)]
    public class CazaFurtiva : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Guardia Civil SEPRONA: Disparos de rifle reportados en coto protegido no autorizado.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~g~[SEPRONA / GUARDIA CIVIL]~w~ Aproxímese sigilosamente. Localice a los cazadores furtivos.", 6000);

            // Cazadores furtivos
            SpawnSuspect(SpawnPoint.Around(10f), "a_m_m_hillbilly_01", "WEAPON_SNIPERRIFLE", 100, 25);
            SpawnSuspect(SpawnPoint.Around(14f), "a_m_m_hillbilly_02", "WEAPON_PUMPSHOTGUN", 110, 0);

            // Todoterreno camuflado en el monte
            SpawnCalloutVehicle(SpawnPoint.Around(18f), "rebel", 0f);

            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive)
                {
                    s.Tasks.FightAgainst(Game.LocalPlayer.Character);
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
                CompleteMission("Caza Furtiva Interceptada", "Armas ilegales y trofeos decomisados por agentes del SEPRONA. Diligencias por delito contra la fauna.");
            }
        }
    }
}
