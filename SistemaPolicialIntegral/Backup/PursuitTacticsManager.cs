using System;
using Rage;
using Rage.Native;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Backup
{
    public static class PursuitTacticsManager
    {
        public static void PerformPitManeuver()
        {
            Ped player = Game.LocalPlayer.Character;
            if (!player.IsInAnyVehicle(false))
            {
                Game.DisplayNotification("~r~Debe estar al volante de un vehículo para ejecutar la maniobra PIT.");
                return;
            }

            Vehicle playerVeh = player.CurrentVehicle;
            Entity[] nearby = World.GetEntities(playerVeh.Position, 10f, GetEntitiesFlags.ConsiderAllVehicles);

            Vehicle targetVeh = null;
            float closestDist = float.MaxValue;

            if (nearby != null)
            {
                foreach (Entity e in nearby)
                {
                    Vehicle v = e as Vehicle;
                    if (v != null && v.Exists() && v != playerVeh)
                    {
                        float d = playerVeh.DistanceTo(v);
                        if (d < closestDist)
                        {
                            closestDist = d;
                            targetVeh = v;
                        }
                    }
                }
            }

            if (targetVeh != null && targetVeh.Exists())
            {
                // Aplicar impulso angular en el cuadrante trasero
                NativeFunction.Natives.APPLY_FORCE_TO_ENTITY(
                    targetVeh,
                    1,
                    6.0f, 0.0f, 0.0f,
                    0.0f, -2.0f, 0.0f,
                    0, true, true, true, false, true
                );

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);
                Game.DisplayNotification("~y~[MANIOBRA PIT]~w~ Impacto angular ejecutado con éxito en el eje posterior del fugitivo.");
            }
            else
            {
                Game.DisplayNotification("~y~Acérquese más a la parte trasera del vehículo sospechoso para iniciar el PIT.");
            }
        }

        public static void OrderBoxingIn()
        {
            Ped player = Game.LocalPlayer.Character;
            if (!player.IsInAnyVehicle(false))
            {
                Game.DisplayNotification("~r~Debe estar en persecución vehicular para coordinar la caja.");
                return;
            }

            AudioEngine.PlaySirdeeOpen();
            Game.DisplaySubtitle("~b~[SIRDEE / TRÁFICO]~w~ «Atención unidades de persecución, coordinando maniobra de caja en 3, 2, 1...»", 5000);

            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(2000);
                    AudioEngine.PlaySirdeeClose();
                    Game.DisplayNotification("~b~[MANIOBRA CAJA]~w~ Patrullas ZETA flanqueando izquierda y derecha para reducir la velocidad.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.PursuitTacticsManager error: " + ex.Message);
                }
            });
        }
    }
}
