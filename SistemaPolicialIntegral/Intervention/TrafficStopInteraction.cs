using System;
using Rage;
using Rage.Native;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Intervention
{
    public static class TrafficStopInteraction
    {
        private static Random rnd = new Random();

        public static void InteractWithDriver(Vehicle veh)
        {
            if (veh == null || !veh.Exists())
            {
                Game.DisplayNotification("~r~No hay ningún vehículo para interactuar.");
                return;
            }

            Ped driver = veh.Driver;
            if (driver == null || !driver.Exists() || !driver.IsAlive)
            {
                Game.DisplayNotification("~y~El vehículo está vacío o el conductor no responde.");
                return;
            }

            // Bajar ventanilla delantera izquierda (conductor)
            NativeFunction.Natives.ROLL_DOWN_WINDOW(veh, 0);

            // Apagar el motor
            NativeFunction.Natives.SET_VEHICLE_ENGINE_ON(veh, false, true, true);

            // El conductor mira al agente
            NativeFunction.Natives.TASK_LOOK_AT_ENTITY(driver, Game.LocalPlayer.Character, -1, 0, 2);

            AudioEngine.PlayMenuSelect();

            int scenario = rnd.Next(100);

            if (scenario < 65)
            {
                // Colaborador normal
                Game.DisplaySubtitle("~w~Conductor:~b~ «Buenas tardes, agente. Aquí tiene mi permiso de conducir y la documentación del vehículo.»", 6000);
                Game.DisplayNotification("~g~[CONTROL DE TRÁFICO]~w~ Conductor tranquilo y colaborador. Documentación entregada.");
            }
            else if (scenario < 85)
            {
                // Conductor nervioso / olor sospechoso
                Game.DisplaySubtitle("~w~Conductor:~y~ «Hola agente... ¿p-pasa algo? Tenía prisa para volver a casa...»", 6000);
                Game.DisplayNotification("~y~[OBSERVACIÓN POLICIAL]~w~ Se percibe un fuerte olor a alcohol/estupefacientes desde el habitáculo. Conductor tembloroso.");
            }
            else
            {
                // Conductor agresivo o sospechoso
                Game.DisplaySubtitle("~w~Conductor:~r~ «¿Por qué me para? ¡No he hecho nada! ¡Esto es acoso policial!»", 6000);
                Game.DisplayNotification("~r~[ALERTA DE SEGURIDAD]~w~ Conductor hostil y alterado. Mantenga la mano sobre la funda reglamentaria.");
            }
        }

        public static void OrderDriverExit(Vehicle veh)
        {
            if (veh == null || !veh.Exists()) return;
            Ped driver = veh.Driver;
            if (driver != null && driver.Exists() && driver.IsAlive)
            {
                driver.Tasks.LeaveVehicle(veh, LeaveVehicleFlags.None);
                Game.DisplaySubtitle("~b~[AGENTE]~w~ «Por favor, apague el motor, descienda del vehículo y acompáñeme a la acera.»", 5000);
            }
        }
    }
}
