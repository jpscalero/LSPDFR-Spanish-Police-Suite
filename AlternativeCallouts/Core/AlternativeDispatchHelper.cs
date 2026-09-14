using System;
using System.Drawing;
using Rage;
using Rage.Native;

namespace AlternativeCallouts.Core
{
    public static class AlternativeDispatchHelper
    {
        public static void SendDispatchNotification(string title, string status, string message)
        {
            try
            {
                PlayRadioBeep();
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~SALA 091 / 062~s~", "~y~" + title + " [" + status + "]~s~", message);
            }
            catch
            {
                Game.DisplaySubtitle("~b~[091/062]~w~ " + title + ": " + message);
            }
        }

        public static void PlayRadioBeep()
        {
            try
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }
            catch { }
        }
    }
}
