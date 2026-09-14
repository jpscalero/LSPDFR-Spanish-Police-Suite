using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.CrimenOrg
{
    [CalloutInfo("Red de Trata de Seres Humanos", CalloutProbability.Medium)]
    public class RedTrata : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Operación UDEV: Localizada nave clandestina utilizada por red de trata de personas.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[UDEV / POLICÍA NACIONAL]~w~ Neutralice a los vigilantes armados y asegure a las víctimas.", 6000);

            // Vigilantes exteriores
            SpawnSuspect(SpawnPoint.Around(5f), "g_m_m_chigoon_01", "WEAPON_PISTOL", 100, 25);
            SpawnSuspect(SpawnPoint.Around(7f), "g_m_m_chigoon_01", "WEAPON_MICROSMG", 110, 50);
            SpawnSuspect(SpawnPoint.Around(9f), "g_m_m_chigoon_02", "WEAPON_PUMPSHOTGUN", 120, 50);

            // Cabecilla de la red
            SpawnSuspect(SpawnPoint.Around(12f), "a_m_m_business_01", "WEAPON_COMBATPISTOL", 100, 100);

            // Víctimas retenidas
            SpawnVictim(SpawnPoint.Around(3f), "a_f_y_bevhills_01");
            SpawnVictim(SpawnPoint.Around(4f), "a_f_y_eastsa_01");
            SpawnVictim(SpawnPoint.Around(6f), "a_f_y_tourist_01");

            // Vehículo de transporte de la red
            SpawnCalloutVehicle(SpawnPoint.Around(10f), "speedo", 90f);

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
                CompleteMission("Red de Trata Desarticulada", "Víctimas puestas bajo protección de los servicios sociales y cabecillas detenidos. Diligencias UDEV abiertas.");
            }
        }
    }
}
