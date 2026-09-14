using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_ShootingPlaneTakeoffAttempt", CalloutProbability.High)]
    public class PC_ShootingPlaneTakeoffAttempt : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_ShootingPlaneTakeoffAttempt"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Intento de Despegue de Avioneta con Fuego de Ráfaga"; } }
        public override string DispatchSubtitle { get { return "Avioneta de helice carretea en pista con tiradores abriendo fuego por la ventanilla."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1700.2f, 3250.4f, 41.1f),
                new Vector3(2130.4f, 4780.2f, 41.0f),
                new Vector3(-970.5f, -3000.2f, 13.9f),
                new Vector3(-1250.4f, -3010.5f, 13.9f),
                new Vector3(2500.4f, 4150.2f, 38.5f),
                new Vector3(1200.4f, 2700.2f, 38.0f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            IsNauticalOrAerial = true;
            CreatePursuitVehicle("VELUM", SpawnPoint, 90f, true, 1.3f);
            CreatePursuitDriver("s_m_y_blackops_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
            CreatePursuitPassenger("s_m_y_blackops_01", SuspectVehicle, 0, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 90);
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            if (Driver != null && Driver.Exists() && SuspectVehicle != null && SuspectVehicle.Exists())
            {
                SuspectVehicle.IsEngineOn = true;
                PursuitConfigurator.ConfigurePlanePilot(Driver, SuspectVehicle, new Vector3(SpawnPoint.X + 800f, SpawnPoint.Y, SpawnPoint.Z + 150f));
            }

            Game.DisplaySubtitle("~r~[DESPEGUE HOSTIL] ¡La aeronave está acelerando en pista! Inmovilícenla antes de ganar altura.");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
