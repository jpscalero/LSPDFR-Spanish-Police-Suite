using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.SEPRONA
{
    [CalloutInfo("SEPRONA: Criadero Clandestino y Maltrato Animal", CalloutProbability.Medium)]
    public class MaltratoAnimal : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "SEPRONA: Inspección urgente por denuncia de criadero clandestino y maltrato continuado.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~g~[SEPRONA]~w~ Inspeccione la parcela rural. Rescate a los ejemplares y detenga al responsable.", 6000);

            // Propietario agresivo
            SpawnSuspect(SpawnPoint.Around(3f), "a_m_m_salton_01", "WEAPON_BAT", 100, 0);

            // Animales en malas condiciones (peds caninos)
            SpawnVictim(SpawnPoint.Around(5f), "a_c_shepherd");
            SpawnVictim(SpawnPoint.Around(7f), "a_c_rottweiler");

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
                CompleteMission("Inspección de Maltrato Concluida", "Animales rescatados y trasladados a protectora concertada. Diligencias Art. 337 CP tramitadas.");
            }
        }
    }
}
