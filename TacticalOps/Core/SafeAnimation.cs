using System;
using Rage;
using Rage.Native;

namespace TacticalOps.Core
{
    public static class SafeAnimation
    {
        public static bool Play(Ped ped, string dict, string anim, float blendInSpeed = 4.0f, AnimationFlags flags = AnimationFlags.None)
        {
            if (ped == null || !ped.Exists() || ped.IsDead) return false;
            if (string.IsNullOrEmpty(dict) || string.IsNullOrEmpty(anim)) return false;

            try
            {
                bool exists = false;
                try
                {
                    exists = NativeFunction.Natives.DOES_ANIM_DICT_EXIST<bool>(dict);
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalOps.SafeAnimation.Check: " + ex.Message);
                }

                if (!exists)
                {
                    Game.LogTrivial("TacticalOps.SafeAnimation: Diccionario de animacion no disponible en GTA V: " + dict);
                    return false;
                }

                NativeFunction.Natives.REQUEST_ANIM_DICT(dict);
                uint start = Game.GameTime;
                while (!NativeFunction.Natives.HAS_ANIM_DICT_LOADED<bool>(dict) && (Game.GameTime - start < 1000))
                {
                    GameFiber.Yield();
                }

                if (NativeFunction.Natives.HAS_ANIM_DICT_LOADED<bool>(dict))
                {
                    NativeFunction.Natives.TASK_PLAY_ANIM(ped, dict, anim, blendInSpeed, -4.0f, -1, (int)flags, 0, false, false, false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps.SafeAnimation.Play error: " + ex.Message);
            }
            return false;
        }
    }
}
