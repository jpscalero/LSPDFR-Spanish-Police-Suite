using System;
using Rage;
using Rage.Native;

namespace PoliceSuite.Common
{
    public static class CalloutEffects
    {
        public static void TriggerExplosion(Vector3 pos, int explosionType, float damageScale)
        {
            try
            {
                NativeFunction.Natives.ADD_EXPLOSION(pos.X, pos.Y, pos.Z, explosionType, damageScale, true, false, 1.0f);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutEffects.TriggerExplosion: " + ex.Message);
            }
        }

        public static void PlayPtfx(string asset, string effect, Vector3 pos, float scale)
        {
            try
            {
                NativeFunction.Natives.REQUEST_NAMED_PTFX_ASSET(asset);
                int timeout = 0;
                while (!NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>(asset) && timeout < 20)
                {
                    GameFiber.Yield();
                    timeout++;
                }

                if (NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>(asset))
                {
                    NativeFunction.Natives.USE_PARTICLE_FX_ASSET_NEXT_CALL(asset);
                    NativeFunction.Natives.START_PARTICLE_FX_NON_LOOPED_AT_COORD(effect, pos.X, pos.Y, pos.Z, 0f, 0f, 0f, scale, false, false, false);
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutEffects.PlayPtfx: " + ex.Message);
            }
        }

        public static void ConfigurePedCombat(Ped ped, int accuracy, int armor, bool canLeaveVehicle, int combatMovement)
        {
            if (ped == null || !ped.Exists()) return;
            try
            {
                ped.Armor = armor;
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 0, true); // BF_CanUseCover
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 1, true); // BF_UseVehicle
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 2, true); // BF_DriveBy
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 3, canLeaveVehicle); // BF_CanLeaveVehicle
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 5, true); // BF_CanFightArmedPedsWhenNotArmed
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 8, true); // BF_BlindFireInCover
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 46, true); // BF_AlwaysFight
                NativeFunction.Natives.SET_PED_COMBAT_ABILITY(ped, 2); // Professional
                NativeFunction.Natives.SET_PED_COMBAT_RANGE(ped, 2); // Far
                NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(ped, combatMovement); // 2 = Advance / Offensive
                NativeFunction.Natives.SET_PED_ACCURACY(ped, accuracy);
                NativeFunction.Natives.SET_PED_SEEING_RANGE(ped, 100.0f);
                NativeFunction.Natives.SET_PED_HEARING_RANGE(ped, 90.0f);
                NativeFunction.Natives.SET_PED_TARGET_LOSS_RESPONSE(ped, 1); // Hunt / Search
                NativeFunction.Natives.SET_PED_FLEE_ATTRIBUTES(ped, 0, false);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutEffects.ConfigurePedCombat: " + ex.Message);
            }
        }

        public static void TriggerVehicleEscape(Ped runner, Vehicle escapeVehicle, Vector3 destination, Action<object> registerCleanup)
        {
            if (runner == null || !runner.Exists() || !runner.IsAlive) return;
            if (escapeVehicle == null || !escapeVehicle.Exists()) return;

            try
            {
                runner.BlockPermanentEvents = true;
                runner.Tasks.ClearImmediately();

                GameFiber fiber = GameFiber.StartNew(delegate
                {
                    try
                    {
                        runner.Tasks.EnterVehicle(escapeVehicle, -1);

                        uint enterStart = Game.GameTime;
                        while (runner.Exists() && runner.IsAlive && !runner.IsInVehicle(escapeVehicle, false) && (Game.GameTime - enterStart < 12000))
                        {
                            GameFiber.Sleep(400);
                        }

                        if (runner.Exists() && runner.IsAlive && runner.IsInVehicle(escapeVehicle, false))
                        {
                            escapeVehicle.IsEngineOn = true;
                            NativeFunction.Natives.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE(
                                runner, escapeVehicle, destination.X, destination.Y, destination.Z, 38.0f, 786603, 15.0f
                            );
                        }
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("CalloutEffects.TriggerVehicleEscape: " + ex.Message);
                    }
                });
                if (registerCleanup != null) registerCleanup(fiber);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutEffects.TriggerVehicleEscape: " + ex.Message);
            }
        }

        public static void TriggerEnvironmentalHazard(Vector3 pos, string fxAsset, string fxName, float radius, int durationMs, Action<object> registerCleanup)
        {
            try
            {
                GameFiber fiber = GameFiber.StartNew(delegate
                {
                    try
                    {
                        uint startTime = Game.GameTime;
                        while (Game.GameTime - startTime < (uint)durationMs)
                        {
                            PlayPtfx(fxAsset, fxName, pos, 2.0f);

                            Ped player = Game.LocalPlayer.Character;
                            if (player != null && player.Exists() && player.Position.DistanceTo(pos) < radius)
                            {
                                NativeFunction.Natives.ANIMPOSTFX_PLAY("CamPushInNeutral", 800, false);
                            }

                            GameFiber.Sleep(1200);
                        }
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("CalloutEffects.TriggerEnvironmentalHazard: " + ex.Message);
                    }
                });
                if (registerCleanup != null) registerCleanup(fiber);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutEffects.TriggerEnvironmentalHazard: " + ex.Message);
            }
        }

        public static void TriggerScriptAlarm(string alarmName, Action<Action> registerCustomCleanup)
        {
            try
            {
                NativeFunction.Natives.START_ALARM(alarmName, true);
                if (registerCustomCleanup != null)
                {
                    registerCustomCleanup(delegate
                    {
                        try
                        {
                            NativeFunction.Natives.STOP_ALARM(alarmName, true);
                        }
                        catch (Exception exStop)
                        {
                            Game.LogTrivial("CalloutEffects.TriggerScriptAlarm.StopAlarm: " + exStop.Message);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutEffects.TriggerScriptAlarm: " + ex.Message);
            }
        }

        public static void ExtractHostageToSafety(Ped hostage, Vector3 safePoint, Action<object> registerCleanup)
        {
            if (hostage == null || !hostage.Exists() || !hostage.IsAlive) return;
            try
            {
                hostage.BlockPermanentEvents = true;
                hostage.Tasks.ClearImmediately();
                hostage.Tasks.FollowNavigationMeshToPosition(safePoint, 0f, 2.5f);

                GameFiber fiber = GameFiber.StartNew(delegate
                {
                    try
                    {
                        uint start = Game.GameTime;
                        while (hostage.Exists() && hostage.IsAlive && hostage.Position.DistanceTo(safePoint) > 3.0f && (Game.GameTime - start < 15000))
                        {
                            GameFiber.Sleep(500);
                        }
                        if (hostage.Exists() && hostage.IsAlive)
                        {
                            hostage.Tasks.Cower(-1);
                        }
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("CalloutEffects.ExtractHostageToSafety: " + ex.Message);
                    }
                });
                if (registerCleanup != null) registerCleanup(fiber);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutEffects.ExtractHostageToSafety: " + ex.Message);
            }
        }
    }
}
