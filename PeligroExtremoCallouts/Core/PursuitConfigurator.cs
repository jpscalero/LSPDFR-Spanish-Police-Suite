using System;
using Rage;
using Rage.Native;

namespace PeligroExtremoCallouts.Core
{
    /// <summary>
    /// Configurador de parametros balisticos, estilo de conduccion evasiva,
    /// refuerzo vehicular y comportamiento de fuego en marcha (drive-by) para persecuciones extremas.
    /// </summary>
    public static class PursuitConfigurator
    {
        public static void ConfigureAggressiveDriver(Ped driver, Vehicle vehicle, float maxSpeed = 45f, int drivingStyle = 786603)
        {
            if (driver == null || !driver.Exists() || vehicle == null || !vehicle.Exists()) return;

            try
            {
                driver.BlockPermanentEvents = false;
                driver.KeepTasks = true;
                NativeFunction.Natives.SET_DRIVER_AGGRESSIVENESS(driver, 1.0f);
                NativeFunction.Natives.SET_DRIVER_ABILITY(driver, 1.0f);
                NativeFunction.Natives.SET_DRIVE_TASK_CRUISE_SPEED(driver, maxSpeed);
                NativeFunction.Natives.SET_DRIVE_TASK_DRIVING_STYLE(driver, drivingStyle);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ConfigureAggressiveDriver: " + ex.Message);
            }
        }

        public static void ConfigureWrongWayDriver(Ped driver, Vehicle vehicle, float maxSpeed = 50f)
        {
            if (driver == null || !driver.Exists() || vehicle == null || !vehicle.Exists()) return;

            try
            {
                driver.BlockPermanentEvents = false;
                driver.KeepTasks = true;
                NativeFunction.Natives.SET_DRIVER_AGGRESSIVENESS(driver, 1.0f);
                NativeFunction.Natives.SET_DRIVER_ABILITY(driver, 1.0f);
                NativeFunction.Natives.SET_DRIVE_TASK_CRUISE_SPEED(driver, maxSpeed);
                // DrivingStyle: 786603 | 262144 (ignora sentido de la circulacion y va en contramano)
                NativeFunction.Natives.SET_DRIVE_TASK_DRIVING_STYLE(driver, 786603 | 262144);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ConfigureWrongWayDriver: " + ex.Message);
            }
        }

        public static void ConfigureEscortDriver(Ped driver, Vehicle escortVeh, Vehicle leadVeh, float speed = 40f)
        {
            if (driver == null || !driver.Exists() || escortVeh == null || !escortVeh.Exists() || leadVeh == null || !leadVeh.Exists()) return;

            try
            {
                driver.BlockPermanentEvents = false;
                driver.KeepTasks = true;
                NativeFunction.Natives.SET_DRIVER_AGGRESSIVENESS(driver, 1.0f);
                NativeFunction.Natives.SET_DRIVER_ABILITY(driver, 1.0f);
                NativeFunction.Natives.TASK_VEHICLE_ESCORT(driver, escortVeh, leadVeh, -1, speed, 786603, 10f, 0, 15f);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ConfigureEscortDriver: " + ex.Message);
            }
        }

        public static void ConfigureRammingDriver(Ped driver, Vehicle vehicle, Ped targetPed, float speed = 45f)
        {
            if (driver == null || !driver.Exists() || vehicle == null || !vehicle.Exists() || targetPed == null || !targetPed.Exists()) return;

            try
            {
                driver.BlockPermanentEvents = false;
                driver.KeepTasks = true;
                NativeFunction.Natives.SET_DRIVER_AGGRESSIVENESS(driver, 1.0f);
                NativeFunction.Natives.SET_DRIVER_ABILITY(driver, 1.0f);
                // Modo 6 en TASK_VEHICLE_MISSION_PED_TARGET corresponde a embestida violenta (Ramming/Follow)
                NativeFunction.Natives.TASK_VEHICLE_MISSION_PED_TARGET(driver, vehicle, targetPed, 6, speed, 786603, 5f, 5f, true);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ConfigureRammingDriver: " + ex.Message);
            }
        }

