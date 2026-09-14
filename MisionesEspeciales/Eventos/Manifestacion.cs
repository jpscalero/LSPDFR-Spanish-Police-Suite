using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Eventos
{
    [CalloutInfo("UPR / UIP: Manifestación Urbana No Autorizada", CalloutProbability.Medium)]
    public class Manifestacion : BaseMisionCallout
    {
        private bool escalated;
        private int crowdTimer;

        public Manifestacion()
        {
            escalated = false;
            crowdTimer = 45;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Orden Público: Concentración multitudinaria de protesta en vía principal con corte de tráfico.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[UPR / ANTIDISTURBIOS]~w~ Establezca línea policial de contención y supervise la masa ciudadana.", 6000);

            // Manifestantes
            SpawnSuspect(SpawnPoint.Around(3f), "a_m_y_hippie_01", "WEAPON_BAT", 100, 0);
            SpawnSuspect(SpawnPoint.Around(5f), "a_m_y_genstreet_01", "", 100, 0);
            SpawnSuspect(SpawnPoint.Around(7f), "a_m_y_skater_02", "WEAPON_BOTTLE", 100, 0);

            // Civiles pacifistas
            SpawnVictim(SpawnPoint.Around(4f), "a_f_y_hippie_01");
            SpawnVictim(SpawnPoint.Around(6f), "a_m_y_vinewood_01");

            // Furgón policial antidisturbios
            SpawnCalloutVehicle(SpawnPoint.Around(15f), "riot", 180f);
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            if (crowdTimer > 0)
            {
                crowdTimer--;
                if (crowdTimer == 15)
                {
                    Game.DisplayNotification("~y~[TENSIÓN CRECIENTE]~w~ Manifestantes radicales comienzan a increpar y arrojar objetos.");
                }
            }
            else if (!escalated)
            {
                escalated = true;
                Game.DisplayNotification("~r~¡DISTURBIOS!~w~ Los manifestantes arremeten contra la línea policial.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);

                foreach (Ped s in Suspects)
                {
                    if (s != null && s.Exists() && s.IsAlive)
                    {
                        s.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
            }

            bool anyAlive = false;
            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive && escalated)
            {
                CompleteMission("Dispositivo de Orden Público Concluido", "Alborotadores detenidos y vía pública reabierta a la circulación con apoyo de la UPR.");
            }
        }
    }
}
