using PoliceSuite.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace AlternativeCallouts.Core
{
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
                SelectedOutcome = (forced >= 0 && forced <= 4) ? (EAlternativeOutcome)forced : (EAlternativeOutcome)_rnd.Next(0, 5);

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
                SpanishVoiceDispatchEngine.AnnounceCalloutDispatched(DisplayName);
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

                float distance = Vector2.Distance(new Vector2(player.Position.X, player.Position.Y), new Vector2(SpawnPoint.X, SpawnPoint.Y));

                if (CurrentPhase == ECalloutPhase.EnRoute && distance <= 350f)
                {
                    CurrentPhase = ECalloutPhase.Staging;
                    GameFiber fiber = GameFiber.StartNew(delegate
                    {
                        try
                        {
                            InitializeScenario();
                            CurrentPhase = ECalloutPhase.Arrival;
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("AlternativeCallouts.BaseCallout.Staging: " + ex.Message);
                        }
                    });
                    Cleaner.RegisterFiber(fiber);
                }

                if (CurrentPhase == ECalloutPhase.Arrival && distance <= 45f)
                {
                    CurrentPhase = ECalloutPhase.Active;
                    if (DestinationBlip != null && DestinationBlip.Exists())
                    {
                        DestinationBlip.DisableRoute();
                        DestinationBlip.Delete();
                        DestinationBlip = null;
                    }

                    for (int i = 0; i < Suspects.Count; i++)
                    {
                        Ped s = Suspects[i];
                        if (s != null && s.Exists() && s.IsAlive) { s.BlockPermanentEvents = false; s.KeepTasks = true; }
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

        // ---------------- Helpers de las 5 Alternativas Delegados ----------------

        protected void TriggerPeacefulMediation(Ped suspect, string dialog)
        {
            AlternativeOutcomeHelper.TriggerPeacefulMediation(suspect, dialog);
        }

        protected void TriggerResistAndFight(Ped suspect, WeaponHash weapon, string shout)
        {
            AlternativeOutcomeHelper.TriggerResistAndFight(suspect, weapon, shout);
        }

        protected void TriggerFootPursuit(Ped runner, string shout)
        {
            ActivePursuit = AlternativeOutcomeHelper.TriggerFootPursuit(runner, shout);
        }

        protected void TriggerVehiclePursuit(Ped runner, Vehicle vehicle, string shout)
        {
            AlternativeOutcomeHelper.TriggerVehiclePursuit(runner, vehicle, shout, delegate(LHandle p) { ActivePursuit = p; }, Cleaner);
        }

        protected void TriggerSevereCrimeTwist(Ped suspect, WeaponHash weapon, string shout)
        {
            AlternativeOutcomeHelper.TriggerSevereCrimeTwist(suspect, weapon, shout);
        }

        // ---------------- Helpers Delegados a PoliceSuite.Common ----------------

        protected float GetGroundZ(Vector3 pos) { return CalloutSpawner.GetGroundZ(pos); }

        protected Ped SpawnPed(string modelName, Vector3 position, float heading, bool snapToGround = true)
        {
            Ped p = CalloutSpawner.SpawnPed(modelName, position, heading, snapToGround);
            if (p != null) Cleaner.RegisterPed(p);
            return p;
        }

        protected Vehicle SpawnVehicle(string modelName, Vector3 position, float heading, bool snapToGround = true)
        {
            Vehicle v = CalloutSpawner.SpawnVehicle(modelName, position, heading, snapToGround);
            if (v != null) { Cleaner.RegisterVehicle(v); Vehicles.Add(v); }
            return v;
        }

        protected Rage.Object SpawnProp(string modelName, Vector3 position, float heading)
        {
            Rage.Object prop = CalloutSpawner.SpawnProp(modelName, position, heading);
            if (prop != null) { Cleaner.RegisterProp(prop); ActiveProps.Add(prop); }
            return prop;
        }

        protected Blip AttachBlipToSuspect(Ped suspect, Color color, float scale = 0.65f)
        {
            Blip b = CalloutBlips.AttachBlip(suspect, color, scale);
            if (b != null) { Cleaner.RegisterBlip(b); SuspectBlips[suspect] = b; }
            return b;
        }

        protected Blip AttachBlipToCivilian(Ped civ, Color color, float scale = 0.55f)
        {
            Blip b = CalloutBlips.AttachBlip(civ, color, scale);
            if (b != null) { Cleaner.RegisterBlip(b); CivilianBlips[civ] = b; }
            return b;
        }

        protected void UpdateEntityBlips()
        {
            CalloutBlips.UpdatePedBlips(SuspectBlips, true);
            CalloutBlips.UpdatePedBlips(CivilianBlips, false);
        }

        protected void ConfigurePedCombat(Ped ped, int accuracy, int armor, bool canLeaveVehicle = true, int combatMovement = 2)
        {
            CalloutEffects.ConfigurePedCombat(ped, accuracy, armor, canLeaveVehicle, combatMovement);
        }

        protected void NotifyCode4()
        {
            CurrentPhase = ECalloutPhase.Cleanup;
            IsCompleted = true;
            AlternativeDispatchHelper.SendDispatchNotification(DisplayName, "Codigo 4", "Situacion bajo control. Unidades disponibles.");
            SpanishVoiceDispatchEngine.AnnounceCode4();
            End();
        }

        public override void End()
        {
            base.End();
            try
            {
                CurrentPhase = ECalloutPhase.Cleanup;
                if (DestinationBlip != null && DestinationBlip.Exists())
                {
                    DestinationBlip.DisableRoute();
                    DestinationBlip.Delete();
                    DestinationBlip = null;
                }
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
