using System;
using Rage;
using Rage.Native;
using TacticalOps.Core;

namespace TacticalOps.Injuries
{
    public static class BleedingSystem
    {
        private static bool isBleeding;
        private static uint lastBleedTime;

        public static bool IsBleeding
        {
            get { return isBleeding; }
            private set { isBleeding = value; }
        }

        static BleedingSystem()
        {
            isBleeding = false;
            lastBleedTime = 0;
        }

        public static void StartBleeding()
        {
            if (!isBleeding)
            {
                isBleeding = true;
                lastBleedTime = Game.GameTime;
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Bed", "WastedSounds", false);
                Game.LogTrivial("TacticalOps: Hemorragia activa iniciada en el sujeto.");
            }
        }

        public static void StopBleeding()
        {
            if (isBleeding)
            {
                isBleeding = false;
                NativeFunction.Natives.ANIMPOSTFX_STOP("Rampage");
                Game.DisplayNotification("~g~Hemorragia cohibida.~w~ El sangrado activo se ha detenido.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "MEDAL_UP", "HUD_MINI_GAME_SOUNDSET", false);
                Game.LogTrivial("TacticalOps: Hemorragia activa detenida.");
            }
        }

        public static void Update()
        {
            if (!isBleeding)
            {
                return;
            }

            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                isBleeding = false;
                return;
            }

            uint currentTime = Game.GameTime;
            uint interval = (uint)(Math.Max(1, ConfigManager.BleedDamageRate) * 1000);

            if (currentTime - lastBleedTime >= interval)
            {
                lastBleedTime = currentTime;

                // Restar 1 HP de vida
                if (player.Health > 10)
                {
                    player.Health -= 1;
                }

                // Efecto visual de sangre pulsante en pantalla
                NativeFunction.Natives.ANIMPOSTFX_PLAY("Rampage", 800, false);

                // Sonido de latido / dolor
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
            }
        }
    }
}
