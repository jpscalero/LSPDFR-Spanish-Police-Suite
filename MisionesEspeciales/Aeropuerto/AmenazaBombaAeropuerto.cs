using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Aeropuerto
{
    [CalloutInfo("Policía Aeroportuaria: Amenaza de Bomba en Terminal", CalloutProbability.Medium)]
    public class AmenazaBombaAeropuerto : BaseMisionCallout
    {
        private int evacuationTimer;
        private bool tedaxArrived;

        public AmenazaBombaAeropuerto()
        {
            evacuationTimer = 60;
            tedaxArrived = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Policía Aeroportuaria: Maleta abandonada sospechosa con cables en terminal de salidas.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[POLICÍA AEROPORTUARIA]~w~ Acordone el perímetro de 50 metros. Desaloje a los pasajeros.", 6000);

            // Pasajeros asustados
            SpawnVictim(SpawnPoint.Around(5f), "a_f_y_business_01");
            SpawnVictim(SpawnPoint.Around(7f), "a_m_y_business_01");
            SpawnVictim(SpawnPoint.Around(9f), "a_m_m_tourist_01");

            // Furgón de policía aeroportuaria
            SpawnCalloutVehicle(SpawnPoint.Around(20f), "policet", 0f);
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            if (evacuationTimer > 0)
            {
                evacuationTimer--;
                if (evacuationTimer == 10)
                {
                    Game.DisplayNotification("~y~[TEDAX]~w~ Equipo de desactivación en aproximación final con robot TEODOR.");
                }
            }
            else if (!tedaxArrived)
            {
                tedaxArrived = true;
                // Técnico TEDAX
                Ped tedax = new Ped("s_m_m_doctor_01", SpawnPoint.Around(12f), 0f);
                if (tedax != null && tedax.Exists())
                {
                    tedax.IsPersistent = true;
                    Allies.Add(tedax);
                    tedax.Tasks.FollowNavigationMeshToPosition(SpawnPoint, 0f, 1.0f);
                }
                Game.DisplayNotification("~g~[TEDAX]~w~ Paquete neutralizado mediante cañón de agua disruptor. Falsa alarma de artefacto inerte.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);

                CompleteMission("Amenaza en Terminal Neutralizada", "Paquete sospechoso analizado y destruido de forma segura. Se reanudan las operaciones aéreas.");
            }
        }
    }
}
