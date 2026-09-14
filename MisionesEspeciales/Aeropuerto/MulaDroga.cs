using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Aeropuerto
{
    [CalloutInfo("Policía Aeroportuaria: Mula de Droga en Filtro de Llegadas", CalloutProbability.Medium)]
    public class MulaDroga : BaseMisionCallout
    {
        private bool suspectFled;

        public MulaDroga()
        {
            suspectFled = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Policía Aeroportuaria / Resguardo Fiscal: Pasajero de vuelo caliente con perfil sospechoso.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[RESGUARDO FISCAL]~w~ Solicite la documentación del sospechoso y proceda al registro de equipaje.", 6000);

            SpawnSuspect(SpawnPoint, "a_m_y_latino_01", "WEAPON_KNIFE", 100, 0);
            SpawnVictim(SpawnPoint.Around(3f), "s_m_m_security_01");
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            if (!suspectFled && Game.LocalPlayer.Character.DistanceTo(SpawnPoint) < 6f)
            {
                suspectFled = true;
                Game.DisplayNotification("~r~¡EL SOSPECHOSO EMPRENDE LA HUIDA!~w~ Corre hacia la salida de la terminal arrojando la maleta.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);

                foreach (Ped s in Suspects)
                {
                    if (s != null && s.Exists() && s.IsAlive)
                    {
                        s.Tasks.Flee(Game.LocalPlayer.Character, 200f, -1);
                    }
                }
                return;
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

            if (!anyAlive && suspectFled)
            {
                CompleteMission("Correo de Droga Interceptado", "Detenido con 4 kg de cocaína ocultos en el doble fondo de la maleta. Diligencias por tráfico a gran escala.");
            }
        }
    }
}
