using System;
using Rage;
using Rage.Native;

namespace SistemaPolicialIntegral.Core
{
    public static class AudioEngine
    {
        public static void PlaySirdeeOpen()
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Event_Start_Text", "GTAO_FM_Events_Soundset", true);
        }

        public static void PlaySirdeeClose()
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Event_Message_Purple", "GTAO_FM_Events_Soundset", true);
        }

        public static void PlayDragerBeep()
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Green", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
        }

        public static void PlayTaserClick()
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
        }

        public static void PlayMenuSelect()
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
        }
    }
}
