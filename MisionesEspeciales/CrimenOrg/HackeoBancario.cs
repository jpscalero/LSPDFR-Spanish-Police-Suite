using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.CrimenOrg
{
    [CalloutInfo("Ciberdelincuencia: Asalto Informático Bancario", CalloutProbability.Medium)]
    public class HackeoBancario : BaseMisionCallout
    {
        private bool dataDrivesRetrieved;

        public HackeoBancario()
        {
            dataDrivesRetrieved = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Alarma Silenciosa en Sucursal Bancaria: Intrusión física y hackeo de terminal central.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[BRIGADA DE CIBERDELINCUENCIA]~w~ Entre en la sucursal. Neutralice a los intrusos y recupere los datos financieros.", 6000);

            // Hacker
            SpawnSuspect(SpawnPoint.Around(2f), "a_m_m_hasjew_01", "", 100, 0);

            // 2 mercenarios de custodia
            SpawnSuspect(SpawnPoint.Around(5f), "s_m_m_marine_01", "WEAPON_SPECIALCARBINE", 140, 100);
            SpawnSuspect(SpawnPoint.Around(7f), "s_m_m_marine_01", "WEAPON_SMG", 120, 50);

            // Vehículo de escape
            SpawnCalloutVehicle(SpawnPoint.Around(12f), "schafter2", 180f);

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

            if (!dataDrivesRetrieved && Game.LocalPlayer.Character.DistanceTo(SpawnPoint) < 4f)
            {
                dataDrivesRetrieved = true;
                Game.DisplayNotification("~g~[EVIDENCIA RECUPERADA]~w~ Portátil y dispositivos USB incautados con transferencias clandestinas.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);
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

            if (!anyAlive && dataDrivesRetrieved)
            {
                CompleteMission("Ciberataque Bancario Neutralizado", "Hackers y escoltas detenidos. Dispositivos remitidos a la Brigada Central de Investigación Tecnológica.");
            }
        }
    }
}
