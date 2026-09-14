using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace PeligroExtremoCallouts.Core
{
    public enum ECalloutPhase
    {
        EnRoute,
        Staging,
        Infiltration,
        ActiveEngagement,
        Escalation,
        Resolution,
        Cleanup
    }

    /// <summary>
    /// Clase base abstracta para llamadas de PeligroExtremoCallouts (Tiroteos, Asaltos y Persecuciones Peligrosas).
    /// Administra ciclo de vida, maquina de estados ECalloutPhase, spawn diferido seguro a <350m,
    /// tracking reactivo de blips por entidad, integracion con GameFiber y limpieza estricta.
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
        protected readonly List<Ped> Victims = new List<Ped>();
        protected readonly List<Ped> FriendlyPeds = new List<Ped>();
        protected readonly List<Vehicle> Vehicles = new List<Vehicle>();
        protected readonly List<Rage.Object> ActiveProps = new List<Rage.Object>();
        protected readonly Dictionary<Ped, Blip> SuspectBlips = new Dictionary<Ped, Blip>();
        protected readonly Dictionary<Ped, Blip> VictimBlips = new Dictionary<Ped, Blip>();

        protected BaseCallout()
        {
            Cleaner = new CleanupManager();
            CurrentPhase = ECalloutPhase.EnRoute;
            IsCompleted = false;
            StagingDistance = 350f;
            EngagementDistance = 75f;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            try
            {
                SpawnPoint = SelectSpawnLocation();
                if (SpawnPoint == Vector3.Zero || SpawnPoint.DistanceTo(Game.LocalPlayer.Character.Position) < 50f)
                {
                    SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(260f));
                }

                CalloutPosition = SpawnPoint;
                CalloutMessage = DisplayName;
                ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 55f);
                AddMinimumDistanceCheck(ConfigManager.GetMinDistance(CalloutIdentifier, 40f), SpawnPoint);

                return base.OnBeforeCalloutDisplayed();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts: Error en OnBeforeCalloutDisplayed: " + ex.Message);
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
                CurrentPhase = ECalloutPhase.EnRoute;

                return base.OnCalloutAccepted();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts: Error en OnCalloutAccepted: " + ex.Message);
                End();
                return false;
            }
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
            Cleaner.CleanAll(true);
        }

        private uint _lastAiTick = 0;
        protected const uint AI_TICK_INTERVAL_MS = 450;

        public override void Process()
        {
            base.Process();

            if (CurrentPhase == ECalloutPhase.Cleanup || IsCompleted) return;

            try
            {
                Ped player = Game.LocalPlayer.Character;
                if (player == null || !player.Exists()) return;

                // Distancia horizontal 2D para no sufrir bloqueos en desniveles o azoteas
                float distance = Vector2.Distance(
                    new Vector2(player.Position.X, player.Position.Y),
                    new Vector2(SpawnPoint.X, SpawnPoint.Y)
                );

                // FASE 1: En camino -> Llegada a radio de staging (<350m): Spawn diferido seguro
                if (CurrentPhase == ECalloutPhase.EnRoute && distance <= StagingDistance)
                {
                    CurrentPhase = ECalloutPhase.Staging;
                    GameFiber fiber = GameFiber.StartNew(delegate
                    {
                        InitializeScenario();
                        CurrentPhase = ECalloutPhase.Infiltration;
                        OnStagingCompleted();
                    });
                    Cleaner.RegisterFiber(fiber);
                }

                // FASE 2: Infiltracion / Aproximacion (<100m)
                if (CurrentPhase == ECalloutPhase.Infiltration)
                {
                    OnInfiltrationTick();

                    if (distance <= EngagementDistance || CheckBreachTriggered())
                    {
                        TransitionToEngagement();
                    }
                }

                // FASE 3, 4 y 5 con Estrangulador de IA (450ms) para evitar Task Thrashing a 60 FPS
                uint gameTime = Game.GameTime;
                if (gameTime - _lastAiTick >= AI_TICK_INTERVAL_MS)
                {
                    _lastAiTick = gameTime;

                    if (CurrentPhase == ECalloutPhase.ActiveEngagement || CurrentPhase == ECalloutPhase.Escalation)
                    {
                        UpdateEntityBlips();
                        CombatRealismManager.ProcessCombatTick(Suspects, player);
                        for (int s = 0; s < Suspects.Count; s++)
                        {
                            TacticalNegotiationManager.CheckAndExecuteSurrender(Suspects[s], player, FriendlyPeds);
                        }
                        OnScenarioTick();
                        CheckCompletionCriteria();
                    }

                    if (CurrentPhase == ECalloutPhase.Resolution)
                    {
                        OnResolutionTick();
                    }
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts: Error en Process tick: " + ex.Message);
            }
        }

        protected virtual void TransitionToEngagement()
        {
            CurrentPhase = ECalloutPhase.ActiveEngagement;

            if (DestinationBlip != null && DestinationBlip.Exists())
            {
                DestinationBlip.DisableRoute();
            }

            // Desbloqueo del motor nativo de IA para sospechosos
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive)
                {
                    s.BlockPermanentEvents = false;
                    s.KeepTasks = true;
                }
            }

            // Desbloqueo del motor nativo de IA para aliados
            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped ally = FriendlyPeds[i];
                if (ally != null && ally.Exists() && ally.IsAlive)
                {
                    ally.BlockPermanentEvents = false;
                    ally.KeepTasks = true;
                }
            }

            OnPlayerArrival();

            if (ConfigManager.AutoBackup)
            {
                DispatchHelper.RequestCode3Backup(SpawnPoint, ThreatLevel >= 5, ThreatLevel >= 4);
            }
        }

        protected virtual bool CheckBreachTriggered()
        {
            return false;
        }

        protected virtual void OnStagingCompleted() { }
        protected virtual void OnInfiltrationTick() { }
        protected virtual void OnResolutionTick() { }

        protected abstract Vector3 SelectSpawnLocation();
        protected abstract void InitializeScenario();
        protected abstract void OnPlayerArrival();
        protected abstract void OnScenarioTick();
        protected abstract void CheckCompletionCriteria();

        // ---------------- Helpers de Entidades y Terreno ----------------

        protected float GetGroundZ(Vector3 pos)
        {
            try
            {
                float groundZ = 0f;
                bool hasGround = NativeFunction.Natives.GET_GROUND_Z_FOR_3D_COORD<bool>(pos.X, pos.Y, pos.Z + 5f, out groundZ, false);

                float waterZ = 0f;
                bool hasWater = NativeFunction.Natives.GET_WATER_HEIGHT<bool>(pos.X, pos.Y, pos.Z, out waterZ);

                if (hasGround && hasWater)
                {
                    // Si hay una estructura sólida (muelles elevados, puentes) por encima del agua
                    if (groundZ >= waterZ - 0.2f)
                    {
                        return groundZ;
                    }
                    return waterZ;
                }

                if (hasGround)
                {
                    return groundZ;
                }

                if (hasWater)
                {
                    return waterZ;
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
                Vector3 safePos;
                if (snapToGround)
                {
                    float groundZ = GetGroundZ(position);
                    safePos = new Vector3(position.X, position.Y, groundZ);
                }
                else
                {
                    safePos = position;
                }

                Ped p = new Ped(m, safePos, heading);
                p.IsPersistent = true;
                p.BlockPermanentEvents = true;
                CombatRealismManager.InitializePedRealism(p);
                Cleaner.RegisterPed(p);
                return p;
            }
            catch
            {
                return null;
            }
        }

        protected Vehicle SpawnVehicle(string modelName, Vector3 position, float heading, bool snapToGround = true)
        {
            try
            {
                Model m = new Model(modelName);
                m.LoadAndWait();
                Vector3 safePos;

                if (snapToGround)
                {
                    float groundZ = GetGroundZ(position);
                    safePos = new Vector3(position.X, position.Y, groundZ + 0.2f);
                }
                else
                {
                    safePos = position;
                }

                Vehicle v = new Vehicle(m, safePos, heading);
                v.IsPersistent = true;
                Cleaner.RegisterVehicle(v);
                Vehicles.Add(v);
                return v;
            }
            catch
            {
                return null;
            }
        }

        protected Rage.Object SpawnProp(string modelName, Vector3 position, float heading)
        {
            try
            {
                Model m = new Model(modelName);
                m.LoadAndWait();
                float groundZ = GetGroundZ(position);
                Vector3 safePos = new Vector3(position.X, position.Y, groundZ);

                Rage.Object prop = new Rage.Object(m, safePos, heading);
                prop.IsPersistent = true;
                Cleaner.RegisterProp(prop);
                ActiveProps.Add(prop);
                return prop;
            }
            catch
            {
                return null;
            }
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
            catch
            {
                return null;
            }
        }

        protected Blip AttachBlipToVictim(Ped victim, Color color, float scale = 0.55f)
        {
            if (victim == null || !victim.Exists()) return null;
            try
            {
                Blip b = victim.AttachBlip();
                b.Color = color;
                b.Scale = scale;
                Cleaner.RegisterBlip(b);
                VictimBlips[victim] = b;
                return b;
            }
            catch
            {
                return null;
            }
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
                    if (b != null && b.Exists())
                    {
                        b.Delete();
                    }
                    deadSuspects.Add(s);
                }
            }
            for (int i = 0; i < deadSuspects.Count; i++)
            {
                SuspectBlips.Remove(deadSuspects[i]);
            }

            List<Ped> resolvedVictims = new List<Ped>();
            foreach (KeyValuePair<Ped, Blip> pair in VictimBlips)
            {
                Ped v = pair.Key;
                Blip b = pair.Value;
                if (v == null || !v.Exists() || !v.IsAlive)
                {
                    if (b != null && b.Exists())
                    {
                        b.Delete();
                    }
                    resolvedVictims.Add(v);
                }
            }
            for (int i = 0; i < resolvedVictims.Count; i++)
            {
                VictimBlips.Remove(resolvedVictims[i]);
            }
        }

        protected void ConfigurePedCombat(Ped ped, int accuracy, int armor, bool canLeaveVehicle = true, int combatMovement = 2)
        {
            if (ped == null || !ped.Exists()) return;
            try
            {
                ped.Armor = armor;
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 0, true); // BF_CanUseCover
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 1, true); // BF_UseVehicle
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 2, true); // BF_DriveBy
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 3, canLeaveVehicle); // BF_CanLeaveVehicle
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 5, true); // BF_CanFightArmedPedsWhenNotArmed
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 8, true); // BF_BlindFireInCover
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 46, true); // BF_AlwaysFight
                NativeFunction.Natives.SET_PED_COMBAT_ABILITY(ped, 2); // Professional
                NativeFunction.Natives.SET_PED_COMBAT_RANGE(ped, 2); // Far
                NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(ped, combatMovement); // 2 = Advance / Offensive
                NativeFunction.Natives.SET_PED_ACCURACY(ped, accuracy);
                NativeFunction.Natives.SET_PED_SEEING_RANGE(ped, 100.0f);
                NativeFunction.Natives.SET_PED_HEARING_RANGE(ped, 90.0f);
                NativeFunction.Natives.SET_PED_TARGET_LOSS_RESPONSE(ped, 1); // Hunt / Search
                NativeFunction.Natives.SET_PED_FLEE_ATTRIBUTES(ped, 0, false);
            }
            catch { }
        }

        public void TriggerExplosion(Vector3 pos, int explosionType = 2, float damageScale = 5.0f)
        {
            try
            {
                NativeFunction.Natives.ADD_EXPLOSION(pos.X, pos.Y, pos.Z, explosionType, damageScale, true, false, 1.0f);
            }
            catch { }
        }

        public void PlayPtfx(string asset, string effect, Vector3 pos, float scale = 1.0f)
        {
            try
            {
                NativeFunction.Natives.REQUEST_NAMED_PTFX_ASSET(asset);
                int timeout = 0;
                while (!NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>(asset) && timeout < 20)
                {
                    GameFiber.Yield();
                    timeout++;
                }

                if (NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>(asset))
                {
                    NativeFunction.Natives.USE_PARTICLE_FX_ASSET_NEXT_CALL(asset);
                    NativeFunction.Natives.START_PARTICLE_FX_NON_LOOPED_AT_COORD(effect, pos.X, pos.Y, pos.Z, 0f, 0f, 0f, scale, false, false, false);
                }
            }
            catch { }
        }

        protected void NotifyCode4()
        {
            CurrentPhase = ECalloutPhase.Cleanup;
            IsCompleted = true;
            DispatchHelper.SendDispatchNotification(DisplayName, "Codigo 4", "Situacion asegurada y bajo control. Buen trabajo.");
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
                }
                Cleaner.CleanAll(false);
                Suspects.Clear();
                Victims.Clear();
                FriendlyPeds.Clear();
                Vehicles.Clear();
                ActiveProps.Clear();
                SuspectBlips.Clear();
                VictimBlips.Clear();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts: Error al finalizar llamada: " + ex.Message);
            }
        }
    }
}
