using System;
using System.Drawing;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;

namespace TacticalAndFederalCallouts.Core
{
    /// <summary>
    /// Especializacion para operaciones tacticas SWAT y NOOSE.
    /// Incorpora ejecucion de rehenes bajo cronometro, temporizador y desactivacion de bombas,
    /// francotiradores en azoteas, granadas aturdidoras / flashbang y asaltos CQC.
    /// </summary>
    public abstract class BaseTacticalCallout : BaseCallout
    {
        protected bool BombArmed { get; set; }
        protected int BombSecondsRemaining { get; set; }
        protected Vector3 BombLocation { get; set; }
        protected bool BombDefused { get; set; }
        protected bool BombExploded { get; set; }
        protected Rage.Object BombProp { get; set; }

        protected bool HostageExecutionActive { get; set; }
        protected int HostageCountdownRemaining { get; set; }
        protected Ped ExecutionerSuspect { get; set; }
        protected Ped TargetedHostage { get; set; }

        protected BaseTacticalCallout()
        {
            BombArmed = false;
            BombDefused = false;
            BombExploded = false;
            HostageExecutionActive = false;
        }

        protected override void OnPlayerArrival()
        {
            Game.DisplaySubtitle("~r~[SWAT EN ACCION]~w~ " + DisplayName + ". Sospechosos armados en el perimetro.");
            EngageHostiles();
        }

        protected virtual void EngageHostiles()
        {
            Ped player = Game.LocalPlayer.Character;
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive)
                {
                    s.BlockPermanentEvents = false;
                    s.KeepTasks = true;
                    if (player != null && player.Exists())
                    {
                        s.Tasks.FightAgainst(player);
                    }
                }
            }

