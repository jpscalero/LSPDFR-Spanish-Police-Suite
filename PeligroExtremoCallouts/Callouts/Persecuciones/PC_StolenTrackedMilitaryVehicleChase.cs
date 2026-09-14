using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_StolenTrackedMilitaryVehicleChase", CalloutProbability.High)]
    public class PC_StolenTrackedMilitaryVehicleChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_StolenTrackedMilitaryVehicleChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Blindado Militar de Asalto Robado"; } }
        public override string DispatchSubtitle { get { return "Vehiculo de combate blindado robado de la base militar causando estragos."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-2250.4f, 3250.8f, 32.8f),
                new Vector3(2800.5f, 3800.2f, 44.5f),
                new Vector3(-1100.2f, 4920.4f, 182.5f),
                new Vector3(1500.4f, 2200.2f, 55.4f),
                new Vector3(-420.5f, 4400.8f, 58.2f),
                new Vector3(2550.2f, -380.4f, 92.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("INSURGENT", SpawnPoint, 0f, true, 1.3f);
            CreatePursuitDriver("s_m_y_blackops_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL50"));
            CreatePursuitPassenger("s_m_y_blackops_01", SuspectVehicle, 0, (WeaponHash)Game.GetHashKey("WEAPON_COMBATMG"), 100);
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[BLINDADO MILITAR] ¡Blindaje impenetrable y ametralladora pesada en marcha!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
