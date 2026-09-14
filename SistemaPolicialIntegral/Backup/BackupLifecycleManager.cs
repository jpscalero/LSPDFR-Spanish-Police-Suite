using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using PoliceSuite.Common;

namespace SistemaPolicialIntegral.Backup
{
    /// <summary>
    /// Gestor centralizado del ciclo de vida de refuerzos policiales SIRDEE.
    /// Evita que los vehiculos, agentes y blips queden atascados en el mapa.
    /// Permite el despido automatico por distancia y despido manual con Codigo 4.
    /// </summary>
    public static class BackupLifecycleManager
    {
        private static readonly List<Vehicle> _activeVehicles = new List<Vehicle>();
        private static readonly List<Ped> _activePeds = new List<Ped>();
        private static readonly List<Blip> _activeBlips = new List<Blip>();
        private static bool _monitorStarted = false;
        private static readonly object _syncLock = new object();

        public static int ActiveUnitsCount
        {
            get { lock (_syncLock) { return _activeVehicles.Count; } }
        }

        public static void Initialize()
        {
            if (_monitorStarted) return;
            _monitorStarted = true;

            GameFiber.StartNew(delegate
            {
                try
                {
                    MonitorLoop();
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("BackupLifecycleManager.MonitorLoop error: " + ex.Message);
                }
            });
        }

        public static void RegisterUnit(Vehicle veh, List<Ped> peds, string unitName, Color blipColor)
        {
            Initialize();

            lock (_syncLock)
            {
                if (veh != null && veh.Exists())
                {
                    veh.IsPersistent = true;
                    _activeVehicles.Add(veh);

                    try
                    {
                        Blip b = veh.AttachBlip();
                        if (b != null && b.Exists())
                        {
                            b.Color = blipColor;
                            b.Scale = 0.70f;
                            b.Name = unitName;
                            _activeBlips.Add(b);
                        }
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("BackupLifecycleManager.RegisterUnit.Blip error: " + ex.Message);
                    }
                }

                if (peds != null)
                {
                    for (int i = 0; i < peds.Count; i++)
                    {
                        Ped p = peds[i];
                        if (p != null && p.Exists())
                        {
                            p.IsPersistent = true;
                            _activePeds.Add(p);
                        }
                    }
                }
            }

            SpanishVoiceDispatchEngine.AnnounceBackupRequested(unitName);
        }

        public static void DismissAllBackup()
        {
            lock (_syncLock)
            {
                // 1. Destruccion inmediata de todos los blips
                for (int i = 0; i < _activeBlips.Count; i++)
                {
                    try
                    {
                        Blip b = _activeBlips[i];
                        if (b != null && b.Exists())
                        {
                            b.DisableRoute();
                            b.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("BackupLifecycleManager.DismissAllBackup.Blips: " + ex.Message);
                    }
                }
                _activeBlips.Clear();

                // 2. Liberacion de peds
                for (int i = 0; i < _activePeds.Count; i++)
                {
                    try
                    {
                        Ped p = _activePeds[i];
                        if (p != null && p.Exists())
                        {
                            p.BlockPermanentEvents = false;
                            p.KeepTasks = false;
                            p.IsPersistent = false;
                            p.Tasks.Clear();
                            p.Dismiss();
                        }
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("BackupLifecycleManager.DismissAllBackup.Peds: " + ex.Message);
                    }
                }
                _activePeds.Clear();

                // 3. Liberacion de vehiculos
                for (int i = 0; i < _activeVehicles.Count; i++)
                {
                    try
                    {
                        Vehicle v = _activeVehicles[i];
                        if (v != null && v.Exists())
                        {
                            v.IsPersistent = false;
                            v.Dismiss();
                        }
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("BackupLifecycleManager.DismissAllBackup.Vehicles: " + ex.Message);
                    }
                }
                _activeVehicles.Clear();
            }

            SpanishVoiceDispatchEngine.AnnounceCode4();
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~SALA 091 / SIRDEE", "~g~CODIGO 4 CONFIRMADO", "~w~Todos los refuerzos policiales han sido liberados y retirados del radar.");
        }

        private static void MonitorLoop()
        {
            while (true)
            {
                GameFiber.Sleep(4000);

                try
                {
                    Ped player = Game.LocalPlayer.Character;
                    if (player == null || !player.Exists()) continue;

                    lock (_syncLock)
                    {
                        if (_activeVehicles.Count == 0 && _activePeds.Count == 0) continue;

                        // Limpieza de entidades destruidas o ya no existentes
                        for (int i = _activeBlips.Count - 1; i >= 0; i--)
                        {
                            Blip b = _activeBlips[i];
                            if (b == null || !b.Exists())
                            {
                                _activeBlips.RemoveAt(i);
                            }
                        }

                        // Comprobar si el jugador se ha alejado a mas de 220 metros
                        bool allFarAway = true;
                        for (int i = 0; i < _activeVehicles.Count; i++)
                        {
                            Vehicle v = _activeVehicles[i];
                            if (v != null && v.Exists())
                            {
                                if (player.Position.DistanceTo(v.Position) < 180f)
                                {
                                    allFarAway = false;
                                    break;
                                }
                            }
                        }

                        if (allFarAway && _activeVehicles.Count > 0)
                        {
                            Game.LogTrivial("BackupLifecycleManager: Jugador alejado a mas de 180m de las unidades de apoyo. Ejecutando autodismiss.");
                            DismissAllBackup();
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("BackupLifecycleManager.MonitorLoop cycle error: " + ex.Message);
                }
            }
        }
    }
}
