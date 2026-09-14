using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.ViolDom
{
    [CalloutInfo("UFAM: Violencia Doméstica con Agresión Activa", CalloutProbability.Medium)]
    public class DenunciaMaltrato : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Aviso UFAM: Gritos de auxilio y presunta agresión física en interior de vivienda.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[UFAM / POLICÍA NACIONAL]~w~ Intervenga de urgencia. Proteja a la víctima y reduzca al agresor.", 6000);

            // Víctima aterrorizada
            SpawnVictim(SpawnPoint.Around(2f), "a_f_y_genhot_01");

            // Agresor violento
            Ped aggressor = SpawnSuspect(SpawnPoint.Around(4f), "a_m_m_og_boss_01", "WEAPON_KNIFE", 100, 0);

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
                CompleteMission("Intervención UFAM Concluida", "Agresor inmovilizado y detenido. Víctima asistida y trasladada para valoración forense y orden de protección.");
            }
        }
    }
}
