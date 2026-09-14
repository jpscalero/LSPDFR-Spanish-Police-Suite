using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.SEPRONA
{
    [CalloutInfo("SEPRONA: Tráfico de Especies Exóticas Protegidas (CITES)", CalloutProbability.Medium)]
    public class TraficoEspecies : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "SEPRONA / Aduanas: Recepción de paquete sospechoso con especímenes protegidos CITES.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~g~[SEPRONA]~w~ Proceda al control de aduanas. Inspeccione el vehículo de carga sospechoso.", 6000);

            SpawnCalloutVehicle(SpawnPoint, "bison", 0f);

            // Traficante de animales
            SpawnSuspect(SpawnPoint.Around(3f), "a_m_y_latino_01", "WEAPON_PISTOL", 100, 25);
            SpawnSuspect(SpawnPoint.Around(5f), "a_m_y_latino_01", "", 100, 0);

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
                        s.Tasks.Cower(-1);
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
                CompleteMission("Tráfico de Especies Abortado", "Ejemplares exóticos protegidos puestos a disposición del Centro de Rescate de Fauna. Diligencias CITES.");
            }
        }
    }
}
