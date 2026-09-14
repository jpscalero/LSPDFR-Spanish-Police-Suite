using System;
using System.Drawing;
using Rage;
using Rage.Native;
using RadioYJusticia.Core;

namespace RadioYJusticia.Radio
{
    public static class PanicButton
    {
        private static DateTime lastPanicTime = DateTime.MinValue;
        private static readonly string[] policeModels = new string[] { "police", "police2", "police3" };
        private static readonly System.Collections.Generic.List<Blip> _panicBlips = new System.Collections.Generic.List<Blip>();
        private static readonly System.Collections.Generic.List<Vehicle> _panicVehicles = new System.Collections.Generic.List<Vehicle>();
        private static readonly System.Collections.Generic.List<Ped> _panicPeds = new System.Collections.Generic.List<Ped>();
        private static readonly object _panicLock = new object();

        public static void Process()
        {
            Game.LogTrivial("RadioYJusticia: Bucle de Boton de Panico 10-99 iniciado.");

            while (true)
            {
                GameFiber.Yield();

                try
                {
                    if (Game.IsKeyDown(ConfigManager.PanicKey))
                    {
                        Ped player = Game.LocalPlayer.Character;
                        if (player != null && player.Exists() && player.IsAlive)
                        {
                            if ((DateTime.Now - lastPanicTime).TotalSeconds > 25)
                            {
                                lastPanicTime = DateTime.Now;
                                TriggerEmergencyPanic();
                            }
                            else
                            {
                                Game.DisplayNotification("~y~Alerta 10-99 ya activa en la malla de radio. Refuerzos en camino.");
                            }
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.PanicButton loop error: " + ex.Message);
                }
            }
        }

        public static void ClearPanicUnits()
        {
            lock (_panicLock)
            {
                for (int i = 0; i < _panicBlips.Count; i++)
                {
                    try
                    {
                        Blip b = _panicBlips[i];
                        if (b != null && b.Exists())
                        {
                            b.DisableRoute();
                            b.Delete();
                        }
                    }
                    catch { }
                }
                _panicBlips.Clear();

                for (int i = 0; i < _panicPeds.Count; i++)
                {
                    try
                    {
                        Ped p = _panicPeds[i];
                        if (p != null && p.Exists())
                        {
                            p.BlockPermanentEvents = false;
                            p.IsPersistent = false;
                            p.Dismiss();
                        }
                    }
                    catch { }
                }
                _panicPeds.Clear();

                for (int i = 0; i < _panicVehicles.Count; i++)
                {
                    try
                    {
                        Vehicle v = _panicVehicles[i];
                        if (v != null && v.Exists())
                        {
                            v.IsPersistent = false;
                            v.Dismiss();
                        }
                    }
                    catch { }
                }
                _panicVehicles.Clear();
            }

            PoliceSuite.Common.SpanishVoiceDispatchEngine.AnnounceCode4();
        }

        private static void TriggerEmergencyPanic()
        {
            ClearPanicUnits();

            GameFiber.StartNew(delegate
            {
                try
                {
                    Ped player = Game.LocalPlayer.Character;
                    if (player == null || !player.Exists()) return;

                // Sonido agudo de alarma de emergencia
                try
                {
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "TIMER_STOP", "HUD_MINI_GAME_SOUNDSET", false);
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET", false);
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.PanicButton.ActivatePanicButton (Audio): " + ex.Message);
                }

                // Locución de voz de alarma en español
                PoliceSuite.Common.SpanishVoiceDispatchEngine.AnnouncePanicAlert();

                // Subtítulo urgente en pantalla
                Game.DisplaySubtitle("~r~¡¡10-99 TRANSMITIDO!!~n~~b~[CENTRAL]~w~ ¡Todas las unidades disponibles acudan de urgencia en Codigo 3 a la posicion del agente!", 7000);

                // Notificación visual de alta prioridad
                Game.DisplayNotification(
                    "3dtextures",
                    "mpgroundlogo_cops",
                    "~r~ALERTA ROJA - 10-99",
                    "~r~AGENTE EN PELIGRO EXTREMO",
                    "~w~Peticion de socorro transmitida a Sala 091.~n~Desplegando 3 unidades ZETA de urgencia."
                );

                Game.LogTrivial("RadioYJusticia: ¡¡10-99 TRANSMITIDO POR EL AGENTE!! Desplegando 3 patrullas de auxilio...");

                // Spawn de 3 vehículos policiales de refuerzo escalonados
                float[] distances = new float[] { 70f, 95f, 120f };
                Vector3 emergencyOrigin = player.Position;

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        Vector3 targetSpawnPos = World.GetNextPositionOnStreet(player.Position.Around(distances[i]));
                        string modelName = policeModels[i % policeModels.Length];

                        Vehicle veh = new Vehicle(modelName, targetSpawnPos);
                        if (!veh.Exists())
                        {
                            veh = new Vehicle("police", targetSpawnPos);
                        }

                        if (veh.Exists())
                        {
                            veh.IsPersistent = true;
                            veh.IsSirenOn = true;
                            veh.IsSirenSilent = false;
                            lock (_panicLock) { _panicVehicles.Add(veh); }

                            // Conductor policial
                            Ped driver = new Ped("s_m_y_cop_01", targetSpawnPos, 0f);
                            if (driver.Exists())
                            {
                                driver.IsPersistent = true;
                                driver.WarpIntoVehicle(veh, -1);
                                driver.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true);
                                driver.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 180, false);
                                driver.Armor = 100;
                                driver.RelationshipGroup = "COP";
                                NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                                driver.Tasks.DriveToPosition(player.Position, 28f, VehicleDrivingFlags.Emergency);
                                lock (_panicLock) { _panicPeds.Add(driver); }
                            }

                            // Acompañante policial
                            Ped passenger = new Ped("s_m_y_cop_01", targetSpawnPos, 0f);
                            if (passenger.Exists())
                            {
                                passenger.IsPersistent = true;
                                passenger.WarpIntoVehicle(veh, 0);
                                passenger.Inventory.GiveNewWeapon(WeaponHash.PumpShotgun, 60, true);
                                passenger.Armor = 100;
                                passenger.RelationshipGroup = "COP";
                                NativeFunction.Natives.SET_PED_AS_COP(passenger, true);
                                lock (_panicLock) { _panicPeds.Add(passenger); }
                            }

                            // Blip policial azul en el minimapa
                            Blip blip = veh.AttachBlip();
                            if (blip != null && blip.Exists())
                            {
                                blip.Color = Color.FromArgb(0, 100, 255);
                                blip.Scale = 0.85f;
                                blip.Name = "Refuerzo Zeta (10-99)";
                                lock (_panicLock) { _panicBlips.Add(blip); }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("RadioYJusticia: Error spawneando unidad de refuerzo 10-99: " + ex.Message);
                    }

                    GameFiber.Sleep(600);
                }

                // Autolimpieza programada: transcurridos 120 segundos o si el jugador se aleja > 180m
                GameFiber.StartNew(delegate
                {
                    try
                    {
                        uint startT = Game.GameTime;
                        while (_panicBlips.Count > 0 && (Game.GameTime - startT < 120000))
                        {
                            GameFiber.Sleep(4000);
                            Ped pl = Game.LocalPlayer.Character;
                            if (pl != null && pl.Exists() && pl.Position.DistanceTo(emergencyOrigin) > 180f && (Game.GameTime - startT > 25000))
                            {
                                Game.LogTrivial("PanicButton: Jugador alejado del lugar de emergencia 10-99. Liberando patrullas y retirando blips.");
                                break;
                            }
                        }
                        ClearPanicUnits();
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception exClear)
                    {
                        Game.LogTrivial("PanicButton auto-clean fiber error: " + exClear.Message);
                    }
                });

                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia: Error en fibra de emergencia 10-99: " + ex.Message);
                }
            });
        }
    }
}
