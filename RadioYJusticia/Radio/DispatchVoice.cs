using System;
using Rage;
using Rage.Native;

namespace RadioYJusticia.Radio
{
    public static class DispatchVoice
    {
        public static void PlayRadioClick()
        {
            try
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CLICK_BACK", "WEB_NAVIGATION_SOUNDS_PHONE", false);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error al reproducir click de radio: " + ex.Message);
            }
        }

        public static void PlayEmergencySound()
        {
            try
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "TIMER_STOP", "HUD_MINI_GAME_SOUNDSET", false);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error al reproducir sonido de emergencia: " + ex.Message);
            }
        }

        public static void PlayDispatchMessage(string message)
        {
            try
            {
                // Click de apertura de microfonía
                PlayRadioClick();

                // Subtítulo oficial en pantalla con indicativo de Sala
                Game.DisplaySubtitle("~b~[SALA 091 / CENTRAL]~w~ " + message, 5000);

                Game.LogTrivial("RadioYJusticia [SALA 091]: " + message);

                // Click de cierre de microfonía tras 4.8 segundos
                GameFiber.StartNew(delegate
                {
                    GameFiber.Sleep(4800);
                    PlayRadioClick();
                });
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error al emitir mensaje de Sala: " + ex.Message);
            }
        }

        public static void PlayOfficerMessage(string indicativo, string message)
        {
            try
            {
                PlayRadioClick();

                Game.DisplaySubtitle("~y~[" + indicativo + "]~w~ " + message, 4500);

                Game.LogTrivial("RadioYJusticia [" + indicativo + "]: " + message);

                GameFiber.StartNew(delegate
                {
                    GameFiber.Sleep(4400);
                    PlayRadioClick();
                });
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error al emitir mensaje de agente: " + ex.Message);
            }
        }
    }
}
