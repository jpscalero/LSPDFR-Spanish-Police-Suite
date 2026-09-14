using System;
using Rage;
using Rage.Native;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Intervention
{
    public static class PedControlManager
    {
        private static Ped grabbedPed = null;
        private static bool isEscorting = false;

        public static bool IsEscorting
        {
            get { return isEscorting; }
        }

        public static Ped GrabbedPed
        {
            get { return grabbedPed; }
        }

        public static void ToggleGrabPed(Ped target)
        {
            if (isEscorting && grabbedPed != null && grabbedPed.Exists())
            {
                // Soltar
                ReleaseGrabbedPed();
            }
            else
            {
                if (target != null && target.Exists() && target.IsAlive)
                {
                    GrabPed(target);
                }
            }
        }

        public static void GrabPed(Ped suspect)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;

            grabbedPed = suspect;
            isEscorting = true;

            suspect.BlockPermanentEvents = true;
            NativeFunction.Natives.SET_ENABLE_HANDCUFFS(suspect, true);

            // Enganchar al sospechoso ligeramente a la derecha y delante del jugador
            NativeFunction.Natives.ATTACH_ENTITY_TO_ENTITY(
                suspect,
                Game.LocalPlayer.Character,
                0,
                0.25f, 0.45f, 0.0f,
                0.0f, 0.0f, 0.0f,
                false, false, false, false, 2, true
            );

            AudioEngine.PlayMenuSelect();
            Game.DisplaySubtitle("~b~[ESCOLTA POLICIAL]~w~ Sujetando al detenido por el brazo. Camine hacia el patrullero.", 4000);
        }

        public static void ReleaseGrabbedPed()
        {
            if (grabbedPed != null && grabbedPed.Exists())
            {
                NativeFunction.Natives.DETACH_ENTITY(grabbedPed, true, true);
                grabbedPed.Tasks.Clear();
                NativeFunction.Natives.SET_ENABLE_HANDCUFFS(grabbedPed, true);
                grabbedPed.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
            }

            grabbedPed = null;
            isEscorting = false;
            AudioEngine.PlayMenuSelect();
            Game.DisplayNotification("~g~[CONTROL]~w~ Detenido soltado.");
        }

        public static void PutInVehicle(Ped suspect, Vehicle vehicle)
        {
            if (suspect == null || !suspect.Exists() || vehicle == null || !vehicle.Exists())
            {
                Game.DisplayNotification("~r~Vehículo o sospechoso no válido.");
                return;
            }

            if (isEscorting && grabbedPed == suspect)
            {
                ReleaseGrabbedPed();
            }

            // Buscar asiento trasero libre (1 o 2)
            int targetSeat = 1;
            if (!vehicle.IsSeatFree(1))
            {
                targetSeat = 2;
            }

            NativeFunction.Natives.SET_ENABLE_HANDCUFFS(suspect, true);
            suspect.WarpIntoVehicle(vehicle, targetSeat);

            // Bloquear puertas traseras para simular cierre de seguridad policial
            NativeFunction.Natives.SET_VEHICLE_DOORS_LOCKED(vehicle, 4);

            AudioEngine.PlaySirdeeClose();
            Game.DisplayNotification("~b~[PATRULLA]~w~ Detenido introducido en el asiento trasero. Puertas de seguridad bloqueadas.");
        }

        public static void RemoveFromVehicle(Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.Exists())
            {
                Game.DisplayNotification("~r~No hay patrullero cerca.");
                return;
            }

            Ped seatedSuspect = null;
            if (!vehicle.IsSeatFree(1))
            {
                seatedSuspect = vehicle.GetPedOnSeat(1);
            }
            else if (!vehicle.IsSeatFree(2))
            {
                seatedSuspect = vehicle.GetPedOnSeat(2);
            }

            if (seatedSuspect != null && seatedSuspect.Exists())
            {
                NativeFunction.Natives.SET_VEHICLE_DOORS_LOCKED(vehicle, 1);
                seatedSuspect.Tasks.LeaveVehicle(vehicle, LeaveVehicleFlags.None);

                GameFiber.StartNew(delegate
                {
                    try
                    {
                        GameFiber.Sleep(2000);
                        if (seatedSuspect != null && seatedSuspect.Exists())
                        {
                            NativeFunction.Natives.SET_ENABLE_HANDCUFFS(seatedSuspect, true);
                            seatedSuspect.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                        }
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("SPI.PedControlManager fiber error: " + ex.Message);
                    }
                });

                AudioEngine.PlayMenuSelect();
                Game.DisplayNotification("~g~[PATRULLA]~w~ Detenido apeado del vehículo.");
            }
            else
            {
                Game.DisplayNotification("~y~No hay ningún detenido en los asientos traseros del vehículo.");
            }
        }

        public static void MakePedKneel(Ped suspect)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;

            if (isEscorting && grabbedPed == suspect) ReleaseGrabbedPed();

            NativeFunction.Natives.SET_ENABLE_HANDCUFFS(suspect, true);
            NativeFunction.Natives.TASK_PLAY_ANIM(suspect, "random@arrests@busted", "idle_a", 8.0f, -8.0f, -1, 1, 0, false, false, false);
            AudioEngine.PlayMenuSelect();
            Game.DisplaySubtitle("~b~[ORDEN]~w~ Sospechoso puesto de rodillas.", 3000);
        }

        public static void MakePedSitOnCurb(Ped suspect)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;

            if (isEscorting && grabbedPed == suspect) ReleaseGrabbedPed();

            NativeFunction.Natives.SET_ENABLE_HANDCUFFS(suspect, true);
            NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(suspect, "WORLD_HUMAN_PICNIC", 0, true);
            AudioEngine.PlayMenuSelect();
            Game.DisplaySubtitle("~b~[ORDEN]~w~ Sospechoso sentado en el suelo / bordillo.", 3000);
        }

        public static void MakePedStandUp(Ped suspect)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;

            suspect.Tasks.Clear();
            NativeFunction.Natives.CLEAR_PED_TASKS(suspect);
            NativeFunction.Natives.SET_ENABLE_HANDCUFFS(suspect, true);
            suspect.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
            AudioEngine.PlayMenuSelect();
            Game.DisplaySubtitle("~g~[ORDEN]~w~ Sospechoso puesto en pie.", 3000);
        }
    }
}
