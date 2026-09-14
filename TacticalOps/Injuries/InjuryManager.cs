using System;
using Rage;
using Rage.Native;

namespace TacticalOps.Injuries
{
    public static class InjuryManager
    {
        public static bool IsLegInjured { get; set; }
        public static bool IsArmInjured { get; set; }
        public static bool IsTorsoInjured { get; set; }

        private static int lastHealth;
        private static Random randomGen;

        static InjuryManager()
        {
            IsLegInjured = false;
            IsArmInjured = false;
            IsTorsoInjured = false;
            lastHealth = 100;
            randomGen = new Random();
        }

        public static void CureLeg()
        {
            IsLegInjured = false;
            Ped player = Game.LocalPlayer.Character;
            if (player != null && player.Exists() && player.IsAlive)
            {
                NativeFunction.Natives.SET_PED_MOVE_RATE_OVERRIDE(player, 1.0f);
            }
            Game.LogTrivial("TacticalOps: Lesion en pierna curada.");
        }

        public static void CureArm()
        {
            IsArmInjured = false;
            NativeFunction.Natives.STOP_GAMEPLAY_CAM_SHAKING(true);
            Game.LogTrivial("TacticalOps: Lesion en brazo curada.");
        }

        public static void CureTorso()
        {
            IsTorsoInjured = false;
            BleedingSystem.StopBleeding();
            Game.LogTrivial("TacticalOps: Lesion en torso curada.");
        }

        public static void ResetAll()
        {
            CureLeg();
            CureArm();
            CureTorso();
        }

        public static void Process()
        {
            Game.LogTrivial("TacticalOps: InjuryManager loop iniciado.");

            try
            {
                Ped playerInit = Game.LocalPlayer.Character;
                if (playerInit != null && playerInit.Exists())
                {
                    lastHealth = playerInit.Health;
                }

                while (true)
                {
                    GameFiber.Yield();

                    try
                    {
                        // Actualizar sistema de hemorragias
                        BleedingSystem.Update();

                        Ped player = Game.LocalPlayer.Character;
                        if (player == null || !player.Exists())
                        {
                            continue;
                        }

                        if (player.IsDead)
                        {
                            if (IsLegInjured || IsArmInjured || IsTorsoInjured || BleedingSystem.IsBleeding)
                            {
                                ResetAll();
                            }
                            lastHealth = 100;
                            GameFiber.Sleep(500);
                            continue;
                        }

                        int currentHealth = player.Health;

                        // Detectar caida brusca de salud (al menos 4 HP por impacto de bala o metralla)
                        if (currentHealth < lastHealth - 3)
                        {
                            int boneId = 0;
                            bool hitBone = false;
                            try { hitBone = NativeFunction.Natives.GET_PED_LAST_DAMAGE_BONE<bool>(player, out boneId); } catch { }
                            try { NativeFunction.Natives.CLEAR_PED_LAST_DAMAGE_BONE(player); } catch { }

                            int zone = 0; // 0 = Torso, 1 = Pierna, 2 = Brazo

                            if (hitBone && boneId != 0)
                            {
                                if (IsBoneLeg(boneId))
                                {
                                    zone = 1;
                                }
                                else if (IsBoneArm(boneId))
                                {
                                    zone = 2;
                                }
                                else
                                {
                                    zone = 0;
                                }
                            }
                            else
                            {
                                // Probabilidad si no se identifica el hueso exacto: 45% Torso, 30% Pierna, 25% Brazo
                                int roll = randomGen.Next(100);
                                if (roll < 45) zone = 0;
                                else if (roll < 75) zone = 1;
                                else zone = 2;
                            }

                            if (zone == 0 && !IsTorsoInjured)
                            {
                                IsTorsoInjured = true;
                                BleedingSystem.StartBleeding();
                                Game.DisplayNotification("~r~¡IMPACTO DE BALA EN TORSO!~w~ Hemorragia masiva. Aplica vendaje IFAK.");
                                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
                            }
                            else if (zone == 1 && !IsLegInjured)
                            {
                                IsLegInjured = true;
                                Game.DisplayNotification("~r~¡IMPACTO DE BALA EN PIERNA!~w~ Movilidad reducida. Aplica torniquete CAT.");
                                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
                            }
                            else if (zone == 2 && !IsArmInjured)
                            {
                                IsArmInjured = true;
                                Game.DisplayNotification("~r~¡IMPACTO DE BALA EN BRAZO!~w~ Temblor muscular y perdida de precision de tiro.");
                                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
                            }
                        }

                        lastHealth = currentHealth;

                        // Mantener efectos persistentes de las lesiones activas
                        if (IsLegInjured)
                        {
                            NativeFunction.Natives.SET_PED_MOVE_RATE_OVERRIDE(player, 0.6f);
                        }

                        if (IsArmInjured)
                        {
                            bool isAiming = false;
                            try { isAiming = NativeFunction.Natives.IS_PLAYER_FREE_AIMING<bool>(Game.LocalPlayer); } catch { }
                            if (isAiming || player.IsShooting)
                            {
                                NativeFunction.Natives.SHAKE_GAMEPLAY_CAM("HAND_SHAKE", 0.75f);
                            }
                        }
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("TacticalOps.InjuryManager.Process loop: " + ex.Message);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException) { }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps.InjuryManager.Process fatal: " + ex.Message);
            }
        }

        private static bool IsBoneLeg(int boneId)
        {
            // SKEL_L_Thigh, SKEL_R_Thigh, SKEL_L_Calf, SKEL_R_Calf, SKEL_L_Foot, SKEL_R_Foot
            return boneId == 0x3779 || boneId == 0xCC4D || boneId == 0x192A ||
                   boneId == 0x9000 || boneId == 0xCA72 || boneId == 0x240B;
        }

        private static bool IsBoneArm(int boneId)
        {
            // SKEL_L_UpperArm, SKEL_R_UpperArm, SKEL_L_Forearm, SKEL_R_Forearm, SKEL_L_Hand, SKEL_R_Hand
            return boneId == 0xB1C5 || boneId == 0x9D4D || boneId == 0xEE5F ||
                   boneId == 0x6E5C || boneId == 0x49D9 || boneId == 0xDEAD;
        }
    }
}
