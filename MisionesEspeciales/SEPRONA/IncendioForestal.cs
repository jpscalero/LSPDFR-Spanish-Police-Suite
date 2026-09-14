using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.SEPRONA
{
    [CalloutInfo("SEPRONA: Incendio Forestal Provocado", CalloutProbability.Medium)]
    public class IncendioForestal : BaseMisionCallout
    {
        private bool fireExtinguished;

        public IncendioForestal()
        {
            fireExtinguished = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "SEPRONA / Bomberos: Foco de incendio forestal activo con sospechoso huyendo a pie.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~g~[SEPRONA]~w~ Foco ígneo detectado. Localice y capture al presunto pirómano.", 6000);

            // Efecto de fuego simulado con explosión suave controlada
            NativeFunction.Natives.ADD_EXPLOSION(SpawnPoint.X, SpawnPoint.Y, SpawnPoint.Z, 3, 2.0f, true, false, 0.5f);

            // Pirómano huyendo
            Ped arsonist = SpawnSuspect(SpawnPoint.Around(15f), "a_m_y_hippy_01", "WEAPON_MOLOTOV", 100, 0);
            if (arsonist != null && arsonist.Exists())
            {
                arsonist.Tasks.Flee(Game.LocalPlayer.Character, 150f, -1);
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
                CompleteMission("Pirómano Forestal Detenido", "Autor material detenido in fraganti con acelerantes y mecheros. Unidades de extinción sofocando el foco.");
            }
        }
    }
}
