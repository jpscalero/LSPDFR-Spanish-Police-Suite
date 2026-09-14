using System;
using Rage;
using Rage.Native;

namespace TacticalOps.Negotiation
{
    public static class TensionSystem
    {
        private static float tension;
        private static bool isActive;

        public static float Tension
        {
            get { return tension; }
            set
            {
                tension = Math.Max(0.0f, Math.Min(100.0f, value));
            }
        }

        public static bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        static TensionSystem()
        {
            tension = 50.0f;
            isActive = false;
        }

        public static void Initialize(float initialTension)
        {
            Tension = initialTension;
            isActive = true;
            Game.LogTrivial("TacticalOps: TensionSystem inicializado con tension: " + (int)Tension + "%");
        }

        public static void AddTension(float amount)
        {
            Tension += amount;
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Bed", "WastedSounds", false);
            Game.LogTrivial("TacticalOps: Tension aumentada en " + (int)amount + "%. Nivel actual: " + (int)Tension + "%");
        }

        public static void ReduceTension(float amount)
        {
            Tension -= amount;
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "MEDAL_UP", "HUD_MINI_GAME_SOUNDSET", false);
            Game.LogTrivial("TacticalOps: Tension reducida en " + (int)amount + "%. Nivel actual: " + (int)Tension + "%");
        }

        public static void Update()
        {
            if (!isActive)
            {
                return;
            }

            DrawHUD();

            // Comprobar umbrales criticos de resolucion
            if (Tension >= 100.0f)
            {
                isActive = false;
                NegotiationManager.TriggerHostileAction();
            }
            else if (Tension <= 10.0f)
            {
                isActive = false;
                NegotiationManager.TriggerSurrender();
            }
        }

        private static void DrawHUD()
        {
            // Coordenadas normalizadas en la parte superior de la pantalla
            float bgX = 0.5f;
            float bgY = 0.055f;
            float bgW = 0.26f;
            float bgH = 0.038f;

            // Rectangulo de fondo negro translucido
            NativeFunction.Natives.DRAW_RECT(bgX, bgY, bgW, bgH, 0, 0, 0, 190);

            // Determinar color de la barra segun nivel de tension
            int r = 46;
            int g = 204;
            int b = 113;

            if (Tension >= 75.0f)
            {
                r = 231;
                g = 76;
                b = 60;
            }
            else if (Tension >= 40.0f)
            {
                r = 243;
                g = 156;
                b = 18;
            }

            // Barra rellena proporcional
            float maxFillW = bgW - 0.008f;
            float fillW = maxFillW * (Tension / 100.0f);
            float fillX = (bgX - (maxFillW / 2.0f)) + (fillW / 2.0f);

            NativeFunction.Natives.DRAW_RECT(fillX, bgY, fillW, bgH - 0.008f, r, g, b, 230);

            // Texto descriptivo sobre la barra
            NativeFunction.Natives.SET_TEXT_FONT(0);
            NativeFunction.Natives.SET_TEXT_SCALE(0.33f, 0.33f);
            NativeFunction.Natives.SET_TEXT_COLOUR(255, 255, 255, 255);
            NativeFunction.Natives.SET_TEXT_CENTRE(true);
            NativeFunction.Natives.BEGIN_TEXT_COMMAND_DISPLAY_TEXT("STRING");
            NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME("NIVEL DE TENSION: " + (int)Tension + "%");
            NativeFunction.Natives.END_TEXT_COMMAND_DISPLAY_TEXT(bgX, bgY - 0.013f);
        }
    }
}
