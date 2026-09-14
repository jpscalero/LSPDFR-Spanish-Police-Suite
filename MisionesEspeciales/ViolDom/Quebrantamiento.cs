using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.ViolDom
{
    [CalloutInfo("UFAM: Quebrantamiento Flagrante de Orden de Alejamiento", CalloutProbability.Medium)]
    public class Quebrantamiento : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Alerta UFAM: Individuo con orden judicial de alejamiento en vigor aporreando puerta de la víctima.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[UFAM]~w~ Alto de policía. Detenga al sujeto por delito de quebrantamiento de condena.", 6000);

            // Agresor forzando la puerta
            Ped aggressor = SpawnSuspect(SpawnPoint, "a_m_m_skidrow_01", "WEAPON_CROWBAR", 100, 0);

            // Víctima dentro
            SpawnVictim(SpawnPoint.Around(3f), "a_f_m_skidrow_01");

            if (aggressor != null && aggressor.Exists())
            {
                aggressor.Tasks.FightAgainst(Game.LocalPlayer.Character);
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
                CompleteMission("Quebrantamiento Sofocado", "Detenido por infracción del Art. 468 del Código Penal. Comparecencia inmediata ante el Juzgado de Violencia sobre la Mujer.");
            }
        }
    }
}
