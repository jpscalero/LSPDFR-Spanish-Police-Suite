using System;
using Rage;
using Rage.Native;
using TacticalOps.Core;

namespace TacticalOps.CQB
{
    public static class BreachSystem
    {
        public static void ExecuteBatteringRam()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            GameFiber.StartNew(delegate
            {
                try
                {
                    // Animacion segura de impacto o patada
                    SafeAnimation.Play(player, "melee@unarmed@streamed_core", "kick_a", 4.0f, AnimationFlags.None);
                    Game.DisplaySubtitle("~y~¡Golpeando marco con Ariete Tactico!~w~", 1500);

                    GameFiber.Sleep(1200);

                    // Sonido contundente de impacto y rotura
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Door_Kick", "LOWRIDER_STUDIO_SOUNDSET", false);
                    NativeFunction.Natives.SHAKE_GAMEPLAY_CAM("MEDIUM_EXPLOSION_SHAKE", 0.35f);

                    // Forzar apertura de puertas cercanas en un radio de 4 metros
                    ForceOpenClosestDoors(player.Position, 4.0f);

                    Game.DisplayNotification("~g~¡Brecha con Ariete exitosa!~w~ Cerrojo y cerradura reventados.");
                    Game.LogTrivial("TacticalOps: Brecha con ariete ejecutada.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalOps.BreachSystem.ExecuteBatteringRam: " + ex.Message);
                }
            });
        }

