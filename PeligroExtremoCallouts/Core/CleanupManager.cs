using System;
using System.Collections.Generic;
using Rage;

namespace PeligroExtremoCallouts.Core
{
    /// <summary>
    /// Gestor centralizado de ciclo de vida de entidades para PeligroExtremoCallouts.
    /// Registra y libera peds, vehiculos, blips, props y fibers generados durante una llamada.
    /// </summary>
    public class CleanupManager
    {
        private readonly List<Ped> _peds = new List<Ped>();
        private readonly List<Vehicle> _vehicles = new List<Vehicle>();
        private readonly List<Blip> _blips = new List<Blip>();
        private readonly List<Rage.Object> _props = new List<Rage.Object>();
        private readonly List<GameFiber> _fibers = new List<GameFiber>();
        private readonly List<Action> _customCleanups = new List<Action>();

        public void RegisterCustomCleanup(Action action)
        {
            if (action != null)
            {
                _customCleanups.Add(action);
            }
        }

        public void RegisterPed(Ped ped)
        {
            if (ped != null && !_peds.Contains(ped))
            {
                _peds.Add(ped);
            }
        }

        public void RegisterVehicle(Vehicle vehicle)
        {
            if (vehicle != null && !_vehicles.Contains(vehicle))
            {
                _vehicles.Add(vehicle);
            }
        }

        public void RegisterBlip(Blip blip)
        {
            if (blip != null && !_blips.Contains(blip))
            {
                _blips.Add(blip);
            }
        }

        public void RegisterProp(Rage.Object prop)
        {
            if (prop != null && !_props.Contains(prop))
            {
                _props.Add(prop);
            }
        }

        public void RegisterFiber(GameFiber fiber)
        {
            if (fiber != null && !_fibers.Contains(fiber))
            {
                _fibers.Add(fiber);
            }
        }

        public void UnregisterBlip(Blip blip)
        {
            if (blip != null && _blips.Contains(blip))
            {
                try
                {
                    if (blip.Exists())
                    {
                        blip.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PeligroExtremoCallouts.CleanupManager.UnregisterBlip: " + ex.Message);
                }
                _blips.Remove(blip);
            }
        }

        /// <summary>
        /// Libera todas las entidades registradas. Si forceDelete es true, elimina de inmediato;
        /// si es false, llama a Dismiss() para permitir interaccion con servicios de emergencias.
        /// </summary>
        public void CleanAll(bool forceDelete = false)
        {
            // Cancelacion y detencion de GameFibers activos
            for (int i = 0; i < _fibers.Count; i++)
            {
                try
                {
                    GameFiber f = _fibers[i];
                    if (f != null && f.IsAlive)
                    {
                        f.Abort();
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PeligroExtremoCallouts.CleanupManager.CleanAll_Fibers: " + ex.Message);
                }
            }
            _fibers.Clear();

            // Ejecucion de cleanups personalizados registrados
            for (int i = 0; i < _customCleanups.Count; i++)
            {
                try
                {
                    if (_customCleanups[i] != null)
                    {
                        _customCleanups[i]();
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PeligroExtremoCallouts.CleanupManager.CleanAll_CustomCleanups: " + ex.Message);
                }
            }
            _customCleanups.Clear();

            // Limpieza prioritaria de Blips en el mapa
            for (int i = 0; i < _blips.Count; i++)
            {
                try
                {
                    Blip b = _blips[i];
                    if (b != null && b.Exists())
                    {
                        b.DisableRoute();
                        b.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PeligroExtremoCallouts.CleanupManager.CleanAll_Blips: " + ex.Message);
                }
            }
            _blips.Clear();

            // Limpieza de Peds sin efecto zombie
            for (int i = 0; i < _peds.Count; i++)
            {
                try
                {
                    Ped p = _peds[i];
                    if (p != null && p.Exists())
                    {
                        p.BlockPermanentEvents = false;
                        p.KeepTasks = false;

                        if (forceDelete)
                        {
                            p.Delete();
                        }
                        else
                        {
                            p.Tasks.Clear();
                            p.IsPersistent = false;
                            p.Dismiss();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PeligroExtremoCallouts.CleanupManager.CleanAll_Peds: " + ex.Message);
                }
            }
            _peds.Clear();

            // Limpieza de Vehiculos
            for (int i = 0; i < _vehicles.Count; i++)
            {
                try
                {
                    Vehicle v = _vehicles[i];
                    if (v != null && v.Exists())
                    {
                        if (forceDelete)
                        {
                            v.Delete();
                        }
                        else
                        {
                            v.IsPersistent = false;
                            v.Dismiss();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PeligroExtremoCallouts.CleanupManager.CleanAll_Vehicles: " + ex.Message);
                }
            }
            _vehicles.Clear();

            // Limpieza de Props
            for (int i = 0; i < _props.Count; i++)
            {
                try
                {
                    Rage.Object obj = _props[i];
                    if (obj != null && obj.Exists())
                    {
                        obj.Detach();
                        obj.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PeligroExtremoCallouts.CleanupManager.CleanAll_Props: " + ex.Message);
                }
            }
            _props.Clear();
        }
    }
}
