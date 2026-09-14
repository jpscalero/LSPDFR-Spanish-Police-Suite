using System;
using Rage;
using Rage.Native;

namespace SistemaPolicialIntegral.Services
{
    public static class MunicipalFlatbedTow
    {
        public static void RequestTow(Vehicle targetVehicle)
        {
            GameFiber.StartNew(delegate
            {
                if (targetVehicle == null || !targetVehicle.Exists())
                {
                    Game.DisplaySubtitle("~r~[ERROR] Vehículo objetivo no válido para la grúa.", 3000);
                    return;
                }

                Game.DisplaySubtitle("~b~[SIRDEE]~w~ Solicitando grúa municipal al lugar...", 4000);
                GameFiber.Sleep(2000);

                Vector3 spawnPos = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(120f));
                Vehicle flatbed = new Vehicle("flatbed", spawnPos);
                if (!flatbed.Exists()) return;
                flatbed.IsPersistent = true;

                Ped driver = new Ped("s_m_m_autoshop_02", spawnPos, 0f);
                if (!driver.Exists())
                {
                    flatbed.Delete();
                    return;
                }
                driver.IsPersistent = true;
                driver.WarpIntoVehicle(flatbed, -1);

                driver.Tasks.DriveToPosition(flatbed, targetVehicle.Position, 15f, VehicleDrivingFlags.Normal, 5f).WaitForCompletion(30000);
                
                while (flatbed.DistanceTo(targetVehicle) > 8f)
                {
                    GameFiber.Sleep(1000);
                    if (!driver.Exists() || !flatbed.Exists()) return;
                }

                driver.Tasks.ClearImmediately();
                Game.DisplaySubtitle("~y~[GRÚA MUNICIPAL]~w~ Operario procediendo a cargar el vehículo...", 5000);
                GameFiber.Sleep(8000);

                if (targetVehicle.Exists() && flatbed.Exists())
                {
                    NativeFunction.Natives.ATTACH_ENTITY_TO_ENTITY(targetVehicle, flatbed, 0, 0f, -2.5f, 1f, 0f, 0f, 0f, false, false, false, false, 2, true);
                    Game.DisplaySubtitle("~g~[GRÚA MUNICIPAL]~w~ Vehículo cargado y en tránsito al depósito municipal.", 5000);
                }

                driver.Tasks.CruiseWithVehicle(flatbed, 15f, VehicleDrivingFlags.Normal);
                GameFiber.Sleep(30000);

                if (targetVehicle.Exists()) targetVehicle.Delete();
                if (driver.Exists()) driver.Delete();
                if (flatbed.Exists()) flatbed.Delete();
            });
        }
    }
}
