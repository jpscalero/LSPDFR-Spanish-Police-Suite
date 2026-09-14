using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace AlternativeCallouts.Core
{
    public enum EAlternativeOutcome
    {
        PeacefulCompliance = 0,     // Alternativa 1: Cooperacion / Resolucion pacifica / Mediacion
        ResistAndFight = 1,          // Alternativa 2: Resistencia activa / Forcejeo / Arma blanca
        FootPursuit = 2,             // Alternativa 3: Fuga a pie por callejones o azoteas
        VehiclePursuit = 3,          // Alternativa 4: Fuga en vehiculo / Persecucion a motor
        SevereCrimeTwist = 4         // Alternativa 5: Giro critico mayor / Crimen encubierto (armas de fuego, rehenes, drogas)
    }

    public enum ECalloutPhase
    {
        EnRoute,
        Staging,
        Arrival,
        Active,
        Resolution,
        Cleanup
    }

    public abstract class BaseAlternativeCallout : Callout
    {
        public abstract string CalloutIdentifier { get; }
        public abstract string DisplayName { get; }
        public abstract string DispatchSubtitle { get; }
        public abstract int ThreatLevel { get; }

        public EAlternativeOutcome SelectedOutcome { get; protected set; }
        public ECalloutPhase CurrentPhase { get; protected set; }

        protected Vector3 SpawnPoint { get; set; }
        protected Blip DestinationBlip { get; set; }
        protected bool IsCompleted { get; set; }
        public AlternativeCleanupManager Cleaner { get; private set; }

        protected readonly List<Ped> Suspects = new List<Ped>();
        protected readonly List<Ped> Civilians = new List<Ped>();
        protected readonly List<Vehicle> Vehicles = new List<Vehicle>();
        protected readonly List<Rage.Object> ActiveProps = new List<Rage.Object>();
        protected readonly Dictionary<Ped, Blip> SuspectBlips = new Dictionary<Ped, Blip>();
        protected readonly Dictionary<Ped, Blip> CivilianBlips = new Dictionary<Ped, Blip>();

        protected LHandle ActivePursuit { get; set; }
        private static readonly Random _rnd = new Random();
        private uint _lastAiTick = 0;
        protected const uint AI_TICK_INTERVAL_MS = 450;

        protected BaseAlternativeCallout()
        {
            Cleaner = new AlternativeCleanupManager();
            CurrentPhase = ECalloutPhase.EnRoute;
            IsCompleted = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            try
            {
                if (!AlternativeConfigManager.IsCalloutEnabled(CalloutIdentifier)) return false;

                int forced = AlternativeConfigManager.GetForcedOutcome(CalloutIdentifier);
                if (forced >= 0 && forced <= 4)
                {
                    SelectedOutcome = (EAlternativeOutcome)forced;
                }
                else
                {
                    SelectedOutcome = (EAlternativeOutcome)_rnd.Next(0, 5);
                }

                SpawnPoint = SelectSpawnLocation();
                if (SpawnPoint == Vector3.Zero || SpawnPoint.DistanceTo(Game.LocalPlayer.Character.Position) < 50f)
                {
                    SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(250f));
                }

                CalloutPosition = SpawnPoint;
                CalloutMessage = DisplayName;
                ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 50f);
                AddMinimumDistanceCheck(40f, SpawnPoint);

                return base.OnBeforeCalloutDisplayed();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts: Error en OnBeforeCalloutDisplayed: " + ex.Message);
                return false;
            }
        }

        public override bool OnCalloutAccepted()
        {
            try
            {
                DestinationBlip = new Blip(SpawnPoint);
                DestinationBlip.Color = Color.Yellow;
                DestinationBlip.EnableRoute(Color.Yellow);
                Cleaner.RegisterBlip(DestinationBlip);

                AlternativeDispatchHelper.SendDispatchNotification(DisplayName, "En Camino", DispatchSubtitle);
                CurrentPhase = ECalloutPhase.EnRoute;

                return base.OnCalloutAccepted();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts: Error en OnCalloutAccepted: " + ex.Message);
                End();
                return false;
            }
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
            Cleaner.CleanAll(true);
        }

        public override void Process()
        {
            base.Process();

            if (CurrentPhase == ECalloutPhase.Cleanup || IsCompleted) return;

            try
            {
                Ped player = Game.LocalPlayer.Character;
                if (player == null || !player.Exists()) return;

                float distance = Vector2.Distance(
                    new Vector2(player.Position.X, player.Position.Y),
                    new Vector2(SpawnPoint.X, SpawnPoint.Y)
                );

                if (CurrentPhase == ECalloutPhase.EnRoute && distance <= 350f)
                {
                    CurrentPhase = ECalloutPhase.Staging;
                    GameFiber fiber = GameFiber.StartNew(delegate
                    {
                        InitializeScenario();
                        CurrentPhase = ECalloutPhase.Arrival;
                    });
                    Cleaner.RegisterFiber(fiber);
                }

                if (CurrentPhase == ECalloutPhase.Arrival && distance <= 45f)
                {
                    CurrentPhase = ECalloutPhase.Active;
                    if (DestinationBlip != null && DestinationBlip.Exists())
                    {
                        DestinationBlip.DisableRoute();
                    }

                    for (int i = 0; i < Suspects.Count; i++)
                    {
                        Ped s = Suspects[i];
                        if (s != null && s.Exists() && s.IsAlive)
                        {
                            s.BlockPermanentEvents = false;
                            s.KeepTasks = true;
                        }
                    }

                    OnPlayerArrival();
                }

                uint gameTime = Game.GameTime;
                if (gameTime - _lastAiTick >= AI_TICK_INTERVAL_MS)
                {
                    _lastAiTick = gameTime;

                    if (CurrentPhase == ECalloutPhase.Active)
                    {
                        UpdateEntityBlips();
                        OnScenarioTick();
                        CheckCompletionCriteria();
                    }
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts: Error en Process: " + ex.Message);
            }
        }

        protected abstract Vector3 SelectSpawnLocation();
        protected abstract void InitializeScenario();
        protected abstract void OnPlayerArrival();
        protected abstract void OnScenarioTick();
        protected abstract void CheckCompletionCriteria();

        // ---------------- Helpers de las 5 Alternativas ----------------

        protected void TriggerPeacefulMediation(Ped suspect, string dialog)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;
            try
            {
                suspect.BlockPermanentEvents = true;
                suspect.Tasks.ClearImmediately();
                NativeFunction.Natives.TASK_STAND_STILL(suspect, 15000);
                Game.DisplaySubtitle(dialog);
            }
            catch { }
        }

        protected void TriggerResistAndFight(Ped suspect, WeaponHash weapon, string shout)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;
            try
            {
                suspect.BlockPermanentEvents = false;
                suspect.KeepTasks = true;
                suspect.Inventory.Weapons.Add(weapon).Ammo = -1;
                ConfigurePedCombat(suspect, 70, 0, true, 2);
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists())
                {
                    suspect.Tasks.FightAgainst(player);
                }
                Game.DisplaySubtitle(shout);
            }
            catch { }
        }

        protected void TriggerFootPursuit(Ped runner, string shout)
        {
            if (runner == null || !runner.Exists() || !runner.IsAlive) return;
            try
            {
                runner.BlockPermanentEvents = false;
                runner.KeepTasks = true;
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists())
                {
                    NativeFunction.Natives.TASK_SMART_FLEE_PED(runner, player, 350f, -1, false, false);
                }
                ActivePursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(ActivePursuit, runner);
                Functions.SetPursuitIsActiveForPlayer(ActivePursuit, true);
                Game.DisplaySubtitle(shout);
            }
            catch { }
        }

        protected void TriggerVehiclePursuit(Ped runner, Vehicle vehicle, string shout)
        {
            if (runner == null || !runner.Exists() || !runner.IsAlive) return;
            if (vehicle == null || !vehicle.Exists()) return;
            try
            {
                runner.BlockPermanentEvents = true;
                runner.Tasks.EnterVehicle(vehicle, -1);

                GameFiber fiber = GameFiber.StartNew(delegate
                {
                    uint start = Game.GameTime;
                    while (runner.Exists() && runner.IsAlive && !runner.IsInVehicle(vehicle, false) && (Game.GameTime - start < 10000))
                    {
                        GameFiber.Sleep(400);
                    }

                    if (runner.Exists() && runner.IsAlive && runner.IsInVehicle(vehicle, false))
                    {
                        runner.BlockPermanentEvents = false;
                        vehicle.IsEngineOn = true;
                        ActivePursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(ActivePursuit, runner);
                        Functions.SetPursuitIsActiveForPlayer(ActivePursuit, true);
                        Game.DisplaySubtitle(shout);
                    }
                });
                Cleaner.RegisterFiber(fiber);
            }
            catch { }
        }

        protected void TriggerSevereCrimeTwist(Ped suspect, WeaponHash weapon, string shout)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;
            try
            {
                suspect.BlockPermanentEvents = false;
                suspect.KeepTasks = true;
                suspect.Inventory.Weapons.Add(weapon).Ammo = -1;
                ConfigurePedCombat(suspect, 80, 50, true, 2);
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists())
                {
                    suspect.Tasks.FightAgainst(player);
                }
                Game.DisplaySubtitle(shout);
            }
            catch { }
        }

        // ---------------- Helpers de Spawneo y Limpieza ----------------

        protected float GetGroundZ(Vector3 pos)
        {
            try
            {
                float groundZ = 0f;
                if (NativeFunction.Natives.GET_GROUND_Z_FOR_3D_COORD<bool>(pos.X, pos.Y, pos.Z + 5f, out groundZ, false))
                {
                    return groundZ;
                }
            }
            catch { }
            return pos.Z;
        }

        protected Ped SpawnPed(string modelName, Vector3 position, float heading, bool snapToGround = true)
        {
            try
            {
                Model m = new Model(modelName);
                m.LoadAndWait();
                Vector3 safePos = snapToGround ? new Vector3(position.X, position.Y, GetGroundZ(position)) : position;
                Ped p = new Ped(m, safePos, heading);
                p.IsPersistent = true;
                p.BlockPermanentEvents = true;
                Cleaner.RegisterPed(p);
                return p;
            }
            catch { return null; }
        }

        protected Vehicle SpawnVehicle(string modelName, Vector3 position, float heading, bool snapToGround = true)
        {
            try
            {
                Model m = new Model(modelName);
                m.LoadAndWait();
                Vector3 safePos = snapToGround ? new Vector3(position.X, position.Y, GetGroundZ(position) + 0.2f) : position;
                Vehicle v = new Vehicle(m, safePos, heading);
                v.IsPersistent = true;
                Cleaner.RegisterVehicle(v);
                Vehicles.Add(v);
                return v;
            }
            catch { return null; }
        }

        protected Rage.Object SpawnProp(string modelName, Vector3 position, float heading)
        {
            try
            {
                Model m = new Model(modelName);
                m.LoadAndWait();
                Vector3 safePos = new Vector3(position.X, position.Y, GetGroundZ(position));
                Rage.Object prop = new Rage.Object(m, safePos, heading);
                prop.IsPersistent = true;
                Cleaner.RegisterProp(prop);
                ActiveProps.Add(prop);
                return prop;
            }
            catch { return null; }
        }

        protected Blip AttachBlipToSuspect(Ped suspect, Color color, float scale = 0.65f)
        {
            if (suspect == null || !suspect.Exists()) return null;
            try
            {
                Blip b = suspect.AttachBlip();
                b.Color = color;
                b.Scale = scale;
                Cleaner.RegisterBlip(b);
                SuspectBlips[suspect] = b;
                return b;
            }
            catch { return null; }
        }

        protected Blip AttachBlipToCivilian(Ped civ, Color color, float scale = 0.55f)
        {
            if (civ == null || !civ.Exists()) return null;
            try
            {
                Blip b = civ.AttachBlip();
                b.Color = color;
                b.Scale = scale;
                Cleaner.RegisterBlip(b);
                CivilianBlips[civ] = b;
                return b;
            }
            catch { return null; }
        }

        protected void UpdateEntityBlips()
        {
            List<Ped> deadSuspects = new List<Ped>();
            foreach (KeyValuePair<Ped, Blip> pair in SuspectBlips)
            {
                Ped s = pair.Key;
                Blip b = pair.Value;
                if (s == null || !s.Exists() || !s.IsAlive || Functions.IsPedArrested(s))
                {
                    if (b != null && b.Exists()) b.Delete();
                    deadSuspects.Add(s);
                }
            }
            for (int i = 0; i < deadSuspects.Count; i++) SuspectBlips.Remove(deadSuspects[i]);
        }

        protected void ConfigurePedCombat(Ped ped, int accuracy, int armor, bool canLeaveVehicle = true, int combatMovement = 2)
        {
            if (ped == null || !ped.Exists()) return;
            try
            {
                ped.Armor = armor;
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 0, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 1, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 2, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 3, canLeaveVehicle);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 5, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 46, true);
                NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(ped, combatMovement);
                NativeFunction.Natives.SET_PED_ACCURACY(ped, accuracy);
                NativeFunction.Natives.SET_PED_SEEING_RANGE(ped, 80.0f);
                NativeFunction.Natives.SET_PED_HEARING_RANGE(ped, 80.0f);
            }
            catch { }
        }

        protected void NotifyCode4()
        {
            CurrentPhase = ECalloutPhase.Cleanup;
            IsCompleted = true;
            AlternativeDispatchHelper.SendDispatchNotification(DisplayName, "Codigo 4", "Situacion bajo control. Unidades disponibles.");
            End();
        }

        public override void End()
        {
            base.End();
            try
            {
                CurrentPhase = ECalloutPhase.Cleanup;
                if (DestinationBlip != null && DestinationBlip.Exists()) DestinationBlip.DisableRoute();
                Cleaner.CleanAll(false);
                Suspects.Clear();
                Civilians.Clear();
                Vehicles.Clear();
                ActiveProps.Clear();
                SuspectBlips.Clear();
                CivilianBlips.Clear();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts: Error al finalizar llamada: " + ex.Message);
            }
        }
    }
}
