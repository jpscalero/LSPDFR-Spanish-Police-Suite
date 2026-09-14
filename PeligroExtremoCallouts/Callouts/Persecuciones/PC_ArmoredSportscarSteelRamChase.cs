using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_ArmoredSportscarSteelRamChase", CalloutProbability.High)]
    public class PC_ArmoredSportscarSteelRamChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_ArmoredSportscarSteelRamChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Deportivo Blindado con Blindaje de Acero"; } }
        public override string DispatchSubtitle { get { return "Superdeportivo modificado con acero balistico y motor sobrealimentado en autopista."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-2095.4f, -320.2f, 13.2f),
                new Vector3(620.4f, 268.2f, 103.2f),
                new Vector3(2580.4f, 360.2f, 108.5f),
                new Vector3(1180.4f, -330.2f, 69.2f),
                new Vector3(-70.4f, -1760.2f, 29.5f),
                new Vector3(265.4f, -1260.2f, 29.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("TURISMOR", SpawnPoint, 0f, true, 2.2f);
            CreatePursuitDriver("g_m_y_mexgang_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
            CreatePursuitPassenger("g_m_y_mexgang_01", SuspectVehicle, 0, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 80);
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[DEPORTIVO DE ALTA POTENCIA] ¡Velocidad punta extrema! Mantengan contacto visual.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
