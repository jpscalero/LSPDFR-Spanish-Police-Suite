using System;
using System.Drawing;
using Rage;
using Rage.Native;
using PoliceSuite.Common;

namespace SistemaPolicialIntegral.Services
{
    /// <summary>
    /// Sistema de simulacion de patrullas policiales en respuesta de emergencia ambiental.
    /// Hace que vehiculos patrulla crucen a toda velocidad por la ciudad/condado con sirenas
    /// y prioritarios respondiendo a casos ficticios, con eliminacion y autodismiss garantizados.
    /// </summary>
    public static class AmbientEmergencyPatrolSystem
    {
        private static bool _isRunning = false;
        private static DateTime _lastPatrolTime = DateTime.MinValue;
        private static readonly string[] _patrolModels = new string[] { "police", "police2", "police3", "sheriff", "sheriff2" };
        private static readonly Random _rnd = new Random();

        public static void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            _lastPatrolTime = DateTime.Now.AddSeconds(15); // Primer despliegue rapido

            GameFiber.StartNew(delegate
            {
                try
                {
                    Game.LogTrivial("SPI: AmbientEmergencyPatrolSystem iniciado.");
                    Loop();
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("AmbientEmergencyPatrolSystem.Start error: " + ex.Message);
                }
            });
        }

        private static void Loop()
        {
            while (_isRunning)
            {
                GameFiber.Sleep(6000);

                try
                {
                    Ped player = Game.LocalPlayer.Character;
                    if (player == null || !player.Exists() || !player.IsAlive) continue;

                    // Intervalo dinamico entre 75 y 110 segundos
                    double elapsed = (DateTime.Now - _lastPatrolTime).TotalSeconds;
                    if (elapsed < 80) continue;

                    _lastPatrolTime = DateTime.Now;
                    SpawnRespondingPatrol(player);
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("AmbientEmergencyPatrolSystem.Loop cycle error: " + ex.Message);
                }
            }
        }

        private static void SpawnRespondingPatrol(Ped player)
        {
            GameFiber.StartNew(delegate
            {
                Vehicle veh = null;
                Ped driver = null;
                Ped passenger = null;
                Blip blip = null;

                try
                {
                    // Buscar posicion en carretera a 120-140m del jugador
                    Vector3 playerPos = player.Position;
                    Vector3 forward = player.ForwardVector;
                    Vector3 spawnCandidate = playerPos - (forward * 110f) + (player.RightVector * 30f);
                    Vector3 spawnPos = World.GetNextPositionOnStreet(spawnCandidate);

                    // Destino hacia adelante atravesando el area
                    Vector3 targetCandidate = playerPos + (forward * 380f) + (player.RightVector * 50f);
                    Vector3 targetPos = World.GetNextPositionOnStreet(targetCandidate);

                    if (spawnPos.DistanceTo(playerPos) < 40f || spawnPos.DistanceTo(targetPos) < 80f) return;

                    string modelName = _patrolModels[_rnd.Next(_patrolModels.Length)];
                    veh = new Vehicle(modelName, spawnPos);
                    if (veh == null || !veh.Exists()) return;

                    veh.IsPersistent = true;
                    veh.IsSirenOn = true;
                    veh.IsSirenSilent = false;

                    // Agentes uniformados
                    driver = new Ped("s_m_y_cop_01", spawnPos, 0f);
                    if (driver != null && driver.Exists())
                    {
                        driver.IsPersistent = true;
                        driver.WarpIntoVehicle(veh, -1);
                        driver.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(driver, true);

                        // Conduccion de maxima urgencia
                        NativeFunction.Natives.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE(
                            driver, veh, targetPos.X, targetPos.Y, targetPos.Z, 34.0f, 786603, 15.0f
                        );
                    }

                    passenger = new Ped("s_m_y_cop_01", spawnPos, 0f);
                    if (passenger != null && passenger.Exists())
                    {
                        passenger.IsPersistent = true;
                        passenger.WarpIntoVehicle(veh, 0);
                        passenger.RelationshipGroup = "COP";
                        NativeFunction.Natives.SET_PED_AS_COP(passenger, true);
                    }

                    // Blip policial azul en el radar
                    try
                    {
                        blip = veh.AttachBlip();
                        if (blip != null && blip.Exists())
                        {
                            blip.Color = Color.FromArgb(0, 100, 255);
                            blip.Scale = 0.65f;
                            blip.Name = "Patrulla ZETA en Emergencia";
                        }
                    }
                    catch (Exception exB)
                    {
                        Game.LogTrivial("AmbientEmergencyPatrolSystem: Error asignando blip: " + exB.Message);
                    }

                    // Transmision por radio en espanol
                    SpanishVoiceDispatchEngine.AnnounceAmbientEmergencyResponse();

                    // Monitorizacion y autodestruccion limpia
                    uint startTime = Game.GameTime;
                    while (veh != null && veh.Exists() && (Game.GameTime - startTime < 42000))
                    {
                        GameFiber.Sleep(2000);

                        if (player != null && player.Exists() && veh.Position.DistanceTo(player.Position) > 190f && (Game.GameTime - startTime > 15000))
                        {
                            break;
                        }

                        if (veh.Position.DistanceTo(targetPos) < 25f)
                        {
                            break;
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("AmbientEmergencyPatrolSystem.SpawnRespondingPatrol error: " + ex.Message);
                }
                finally
                {
                    // Limpieza garantizada de entidades y blip
                    try
                    {
                        if (blip != null && blip.Exists())
                        {
                            blip.DisableRoute();
                            blip.Delete();
                        }
                    }
                    catch { }

                    try
                    {
                        if (driver != null && driver.Exists())
                        {
                            driver.BlockPermanentEvents = false;
                            driver.IsPersistent = false;
                            driver.Dismiss();
                        }
                    }
                    catch { }

                    try
                    {
                        if (passenger != null && passenger.Exists())
                        {
                            passenger.BlockPermanentEvents = false;
                            passenger.IsPersistent = false;
                            passenger.Dismiss();
                        }
                    }
                    catch { }

                    try
                    {
                        if (veh != null && veh.Exists())
                        {
                            veh.IsPersistent = false;
                            veh.Dismiss();
                        }
                    }
                    catch { }
                }
            });
        }
    }
}
