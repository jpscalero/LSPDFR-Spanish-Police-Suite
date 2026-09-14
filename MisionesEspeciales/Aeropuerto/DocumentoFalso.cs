using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Aeropuerto
{
    [CalloutInfo("Policía Aeroportuaria: Uso de Documento de Identidad Falso", CalloutProbability.Medium)]
    public class DocumentoFalso : BaseMisionCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Policía Aeroportuaria: Detección de pasaporte falsificado en cabina de control fronterizo.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[CONTROL FRONTERIZO]~w~ Interrogue al pasajero retenido e inspeccione el documento irregular.", 6000);

            // Viajero con pasaporte falso
            SpawnSuspect(SpawnPoint, "a_m_m_eastsa_01", "", 100, 0);

            // Agente de fronteras
            SpawnVictim(SpawnPoint.Around(2f), "s_m_m_security_01");

            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive)
                {
                    s.Tasks.Cower(-1);
                }
            }
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

            if (!anyAlive)
            {
                CompleteMission("Falsedad Documental Confirmada", "Pasaporte biométrico manipulado intervenido. Detenido trasladado a dependencias de extranjería y fronteras.");
            }
        }
    }
}
