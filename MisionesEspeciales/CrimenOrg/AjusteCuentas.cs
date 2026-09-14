using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.CrimenOrg
{
    [CalloutInfo("Ajuste de Cuentas entre Bandas Rivales", CalloutProbability.Medium)]
    public class AjusteCuentas : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "¡10-99! Múltiples llamadas al 112: tiroteo masivo con armas automáticas en vía pública.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~r~¡TIROTEO EN CURSO!~w~ Dos facciones de crimen organizado enfrentándose. Solicite refuerzos UPR/GEO.", 6000);

            // Facción 1
            Ped f1_1 = SpawnSuspect(SpawnPoint.Around(6f), "g_m_y_famdnf_01", "WEAPON_MICROSMG", 120, 50);
            Ped f1_2 = SpawnSuspect(SpawnPoint.Around(8f), "g_m_y_famca_01", "WEAPON_ASSAULTRIFLE", 140, 50);
            Ped f1_3 = SpawnSuspect(SpawnPoint.Around(10f), "g_m_y_famfor_01", "WEAPON_PISTOL", 100, 25);

            // Facción 2
            Ped f2_1 = SpawnSuspect(SpawnPoint.Around(16f), "g_m_y_ballaeast_01", "WEAPON_COMPACTRIFLE", 130, 50);
            Ped f2_2 = SpawnSuspect(SpawnPoint.Around(18f), "g_m_y_ballaorig_01", "WEAPON_PUMPSHOTGUN", 120, 25);
            Ped f2_3 = SpawnSuspect(SpawnPoint.Around(20f), "g_m_y_ballasout_01", "WEAPON_MICROSMG", 110, 50);

            // Vehículos cruzados
            SpawnCalloutVehicle(SpawnPoint.Around(7f), "buccaneer", 45f);
            SpawnCalloutVehicle(SpawnPoint.Around(17f), "chino", 220f);

            // Hacer que se ataquen entre sí y luego al jugador
            if (f1_1 != null && f1_1.Exists() && f2_1 != null && f2_1.Exists())
            {
                f1_1.Tasks.FightAgainst(f2_1);
                f2_1.Tasks.FightAgainst(f1_1);
            }
            if (f1_2 != null && f1_2.Exists() && f2_2 != null && f2_2.Exists())
            {
                f1_2.Tasks.FightAgainst(f2_2);
                f2_2.Tasks.FightAgainst(f1_2);
            }
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            // Retargeting contra el jugador si se acerca
            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    if (s.DistanceTo(Game.LocalPlayer.Character) < 25f && !s.IsInCombat)
                    {
                        s.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
            }

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
                CompleteMission("Ajuste de Cuentas Sofocado", "Tiradores neutralizados y armas de guerra incautadas. Servicios de SAMUR y Policía Científica despachados.");
            }
        }
    }
}
