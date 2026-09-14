using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_OffroadMountainRiverChase", CalloutProbability.High)]
    public class PC_OffroadMountainRiverChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_OffroadMountainRiverChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Huida Off-Road por Lechos de Rio y Riscos"; } }
        public override string DispatchSubtitle { get { return "Todoterreno de competicion huyendo campo a traves por barrancos de dificil acceso."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-1605.2f, 4520.8f, 125.4f),
                new Vector3(780.5f, 4175.2f, 165.2f),
                new Vector3(-1115.4f, 4925.2f, 218.4f),
                new Vector3(490.2f, 5585.4f, 792.5f),
                new Vector3(1905.4f, 5320.2f, 175.4f),
                new Vector3(-2180.4f, 4270.8f, 48.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("REBEL2", SpawnPoint, 0f, true, 1.5f);
            CreatePursuitDriver("g_m_m_armgoon_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"));
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[OFF-ROAD EXTREMO] ¡El sospechoso se desvía por el cauce rocoso del río!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
