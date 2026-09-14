using System;
using System.Collections.Generic;
using Rage;

namespace AlternativeCallouts.Core
{
    public class AlternativeCleanupManager
    {
        private readonly List<Ped> _peds = new List<Ped>();
        private readonly List<Vehicle> _vehicles = new List<Vehicle>();
        private readonly List<Blip> _blips = new List<Blip>();
        private readonly List<Rage.Object> _props = new List<Rage.Object>();
        private readonly List<GameFiber> _fibers = new List<GameFiber>();
        private readonly List<Action> _customCleanups = new List<Action>();

        public void RegisterPed(Ped ped)
        {
            if (ped != null && !_peds.Contains(ped)) _peds.Add(ped);
        }

        public void RegisterVehicle(Vehicle vehicle)
        {
            if (vehicle != null && !_vehicles.Contains(vehicle)) _vehicles.Add(vehicle);
        }

        public void RegisterBlip(Blip blip)
        {
            if (blip != null && !_blips.Contains(blip)) _blips.Add(blip);
        }

        public void RegisterProp(Rage.Object prop)
        {
            if (prop != null && !_props.Contains(prop)) _props.Add(prop);
        }

        public void RegisterFiber(GameFiber fiber)
        {
            if (fiber != null && !_fibers.Contains(fiber)) _fibers.Add(fiber);
        }

        public void RegisterCustomCleanup(Action action)
        {
            if (action != null) _customCleanups.Add(action);
        }

        public void CleanAll(bool forceDelete = false)
        {
            for (int i = 0; i < _fibers.Count; i++)
            {
                try { if (_fibers[i] != null && _fibers[i].IsAlive) _fibers[i].Abort(); } catch (Exception ex) { Game.LogTrivial("AlternativeCallouts.AlternativeCleanupManager.CleanAll_Fibers: " + ex.Message); }
            }
            _fibers.Clear();

            for (int i = 0; i < _customCleanups.Count; i++)
            {
                try { if (_customCleanups[i] != null) _customCleanups[i](); } catch (Exception ex) { Game.LogTrivial("AlternativeCallouts.AlternativeCleanupManager.CleanAll_Custom: " + ex.Message); }
            }
            _customCleanups.Clear();

            for (int i = 0; i < _blips.Count; i++)
            {
                try { if (_blips[i] != null && _blips[i].Exists()) { _blips[i].DisableRoute(); _blips[i].Delete(); } } catch (Exception ex) { Game.LogTrivial("AlternativeCallouts.AlternativeCleanupManager.CleanAll_Blips: " + ex.Message); }
            }
            _blips.Clear();

            for (int i = 0; i < _peds.Count; i++)
            {
                try
                {
                    Ped p = _peds[i];
                    if (p != null && p.Exists())
                    {
                        p.BlockPermanentEvents = false;
                        p.KeepTasks = false;
                        if (forceDelete) p.Delete();
                        else { p.Tasks.Clear(); p.IsPersistent = false; p.Dismiss(); }
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("AlternativeCallouts.AlternativeCleanupManager.CleanAll_Peds: " + ex.Message);
                }
            }
            _peds.Clear();

            for (int i = 0; i < _vehicles.Count; i++)
            {
                try
                {
                    Vehicle v = _vehicles[i];
                    if (v != null && v.Exists())
                    {
                        if (forceDelete) v.Delete();
                        else { v.IsPersistent = false; v.Dismiss(); }
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("AlternativeCallouts.AlternativeCleanupManager.CleanAll_Vehicles: " + ex.Message);
                }
            }
            _vehicles.Clear();

            for (int i = 0; i < _props.Count; i++)
            {
                try { if (_props[i] != null && _props[i].Exists()) _props[i].Delete(); } catch (Exception ex) { Game.LogTrivial("AlternativeCallouts.AlternativeCleanupManager.CleanAll_Props: " + ex.Message); }
            }
            _props.Clear();
        }
    }
}
