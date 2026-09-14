using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_SupercarWrongWayPursuit", CalloutProbability.High)]
    public class PC_SupercarWrongWayPursuit : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_SupercarWrongWayPursuit"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Superdeportivo en Sentido Contrario en Autopista"; } }
        public override string DispatchSubtitle { get { return "Fugitivo en superdeportivo a contramano esquivando a gran velocidad el trafico civil."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(2580.4f, 360.2f, 108.5f),
                new Vector3(1180.4f, -330.2f, 69.2f),
                new Vector3(-2095.4f, -320.2f, 13.2f),
                new Vector3(620.4f, 268.2f, 103.2f),
                new Vector3(-70.4f, -1760.2f, 29.5f),
                new Vector3(265.4f, -1260.2f, 29.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            UseCustomDriverConfig = true;
            CreatePursuitVehicle("ENTITYXF", SpawnPoint, 0f, true, 2.0f);
            CreatePursuitDriver("g_m_y_mexgang_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));

            if (Driver != null && SuspectVehicle != null)
            {
                PursuitConfigurator.ConfigureWrongWayDriver(Driver, SuspectVehicle, 52f);
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            if (Driver != null && Driver.Exists() && SuspectVehicle != null && SuspectVehicle.Exists())
            {
                PursuitConfigurator.ConfigureWrongWayDriver(Driver, SuspectVehicle, 55f);
            }

            Game.DisplaySubtitle("~r~[CONTRAMANO EN AUTOPISTA] ¡El superdeportivo circula en dirección contraria entre el tráfico!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
