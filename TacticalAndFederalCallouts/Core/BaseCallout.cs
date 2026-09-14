using PoliceSuite.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace TacticalAndFederalCallouts.Core
{
    public enum ECalloutPhase { EnRoute, Staging, Infiltration, ActiveEngagement, Escalation, Resolution, Cleanup }

    /// <summary>
    /// BaseCallout refactorizada (<300 lineas): Ciclo de vida y delegacion a helpers de PoliceSuite.Common.
    /// </summary>
    public abstract class BaseCallout : Callout
    {
        public abstract string CalloutIdentifier { get; }
        public abstract string DisplayName { get; }
        public abstract string DispatchSubtitle { get; }
        public abstract int ThreatLevel { get; }
        public ECalloutPhase CurrentPhase { get; protected set; }
        protected Vector3 SpawnPoint { get; set; }
        protected Blip DestinationBlip { get; set; }
        protected bool IsCompleted { get; set; }
        public CleanupManager Cleaner { get; private set; }
        public float StagingDistance { get; protected set; }
        public float EngagementDistance { get; protected set; }
        protected readonly List<Ped> Suspects = new List<Ped>();
        protected readonly List<Ped> Hostages = new List<Ped>();
        protected readonly List<Ped> FriendlyPeds = new List<Ped>();
        protected readonly List<Vehicle> Vehicles = new List<Vehicle>();
        protected readonly List<Rage.Object> ActiveProps = new List<Rage.Object>();
        protected readonly Dictionary<Ped, Blip> SuspectBlips = new Dictionary<Ped, Blip>();
        protected readonly Dictionary<Ped, Blip> HostageBlips = new Dictionary<Ped, Blip>();
        private uint _lastAiTick = 0;
        protected const uint AI_TICK_INTERVAL_MS = 450;
        protected BaseCallout()
        {
            Cleaner = new CleanupManager();
            CurrentPhase = ECalloutPhase.EnRoute;
            IsCompleted = false;
            StagingDistance = 350f;
            EngagementDistance = 55f;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            try
            {
                SpawnPoint = SelectSpawnLocation();
                if (SpawnPoint == Vector3.Zero || SpawnPoint.DistanceTo(Game.LocalPlayer.Character.Position) < 50f)
                    SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(260f));

                CalloutPosition = SpawnPoint;
                CalloutMessage = DisplayName;
                ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 50f);
                AddMinimumDistanceCheck(ConfigManager.GetMinDistance(CalloutIdentifier, 40f), SpawnPoint);
                return base.OnBeforeCalloutDisplayed();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts: Error en OnBeforeCalloutDisplayed: " + ex.Message);
                return false;
            }
        }

        public override bool OnCalloutAccepted()
        {
            try
            {
                DestinationBlip = new Blip(SpawnPoint);
                DestinationBlip.Color = Color.Red;
                DestinationBlip.EnableRoute(Color.Red);
                Cleaner.RegisterBlip(DestinationBlip);
                DispatchHelper.SendDispatchNotification(DisplayName, "Unidad en Camino", DispatchSubtitle);
                SpanishVoiceDispatchEngine.AnnounceCalloutDispatched(DisplayName);
                CurrentPhase = ECalloutPhase.EnRoute;
                return base.OnCalloutAccepted();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts: Error en OnCalloutAccepted: " + ex.Message);
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

                if (CurrentPhase == ECalloutPhase.EnRoute && distance <= StagingDistance)
                {
                    CurrentPhase = ECalloutPhase.Staging;
                    GameFiber fiber = GameFiber.StartNew(delegate
                    {
                        try
                        {
                            InitializeScenario();
                            CurrentPhase = ECalloutPhase.Infiltration;
                            OnStagingCompleted();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("TacticalAndFederalCallouts.BaseCallout.Staging: " + ex.Message);
                        }
                    });
                    Cleaner.RegisterFiber(fiber);
                }

                if (CurrentPhase == ECalloutPhase.Infiltration)
                {
                    OnInfiltrationTick();
                    if (distance <= EngagementDistance || CheckBreachTriggered()) TransitionToEngagement();
                }

                uint gameTime = Game.GameTime;
                if (gameTime - _lastAiTick >= AI_TICK_INTERVAL_MS)
                {
                    _lastAiTick = gameTime;
                    if (CurrentPhase == ECalloutPhase.ActiveEngagement || CurrentPhase == ECalloutPhase.Escalation)
                    {
                        UpdateEntityBlips();
                        CombatRealismManager.ProcessCombatTick(Suspects, player);
                        for (int s = 0; s < Suspects.Count; s++)
                            TacticalNegotiationManager.CheckAndExecuteSurrender(Suspects[s], player, FriendlyPeds);
                        OnScenarioTick();
                        CheckCompletionCriteria();
                    }
                    if (CurrentPhase == ECalloutPhase.Resolution) OnResolutionTick();
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts: Error en Process tick: " + ex.Message);
            }
        }

        protected virtual void TransitionToEngagement()
        {
            CurrentPhase = ECalloutPhase.ActiveEngagement;
            if (DestinationBlip != null && DestinationBlip.Exists())
            {
                DestinationBlip.DisableRoute();
                DestinationBlip.Delete();
                DestinationBlip = null;
            }
            UnlockPeds(Suspects);
            UnlockPeds(FriendlyPeds);
            OnPlayerArrival();
            if (ConfigManager.AutoSwatBackup && ConfigManager.RequiresSwatBackup(CalloutIdentifier, true))
            {
                DispatchHelper.RequestTacticalBackup(SpawnPoint, true, ConfigManager.AutoAirSupport);
            }
        }

        private void UnlockPeds(List<Ped> peds)
        {
            for (int i = 0; i < peds.Count; i++)
            {
                Ped p = peds[i];
                if (p != null && p.Exists() && p.IsAlive) { p.BlockPermanentEvents = false; p.KeepTasks = true; }
            }
        }

        protected virtual bool CheckBreachTriggered() { return false; }
        protected virtual void OnStagingCompleted() { }
        protected virtual void OnInfiltrationTick() { }
        protected virtual void OnResolutionTick() { }
        protected abstract Vector3 SelectSpawnLocation();
        protected abstract void InitializeScenario();
        protected abstract void OnPlayerArrival();
        protected abstract void OnScenarioTick();
        protected abstract void CheckCompletionCriteria();
        // ---------------- Helpers Delegados a PoliceSuite.Common ----------------
        protected float GetGroundZ(Vector3 pos) { return CalloutSpawner.GetGroundZ(pos); }

        protected Ped SpawnPed(string modelName, Vector3 position, float heading, bool snapToGround = true)
        {
            Ped p = CalloutSpawner.SpawnPed(modelName, position, heading, snapToGround);
            if (p != null) { CombatRealismManager.InitializePedRealism(p); Cleaner.RegisterPed(p); }
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

        protected Blip AttachBlipToHostage(Ped target, Color color, float scale = 0.55f)
        {
            Blip b = CalloutBlips.AttachBlip(target, color, scale);
            if (b != null) { Cleaner.RegisterBlip(b); HostageBlips[target] = b; }
            return b;
        }

        protected void UpdateEntityBlips()
        {
            CalloutBlips.UpdatePedBlips(SuspectBlips, true);
            CalloutBlips.UpdatePedBlips(HostageBlips, false);
        }

        protected void ConfigurePedCombat(Ped ped, int accuracy, int armor, bool canLeaveVehicle = true, int combatMovement = 2)
        {
            CalloutEffects.ConfigurePedCombat(ped, accuracy, armor, canLeaveVehicle, combatMovement);
        }

        public void TriggerExplosion(Vector3 pos, int explosionType = 2, float damageScale = 5.0f)
        {
            CalloutEffects.TriggerExplosion(pos, explosionType, damageScale);
        }

        public void PlayPtfx(string asset, string effect, Vector3 pos, float scale = 1.0f)
        {
            CalloutEffects.PlayPtfx(asset, effect, pos, scale);
        }

        public Vehicle SpawnHostileReinforcementVehicle(string vehicleModel, string pedModel, WeaponHash weapon, int suspectCount, Vector3 spawnCoord, Vector3 targetCoord)
        {
            return CalloutSpawner.SpawnHostileReinforcements(vehicleModel, pedModel, weapon, suspectCount, spawnCoord, targetCoord, Suspects, SuspectBlips,
                delegate (object obj) {
                    if (obj is Ped) Cleaner.RegisterPed((Ped)obj);
                    else if (obj is Vehicle) Cleaner.RegisterVehicle((Vehicle)obj);
                    else if (obj is Blip) Cleaner.RegisterBlip((Blip)obj);
                    else if (obj is GameFiber) Cleaner.RegisterFiber((GameFiber)obj);
                },
                delegate (Ped p) { CombatRealismManager.InitializePedRealism(p); }, ConfigManager.SuspectAccuracy);
        }

        public void TriggerVehicleEscape(Ped runner, Vehicle escapeVehicle, Vector3 destination)
        {
            CalloutEffects.TriggerVehicleEscape(runner, escapeVehicle, destination, delegate (object f) { if (f is GameFiber) Cleaner.RegisterFiber((GameFiber)f); });
        }

        public void TriggerEnvironmentalHazard(Vector3 pos, string fxAsset, string fxName, float radius, int durationMs)
        {
            CalloutEffects.TriggerEnvironmentalHazard(pos, fxAsset, fxName, radius, durationMs, delegate (object f) { if (f is GameFiber) Cleaner.RegisterFiber((GameFiber)f); });
        }

        public void TriggerScriptAlarm(string alarmName)
        {
            CalloutEffects.TriggerScriptAlarm(alarmName, delegate (Action cleanupAction) { Cleaner.RegisterCustomCleanup(cleanupAction); });
        }

        public void ExtractHostageToSafety(Ped hostage, Vector3 safePoint)
        {
            CalloutEffects.ExtractHostageToSafety(hostage, safePoint, delegate (object f) { if (f is GameFiber) Cleaner.RegisterFiber((GameFiber)f); });
        }

        protected void NotifyCode4()
        {
            CurrentPhase = ECalloutPhase.Cleanup;
            IsCompleted = true;
            DispatchHelper.SendDispatchNotification(DisplayName, "Codigo 4", "Objetivo asegurado. La situacion esta bajo control.");
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
                Hostages.Clear();
                FriendlyPeds.Clear();
                Vehicles.Clear();
                ActiveProps.Clear();
                SuspectBlips.Clear();
                HostageBlips.Clear();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts: Error al finalizar llamada: " + ex.Message);
            }
        }
    }
}
