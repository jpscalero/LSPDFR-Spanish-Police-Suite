using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.SEPRONA
{
    [CalloutInfo("SEPRONA: Pesca Furtiva con Redes Prohibidas", CalloutProbability.Medium)]
    public class PescaIlegal : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "SEPRONA / Servicio Marítimo: Embarcación furtiva calando artes de pesca prohibidas en costa.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~g~[SEPRONA]~w~ Acérquese a la orilla/muelle. Intercepte la embarcación y decomise las capturas.", 6000);

            SpawnCalloutVehicle(SpawnPoint, "dinghy", 180f);

            // Pescadores furtivos
            SpawnSuspect(SpawnPoint.Around(2f), "a_m_m_beach_01", "WEAPON_KNIFE", 100, 0);
            SpawnSuspect(SpawnPoint.Around(4f), "a_m_m_beach_02", "", 100, 0);

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
                CompleteMission("Pesca Furtiva Interceptada", "Redes de arrastre ilegales y capturas protegidas decomisadas. Propuesta de sanción muy grave.");
            }
        }
    }
}