            // Oficiales y personal aliado (soldados, guardias penitenciarios) responden activamente al fuego
            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped ally = FriendlyPeds[i];
                if (ally != null && ally.Exists() && ally.IsAlive)
                {
                    ally.BlockPermanentEvents = false;
                    ally.KeepTasks = true;
                    Ped target = GetNearestSuspect(ally.Position);
                    if (target != null && target.Exists())
                    {
                        ally.Tasks.FightAgainst(target);
                    }
                }
            }
        }

        protected override void OnScenarioTick()
        {
            Ped player = Game.LocalPlayer.Character;
            bool playerValid = player != null && player.Exists() && player.IsAlive;

            // 1. Mantenimiento de tareas de rehenes vivos (Sin reiniciar cada frame)
            for (int i = 0; i < Hostages.Count; i++)
            {
                Ped h = Hostages[i];
                if (h != null && h.Exists() && h.IsAlive)
                {
                    if (h.IsInAnyVehicle(false) || h.IsRagdoll) continue;

                    if (!h.IsInCombat && !NativeFunction.Natives.GET_IS_TASK_ACTIVE<bool>(h, 125)) // 125 = CTaskCower
                    {
                        h.Tasks.Cower(-1);
                    }
                }
            }

            // 2. Mantenimiento activo de sospechosos (Evita que se queden congelados)
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    if (HostageExecutionActive && s == ExecutionerSuspect) continue;

                    if (!s.IsInCombat)
                    {
                        Ped target = GetNearestFriendlyOrPlayer(s.Position);
                        if (target != null && target.Exists() && target.IsAlive)
                        {
                            s.BlockPermanentEvents = false;
                            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(s, 46, true);
                            s.Tasks.FightAgainst(target);
                        }
                    }
                }
            }

            // 3. Refresco de combate para aliados (Retargeting inmediato al morir el blanco)
            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped ally = FriendlyPeds[i];
                if (ally != null && ally.Exists() && ally.IsAlive)
                {
                    Ped currentCombatTarget = GetPedCombatTarget(ally);
                    bool targetDeadOrNull = (currentCombatTarget == null || !currentCombatTarget.Exists() || !currentCombatTarget.IsAlive);

                    if (targetDeadOrNull)
                    {
                        Ped target = GetNearestSuspect(ally.Position);
                        if (target != null && target.Exists() && target.IsAlive)
                        {
                            ally.Tasks.FightAgainst(target);
                        }
                    }
                }
            }

            // 4. Gestion de desactivacion interactiva de bomba
            if (BombArmed && !BombDefused && !BombExploded)
            {
                if (playerValid && player.Position.DistanceTo(BombLocation) < 2.5f)
                {
                    Game.DisplayHelp("Presiona ~INPUT_CONTEXT~ para ~g~desactivar el artefacto explosivo~w~.");
                    if (Game.IsControlJustPressed(0, GameControl.Context))
                    {
                        DefuseBomb();
                    }
                }
            }
        }

        private Ped GetPedCombatTarget(Ped ped)
        {
            try
            {
                return NativeFunction.Natives.GET_PED_TARGET_FROM_COMBAT_PED<Ped>(ped, 0);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("BaseTacticalCallout.GetPedCombatTarget: " + ex.Message);
                return null;
            }
        }

        protected Ped GetNearestFriendlyOrPlayer(Vector3 pos)
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
            if (BombExploded)
            {
                CurrentPhase = ECalloutPhase.Cleanup;
                IsCompleted = true;
                DispatchHelper.SendDispatchNotification(DisplayName, "Operacion Fallida", "El artefacto explosivo ha detonado. Bajas catastroficas.");
                End();
                return;
            }

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

            if (!anyHostileActive && (!BombArmed || BombDefused))
            {
                NotifyCode4();
            }
        }

        // ---------------- Mecanicas Tacticas SWAT ----------------

        protected void StartHostageExecutionCountdown(int seconds, Ped executioner, Ped victim, string warning)
        {
            HostageExecutionActive = true;
            HostageCountdownRemaining = seconds;
            ExecutionerSuspect = executioner;
            TargetedHostage = victim;

            if (ExecutionerSuspect != null && ExecutionerSuspect.Exists() && TargetedHostage != null && TargetedHostage.Exists())
            {
                ExecutionerSuspect.Tasks.AimWeaponAt(TargetedHostage, -1);
            }

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                try
                {
                    while (HostageExecutionActive && HostageCountdownRemaining > 0)
                    {
                        if (ExecutionerSuspect == null || !ExecutionerSuspect.Exists() || !ExecutionerSuspect.IsAlive || Functions.IsPedArrested(ExecutionerSuspect))
                        {
                            HostageExecutionActive = false;
                            Game.DisplaySubtitle("~g~[SWAT]~w~ ¡Tirador neutralizado! Rehén a salvo.");
                            break;
                        }

                        if (TargetedHostage == null || !TargetedHostage.Exists() || !TargetedHostage.IsAlive)
                        {
                            HostageExecutionActive = false;
                            break;
                        }

                        Game.DisplayHelp(string.Format("~r~AMENAZA A REHÉN: ~w~{0}s antes de ejecución. {1}", HostageCountdownRemaining, warning));
                        GameFiber.Sleep(1000);
                        HostageCountdownRemaining--;
                    }

                    if (HostageExecutionActive && HostageCountdownRemaining <= 0)
                    {
                        if (ExecutionerSuspect != null && ExecutionerSuspect.Exists() && ExecutionerSuspect.IsAlive &&
                            TargetedHostage != null && TargetedHostage.Exists() && TargetedHostage.IsAlive)
                        {
                            ExecutionerSuspect.Tasks.FightAgainst(TargetedHostage);
                            Game.DisplaySubtitle("~r~[SWAT] ¡Dispararon al rehén! ¡Abran fuego!");

                            // Retargeting automático hacia el jugador tras abatir al rehén
                            GameFiber.StartNew(delegate
                            {
                                try
                                {
                                    GameFiber.Sleep(1800);
                                    if (ExecutionerSuspect != null && ExecutionerSuspect.Exists() && ExecutionerSuspect.IsAlive)
                                    {
                                        ExecutionerSuspect.BlockPermanentEvents = false;
                                        ExecutionerSuspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                                    }
                                }
                                catch (System.Threading.ThreadAbortException) { }
                                catch (Exception ex)
                                {
                                    Game.LogTrivial("BaseTacticalCallout.RetargetingFiber: " + ex.Message);
                                }
                            });
                        }
                        HostageExecutionActive = false;
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("BaseTacticalCallout.StartHostageExecutionThreat: " + ex.Message);
                }
            });
            Cleaner.RegisterFiber(fiber);
        }

        protected void StartBombCountdown(Vector3 bombPos, int seconds, string bombPropModel = "prop_c4_final_sb")
        {
            BombArmed = true;
            BombDefused = false;
            BombExploded = false;
            BombSecondsRemaining = seconds;
            BombLocation = bombPos;

            if (!string.IsNullOrEmpty(bombPropModel))
            {
                BombProp = SpawnProp(bombPropModel, bombPos, 0f);
            }

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                try
                {
                    while (BombArmed && !BombDefused && BombSecondsRemaining > 0)
                    {
                        Game.DisplayHelp(string.Format("~r~ARTEFACTO EXPLOSIVO ACTIVO: ~w~{0}s. Acércate y presiona ~INPUT_CONTEXT~ para desactivar.", BombSecondsRemaining));
                        GameFiber.Sleep(1000);
                        BombSecondsRemaining--;
                    }

                    if (BombArmed && !BombDefused && BombSecondsRemaining <= 0)
                    {
                        TriggerExplosion(BombLocation, 7, 8.0f);
                        Game.DisplaySubtitle("~r~[SWAT] ¡El artefacto ha detonado! Bajas catastróficas.");
                        BombArmed = false;
                        BombExploded = true;
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("BaseTacticalCallout.StartBombCountdown: " + ex.Message);
                }
            });
            Cleaner.RegisterFiber(fiber);
        }

        protected void DefuseBomb()
        {
            BombDefused = true;
            BombArmed = false;

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
                        NativeFunction.Natives.TASK_PLAY_ANIM(player, "mini@repair", "fixing_a_player", 8.0f, -8.0f, 4000, 0, 0f, false, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("BaseTacticalCallout.DefuseBomb: " + ex.Message);
            }

            if (BombProp != null && BombProp.Exists())
            {
                BombProp.Delete();
            }

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~SWAT Técnico EOD", "~g~Bomba Desactivada", "El artefacto explosivo ha sido neutralizado con éxito.");
        }

        protected void DetonateFlashbang(Vector3 pos)
        {
            PlayPtfx("core", "ent_sht_smoke", pos, 2.5f);
            TriggerExplosion(pos, 25, 0.1f);
            TacticalBreachManager.DeployFlashbang(pos, Suspects);

            Ped player = Game.LocalPlayer.Character;
            if (player != null && player.Exists() && player.Position.DistanceTo(pos) < 25f)
            {
                NativeFunction.Natives.ANIMPOSTFX_PLAY("RaceTurbo", 4000, false);
            }

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                try
                {
                    // Fase 1: Aturdimiento
                    for (int i = 0; i < Suspects.Count; i++)
                    {
                        Ped s = Suspects[i];
                        if (s != null && s.Exists() && s.IsAlive && s.Position.DistanceTo(pos) < 22f)
                        {
                            s.Tasks.Cower(5000);
                        }
                    }

                    GameFiber.Sleep(5200);

                    // Fase 2: Recuperacion activa (Descongelamiento garantizado)
                    for (int i = 0; i < Suspects.Count; i++)
                    {
                        Ped s = Suspects[i];
                        if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                        {
                            s.BlockPermanentEvents = false;
                            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(s, 46, true);
                            NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(s, 2); // Ofensivo / Avance
                            Ped target = GetNearestFriendlyOrPlayer(s.Position);
                            if (target != null && target.Exists())
                            {
                                s.Tasks.FightAgainst(target);
                            }
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("BaseTacticalCallout.DetonateFlashbang: " + ex.Message);
                }
            });
            Cleaner.RegisterFiber(fiber);
        }

        protected Ped CreateTacticalSuspect(string model, Vector3 pos, float heading, WeaponHash weapon, int armor = 100, bool snapToGround = true)
        {
            Ped p = SpawnPed(model, pos, heading, snapToGround);
            if (p != null)
            {
                p.Inventory.Weapons.Add(weapon).Ammo = -1;
                ConfigurePedCombat(p, ConfigManager.SuspectAccuracy, armor, true, 2);
                AttachBlipToSuspect(p, Color.Red, 0.65f);
                Suspects.Add(p);
            }
            return p;
        }

        protected Ped CreateTacticalHostage(string model, Vector3 pos, float heading, bool snapToGround = true)
        {
            Ped p = SpawnPed(model, pos, heading, snapToGround);
            if (p != null)
            {
                p.Tasks.Cower(-1);
                AttachBlipToHostage(p, Color.Green, 0.55f);
                Hostages.Add(p);
            }
            return p;
        }

        protected Ped CreateRoofSniper(string model, Vector3 pos, WeaponHash weapon)
        {
            // snapToGround = false para preservar altura exacta en torres y azoteas
            Ped p = SpawnPed(model, pos, 0f, false);
            if (p != null)
            {
                p.Inventory.Weapons.Add(weapon).Ammo = -1;
                p.Armor = 100;
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(p, 46, true);
                NativeFunction.Natives.SET_PED_COMBAT_RANGE(p, 3); // Very Far
                NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(p, 0); // 0 = Stationary (No caminar hacia el vacio)
                NativeFunction.Natives.SET_PED_ACCURACY(p, ConfigManager.SuspectAccuracy + 20);

                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists())
                {
                    p.Tasks.AimWeaponAt(player, -1);
                }

                AttachBlipToSuspect(p, Color.DarkRed, 0.60f);
                Suspects.Add(p);
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
    }
}
