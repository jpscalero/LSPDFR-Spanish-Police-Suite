using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;

namespace SistemaPolicialIntegral.Backup
{
    public static class UnitSpawner
    {
        public static void SpawnPatrullaZeta()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(85f));
                    Vehicle veh = new Vehicle("police3", spawnPos);
                    if (!veh.Exists()) veh = new Vehicle("police", spawnPos);
                    if (!veh.Exists()) return;

                    veh.IsPersistent = true;
                    veh.IsSirenOn = true;
                    veh.IsSirenSilent = false;

                    Ped driver = new Ped("s_m_y_cop_01", spawnPos, 0f);
                    Ped passenger = new Ped("s_m_y_cop_01", spawnPos, 0f);
                    List<Ped> peds = new List<Ped>();

                    if (driver.Exists())
                    {
                        driver.WarpIntoVehicle(veh, -1);
                        driver.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                        driver.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                        driver.Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 22f, VehicleDrivingFlags.Emergency);
                        peds.Add(driver);
                    }
                    if (passenger.Exists())
                    {
                        passenger.WarpIntoVehicle(veh, 0);
                        passenger.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                        passenger.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(passenger, true);
                        peds.Add(passenger);
                    }

                    BackupLifecycleManager.RegisterUnit(veh, peds, "Patrulla ZETA (091)", Color.FromArgb(0, 100, 255));
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.UnitSpawner.SpawnPatrullaZeta error: " + ex.Message);
                }
            });
        }

        public static void SpawnATGCTrafico()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(90f));
                    Vehicle veh = new Vehicle("pranger", spawnPos);
                    if (!veh.Exists()) veh = new Vehicle("police", spawnPos);
                    if (!veh.Exists()) return;

                    veh.IsPersistent = true;
                    veh.IsSirenOn = true;
                    veh.IsSirenSilent = false;

                    Ped driver = new Ped("s_m_y_hwaycop_01", spawnPos, 0f);
                    Ped passenger = new Ped("s_m_y_hwaycop_01", spawnPos, 0f);
                    List<Ped> peds = new List<Ped>();

                    if (driver.Exists())
                    {
                        driver.WarpIntoVehicle(veh, -1);
                        driver.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                        driver.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                        driver.Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 22f, VehicleDrivingFlags.Emergency);
                        peds.Add(driver);
                    }
                    if (passenger.Exists())
                    {
                        passenger.WarpIntoVehicle(veh, 0);
                        passenger.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                        passenger.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(passenger, true);
                        peds.Add(passenger);
                    }

                    BackupLifecycleManager.RegisterUnit(veh, peds, "Atestados Trafico ATGC", Color.FromArgb(0, 200, 50));
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.UnitSpawner.SpawnATGCTrafico error: " + ex.Message);
                }
            });
        }

        public static void SpawnUPRAntidisturbios()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(95f));
                    Vehicle veh = new Vehicle("riot", spawnPos);
                    if (!veh.Exists()) veh = new Vehicle("fbi2", spawnPos);
                    if (!veh.Exists()) return;

                    veh.IsPersistent = true;
                    veh.IsSirenOn = true;
                    veh.IsSirenSilent = false;

                    List<Ped> peds = new List<Ped>();
                    for (int i = 0; i < 4; i++)
                    {
                        Ped p = new Ped("s_m_y_swat_01", spawnPos, 0f);
                        if (p.Exists())
                        {
                            p.WarpIntoVehicle(veh, i - 1);
                            p.Inventory.GiveNewWeapon(WeaponHash.PumpShotgun, 100, true);
                            p.Armor = 100;
                            p.RelationshipGroup = "COP";
                            NativeFunction.Natives.SET_PED_AS_COP(p, true);
                            peds.Add(p);
                        }
                    }

                    if (peds.Count > 0 && peds[0].Exists())
                    {
                        peds[0].Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 22f, VehicleDrivingFlags.Emergency);
                    }

                    BackupLifecycleManager.RegisterUnit(veh, peds, "UPR Antidisturbios", Color.FromArgb(0, 50, 180));
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.UnitSpawner.SpawnUPRAntidisturbios error: " + ex.Message);
                }
            });
        }

        public static void SpawnGEOTactico()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(95f));
                    Vehicle veh = new Vehicle("fbi2", spawnPos);
                    if (!veh.Exists()) veh = new Vehicle("police3", spawnPos);
                    if (!veh.Exists()) return;

                    veh.IsPersistent = true;
                    veh.IsSirenOn = true;
                    veh.IsSirenSilent = false;

                    List<Ped> peds = new List<Ped>();
                    for (int i = 0; i < 4; i++)
                    {
                        Ped p = new Ped("s_m_y_swat_01", spawnPos, 0f);
                        if (p.Exists())
                        {
                            p.WarpIntoVehicle(veh, i - 1);
                            p.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 200, true);
                            p.Armor = 200;
                            NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(p, 2);
                            p.RelationshipGroup = "COP";
                            NativeFunction.Natives.SET_PED_AS_COP(p, true);
                            peds.Add(p);
                        }
                    }

                    if (peds.Count > 0 && peds[0].Exists())
                    {
                        peds[0].Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 22f, VehicleDrivingFlags.Emergency);
                    }

                    BackupLifecycleManager.RegisterUnit(veh, peds, "GEO / UEI Tactico", Color.FromArgb(180, 0, 0));
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.UnitSpawner.SpawnGEOTactico error: " + ex.Message);
                }
            });
        }

        public static void SpawnK9Canino()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(85f));
                    Vehicle veh = new Vehicle("police", spawnPos);
                    if (!veh.Exists()) return;

                    veh.IsPersistent = true;
                    veh.IsSirenOn = true;
                    veh.IsSirenSilent = false;

                    Ped driver = new Ped("s_m_y_cop_01", spawnPos, 0f);
                    Ped dog = new Ped("a_c_shepherd", spawnPos, 0f);
                    List<Ped> peds = new List<Ped>();

                    if (driver.Exists())
                    {
                        driver.WarpIntoVehicle(veh, -1);
                        driver.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                        driver.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                        driver.Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 22f, VehicleDrivingFlags.Emergency);
                        peds.Add(driver);
                    }
                    if (dog.Exists())
                    {
                        dog.WarpIntoVehicle(veh, 0);
                        dog.RelationshipGroup = "COP";
                        peds.Add(dog);
                    }

                    BackupLifecycleManager.RegisterUnit(veh, peds, "Guias Caninos K9", Color.FromArgb(220, 120, 0));
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.UnitSpawner.SpawnK9Canino error: " + ex.Message);
                }
            });
        }

        public static void SpawnHelicopteroCondor()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    Vector3 spawnPos = Game.LocalPlayer.Character.Position;
                    spawnPos.Z += 100f;

                    Vehicle veh = new Vehicle("polmav", spawnPos);
                    if (!veh.Exists()) return;

                    veh.IsPersistent = true;
                    veh.IsEngineOn = true;
                    NativeFunction.Natives.SET_HELI_BLADES_FULL_SPEED(veh);

                    Ped driver = new Ped("s_m_y_cop_01", spawnPos, 0f);
                    Ped gunner = new Ped("s_m_y_cop_01", spawnPos, 0f);
                    List<Ped> peds = new List<Ped>();

                    if (driver.Exists())
                    {
                        driver.WarpIntoVehicle(veh, -1);
                        driver.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                        NativeFunction.Natives.TASK_HELI_CHASE(driver, Game.LocalPlayer.Character, 0f, 0f, 50f);
                        peds.Add(driver);
                    }
                    if (gunner.Exists())
                    {
                        gunner.WarpIntoVehicle(veh, 0);
                        gunner.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(gunner, true);
                        peds.Add(gunner);
                    }

                    NativeFunction.Natives.SET_VEHICLE_SEARCHLIGHT(veh, true, true);
                    BackupLifecycleManager.RegisterUnit(veh, peds, "Helicoptero Condor", Color.FromArgb(0, 220, 255));
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.UnitSpawner.SpawnHelicopteroCondor error: " + ex.Message);
                }
            });
        }
    }
}
