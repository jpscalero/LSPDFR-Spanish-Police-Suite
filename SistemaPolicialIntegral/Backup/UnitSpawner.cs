using System;
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
                Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(80f));
                Vehicle veh = new Vehicle("police3", spawnPos);
                if (!veh.Exists()) veh = new Vehicle("police2", spawnPos);
                if (!veh.Exists()) return;
                
                veh.IsPersistent = true;
                
                Ped driver = new Ped("s_m_y_cop_01", spawnPos, 0f);
                Ped passenger = new Ped("s_m_y_cop_01", spawnPos, 0f);
                
                driver.WarpIntoVehicle(veh, -1);
                passenger.WarpIntoVehicle(veh, 0);
                
                driver.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                passenger.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                
                driver.RelationshipGroup = "COP";
                passenger.RelationshipGroup = "COP";
                
                NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                NativeFunction.Natives.SET_PED_AS_COP(passenger, true);
                
                driver.Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 20f, VehicleDrivingFlags.Emergency);
            });
        }

        public static void SpawnATGCTrafico()
        {
            GameFiber.StartNew(delegate
            {
                Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(80f));
                Vehicle veh = new Vehicle("policet", spawnPos);
                if (!veh.Exists()) veh = new Vehicle("sheriff2", spawnPos);
                if (!veh.Exists()) return;
                
                veh.IsPersistent = true;
                
                Ped driver = new Ped("s_m_y_sheriff_01", spawnPos, 0f);
                Ped passenger = new Ped("s_m_y_sheriff_01", spawnPos, 0f);
                
                driver.WarpIntoVehicle(veh, -1);
                passenger.WarpIntoVehicle(veh, 0);
                
                driver.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                passenger.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                
                driver.RelationshipGroup = "COP";
                passenger.RelationshipGroup = "COP";
                
                NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                NativeFunction.Natives.SET_PED_AS_COP(passenger, true);
                
                driver.Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 20f, VehicleDrivingFlags.Emergency);
            });
        }

        public static void SpawnUPRAntidisturbios()
        {
            GameFiber.StartNew(delegate
            {
                Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(80f));
                Vehicle veh = new Vehicle("riot", spawnPos);
                if (!veh.Exists()) return;
                
                veh.IsPersistent = true;
                
                Ped[] peds = new Ped[4];
                for(int i = 0; i < 4; i++)
                {
                    peds[i] = new Ped("s_m_y_swat_01", spawnPos, 0f);
                    peds[i].WarpIntoVehicle(veh, i - 1);
                    peds[i].Inventory.GiveNewWeapon(WeaponHash.PumpShotgun, 100, true);
                    peds[i].Armor = 100;
                    peds[i].RelationshipGroup = "COP";
                    NativeFunction.Natives.SET_PED_AS_COP(peds[i], true);
                }
                
                peds[0].Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 20f, VehicleDrivingFlags.Emergency);
            });
        }

        public static void SpawnGEOTactico()
        {
            GameFiber.StartNew(delegate
            {
                Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(80f));
                Vehicle veh = new Vehicle("fbi2", spawnPos);
                if (!veh.Exists()) return;
                
                veh.IsPersistent = true;
                
                Ped[] peds = new Ped[4];
                for(int i = 0; i < 4; i++)
                {
                    peds[i] = new Ped("s_m_y_swat_01", spawnPos, 0f);
                    peds[i].WarpIntoVehicle(veh, i - 1);
                    peds[i].Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 200, true);
                    peds[i].Armor = 200;
                    NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(peds[i], 2);
                    peds[i].RelationshipGroup = "COP";
                    NativeFunction.Natives.SET_PED_AS_COP(peds[i], true);
                }
                
                peds[0].Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 20f, VehicleDrivingFlags.Emergency);
            });
        }

        public static void SpawnK9Canino()
        {
            GameFiber.StartNew(delegate
            {
                Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(80f));
                Vehicle veh = new Vehicle("police", spawnPos);
                if (!veh.Exists()) return;
                
                veh.IsPersistent = true;
                
                Ped driver = new Ped("s_m_y_cop_01", spawnPos, 0f);
                Ped dog = new Ped("a_c_shepherd", spawnPos, 0f);
                
                driver.WarpIntoVehicle(veh, -1);
                dog.WarpIntoVehicle(veh, 0);
                
                driver.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                driver.RelationshipGroup = "COP";
                NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                
                dog.RelationshipGroup = "COP";
                
                driver.Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 20f, VehicleDrivingFlags.Emergency);
            });
        }

        public static void SpawnHelicopteroCondor()
        {
            GameFiber.StartNew(delegate
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
                
                driver.WarpIntoVehicle(veh, -1);
                gunner.WarpIntoVehicle(veh, 0);
                
                driver.RelationshipGroup = "COP";
                gunner.RelationshipGroup = "COP";
                NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                NativeFunction.Natives.SET_PED_AS_COP(gunner, true);
                
                NativeFunction.Natives.SET_VEHICLE_SEARCHLIGHT(veh, true, true);
                
                NativeFunction.Natives.TASK_HELI_CHASE(driver, Game.LocalPlayer.Character, 0f, 0f, 50f);
            });
        }
    }
}
