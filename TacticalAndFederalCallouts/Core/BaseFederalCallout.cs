using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;

namespace TacticalAndFederalCallouts.Core
{
    public class EvidenceItem
    {
        public Rage.Object Prop { get; set; }
        public Vector3 Position { get; set; }
        public string Description { get; set; }
        public bool IsCollected { get; set; }
        public Blip EvidenceBlip { get; set; }
    }

    /// <summary>
    /// Especializacion para misiones de investigacion federal, operaciones encubiertas y crimenes mayores FIB.
    /// Incorpora recoleccion e inspeccion interactiva de pruebas reales, escenas del crimen acordonadas,
    /// hackeo de servidores y recuperacion de datos cifrados bajo cronometro.
    /// </summary>
    public abstract class BaseFederalCallout : BaseCallout
    {
        protected readonly List<EvidenceItem> EvidenceList = new List<EvidenceItem>();
        protected int EvidenceCollectedCount { get; set; }

        protected bool DataWipeActive { get; set; }
        protected int DataWipeSecondsRemaining { get; set; }
        protected Vector3 ServerTerminalLocation { get; set; }
        protected bool DataWipePrevented { get; set; }

        protected RelationshipGroup UndercoverGroup { get; set; }
        protected RelationshipGroup SuspectGroup { get; set; }

        private uint _lastCombatRefreshTime = 0;

        protected BaseFederalCallout()
        {
            EvidenceCollectedCount = 0;
            DataWipeActive = false;
            DataWipePrevented = false;

            try
            {
                SuspectGroup = new RelationshipGroup("FEDERAL_SUSPECTS");
                UndercoverGroup = new RelationshipGroup("FEDERAL_UNDERCOVER");
                SetupFactions();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("BaseFederalCallout.InitializeFederalGroups: " + ex.Message);
            }
        }

        protected override void OnPlayerArrival()
        {
            Game.DisplaySubtitle("~b~[OPERACION FEDERAL FIB]~w~ " + DisplayName + ". Aseguren la zona e inicien el operativo.");
            EngageFederalTargets();
        }

        private void SetupFactions()
        {
            try
            {
                RelationshipGroup playerGroup = new RelationshipGroup("PLAYER");
                RelationshipGroup copGroup = new RelationshipGroup("COP");

                Game.SetRelationshipBetweenRelationshipGroups(SuspectGroup, playerGroup, Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups(playerGroup, SuspectGroup, Relationship.Hate);

                Game.SetRelationshipBetweenRelationshipGroups(SuspectGroup, copGroup, Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups(copGroup, SuspectGroup, Relationship.Hate);

                Game.SetRelationshipBetweenRelationshipGroups(SuspectGroup, UndercoverGroup, Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups(UndercoverGroup, SuspectGroup, Relationship.Hate);

                Game.SetRelationshipBetweenRelationshipGroups(UndercoverGroup, playerGroup, Relationship.Companion);
                Game.SetRelationshipBetweenRelationshipGroups(playerGroup, UndercoverGroup, Relationship.Companion);

                Game.SetRelationshipBetweenRelationshipGroups(UndercoverGroup, copGroup, Relationship.Companion);
                Game.SetRelationshipBetweenRelationshipGroups(copGroup, UndercoverGroup, Relationship.Companion);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("BaseFederalCallout.SetupFactions: " + ex.Message);
            }
        }

        protected virtual void EngageFederalTargets()
        {
            Ped player = Game.LocalPlayer.Character;
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive)
                {
                    s.BlockPermanentEvents = false;
                    s.KeepTasks = true;
                    Ped opponent = GetNearestOpponentForSuspect(s.Position);
                    s.Tasks.FightAgainst(opponent ?? player);
                }
            }

            // Agentes encubiertos entran en combate activo contra los sospechosos
            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped undercover = FriendlyPeds[i];
                if (undercover != null && undercover.Exists() && undercover.IsAlive)
                {
                    undercover.BlockPermanentEvents = false;
                    undercover.KeepTasks = true;
                    Ped nearestSuspect = GetNearestSuspect(undercover.Position);
                    if (nearestSuspect != null && nearestSuspect.Exists())
                    {
                        undercover.Tasks.FightAgainst(nearestSuspect);
                    }
                }
            }
        }

        protected override void OnScenarioTick()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists()) return;

            // Recoleccion interactiva de evidencias reales
            for (int i = 0; i < EvidenceList.Count; i++)
            {
                EvidenceItem item = EvidenceList[i];
                if (!item.IsCollected && player.Position.DistanceTo(item.Position) < 2.5f)
                {
                    Game.DisplayHelp(string.Format("Presiona ~INPUT_CONTEXT~ para ~y~asegurar evidencia~w~: {0}", item.Description));

                    if (Game.IsControlJustPressed(0, GameControl.Context))
                    {
                        CollectEvidence(item);
                        break;
                    }
                }
            }

