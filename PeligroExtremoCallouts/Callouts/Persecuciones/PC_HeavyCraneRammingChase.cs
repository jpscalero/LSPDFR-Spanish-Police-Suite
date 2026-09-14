using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_HeavyCraneRammingChase", CalloutProbability.High)]
    public class PC_HeavyCraneRammingChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_HeavyCraneRammingChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Grua Pesada de Gran Tonelaje Embestidora"; } }
        public override string DispatchSubtitle { get { return "Grua industrial blindada de gran tonelaje apartando patrullas y barricadas."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1250.4f, -2050.2f, 51.5f),
                new Vector3(710.4f, -1080.2f, 22.5f),
                new Vector3(-580.4f, -1780.2f, 22.5f),
                new Vector3(350.4f, -1450.2f, 18.5f),
                new Vector3(-1050.2f, -550.4f, 24.5f),
                new Vector3(-850.4f, -150.2f, 22.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("TOWTRUCK", SpawnPoint, 0f, true, 1.3f);
            CreatePursuitDriver("s_m_m_dockwork_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[GRÚA PESADA] ¡Vehículo de gran masa arremetiendo contra el tráfico!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
