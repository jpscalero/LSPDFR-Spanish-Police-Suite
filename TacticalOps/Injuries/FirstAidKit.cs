using System;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using TacticalOps.Core;

namespace TacticalOps.Injuries
{
    public static class FirstAidKit
    {
        private static MenuPool pool;
        private static UIMenu menu;

        public static void Process()
        {
            pool = new MenuPool();
            menu = new UIMenu("BOTIQUIN TACTICO IFAK", "~b~SOPORTE VITAL EN COMBATE (TCCC)");
            pool.Add(menu);

            UIMenuItem itemCat = new UIMenuItem("Aplicar Torniquete Tactico CAT", "Detiene la hemorragia exanguinante en extremidades tras 4 segundos de aplicacion.");
            UIMenuItem itemVendaje = new UIMenuItem("Aplicar Vendaje Hemostatico", "Empaqueta heridas penetrantes en torso, cohibe el sangrado activo y restaura 25 HP.");
            UIMenuItem itemNaloxona = new UIMenuItem("Administrar Naloxona", "Revierte parada respiratoria y sobredosis por sustancias en ciudadanos inconscientes.");
            UIMenuItem itemDea = new UIMenuItem("Desfibrilador Semiautomatico (DEA)", "Aplica descargas de reanimacion cardiopulmonar a sujetos en parada cardiaca.");

            menu.AddItem(itemCat);
            menu.AddItem(itemVendaje);
            menu.AddItem(itemNaloxona);
            menu.AddItem(itemDea);

            menu.OnItemSelect += delegate(UIMenu sender, UIMenuItem selectedItem, int index)
            {
                pool.CloseAllMenus();

                if (selectedItem == itemCat)
                {
                    ApplyCatTourniquet();
                }
                else if (selectedItem == itemVendaje)
                {
                    ApplyHemostaticBandage();
                }
                else if (selectedItem == itemNaloxona)
                {
                    AdministerNaloxone();
                }
                else if (selectedItem == itemDea)
                {
                    UseDefibrillator();
                }
            };

            Game.LogTrivial("TacticalOps: FirstAidKit menu inicializado.");

            while (true)
            {
                GameFiber.Yield();

                if (Game.IsKeyDown(ConfigManager.MedicalKey))
                {
                    if (!pool.IsAnyMenuOpen())
                    {
                        menu.Visible = true;
                    }
                    else
                    {
                        pool.CloseAllMenus();
                    }
                }

                pool.ProcessMenus();
            }
        }

        private static void ApplyCatTourniquet()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            // Animacion medica
            player.Tasks.PlayAnimation("anim@heists@narcotics@funding@gang_cash_holding", "idle", 2.0f, AnimationFlags.UpperBodyOnly);
            Game.DisplaySubtitle("~y~Colocando y ciñendo Torniquete Tactico CAT en extremidad... (4s)~w~", 4000);

            GameFiber.Sleep(4000);