            // Refresco de combate throttled (1000ms) para evitar Task Stomping a 60 FPS
            uint now = Game.GameTime;
            if (now - _lastCombatRefreshTime > 1000)
            {
                _lastCombatRefreshTime = now;
                RefreshFederalCombatAI();
            }

            // Interaccion con terminal de borrado de datos
            if (DataWipeActive && !DataWipePrevented)
            {
                if (player.Position.DistanceTo(ServerTerminalLocation) < 2.5f)
                {
                    Game.DisplayHelp("Presiona ~INPUT_CONTEXT~ para ~g~detener la destruccion de archivos~w~.");
                    if (Game.IsControlJustPressed(0, GameControl.Context))
                    {
                        StopDataWipe();
                    }
                }
            }
        }

        private void RefreshFederalCombatAI()
        {
            // Reasignar agentes encubiertos si su blanco cayo
            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped undercover = FriendlyPeds[i];
                if (undercover != null && undercover.Exists() && undercover.IsAlive)
                {
                    Ped currentTarget = NativeFunction.Natives.GET_PED_TARGET_FROM_COMBAT_PED<Ped>(undercover);
                    if (currentTarget == null || !currentTarget.Exists() || !currentTarget.IsAlive)
                    {
                        Ped newTarget = GetNearestSuspect(undercover.Position);
                        if (newTarget != null && newTarget.Exists() && newTarget.IsAlive)
                        {
                            undercover.Tasks.FightAgainst(newTarget);
                        }
                    }
                }
            }

            // Reasignar sospechosos si su blanco cayo o salieron de combate
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    Ped currentTarget = NativeFunction.Natives.GET_PED_TARGET_FROM_COMBAT_PED<Ped>(s);
                    if (currentTarget == null || !currentTarget.Exists() || !currentTarget.IsAlive)
                    {
                        Ped newTarget = GetNearestOpponentForSuspect(s.Position);
                        if (newTarget != null && newTarget.Exists() && newTarget.IsAlive)
                        {
                            s.BlockPermanentEvents = false;
                            s.Tasks.FightAgainst(newTarget);
                        }
                    }
                }
            }
        }

        protected Ped GetNearestOpponentForSuspect(Vector3 pos)
        {
            Ped nearest = Game.LocalPlayer.Character;
            float minDist = (nearest != null && nearest.Exists() && nearest.IsAlive) ? nearest.Position.DistanceTo(pos) : float.MaxValue;

            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped f = FriendlyPeds[i];
                if (f != null && f.Exists() && f.IsAlive)
                {
                    float d = f.Position.DistanceTo(pos);
                    if (d < minDist)
                    {
                        minDist = d;
                        nearest = f;
                    }
                }
            }
            return nearest;
        }

        protected override void CheckCompletionCriteria()
        {
            bool anyHostileActive = false;
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    anyHostileActive = true;
                    break;
                }
            }

            bool allEvidenceCollected = (EvidenceList.Count == 0 || EvidenceCollectedCount >= EvidenceList.Count);

            if (!anyHostileActive && allEvidenceCollected && (!DataWipeActive || DataWipePrevented))
            {
                // Liberar rehenes/victimas civiles si habia
                for (int i = 0; i < Hostages.Count; i++)
                {
                    Ped h = Hostages[i];
                    if (h != null && h.Exists() && h.IsAlive)
                    {
                        h.BlockPermanentEvents = false;
                        h.Tasks.ClearImmediately();
                        h.Tasks.ReactAndFlee(Game.LocalPlayer.Character);
                    }
                }
                NotifyCode4();
            }
        }

        // ---------------- Mecanicas de Investigacion Federal ----------------

        protected void AddEvidenceItem(string propModel, Vector3 pos, string description)
        {
            Rage.Object prop = SpawnProp(propModel, pos, 0f);
            Blip b = null;
            if (prop != null)
            {
                b = prop.AttachBlip();
                b.Color = Color.Yellow;
                b.Scale = 0.55f;
                Cleaner.RegisterBlip(b);
            }

            EvidenceList.Add(new EvidenceItem
            {
                Prop = prop,
                Position = pos,
                Description = description,
                IsCollected = false,
                EvidenceBlip = b
            });
        }

        protected void CollectEvidence(EvidenceItem item)
        {
            item.IsCollected = true;
            EvidenceCollectedCount++;

            if (item.EvidenceBlip != null && item.EvidenceBlip.Exists())
            {
                item.EvidenceBlip.Delete();
            }

            Ped player = Game.LocalPlayer.Character;
            if (player != null && player.Exists())
            {
                NativeFunction.Natives.REQUEST_ANIM_DICT("amb@medic@standing@kneel@idle_a");
                int timeout = 0;
                while (!NativeFunction.Natives.HAS_ANIM_DICT_LOADED<bool>("amb@medic@standing@kneel@idle_a") && timeout < 25)
                {
                    GameFiber.Yield();
                    timeout++;
                }

                if (NativeFunction.Natives.HAS_ANIM_DICT_LOADED<bool>("amb@medic@standing@kneel@idle_a"))
                {
                    NativeFunction.Natives.TASK_PLAY_ANIM(player, "amb@medic@standing@kneel@idle_a", "idle_a", 8.0f, -8.0f, 2500, 0, 0f, false, false, false);
                }
            }

            if (item.Prop != null && item.Prop.Exists())
            {
                item.Prop.Delete();
            }

            ImmersionManager.TakeForensicPhoto(player, item.Position, item.Description);
        }

        protected void StartDataWipeCountdown(Vector3 terminalPos, int seconds)
        {
            DataWipeActive = true;
            DataWipePrevented = false;
            DataWipeSecondsRemaining = seconds;
            ServerTerminalLocation = terminalPos;

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                try
                {
                    while (DataWipeActive && !DataWipePrevented && DataWipeSecondsRemaining > 0)
                    {
                        Game.DisplayHelp(string.Format("~r~BORRADO DE ARCHIVOS EN CURSO: ~w~{0}s. Llega a la terminal y pulsa ~INPUT_CONTEXT~.", DataWipeSecondsRemaining));
                        GameFiber.Sleep(1000);
                        DataWipeSecondsRemaining--;
                    }

                    if (DataWipeActive && !DataWipePrevented && DataWipeSecondsRemaining <= 0)
                    {
                        Game.DisplaySubtitle("~r~[FIB] Los sospechosos formatearon los servidores. Evidencia digital destruida.");
                        DataWipeActive = false;
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("BaseFederalCallout.StartDataWipeCountdown: " + ex.Message);
                }
            });
            Cleaner.RegisterFiber(fiber);
        }

        protected void StopDataWipe()
        {
            DataWipePrevented = true;
            DataWipeActive = false;

            try
            {
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists())
                {
                    NativeFunction.Natives.REQUEST_ANIM_DICT("mini@repair");
                    int timeout = 0;
                    while (!NativeFunction.Natives.HAS_ANIM_DICT_LOADED<bool>("mini@repair") && timeout < 25)
                    {
                        GameFiber.Yield();
                        timeout++;
                    }

                    if (NativeFunction.Natives.HAS_ANIM_DICT_LOADED<bool>("mini@repair"))
                    {
                        NativeFunction.Natives.TASK_PLAY_ANIM(player, "mini@repair", "fixing_a_player", 8.0f, -8.0f, 3000, 0, 0f, false, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("BaseFederalCallout.StopDataWipe: " + ex.Message);
            }

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~FIB Ciberseguridad", "~g~Terminal Asegurada", "La destruccion de archivos ha sido interrumpida con exito.");
        }

        protected Ped CreateFederalTarget(string model, Vector3 pos, float heading, WeaponHash weapon, int armor = 60, bool snapToGround = true)
        {
            Ped p = SpawnPed(model, pos, heading, snapToGround);
            if (p != null)
            {
                p.RelationshipGroup = SuspectGroup;
                p.Inventory.Weapons.Add(weapon).Ammo = -1;
                ConfigurePedCombat(p, ConfigManager.SuspectAccuracy, armor, true, 2);
                AttachBlipToSuspect(p, Color.Orange, 0.65f);
                Suspects.Add(p);
            }
            return p;
        }

        protected Ped CreateUndercoverAgent(string model, Vector3 pos, float heading, WeaponHash weapon)
        {
            Ped p = SpawnPed(model, pos, heading);
            if (p != null)
            {
                p.RelationshipGroup = UndercoverGroup;
                p.Inventory.Weapons.Add(weapon).Ammo = -1;
                ConfigurePedCombat(p, 65, 100, true, 2);
                p.BlockPermanentEvents = false;
                Blip b = p.AttachBlip();
                b.Color = Color.Blue;
                b.Scale = 0.55f;
                Cleaner.RegisterBlip(b);
                FriendlyPeds.Add(p);
            }
            return p;
        }

        protected Ped GetNearestSuspect(Vector3 position)
        {
            Ped nearest = null;
            float minDistance = float.MaxValue;

            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive)
                {
                    float d = s.Position.DistanceTo(position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = s;
                    }
                }
            }
            return nearest;
        }

        protected void SpawnEvidenceProp(string propModel, Vector3 pos, string description)
        {
            AddEvidenceItem(propModel, pos, description);
        }

        protected void StartDataDestructionCountdown(Vector3 terminalPos, int seconds)
        {
            StartDataWipeCountdown(terminalPos, seconds);
        }

        protected void StartDataDestructionCountdown(int seconds, Vector3 terminalPos, string reason = null)
        {
            StartDataWipeCountdown(terminalPos, seconds);
            if (!string.IsNullOrEmpty(reason))
            {
                Game.DisplaySubtitle("~r~[ALERTA FORENSE]~w~ " + reason);
            }
        }
    }
}
