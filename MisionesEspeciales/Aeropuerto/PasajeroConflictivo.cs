using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Aeropuerto
{
    [CalloutInfo("Policía Aeroportuaria: Pasajero Conflictivo y Agresivo", CalloutProbability.Medium)]
    public class PasajeroConflictivo : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Policía Aeroportuaria: Pasajero ebrio y muy violento agrediendo al personal en puerta de embarque.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[POLICÍA AEROPORTUARIA]~w~ Reduzca al pasajero conflictivo sin usar fuerza letal.", 6000);

            // Pasajero agresivo desarmado
            Ped troublemaker = SpawnSuspect(SpawnPoint, "a_m_y_hasjew_01", "", 120, 0);

            // Azafata / personal de tierra
            SpawnVictim(SpawnPoint.Around(2f), "s_f_y_airhostess_01");

            if (troublemaker != null && troublemaker.Exists())
            {
                troublemaker.Tasks.FightAgainst(Game.LocalPlayer.Character);
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
                CompleteMission("Pasajero Reducido con Éxito", "Individuo inmovilizado y trasladado a dependencias policiales del aeropuerto por desórdenes y atentado leve.");
            }
        }
    }
}
