using System;
using Rage;
using Rage.Native;
using PoliceLifeSystem.Core;

namespace PoliceLifeSystem.Economy
{
    public static class ExpenseTracker
    {
        private static int _cyclesSinceLastRent;

        static ExpenseTracker()
        {
            _cyclesSinceLastRent = 0;
        }

        public static void DeductRent()
        {
            int rent = ConfigManager.RentCost;
            if (rent <= 0) rent = 600;

            SalaryManager.DeductMoney(rent);

            string msg = string.Format(
                "~w~Alquiler mensual de vivienda: ~r~-{0}€~w~~n~Saldo disponible: ~b~{1}€~w~",
                rent,
                SalaryManager.CurrentBalance
            );

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~GASTO PERIÓDICO", "~o~Alquiler Inmobiliario", msg);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", false);
        }

        public static void DeductFuel()
        {
            int fuel = ConfigManager.FuelCost;
            if (fuel <= 0) fuel = 50;

            SalaryManager.DeductMoney(fuel);

            string msg = string.Format(
                "~w~Repostaje vehículo de servicio: ~r~-{0}€~w~~n~Saldo disponible: ~b~{1}€~w~",
                fuel,
                SalaryManager.CurrentBalance
            );

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~GASTO DE PATRULLA", "~b~Estación de Servicio CNP", msg);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", false);
        }

        public static void RepairPatrolVehicle(Vehicle veh)
        {
            if (veh == null || !veh.Exists())
            {
                Game.DisplayNotification("~r~No hay ningún vehículo policial para reparar.");
                return;
            }

            int health = veh.Health;
            if (health >= 1000)
            {
                Game.DisplayNotification("~g~El vehículo patrulla se encuentra en perfecto estado mecánico y de chapa.");
                return;
            }

            int missingHealth = Math.Max(0, 1000 - health);
            int repairCost = (missingHealth / 3) + 75;
            if (repairCost > 500) repairCost = 500;

            if (SalaryManager.CurrentBalance < repairCost)
            {
                string warnMsg = string.Format("~r~Fondos insuficientes para reparar el patrulla.~n~~w~Coste: ~y~{0}€~w~ | Saldo: ~y~{1}€~w~.", repairCost, SalaryManager.CurrentBalance);
                Game.DisplayNotification(warnMsg);
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "ERROR", "HUD_AMMO_SHOP_SOUNDSET", false);
                return;
            }

            SalaryManager.DeductMoney(repairCost);
            veh.Repair();
            veh.Health = 1000;
            veh.Wash();

            string okMsg = string.Format(
                "~w~Reparación completa de carrocería y motor.~n~Coste del taller: ~r~-{0}€~w~~n~Saldo restante: ~b~{1}€~w~",
                repairCost,
                SalaryManager.CurrentBalance
            );

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~g~TALLER DE AUTOMOCIÓN CNP", "~w~Vehículo Reparado", okMsg);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", false);
        }

        public static void ProcessExpenses()
        {
            // Descontar combustible en cada ciclo de turno
            DeductFuel();

            // Descontar alquiler cada 3 ciclos de salario (ej. simula ciclo de semanas/mes)
            _cyclesSinceLastRent++;
            if (_cyclesSinceLastRent >= 3)
            {
                DeductRent();
                _cyclesSinceLastRent = 0;
            }

            // Comprobar si el jugador está en un vehículo dañado y advertir
            if (Game.LocalPlayer.Character != null && Game.LocalPlayer.Character.Exists())
            {
                Vehicle currentVeh = Game.LocalPlayer.Character.CurrentVehicle;
                if (currentVeh != null && currentVeh.Exists() && currentVeh.Health < 800)
                {
                    Game.DisplayNotification("~o~AVISO MECÁNICO:~n~~w~Tu vehículo de patrulla tiene daños. Puedes repararlo en el menú de gestión.");
                }
            }
        }
    }
}