            if (player.Exists() && player.IsAlive)
            {
                player.Tasks.Clear();
                InjuryManager.CureLeg();
                InjuryManager.CureArm();

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "MEDAL_UP", "HUD_MINI_GAME_SOUNDSET", false);
                Game.DisplayNotification("~g~Torniquete CAT bloqueado con exito.~w~ Hemorragia arterial en extremidades neutralizada.");
            }
        }

        private static void ApplyHemostaticBandage()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            player.Tasks.PlayAnimation("missheistdockssetup1clipboard@idle_a", "idle_a", 2.0f, AnimationFlags.UpperBodyOnly);
            Game.DisplaySubtitle("~y~Empaquetando herida con gasa hemostatica de coagulacion rapida... (3s)~w~", 3000);

            GameFiber.Sleep(3000);

            if (player.Exists() && player.IsAlive)
            {
                player.Tasks.Clear();
                InjuryManager.CureTorso();
                BleedingSystem.StopBleeding();

                // Recuperar 25 HP
                int newHealth = Math.Min(player.MaxHealth, player.Health + 25);
                player.Health = newHealth;

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "MEDAL_UP", "HUD_MINI_GAME_SOUNDSET", false);
                Game.DisplayNotification("~g~Vendaje hemostatico sellado.~w~ Sangrado detenido y ~g~+25 HP~w~ restablecidos.");
            }
        }

        private static void AdministerNaloxone()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            Ped target = FindClosestInjuredOrUnconsciousPed(3.5f);
            if (target == null)
            {
                Game.DisplayNotification("~r~No hay ningun peaton inconsciente o en sobredosis en las proximidades.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "ERROR", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                return;
            }

            player.Tasks.PlayAnimation("amb@medic@standing@kneel@base", "base", 2.0f, AnimationFlags.None);
            Game.DisplaySubtitle("~b~Administrando spray intranasal de Naloxona (clorhidrato 4mg)...~w~", 3500);

            GameFiber.Sleep(3500);

            player.Tasks.Clear();

            if (target != null && target.Exists())
            {
                if (target.IsDead)
                {
                    NativeFunction.Natives.RESURRECT_PED(target);
                    NativeFunction.Natives.REVIVE_INJURED_PED(target);
                    target.Health = 50;
                }

                target.Tasks.Clear();
                NativeFunction.Natives.CLEAR_PED_TASKS_IMMEDIATELY(target);
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(target, 200, 200, 0, false, false, false);
                GameFiber.Sleep(500);

                if (target.Exists() && target.IsAlive)
                {
                    target.Tasks.ReactAndFlee(player);
                }

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET", false);
                Game.DisplayNotification("~g~Naloxona administrada con exito.~w~ El sujeto recobra la funcion respiratoria.");
            }
        }

        private static void UseDefibrillator()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            Ped target = FindClosestDeadPed(3.5f);
            if (target == null)
            {
                Game.DisplayNotification("~r~No hay ningun paciente en parada cardiorrespiratoria en el radio de accion.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "ERROR", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                return;
            }

            player.Tasks.PlayAnimation("amb@medic@standing@kneel@base", "base", 2.0f, AnimationFlags.None);

            Game.DisplaySubtitle("~b~DEA:~w~ 'Electrodos adheridos. Analizando ritmo cardiaco... ¡No toque al paciente!'", 3000);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
            GameFiber.Sleep(3000);

            Game.DisplaySubtitle("~r~DEA:~w~ '¡Ritmo desfibrilable detectado! Cargando condensador... ¡Descarga en 3, 2, 1!'", 3000);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Green", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
            GameFiber.Sleep(3000);

            // Espasmo muscular por la descarga electrica
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "ScreenFlash", "WastedSounds", false);
            NativeFunction.Natives.SET_PED_TO_RAGDOLL(target, 1500, 1500, 0, false, false, false);
            GameFiber.Sleep(1500);

            if (target != null && target.Exists())
            {
                NativeFunction.Natives.RESURRECT_PED(target);
                NativeFunction.Natives.REVIVE_INJURED_PED(target);
                target.Health = 60;
                target.Tasks.Clear();

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET", false);
                Game.DisplaySubtitle("~g~DEA:~w~ '¡Ritmo sinusal restablecido! Pulso carotideo presente.'", 4000);
                Game.DisplayNotification("~g~¡Paciente reanimado!~w~ El DEA ha logrado revertir la fibrilacion ventricular.");
            }

            player.Tasks.Clear();
        }

        private static Ped FindClosestInjuredOrUnconsciousPed(float radius)
        {
            Ped player = Game.LocalPlayer.Character;
            Entity[] entities = World.GetEntities(player.Position, radius, GetEntitiesFlags.ConsiderAllPeds);
            Ped closest = null;
            float minDist = float.MaxValue;

            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Ped p = entities[i] as Ped;
                    if (p != null && p.Exists() && p != player)
                    {
                        bool isRagdoll = NativeFunction.Natives.IS_PED_RAGDOLL<bool>(p);
                        if (p.IsDead || isRagdoll || p.Health < 30)
                        {
                            float d = player.DistanceTo(p);
                            if (d < minDist)
                            {
                                minDist = d;
                                closest = p;
                            }
                        }
                    }
                }
            }

            return closest;
        }

        private static Ped FindClosestDeadPed(float radius)
        {
            Ped player = Game.LocalPlayer.Character;
            Entity[] entities = World.GetEntities(player.Position, radius, GetEntitiesFlags.ConsiderAllPeds);
            Ped closest = null;
            float minDist = float.MaxValue;

            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Ped p = entities[i] as Ped;
                    if (p != null && p.Exists() && p != player && p.IsDead)
                    {
                        float d = player.DistanceTo(p);
                        if (d < minDist)
                        {
                            minDist = d;
                            closest = p;
                        }
                    }
                }
            }

            return closest;
        }
    }
}