        public static void ConfigureBoatDriver(Ped driver, Vehicle boat, Ped targetPlayer)
        {
            if (driver == null || !driver.Exists() || boat == null || !boat.Exists()) return;

            try
            {
                driver.BlockPermanentEvents = false;
                driver.KeepTasks = true;
                if (targetPlayer != null && targetPlayer.Exists())
                {
                    NativeFunction.Natives.TASK_BOAT_MISSION(driver, boat, 0, targetPlayer, 0f, 0f, 0f, 4, 38f, 786603, 15f, 7);
                }
                else
                {
                    NativeFunction.Natives.TASK_BOAT_MISSION(driver, boat, 0, 0, 0f, 0f, 0f, 1, 35f, 786603, 10f, 7);
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ConfigureBoatDriver: " + ex.Message);
            }
        }

        public static void ConfigurePlanePilot(Ped pilot, Vehicle plane, Vector3 targetCoord)
        {
            if (pilot == null || !pilot.Exists() || plane == null || !plane.Exists()) return;

            try
            {
                pilot.BlockPermanentEvents = false;
                pilot.KeepTasks = true;
                NativeFunction.Natives.TASK_PLANE_MISSION(pilot, plane, 0, 0, targetCoord.X, targetCoord.Y, targetCoord.Z + 150f, 4, 55f, 0f, 0f, 60f, 25f);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ConfigurePlanePilot: " + ex.Message);
            }
        }

        public static void ConfigureHeliPilot(Ped pilot, Vehicle heli, Ped targetPed)
        {
            if (pilot == null || !pilot.Exists() || heli == null || !heli.Exists() || targetPed == null || !targetPed.Exists()) return;

            try
            {
                pilot.BlockPermanentEvents = false;
                pilot.KeepTasks = true;
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(pilot, 1, true); // BF_CanUseVehicles
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(pilot, 2, true); // BF_CanDoDrivebys
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(pilot, 3, false); // BF_LeaveVehicles = false
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(pilot, 20, true); // BF_VehicleCombatUseWeapons
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(pilot, 46, true); // BF_AlwaysFight
                NativeFunction.Natives.SET_PED_COMBAT_ABILITY(pilot, 2); // Professional
                NativeFunction.Natives.TASK_HELI_MISSION(pilot, heli, 0, targetPed, 0f, 0f, 0f, 4, 45f, 15f, 0f, 35, 25, 0f, 0);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ConfigureHeliPilot: " + ex.Message);
            }
        }

        public static void ConfigureDriveByShooter(Ped shooter, Ped target, WeaponHash weapon, int accuracy = 65)
        {
            if (shooter == null || !shooter.Exists()) return;

            try
            {
                shooter.BlockPermanentEvents = false;
                shooter.KeepTasks = true;
                shooter.Inventory.Weapons.Add(weapon).Ammo = -1;
                shooter.Inventory.EquippedWeapon = weapon;

                // BF_CanUseVehicles = 1, BF_CanDoDrivebys = 2, BF_LeaveVehicles = 3
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(shooter, 1, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(shooter, 2, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(shooter, 3, false); // No abandonar vehiculo mientras este en marcha
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(shooter, 46, true); // BF_AlwaysFight

                NativeFunction.Natives.SET_PED_COMBAT_ABILITY(shooter, 2); // Professional
                NativeFunction.Natives.SET_PED_COMBAT_RANGE(shooter, 2); // Far
                NativeFunction.Natives.SET_PED_ACCURACY(shooter, accuracy);
                NativeFunction.Natives.SET_PED_FIRING_PATTERN(shooter, unchecked((int)0xC6EE6B4C)); // FullAutomatic hash

                if (target != null && target.Exists())
                {
                    shooter.Tasks.FightAgainst(target);
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ConfigureDriveByShooter: " + ex.Message);
            }
        }

        public static void EnableVehicleBailout(Ped ped)
        {
            if (ped == null || !ped.Exists()) return;
            try
            {
                ped.BlockPermanentEvents = false;
                ped.KeepTasks = true;
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 3, true); // BF_LeaveVehicles = true
                NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(ped, 2); // Advance
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.EnableVehicleBailout: " + ex.Message);
            }
        }

        public static void ReinforceVehicle(Vehicle vehicle, bool heavyArmor = false, float powerMultiplier = 1.0f)
        {
            if (vehicle == null || !vehicle.Exists()) return;

            try
            {
                if (heavyArmor)
                {
                    NativeFunction.Natives.SET_VEHICLE_STRONG(vehicle, true);
                    NativeFunction.Natives.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE(vehicle, false);
                    vehicle.Health = 3500;
                }
                NativeFunction.Natives.SET_VEHICLE_TYRES_CAN_BURST(vehicle, !heavyArmor);

                if (powerMultiplier > 1.0f)
                {
                    NativeFunction.Natives.SET_VEHICLE_ENGINE_POWER_MULTIPLIER(vehicle, powerMultiplier);
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.PursuitConfigurator.ReinforceVehicle: " + ex.Message);
            }
        }
    }
}
