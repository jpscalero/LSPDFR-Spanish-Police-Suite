using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.ViolDom
{
    [CalloutInfo("UFAM / GEO: Atrincherado en Domicilio con Arma Larga", CalloutProbability.Medium)]
    public class AmenazasConArma : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "¡MÁXIMA URGENCIA! Agresor atrincherado con escopeta amenazando de muerte a su cónyuge.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~r~¡SUJETO ARMADO ATRINCHERADO!~w~ Mantenga la cobertura. Evalúe negociación o asalto táctico.", 6000);

            // Agresor con escopeta
            Ped gunman = SpawnSuspect(SpawnPoint, "a_m_m_og_boss_01", "WEAPON_SAWNOFFSHOTGUN", 150, 50);

            // Cónyuge retenida
            SpawnVictim(SpawnPoint.Around(2f), "a_f_y_gencaspat_01");

            if (gunman != null && gunman.Exists())
            {
                gunman.Tasks.FightAgainst(Game.LocalPlayer.Character);
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
                CompleteMission("Situación de Atrincheramiento Resuelta", "Amenaza neutralizada y rehén liberada sin lesiones. Escopeta de cañones recortados incautada.");
            }
        }
    }
}
