using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.CrimenOrg
{
    [CalloutInfo("Alerta Antiterrorista: Amenaza Coche Bomba", CalloutProbability.Medium)]
    public class AtentadoTerrorista : BaseMisionCallout
    {
        private Vehicle bombVehicle;
        private int secondsToDetonate;
        private bool bombExploded;

        public AtentadoTerrorista()
        {
            secondsToDetonate = 90;
            bombExploded = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Nivel 4 Alerta Antiterrorista: Furgoneta con explosivos estacionada en zona concurrida.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~r~¡ALERTA TEDAX!~w~ Desaloje a los peatones inmediatamente y neutralice al terrorista.", 6000);

            bombVehicle = SpawnCalloutVehicle(SpawnPoint, "speedo", 0f);

            // Terrorista vigilando con detonador
            SpawnSuspect(SpawnPoint.Around(10f), "g_m_m_armgoon_01", "WEAPON_COMBATMG", 150, 100);
            SpawnSuspect(SpawnPoint.Around(12f), "g_m_m_armgoon_01", "WEAPON_ASSAULTRIFLE", 130, 50);

            // Civiles asustados
            SpawnVictim(SpawnPoint.Around(6f), "a_f_y_bevhills_02");
            SpawnVictim(SpawnPoint.Around(8f), "a_m_m_skater_01");

            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive)
                {
                    s.Tasks.FightAgainst(Game.LocalPlayer.Character);
                }
            }
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            if (secondsToDetonate > 0)
            {
                secondsToDetonate--;
                if (secondsToDetonate == 20)
                {
                    Game.DisplayNotification("~r~¡CUENTA ATRÁS CRÍTICA!~w~ El temporizador de la furgoneta bomba marca 20 segundos.");
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "5s", "MP_MISSION_COUNTDOWN_SOUNDSET", false);
                }
            }
            else if (!bombExploded)
            {
                bombExploded = true;
                if (bombVehicle != null && bombVehicle.Exists())
                {
                    NativeFunction.Natives.ADD_EXPLOSION(bombVehicle.Position.X, bombVehicle.Position.Y, bombVehicle.Position.Z, 4, 15.0f, true, false, 2.0f);
                }
                Game.DisplayNotification("~r~¡DETONACIÓN!~w~ El coche bomba ha sido activado. Evacúe a los heridos.");
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

            if (!anyAlive)
            {
                CompleteMission("Dispositivo Antiterrorista Finalizado", "Célula yihadista neutralizada y artefacto explosivo bajo control de TEDAX-NRBQ.");
            }
        }
    }
}
