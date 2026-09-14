using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;

namespace PeligroExtremoCallouts.Core
{
    /// <summary>
    /// Motor de fisica avanzada de persecucion, chispas en llantas, radiador hirviendo,
    /// perfiles de conduccion y emboscada de desembarco (tactical bailout).
    /// </summary>
    public static class PursuitRealismManager
    {
        private static readonly Random _rnd = new Random();

        public static void ProcessVehiclePursuitTick(Vehicle vehicle, Ped driver, Ped player)
        {
            if (vehicle == null || !vehicle.Exists()) return;

            try
            {
                // 1. Chispas continuas y perdida de adherencia si rueda sin neumatico
                bool tyreBurst = false;
                for (int t = 0; t < 4; t++)
                {
                    if (NativeFunction.Natives.IS_VEHICLE_TYRE_BURST<bool>(vehicle, t, false))
                    {
                        tyreBurst = true;
                        break;
                    }
                }

                if (tyreBurst && vehicle.Speed > 4.0f)
                {
                    TriggerRimSparks(vehicle);
                }

                // 2. Radiador hirviendo vapor blanco si tiene dano severo de motor
                if (vehicle.EngineHealth < 400f && vehicle.IsEngineOn)
                {
                    TriggerRadiatorSteam(vehicle);
                }
            }
            catch { }
        }

        private static void TriggerRimSparks(Vehicle vehicle)
        {
            try
            {
                NativeFunction.Natives.REQUEST_NAMED_PTFX_ASSET("core");
                if (NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>("core"))
                {
                    NativeFunction.Natives.USE_PARTICLE_FX_ASSET("core");
                    NativeFunction.Natives.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY(
                        "wheel_rim_sparks", vehicle, 0f, -1.0f, -0.3f, 0f, 0f, 0f,
                        1.0f, false, false, false
                    );
                }
            }
            catch { }
        }

        private static void TriggerRadiatorSteam(Vehicle vehicle)
        {
            try
            {
                NativeFunction.Natives.REQUEST_NAMED_PTFX_ASSET("core");
                if (NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>("core"))
                {
                    NativeFunction.Natives.USE_PARTICLE_FX_ASSET("core");
                    NativeFunction.Natives.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY(
                        "ent_amb_smoke_scrap", vehicle, 0f, 1.8f, 0.4f, 0f, 0f, 0f,
                        0.7f, false, false, false
                    );
                }
            }
            catch { }
        }

        public static void ExecuteTacticalBailout(Ped suspect, Vehicle vehicle, Ped player, bool aggressiveAmbush)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;

            try
            {
                suspect.BlockPermanentEvents = false;
                suspect.KeepTasks = true;

                if (suspect.IsInAnyVehicle(false))
                {
                    suspect.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut | LeaveVehicleFlags.LeaveDoorOpen);
                }

                if (aggressiveAmbush)
                {
                    // Desembarco con emboscada inmediata tras la puerta del vehiculo
                    GameFiber.StartNew(delegate
                    {
                        GameFiber.Sleep(1200);
                        if (suspect != null && suspect.Exists() && suspect.IsAlive && player != null && player.Exists())
                        {
                            suspect.Tasks.FightAgainst(player);
                            NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(suspect, 2); // Advance
                            NativeFunction.Natives.PLAY_PED_AMBIENT_SPEECH_NATIVE(suspect, "GENERIC_WAR_CRY", "SPEECH_PARAMS_FORCE_SHOUTED");
                        }
                    });
                }
                else
                {
                    // Huida a pie evasiva por callejones
                    GameFiber.StartNew(delegate
                    {
                        GameFiber.Sleep(1000);
                        if (suspect != null && suspect.Exists() && suspect.IsAlive && player != null && player.Exists())
                        {
                            suspect.Tasks.Flee(player, 250f, 60000);
                        }
                    });
                }
            }
            catch { }
        }
    }
}
