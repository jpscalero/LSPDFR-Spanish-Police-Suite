using PoliceSuite.Common;
using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;

namespace AlternativeCallouts.Core
{
    public static class AlternativeOutcomeHelper
    {
        public static void TriggerPeacefulMediation(Ped suspect, string dialog)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;
            try
            {
                suspect.BlockPermanentEvents = true;
                suspect.Tasks.ClearImmediately();
                NativeFunction.Natives.TASK_STAND_STILL(suspect, 15000);
                Game.DisplaySubtitle(dialog);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts.AlternativeOutcomeHelper.TriggerPeacefulMediation: " + ex.Message);
            }
        }

        public static void TriggerResistAndFight(Ped suspect, WeaponHash weapon, string shout)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;
            try
            {
                suspect.BlockPermanentEvents = false;
                suspect.KeepTasks = true;
                suspect.Inventory.Weapons.Add(weapon).Ammo = -1;
                CalloutEffects.ConfigurePedCombat(suspect, 70, 0, true, 2);
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists()) suspect.Tasks.FightAgainst(player);
                Game.DisplaySubtitle(shout);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts.AlternativeOutcomeHelper.TriggerResistAndFight: " + ex.Message);
            }
        }

        public static LHandle TriggerFootPursuit(Ped runner, string shout)
        {
            if (runner == null || !runner.Exists() || !runner.IsAlive) return null;
            try
            {
                runner.BlockPermanentEvents = false;
                runner.KeepTasks = true;
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists())
                {
                    NativeFunction.Natives.TASK_SMART_FLEE_PED(runner, player, 350f, -1, false, false);
                }
                LHandle pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, runner);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                Game.DisplaySubtitle(shout);
                return pursuit;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts.AlternativeOutcomeHelper.TriggerFootPursuit: " + ex.Message);
                return null;
            }
        }

        public static void TriggerVehiclePursuit(Ped runner, Vehicle vehicle, string shout, Action<LHandle> onPursuitCreated, AlternativeCleanupManager cleaner)
        {
            if (runner == null || !runner.Exists() || !runner.IsAlive || vehicle == null || !vehicle.Exists()) return;
            try
            {
                runner.BlockPermanentEvents = true;
                runner.Tasks.EnterVehicle(vehicle, -1);

                GameFiber fiber = GameFiber.StartNew(delegate
                {
                    try
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
                            LHandle pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(pursuit, runner);
                            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                            if (onPursuitCreated != null) onPursuitCreated(pursuit);
                            Game.DisplaySubtitle(shout);
                        }
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("AlternativeOutcomeHelper.MakeSuspectFleeInVehicle: " + ex.Message);
                    }
                });
                if (cleaner != null) cleaner.RegisterFiber(fiber);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts.AlternativeOutcomeHelper.TriggerVehiclePursuit: " + ex.Message);
            }
        }

        public static void TriggerSevereCrimeTwist(Ped suspect, WeaponHash weapon, string shout)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return;
            try
            {
                suspect.BlockPermanentEvents = false;
                suspect.KeepTasks = true;
                suspect.Inventory.Weapons.Add(weapon).Ammo = -1;
                CalloutEffects.ConfigurePedCombat(suspect, 80, 50, true, 2);
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists()) suspect.Tasks.FightAgainst(player);
                Game.DisplaySubtitle(shout);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts.AlternativeOutcomeHelper.TriggerSevereCrimeTwist: " + ex.Message);
            }
        }
    }
}
