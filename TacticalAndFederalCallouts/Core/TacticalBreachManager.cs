using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;

namespace TacticalAndFederalCallouts.Core
{
    /// <summary>
    /// Gestion de brechas explosivas SWAT, granadas aturdidoras (flashbang) y humo lacrimogeno CS.
    /// </summary>
    public static class TacticalBreachManager
    {
        public static void ExecuteBreachingCharge(Vector3 doorPos, List<Ped> occupants)
        {
            try
            {
                // Detonacion y onda expansiva controlada
                NativeFunction.Natives.ADD_EXPLOSION(doorPos.X, doorPos.Y, doorPos.Z, 2, 2.5f, true, false, 0.8f);

                // Concusion a sospechosos en la sala
                for (int i = 0; i < occupants.Count; i++)
                {
                    Ped p = occupants[i];
                    if (p != null && p.Exists() && p.IsAlive)
                    {
                        if (p.Position.DistanceTo(doorPos) < 9.0f)
                        {
                            ApplyStunEffect(p, 6000);
                        }
                    }
                }

                Game.DisplaySubtitle("~r~[SWAT BREACH]~w~ ¡Carga de brecha detonada! Entrada forzada autorizada.");
            }
            catch { }
        }

        public static void DeployFlashbang(Vector3 blastPos, List<Ped> suspects)
        {
            try
            {
                NativeFunction.Natives.ADD_EXPLOSION(blastPos.X, blastPos.Y, blastPos.Z, 25, 0.1f, true, false, 1.5f);

                for (int i = 0; i < suspects.Count; i++)
                {
                    Ped s = suspects[i];
                    if (s != null && s.Exists() && s.IsAlive)
                    {
                        if (s.Position.DistanceTo(blastPos) < 12.0f)
                        {
                            ApplyStunEffect(s, 7000);
                        }
                    }
                }

                Game.DisplaySubtitle("~y~[FLASHBANG]~w~ ¡Granada aturdidora detonada! Sospechosos desorientados.");
            }
            catch { }
        }

        public static void DeployTearGas(Vector3 gasPos, List<Ped> suspects)
        {
            try
            {
                NativeFunction.Natives.ADD_EXPLOSION(gasPos.X, gasPos.Y, gasPos.Z, 20, 1.0f, true, false, 0.0f);

                for (int i = 0; i < suspects.Count; i++)
                {
                    Ped s = suspects[i];
                    if (s != null && s.Exists() && s.IsAlive)
                    {
                        if (s.Position.DistanceTo(gasPos) < 8.0f)
                        {
                            s.Tasks.Cower(5000);
                            NativeFunction.Natives.SET_PED_SEEING_RANGE(s, 5.0f);
                            NativeFunction.Natives.PLAY_PED_AMBIENT_SPEECH_NATIVE(s, "GENERIC_COUGH", "SPEECH_PARAMS_FORCE_SHOUTED");
                        }
                    }
                }
            }
            catch { }
        }

        private static void ApplyStunEffect(Ped ped, int durationMs)
        {
            try
            {
                ped.BlockPermanentEvents = false;
                ped.KeepTasks = true;
                NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, durationMs / 2, durationMs, 0, false, false, false);
                NativeFunction.Natives.SET_PED_ACCURACY(ped, 10);
                NativeFunction.Natives.PLAY_PED_AMBIENT_SPEECH_NATIVE(ped, "GENERIC_FRIGHTENED_HIGH", "SPEECH_PARAMS_FORCE_SHOUTED");
            }
            catch { }
        }
    }
}
