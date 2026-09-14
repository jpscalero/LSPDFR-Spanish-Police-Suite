using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Eventos
{
    [CalloutInfo("Cápsula de Seguridad: Visita de Jefe de Estado", CalloutProbability.Medium)]
    public class VisitaEstado : BaseMisionCallout
    {
        private bool attackersAmbushed;

        public VisitaEstado()
        {
            attackersAmbushed = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Dispositivo de Estado: Comitiva oficial en trayecto hacia el Palacio de Congresos.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[CÁPSULA PRESIDENCIAL]~w~ Asegure la intersección y supervise azoteas en busca de francotiradores.", 6000);

            // Caravana oficial
            SpawnCalloutVehicle(SpawnPoint, "cognoscenti", 0f);
            SpawnCalloutVehicle(SpawnPoint.Around(12f), "fbi2", 0f);

            // Escolta armada
            Ped agent1 = new Ped("s_m_m_armoured_01", SpawnPoint.Around(3f), 0f);
            if (agent1 != null && agent1.Exists())
            {
                agent1.IsPersistent = true;
                agent1.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 300, true);
                Allies.Add(agent1);
            }
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            if (!attackersAmbushed && Game.LocalPlayer.Character.DistanceTo(SpawnPoint) < 15f)
            {
                attackersAmbushed = true;
                Game.DisplayNotification("~r~¡TIRADORES DETECTADOS!~w~ Dos atacantes abren fuego contra el vehículo oficial.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);

                SpawnSuspect(SpawnPoint.Around(25f), "g_m_m_armgoon_01", "WEAPON_SNIPERRIFLE", 140, 50);
                SpawnSuspect(SpawnPoint.Around(22f), "g_m_m_armgoon_01", "WEAPON_ASSAULTRIFLE", 130, 50);

                foreach (Ped s in Suspects)
                {
                    if (s != null && s.Exists() && s.IsAlive)
                    {
                        s.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
                return;
            }

            if (attackersAmbushed)
            {
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
                    CompleteMission("Cápsula de Estado Protegida", "Tiradores neutralizados con éxito y convoy oficial en ruta segura hacia el destino.");
                }
            }
        }
    }
}
