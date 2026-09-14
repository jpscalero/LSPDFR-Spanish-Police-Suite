using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;

namespace PoliceSuite.Common
{
    public static class CalloutSpawner
    {
        public static float GetGroundZ(Vector3 pos)
        {
            try
            {
                float groundZ = 0.0f;
                bool hasGround = NativeFunction.Natives.GET_GROUND_Z_FOR_3D_COORD<bool>(pos.X, pos.Y, pos.Z + 2.0f, out groundZ, false);
                float waterZ = 0.0f;
                bool hasWater = NativeFunction.Natives.GET_WATER_HEIGHT<bool>(pos.X, pos.Y, pos.Z, out waterZ);

                if (hasGround && hasWater)
                {
                    if (groundZ >= waterZ - 0.2f) return groundZ;
                    return waterZ;
                }

                if (hasGround) return groundZ;
                if (hasWater) return waterZ;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutSpawner.GetGroundZ: " + ex.Message);
            }
            return pos.Z;
        }

        public static Ped SpawnPed(string modelName, Vector3 position, float heading, bool snapToGround)
        {
            try
            {
                Model m = new Model(modelName);
                m.LoadAndWait();
                Vector3 safePos;
                if (snapToGround)
                {
                    float groundZ = GetGroundZ(position);
                    safePos = new Vector3(position.X, position.Y, groundZ);
                }
                else
                {
                    safePos = position;
                }

                Ped p = new Ped(m, safePos, heading);
                p.IsPersistent = true;
                p.BlockPermanentEvents = true;
                return p;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutSpawner.SpawnPed: " + ex.Message);
                return null;
            }
        }

        public static Vehicle SpawnVehicle(string modelName, Vector3 position, float heading, bool snapToGround)
        {
            try
            {
                Model m = new Model(modelName);
                m.LoadAndWait();
                Vector3 safePos;
                if (snapToGround)
                {
                    float groundZ = GetGroundZ(position);
                    safePos = new Vector3(position.X, position.Y, groundZ + 0.2f);
                }
                else
                {
                    safePos = position;
                }

                Vehicle v = new Vehicle(m, safePos, heading);
                v.IsPersistent = true;
                return v;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutSpawner.SpawnVehicle: " + ex.Message);
                return null;
            }
        }

        public static Rage.Object SpawnProp(string modelName, Vector3 position, float heading)
        {
            try
            {
                Model m = new Model(modelName);
                m.LoadAndWait();
                float groundZ = GetGroundZ(position);
                Vector3 safePos = new Vector3(position.X, position.Y, groundZ);

                Rage.Object prop = new Rage.Object(m, safePos, heading);
                prop.IsPersistent = true;
                return prop;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutSpawner.SpawnProp: " + ex.Message);
                return null;
            }
        }

        public static Vehicle SpawnHostileReinforcements(string vehicleModel, string pedModel, WeaponHash weapon, int suspectCount, Vector3 spawnCoord, Vector3 targetCoord, List<Ped> outSuspects, Dictionary<Ped, Blip> outBlips, Action<object> registerCleanup, Action<Ped> initPedRealism, int accuracy)
        {
            try
            {
                Vehicle veh = SpawnVehicle(vehicleModel, spawnCoord, 0f, true);
                if (veh == null) return null;
                if (registerCleanup != null) registerCleanup(veh);

                List<Ped> occupants = new List<Ped>();
                for (int i = 0; i < suspectCount; i++)
                {
                    Ped s = SpawnPed(pedModel, spawnCoord, 0f, true);
                    if (s != null)
                    {
                        s.Inventory.Weapons.Add(weapon).Ammo = -1;
                        CalloutEffects.ConfigurePedCombat(s, accuracy, 100, true, 2);
                        if (initPedRealism != null) initPedRealism(s);

                        Blip b = CalloutBlips.AttachBlip(s, Color.Red, 0.65f);
                        if (registerCleanup != null)
                        {
                            registerCleanup(s);
                            if (b != null) registerCleanup(b);
                        }

                        if (outBlips != null && b != null) outBlips[s] = b;
                        if (outSuspects != null) outSuspects.Add(s);
                        occupants.Add(s);

                        if (i == 0) s.WarpIntoVehicle(veh, -1);
                        else s.WarpIntoVehicle(veh, i - 1);
                    }
                }

                if (occupants.Count > 0 && occupants[0].Exists())
                {
                    Ped driver = occupants[0];
                    driver.BlockPermanentEvents = true;
                    NativeFunction.Natives.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE(
                        driver, veh, targetCoord.X, targetCoord.Y, targetCoord.Z, 30.0f, 786603, 10.0f
                    );

                    GameFiber fiber = GameFiber.StartNew(delegate
                    {
                        try
                        {
                            uint startTime = Game.GameTime;
                            while (veh.Exists() && driver.Exists() && driver.IsAlive && veh.Position.DistanceTo(targetCoord) > 16.0f && (Game.GameTime - startTime < 22000))
                            {
                                if (veh.Speed < 1.0f && Game.GameTime - startTime > 5000) break;
                                GameFiber.Sleep(500);
                            }

                            if (veh.Exists())
                            {
                                NativeFunction.Natives.SET_VEHICLE_HANDBRAKE(veh, true);
                            }

                            for (int i = 0; i < occupants.Count; i++)
                            {
                                Ped occ = occupants[i];
                                if (occ != null && occ.Exists() && occ.IsAlive)
                                {
                                    occ.BlockPermanentEvents = false;
                                    occ.KeepTasks = true;
                                    occ.Tasks.LeaveVehicle(veh, LeaveVehicleFlags.None);
                                }
                            }

                            GameFiber.Sleep(1200);

                            Ped player = Game.LocalPlayer.Character;
                            for (int i = 0; i < occupants.Count; i++)
                            {
                                Ped occ = occupants[i];
                                if (occ != null && occ.Exists() && occ.IsAlive)
                                {
                                    occ.BlockPermanentEvents = false;
                                    occ.KeepTasks = true;
                                    if (player != null && player.Exists())
                                    {
                                        occ.Tasks.FightAgainst(player);
                                    }
                                }
                            }
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("CalloutSpawner.SpawnHostileReinforcements.Fiber: " + ex.Message);
                        }
                    });

                    if (registerCleanup != null) registerCleanup(fiber);
                }

                return veh;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutSpawner.SpawnHostileReinforcements: " + ex.Message);
                return null;
            }
        }
    }
}
