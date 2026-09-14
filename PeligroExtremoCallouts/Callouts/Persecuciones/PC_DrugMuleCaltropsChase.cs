using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_DrugMuleCaltropsChase", CalloutProbability.High)]
    public class PC_DrugMuleCaltropsChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_DrugMuleCaltropsChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Mula de la Droga Desplegando Abrojos (Caltrops)"; } }
        public override string DispatchSubtitle { get { return "Traficante arroja cadenas de puas y abrojos metalicos para reventar neumaticos de patrullas."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1450.4f, 2600.2f, 45.5f),
                new Vector3(2100.2f, 3200.4f, 48.0f),
                new Vector3(-800.5f, 3500.2f, 30.5f),
                new Vector3(550.4f, 1200.2f, 65.4f),
                new Vector3(-1400.2f, 2200.4f, 25.5f),
                new Vector3(1850.5f, 1500.4f, 52.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("BURRITO", SpawnPoint, 0f, true, 1.3f);
            CreatePursuitDriver("g_m_y_mexgang_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[ABROJOS EN CALZADA] ¡Cuidado! El fugitivo está arrojando pinchos y obstáculos tras de sí.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

            if (new Random().Next(0, 450) == 77)
            {
                DropCaltropsBehindVehicle();
            }
        
        }
    }
}
