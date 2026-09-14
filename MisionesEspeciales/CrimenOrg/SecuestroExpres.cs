using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.CrimenOrg
{
    [CalloutInfo("Secuestro Exprés en Vehículo a la Fuga", CalloutProbability.Medium)]
    public class SecuestroExpres : BaseMisionCallout
    {
        private Vehicle getawayCar;
        private Ped driver;
        private Ped gunman;
        private bool pursuitStarted;

        public SecuestroExpres()
        {
            pursuitStarted = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "¡URGENTE! Ciudadano introducido por la fuerza en maletero de turismo negro en fuga.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            getawayCar = SpawnCalloutVehicle(SpawnPoint, "felon", 0f);
            driver = SpawnSuspect(SpawnPoint, "g_m_y_korean_01", "WEAPON_PISTOL", 100, 25);
            gunman = SpawnSuspect(SpawnPoint, "g_m_y_korlieut_01", "WEAPON_MICROSMG", 120, 50);

            if (driver != null && driver.Exists() && getawayCar != null && getawayCar.Exists())
            {
                driver.WarpIntoVehicle(getawayCar, -1);
            }
            if (gunman != null && gunman.Exists() && getawayCar != null && getawayCar.Exists())
            {
                gunman.WarpIntoVehicle(getawayCar, 0);
            }

            // Víctima en maletero (simulada)
            SpawnVictim(SpawnPoint, "a_f_y_gencaspat_01");

            Game.DisplaySubtitle("~r~¡VEHÍCULO LOCALIZADO!~w~ El turismo sospechoso emprende la huida. ¡Deténgalo!", 5000);

            if (driver != null && driver.Exists())
            {
                driver.Tasks.CruiseWithVehicle(getawayCar, 45f, VehicleDrivingFlags.Emergency);
            }

            LSPD_First_Response.Mod.API.LHandle pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(pursuit, driver);
            Functions.AddPedToPursuit(pursuit, gunman);
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
                CompleteMission("Secuestro Exprés Resuelto", "Secuestradores detenidos y rehén rescatado en perfecto estado de salud.");
            }
        }
    }
}
