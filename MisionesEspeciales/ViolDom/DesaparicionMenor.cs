using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.ViolDom
{
    [CalloutInfo("UFAM: Sustracción Parental de Menor en Fuga", CalloutProbability.Medium)]
    public class DesaparicionMenor : BaseMisionCallout
    {
        private Vehicle suspectCar;
        private Ped suspectDriver;
        private bool pursuitStarted;

        public DesaparicionMenor()
        {
            pursuitStarted = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Aviso UFAM: Progenitor sin custodia sustrae a menor y huye a gran velocidad en turismo familiar.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            suspectCar = SpawnCalloutVehicle(SpawnPoint, "stanier", 0f);
            suspectDriver = SpawnSuspect(SpawnPoint, "a_m_m_business_01", "", 100, 0);

            if (suspectDriver != null && suspectDriver.Exists() && suspectCar != null && suspectCar.Exists())
            {
                suspectDriver.WarpIntoVehicle(suspectCar, -1);
            }

            // Menor en asiento de atrás (simulado)
            SpawnVictim(SpawnPoint, "a_f_y_bevhills_04");

            Game.DisplaySubtitle("~b~[UFAM]~w~ Vehículo del progenitor localizado. Intercéptele garantizando la seguridad del menor.", 6000);

            if (suspectDriver != null && suspectDriver.Exists())
            {
                suspectDriver.Tasks.CruiseWithVehicle(suspectCar, 35f, VehicleDrivingFlags.Normal);
            }

            LSPD_First_Response.Mod.API.LHandle pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(pursuit, suspectDriver);
            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
            pursuitStarted = true;
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            bool anyAlive = false;
            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive && pursuitStarted)
            {
                CompleteMission("Menor Recuperado Ileso", "Vehículo detenido sin colisión. Menor bajo custodia policial para restitución formal al tutor legal.");
            }
        }
    }
}
