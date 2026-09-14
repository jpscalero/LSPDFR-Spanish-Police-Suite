using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Backup
{
    public static class RoadblockManager
    {
        private static List<Vehicle> roadblockVehicles = new List<Vehicle>();
        private static List<Ped> roadblockOfficers = new List<Ped>();
        private static List<Blip> roadblockBlips = new List<Blip>();

        public static void RequestTacticalRoadblock()
        {
            Ped player = Game.LocalPlayer.Character;
            Vector3 forward = player.ForwardVector;
            Vector3 targetRoadPos = World.GetNextPositionOnStreet(player.Position + (forward * 220f));

            AudioEngine.PlaySirdeeOpen();
            Game.DisplaySubtitle("~b~[SALA 091 / SIRDEE]~w~ Estableciendo corte de calzada y bloqueo táctico 200m más adelante...", 5000);

            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(2500);

                // Vehículo 1 (bloqueo izquierdo)
                Vehicle v1 = new Vehicle("police3", targetRoadPos.Around(4f), player.Heading + 45f);
                if (v1 != null && v1.Exists())
                {
                    v1.IsPersistent = true;
                    v1.IsSirenOn = true;
                    roadblockVehicles.Add(v1);

                    Ped cop1 = new Ped("s_m_y_swat_01", v1.Position.Around(2f), player.Heading + 180f);
                    if (cop1 != null && cop1.Exists())
                    {
                        cop1.IsPersistent = true;
                        cop1.RelationshipGroup = "COP";
                        cop1.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 300, true);
                        cop1.Tasks.TakeCoverFrom(player.Position, -1);
                        roadblockOfficers.Add(cop1);
                    }
                }

                // Vehículo 2 (bloqueo derecho)
                Vehicle v2 = new Vehicle("police2", targetRoadPos.Around(8f), player.Heading - 45f);
                if (v2 != null && v2.Exists())
                {
                    v2.IsPersistent = true;
                    v2.IsSirenOn = true;
                    roadblockVehicles.Add(v2);

                    Ped cop2 = new Ped("s_m_y_swat_01", v2.Position.Around(2f), player.Heading + 180f);
                    if (cop2 != null && cop2.Exists())
                    {
                        cop2.IsPersistent = true;
                        cop2.RelationshipGroup = "COP";
                        cop2.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 300, true);
                        cop2.Tasks.TakeCoverFrom(player.Position, -1);
                        roadblockOfficers.Add(cop2);
                    }
                }

                Blip b = new Blip(targetRoadPos, 15f);
                b.Color = System.Drawing.Color.Blue;
                b.Scale = 0.8f;
                roadblockBlips.Add(b);

                AudioEngine.PlaySirdeeClose();
                PoliceSuite.Common.SpanishVoiceDispatchEngine.AnnounceRoadblockDeployed();
                Game.DisplayNotification("~b~[BLOQUEO LISTO]~w~ Patrullas en formación de embudo con agentes armados en cobertura.");

                // Monitorizacion de autodestruccion si el jugador se aleja a mas de 220 metros
                GameFiber.StartNew(delegate
                {
                    try
                    {
                        uint deployedTime = Game.GameTime;
                        while (roadblockBlips.Count > 0 && (Game.GameTime - deployedTime < 180000))
                        {
                            GameFiber.Sleep(5000);
                            Ped pl = Game.LocalPlayer.Character;
                            if (pl != null && pl.Exists() && pl.Position.DistanceTo(targetRoadPos) > 230f && (Game.GameTime - deployedTime > 30000))
                            {
                                Game.LogTrivial("RoadblockManager: Jugador alejado a mas de 230m del corte de carretera. Levantando bloqueo.");
                                ClearRoadblock();
                                break;
                            }
                        }
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("RoadblockManager auto-clear fiber error: " + ex.Message);
                    }
                });
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.RoadblockManager error: " + ex.Message);
                }
            });
        }

        public static void ClearRoadblock()
        {
            foreach (Blip b in roadblockBlips)
            {
                if (b != null && b.Exists())
                {
                    b.DisableRoute();
                    b.Delete();
                }
            }
            roadblockBlips.Clear();

            foreach (Ped p in roadblockOfficers)
            {
                if (p != null && p.Exists()) p.Dismiss();
            }
            roadblockOfficers.Clear();

            foreach (Vehicle v in roadblockVehicles)
            {
                if (v != null && v.Exists()) v.Dismiss();
            }
            roadblockVehicles.Clear();

            AudioEngine.PlayMenuSelect();
            Game.DisplayNotification("~g~[BLOQUEO]~w~ Dispositivo de bloqueo levantado.");
        }
    }
}
