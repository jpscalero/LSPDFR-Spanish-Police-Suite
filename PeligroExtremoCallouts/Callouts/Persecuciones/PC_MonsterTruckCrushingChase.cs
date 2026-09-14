using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_MonsterTruckCrushingChase", CalloutProbability.High)]
    public class PC_MonsterTruckCrushingChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_MonsterTruckCrushingChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Monster Truck Aplastando Turismos en Trafico"; } }
        public override string DispatchSubtitle { get { return "Vehiculo de ruedas gigantes aplasta el trafico civil saltando barreras viales."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1200.4f, 2700.2f, 38.0f),
                new Vector3(2500.4f, 4150.2f, 38.5f),
                new Vector3(1700.2f, 3250.4f, 41.1f),
                new Vector3(-2250.4f, 3250.8f, 32.8f),
                new Vector3(2800.5f, 3800.2f, 44.5f),
                new Vector3(-1100.2f, 4920.4f, 182.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("MONSTER", SpawnPoint, 0f, true, 1.4f);
            CreatePursuitDriver("g_m_y_mexgang_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_SAWNOFFSHOTGUN"));
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[MONSTER TRUCK] ¡Masa vehicular gigantesca aplastando vehículos a su paso!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
