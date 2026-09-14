using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_OverpoweredMuscleCarChase", CalloutProbability.High)]
    public class PC_OverpoweredMuscleCarChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_OverpoweredMuscleCarChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Muscle Car 500HP Modificado a Gran Velocidad"; } }
        public override string DispatchSubtitle { get { return "Vehiculo de competicion con aceleracion brutal derrapando en curvas urbanas."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(430.4f, -650.2f, 28.5f),
                new Vector3(-210.4f, -1020.2f, 30.5f),
                new Vector3(250.4f, -1200.2f, 38.5f),
                new Vector3(1150.4f, -420.2f, 67.2f),
                new Vector3(-800.4f, -150.2f, 37.5f),
                new Vector3(1690.4f, 3280.2f, 41.1f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("DOMINATOR", SpawnPoint, 0f, false, 2.5f);
            CreatePursuitDriver("g_m_y_ballasout_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[500HP TRUCADO] ¡Aceleración masiva y derrapes evasivos a alta velocidad!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
