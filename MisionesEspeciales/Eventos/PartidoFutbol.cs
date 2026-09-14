using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Eventos
{
    [CalloutInfo("Seguridad Ciudadana: Enfrentamiento de Grupos Ultra", CalloutProbability.Medium)]
    public class PartidoFutbol : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Dispositivo Estadio: Pelea multitudinaria entre facciones ultra con bengalas y botellas.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[COORDINADOR DE SEGURIDAD]~w~ Intervenga en los aledaños del estadio. Neutralice a los cabecillas violentos.", 6000);

            // Ultras bando 1
            Ped u1 = SpawnSuspect(SpawnPoint.Around(3f), "a_m_y_golfer_01", "WEAPON_BOTTLE", 110, 0);
            Ped u2 = SpawnSuspect(SpawnPoint.Around(5f), "a_m_y_genstreet_02", "WEAPON_BAT", 120, 0);

            // Ultras bando 2
            Ped u3 = SpawnSuspect(SpawnPoint.Around(8f), "a_m_y_soucent_02", "WEAPON_KNIFE", 110, 0);
            Ped u4 = SpawnSuspect(SpawnPoint.Around(10f), "a_m_y_stbla_02", "WEAPON_CROWBAR", 120, 0);

            // Furgón policial
            SpawnCalloutVehicle(SpawnPoint.Around(16f), "fbi2", 0f);

            if (u1 != null && u1.Exists() && u3 != null && u3.Exists())
            {
                u1.Tasks.FightAgainst(u3);
                u3.Tasks.FightAgainst(u1);
            }
            if (u2 != null && u2.Exists() && u4 != null && u4.Exists())
            {
                u2.Tasks.FightAgainst(u4);
                u4.Tasks.FightAgainst(u2);
            }
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    if (s.DistanceTo(Game.LocalPlayer.Character) < 15f && !s.IsInCombat)
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
                CompleteMission("Rivalidad Ultra Neutralizada", "Ultras violentos inmovilizados y detenidos por riña tumultuaria y desórdenes públicos agravados.");
            }
        }
    }
}
