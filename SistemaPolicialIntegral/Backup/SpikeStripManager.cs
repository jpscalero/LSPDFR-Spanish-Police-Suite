using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Backup
{
    public static class SpikeStripManager
    {
        private static List<Rage.Object> activeSpikes = new List<Rage.Object>();
        private static bool isMonitoring = false;

        public static void DeploySpikeStrips()
        {
            Ped player = Game.LocalPlayer.Character;
            Vector3 forward = player.ForwardVector;
            Vector3 spawnPos = player.Position + (forward * 3.5f);

            Rage.Object spike = new Rage.Object("p_ld_stinger_s", spawnPos);
            if (spike != null && spike.Exists())
            {
                spike.Heading = player.Heading + 90f;
                spike.IsPersistent = true;
                activeSpikes.Add(spike);

                AudioEngine.PlaySirdeeOpen();
                Game.DisplayNotification("~y~[CLAVOS]~w~ Banda de clavos táctica desplegada en la calzada.");

                if (!isMonitoring)
                {
                    isMonitoring = true;
                    GameFiber.StartNew(MonitorSpikes);
                }
            }
        }

        public static void RemoveSpikeStrips()
        {
            foreach (Rage.Object spike in activeSpikes)
            {
                if (spike != null && spike.Exists())
                {
                    spike.Delete();
                }
            }
            activeSpikes.Clear();
            isMonitoring = false;

            AudioEngine.PlayMenuSelect();
            Game.DisplayNotification("~g~[CLAVOS]~w~ Banda de clavos retirada de la vía pública.");
        }

        private static void MonitorSpikes()
        {
            while (isMonitoring && activeSpikes.Count > 0)
            {
                GameFiber.Sleep(100);

                for (int i = 0; i < activeSpikes.Count; i++)
                {
                    Rage.Object spike = activeSpikes[i];
                    if (spike != null && spike.Exists())
                    {
                        Entity[] nearby = World.GetEntities(spike.Position, 4.5f, GetEntitiesFlags.ConsiderAllVehicles);
                        if (nearby != null)
                        {
                            foreach (Entity e in nearby)
                            {
                                Vehicle v = e as Vehicle;
                                if (v != null && v.Exists() && v != Game.LocalPlayer.Character.CurrentVehicle)
                                {
                                    // Reventar todas las ruedas
                                    NativeFunction.Natives.SET_VEHICLE_TYRE_BURST(v, 0, true, 1000.0f);
                                    NativeFunction.Natives.SET_VEHICLE_TYRE_BURST(v, 1, true, 1000.0f);
                                    NativeFunction.Natives.SET_VEHICLE_TYRE_BURST(v, 4, true, 1000.0f);
                                    NativeFunction.Natives.SET_VEHICLE_TYRE_BURST(v, 5, true, 1000.0f);

                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);
                                    Game.DisplayNotification("~r~¡VEHÍCULO PINCHADO!~w~ Las 4 ruedas del objetivo han reventado al cruzar los clavos.");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