        public static void ExecuteShotgunBreach()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            GameFiber.StartNew(delegate
            {
                try
                {
                    // Equipar o asegurar escopeta
                    if (player.Inventory != null)
                    {
                        player.Inventory.GiveNewWeapon("WEAPON_PUMPSHOTGUN", 16, true);
                    }

                    Game.DisplaySubtitle("~y~¡Fuego de brecha con cartuchos frangibles a la cerradura!~w~", 1500);
                    GameFiber.Sleep(800);

                    Vector3 muzzle = player.Position + new Vector3(0, 0, 0.4f);
                    Vector3 breachTarget = player.Position + (player.ForwardVector * 2.0f) + new Vector3(0, 0, 0.1f);

                    uint shotgunHash = Game.GetHashKey("WEAPON_PUMPSHOTGUN");

                    // Doble disparo de brecha balistica
                    NativeFunction.Natives.SHOOT_SINGLE_BULLET_BETWEEN_COORDS(muzzle.X, muzzle.Y, muzzle.Z, breachTarget.X, breachTarget.Y, breachTarget.Z, 120, true, shotgunHash, player, true, false, -1.0f);
                    NativeFunction.Natives.SHAKE_GAMEPLAY_CAM("SMALL_EXPLOSION_SHAKE", 0.4f);

                    GameFiber.Sleep(300);

                    NativeFunction.Natives.SHOOT_SINGLE_BULLET_BETWEEN_COORDS(muzzle.X, muzzle.Y, muzzle.Z, breachTarget.X, breachTarget.Y, breachTarget.Z, 120, true, shotgunHash, player, true, false, -1.0f);

                    // Desbloquear puertas
                    ForceOpenClosestDoors(player.Position, 4.0f);

                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                    Game.DisplayNotification("~g~¡Brecha balistica completada!~w~ Mecanismo de cierre destruido.");
                    Game.LogTrivial("TacticalOps: Brecha con escopeta ejecutada.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalOps.BreachSystem.ExecuteShotgunBreach: " + ex.Message);
                }
            });
        }

        public static void ExecuteExplosiveBreach()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            GameFiber.StartNew(delegate
            {
                try
                {
                    // Animacion segura de colocacion de carga explosiva
                    SafeAnimation.Play(player, "weapons@first_person@aim_rng@generic@projectile@sticky_bomb@", "plant_vertical", 2.0f, AnimationFlags.None);
                    Game.DisplaySubtitle("~y~Fijando carga explosiva lineal de corte...~w~", 2500);

                    GameFiber.Sleep(2500);

                    player.Tasks.Clear();

                    Game.DisplaySubtitle("~r~¡Detonacion en 3... 2... 1... ¡BRECHA!~w~", 2000);
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
                    GameFiber.Sleep(2000);

                    Vector3 explosionPos = player.Position + (player.ForwardVector * 1.8f) + new Vector3(0, 0, 0.3f);

                    // Pequeña explosion controlada de carga hueca (tipo 2)
                    NativeFunction.Natives.ADD_EXPLOSION(explosionPos.X, explosionPos.Y, explosionPos.Z, 2, 0.4f, true, false, 0.4f);
                    NativeFunction.Natives.SHAKE_GAMEPLAY_CAM("MEDIUM_EXPLOSION_SHAKE", 0.6f);

                    ForceOpenClosestDoors(player.Position, 5.0f);

                    Game.DisplayNotification("~r~¡Carga detonada!~w~ Puerta y marco pulverizados.");
                    Game.LogTrivial("TacticalOps: Brecha explosiva ejecutada.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalOps.BreachSystem.ExecuteExplosiveBreach: " + ex.Message);
                }
            });
        }

        public static void ThrowFlashbang()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            GameFiber.StartNew(delegate
            {
                try
                {
                    SafeAnimation.Play(player, "weapons@projectile@grenade_str", "throw_fb_long", 2.0f, AnimationFlags.None);
                    Game.DisplaySubtitle("~y~¡¡LANZANDO FLASHBANG!!~w~", 1500);

                    GameFiber.Sleep(1200);

                    Vector3 targetPos = player.Position + (player.ForwardVector * 7.0f);

                    // Efecto de destello blanco cegador en pantalla
                    NativeFunction.Natives.ANIMPOSTFX_PLAY("MP_job_load", 3000, false);
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Explosion_Small", "Generic_Sounds", false);
                    NativeFunction.Natives.SHAKE_GAMEPLAY_CAM("SMALL_EXPLOSION_SHAKE", 0.7f);

                    // Aturdir a sospechosos en la estancia durante 5 segundos
                    Entity[] nearbyEntities = World.GetEntities(targetPos, 15.0f, GetEntitiesFlags.ConsiderAllPeds);
                    int stunnedCount = 0;

                    if (nearbyEntities != null)
                    {
                        for (int i = 0; i < nearbyEntities.Length; i++)
                        {
                            Ped ped = nearbyEntities[i] as Ped;
                            if (ped != null && ped.Exists() && ped.IsAlive && ped != player)
                            {
                                uint pedType = NativeFunction.Natives.GET_PED_TYPE<uint>(ped);
                                if (pedType != 6 && pedType != 27 && ped.RelationshipGroup != "COP")
                                {
                                    // Desarmar al sospechoso
                                    uint weaponHash = NativeFunction.Natives.GET_SELECTED_PED_WEAPON<uint>(ped);
                                    if (weaponHash != 0 && weaponHash != Game.GetHashKey("WEAPON_UNARMED"))
                                    {
                                        NativeFunction.Natives.SET_PED_DROPS_INVENTORY_WEAPON(ped, weaponHash, 0.0f, 0.0f, 0.0f, 0);
                                    }

                                    // 5 segundos de aturdimiento en el suelo (ragdoll)
                                    NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 5000, 5000, 0, true, true, false);
                                    stunnedCount++;
                                }
                            }
                        }
                    }

                    Game.DisplayNotification("~b~¡Flashbang detonada!~w~ " + stunnedCount + " sujetos aturdidos e incapacitados durante 5s.");
                    Game.LogTrivial("TacticalOps: Flashbang detonada con " + stunnedCount + " sospechosos aturdidos.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalOps.BreachSystem.ThrowFlashbang: " + ex.Message);
                }
            });
        }

        public static void ThrowTearGas()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            GameFiber.StartNew(delegate
            {
                try
                {
                    SafeAnimation.Play(player, "weapons@projectile@grenade_str", "throw_fb_long", 2.0f, AnimationFlags.None);
                    Game.DisplaySubtitle("~y~¡¡DESPLEGANDO GAS LACRIMOGENO (CS)!!~w~", 1500);

                    GameFiber.Sleep(1200);

                    Vector3 gasPos = player.Position + (player.ForwardVector * 7.0f);

                    // Desplegar bote de gas lacrimogeno (tipo de explosion 12 = gas CS)
                    NativeFunction.Natives.ADD_EXPLOSION(gasPos.X, gasPos.Y, gasPos.Z, 12, 0.1f, true, false, 0.0f);

                    Entity[] nearbyEntities = World.GetEntities(gasPos, 14.0f, GetEntitiesFlags.ConsiderAllPeds);
                    int affectedCount = 0;

                    if (nearbyEntities != null)
                    {
                        for (int i = 0; i < nearbyEntities.Length; i++)
                        {
                            Ped ped = nearbyEntities[i] as Ped;
                            if (ped != null && ped.Exists() && ped.IsAlive && ped != player)
                            {
                                uint pedType = NativeFunction.Natives.GET_PED_TYPE<uint>(ped);
                                if (pedType != 6 && pedType != 27 && ped.RelationshipGroup != "COP")
                                {
                                    SafeAnimation.Play(ped, "missheist_agency2aig_13", "cough_loop", 4.0f, AnimationFlags.None);
                                    affectedCount++;
                                }
                            }
                        }
                    }

                    Game.DisplayNotification("~y~Gas lacrimogeno activo.~w~ " + affectedCount + " individuos afectados por irritacion severa.");
                    Game.LogTrivial("TacticalOps: Gas lacrimogeno desplegado.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalOps.BreachSystem.ThrowTearGas: " + ex.Message);
                }
            });
        }

        private static void ForceOpenClosestDoors(Vector3 pos, float radius)
        {
            try
            {
                NativeFunction.Natives.SET_STATE_OF_CLOSEST_DOOR_OF_TYPE(0, pos.X, pos.Y, pos.Z, false, 0.0f, false);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps.BreachSystem.ForceOpenClosestDoors: " + ex.Message);
            }
        }
    }
}
