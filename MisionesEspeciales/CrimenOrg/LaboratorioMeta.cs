using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.CrimenOrg
{
    [CalloutInfo("Laboratorio de Metanfetamina Clandestino", CalloutProbability.Medium)]
    public class LaboratorioMeta : BaseMisionCallout
    {
        private int secondsUntilExplosion;
        private bool explosionTriggered;

        public LaboratorioMeta()
        {
            secondsUntilExplosion = 120;
            explosionTriggered = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Operación UDYCO: Localizado presunto laboratorio clandestino de metanfetamina.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[UDYCO / CNP]~w~ Agente, asegure el perímetro. Hay reactivos químicos volátiles.", 5000);

            // Spawneo de sospechosos armados
            SpawnSuspect(SpawnPoint.Around(5f), "g_m_m_mexgoon_01", "WEAPON_MICROSMG", 120, 50);
            SpawnSuspect(SpawnPoint.Around(8f), "g_m_m_mexgoon_01", "WEAPON_PISTOL", 100, 25);
            SpawnSuspect(SpawnPoint.Around(10f), "g_m_y_salvagoon_01", "WEAPON_PUMPSHOTGUN", 120, 50);
            SpawnSuspect(SpawnPoint.Around(12f), "g_m_y_salvagoon_01", "WEAPON_PISTOL", 100, 0);

            // Spawneo de trabajadores forzados
            SpawnVictim(SpawnPoint.Around(4f), "a_m_m_farmer_01");

            // Vehículo de los narcos
            SpawnCalloutVehicle(SpawnPoint.Around(15f), "burrito3", 0f);

            // Hacer que los sospechosos ataquen al jugador
            foreach (Ped suspect in Suspects)
            {
                if (suspect != null && suspect.Exists() && suspect.IsAlive)
                {
                    suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                }
            }
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            // Cuenta atrás de riesgo de explosión
            if (secondsUntilExplosion > 0)
            {
                secondsUntilExplosion--;
                if (secondsUntilExplosion == 30)
                {
                    Game.DisplayNotification("~r~¡PELIGRO!~w~ Fugas de gas en el laboratorio químico. Riesgo inminente de explosión.");
                }
            }
            else if (!explosionTriggered)
            {
                explosionTriggered = true;
                NativeFunction.Natives.ADD_EXPLOSION(SpawnPoint.X, SpawnPoint.Y, SpawnPoint.Z, 2, 5.0f, true, false, 1.0f);
                Game.DisplayNotification("~r~¡DEFLAGRACIÓN!~w~ El reactor químico del laboratorio ha explotado.");
            }

            // Comprobar si todos los sospechosos han sido neutralizados o arrestados
            bool anyAlive = false;
            foreach (Ped suspect in Suspects)
            {
                if (suspect != null && suspect.Exists() && suspect.IsAlive && !Functions.IsPedArrested(suspect))
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive)
            {
                CompleteMission("Laboratorio Desmantelado", "Se han incautado 250 kg de metanfetamina cristalina y precursores químicos. Zona asegurada.");
            }
        }
    }
}
