using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Eventos
{
    [CalloutInfo("Seguridad Ciudadana: Altercados en Festival Musical", CalloutProbability.Medium)]
    public class Concierto : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Macroevento Musical: Pelea múltiple y personas intoxicadas en recinto de festival.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[DISPOSITIVO FESTIVAL]~w~ Restablezca el orden en el acceso principal al recinto.", 6000);

            // Intoxicados / asistidos
            SpawnVictim(SpawnPoint.Around(2f), "a_f_y_festival_01");

            // Agresores
            Ped brawler1 = SpawnSuspect(SpawnPoint.Around(4f), "a_m_y_clubcust_01", "WEAPON_BOTTLE", 100, 0);
            Ped brawler2 = SpawnSuspect(SpawnPoint.Around(6f), "a_m_y_clubcust_02", "", 100, 0);

            if (brawler1 != null && brawler1.Exists())
            {
                brawler1.Tasks.FightAgainst(Game.LocalPlayer.Character);
            }
            if (brawler2 != null && brawler2.Exists())
            {
                brawler2.Tasks.FightAgainst(Game.LocalPlayer.Character);
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
                CompleteMission("Incidencias en Festival Resueltas", "Agresores puestos a disposición de seguridad ciudadana y sanitarios del SAMUR asistiendo a afectados.");
            }
        }
    }
}
