using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;

namespace PeligroExtremoCallouts.Core
{
    /// <summary>
    /// Gestion de rendiciones realistas bajo fuerza abrumadora y negociacion con rehenes.
    /// </summary>
    public static class TacticalNegotiationManager
    {
        private static readonly HashSet<Ped> _surrenderedPeds = new HashSet<Ped>();

        public static bool CheckAndExecuteSurrender(Ped suspect, Ped player, List<Ped> nearbyOfficers)
        {
            if (suspect == null || !suspect.Exists() || !suspect.IsAlive) return false;
            if (_surrenderedPeds.Contains(suspect)) return true;

            try
            {
                // Condiciones de rendicion realista:
                // 1. El jugador le apunta directamente con arma
                // 2. Esta a menos de 10 metros
                // 3. Su salud ha recibido dano o esta acorralado sin coberturas vivas
                bool playerAimingAtSuspect = NativeFunction.Natives.IS_PLAYER_FREE_AIMING_AT_ENTITY<bool>(Game.LocalPlayer, suspect);
                float distToPlayer = suspect.Position.DistanceTo(player.Position);

                int officerPressure = (playerAimingAtSuspect ? 1 : 0);
                for (int i = 0; i < nearbyOfficers.Count; i++)
                {
                    Ped off = nearbyOfficers[i];
                    if (off != null && off.Exists() && off.IsAlive && off.Position.DistanceTo(suspect.Position) < 12.0f)
                    {
                        officerPressure++;
                    }
                }

                if (officerPressure >= 2 && distToPlayer < 9.0f && (suspect.Health < 80 || suspect.Armor <= 0))
                {
                    ExecuteSurrender(suspect);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.TacticalNegotiationManager.EvaluateSurrenderConditions: " + ex.Message);
            }

            return false;
        }

        private static void ExecuteSurrender(Ped suspect)
        {
            try
            {
                _surrenderedPeds.Add(suspect);

                suspect.BlockPermanentEvents = true;
                suspect.Tasks.ClearImmediately();

                // Soltar arma al suelo
                NativeFunction.Natives.SET_PED_DROPS_WEAPON_WHEN_DEAD(suspect, true);
                NativeFunction.Natives.SET_PED_DROPS_WEAPONS_WHEN_HIT_BY_VEHICLE(suspect, true);
                NativeFunction.Natives.SET_CURRENT_PED_WEAPON(suspect, NativeFunction.Natives.GET_HASH_KEY<uint>("WEAPON_UNARMED"), true);

                // Animacion de manos en la nuca y de rodillas
                NativeFunction.Natives.REQUEST_ANIM_DICT("random@arrests");
                NativeFunction.Natives.REQUEST_ANIM_DICT("random@arrests@busted");

                GameFiber.StartNew(delegate
                {
                    int timeout = 0;
                    while (!NativeFunction.Natives.HAS_ANIM_DICT_LOADED<bool>("random@arrests") && timeout < 20)
                    {
                        GameFiber.Yield();
                        timeout++;
                    }

                    if (suspect != null && suspect.Exists() && suspect.IsAlive)
                    {
                        NativeFunction.Natives.TASK_PLAY_ANIM(suspect, "random@arrests", "kneeling_arrest_idle", 8.0f, -8.0f, -1, 1, 0f, false, false, false);
                        NativeFunction.Natives.PLAY_PED_AMBIENT_SPEECH_NATIVE(suspect, "GENERIC_SURRENDER", "SPEECH_PARAMS_FORCE_SHOUTED");
                        Game.DisplaySubtitle("~g~[RENDICION TACTICA]~w~ ¡El sospechoso se rinde y se arrodilla con las manos en la nuca! Esposenlo.");
                    }
                });
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts.TacticalNegotiationManager.ExecuteSurrender: " + ex.Message);
            }
        }

        public static bool IsSurrendered(Ped ped)
        {
            return _surrenderedPeds.Contains(ped);
        }

        public static void Clear()
        {
            _surrenderedPeds.Clear();
        }
    }
}
