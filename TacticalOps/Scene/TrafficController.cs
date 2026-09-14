using System;
using Rage;
using Rage.Native;

namespace TacticalOps.Scene
{
    public static class TrafficController
    {
        public static bool IsDiversionActive { get; set; }

        static TrafficController()
        {
            IsDiversionActive = false;
        }

        public static void Process()
        {
            try
            {
                Game.LogTrivial("TacticalOps: TrafficController iniciado.");

                while (true)
                {
                    GameFiber.Yield();

                    try
                    {
                        bool shouldManageTraffic = IsDiversionActive || SceneObjectPlacer.HasObjects;

                        if (shouldManageTraffic)
                        {
                            // Reducir densidad de trafico global en el area inmediata
                            NativeFunction.Natives.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME(0.08f);
                            NativeFunction.Natives.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME(0.08f);
                            NativeFunction.Natives.SET_PARKED_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME(0.0f);

                            Ped player = Game.LocalPlayer.Character;
                            if (player != null && player.Exists() && player.IsAlive)
                            {
                                // Obtener vehiculos cercanos dentro del perimetro de 35 metros
                                Entity[] nearbyEntities = World.GetEntities(player.Position, 35.0f, GetEntitiesFlags.ConsiderAllVehicles);
                                if (nearbyEntities != null)
                                {
                                    for (int i = 0; i < nearbyEntities.Length; i++)
                                    {
                                        Vehicle veh = nearbyEntities[i] as Vehicle;
                                        if (veh != null && veh.Exists() && veh != player.CurrentVehicle)
                                        {
                                            Ped driver = veh.Driver;
                                            if (driver != null && driver.Exists() && driver != player && driver.IsAlive)
                                            {
                                                float dist = player.DistanceTo(veh);

                                                // Si el vehiculo entra en la zona de exclusion (25m), frenar y desviar
                                                if (dist < 25.0f)
                                                {
                                                    if (veh.Speed > 2.0f)
                                                    {
                                                        NativeFunction.Natives.SET_VEHICLE_FORWARD_SPEED(veh, Math.Max(0.0f, veh.Speed - 1.5f));
                                                    }

                                                    // Tarea de desaceleracion y parada de emergencia (accion 27 = slow down and stop)
                                                    NativeFunction.Natives.TASK_VEHICLE_TEMP_ACTION(driver, veh, 27, 2500);

                                                    // Si esta muy cerca (12m), forzar al conductor a detenerse totalmente
                                                    if (dist < 12.0f && veh.Speed > 0.1f)
                                                    {
                                                        NativeFunction.Natives.BRING_VEHICLE_TO_HALT(veh, 5.0f, 2, false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            GameFiber.Sleep(300);
                        }
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("TacticalOps.TrafficController.Process loop: " + ex.Message);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException) { }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps.TrafficController.Process fatal: " + ex.Message);
            }
        }
    }
}
