using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_CarBombFleeingDowntown", CalloutProbability.High)]
    public class PC_CarBombFleeingDowntown : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_CarBombFleeingDowntown"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Coche Bomba en Huida por el Centro Urbano"; } }
        public override string DispatchSubtitle { get { return "Vehiculo cargado con explosivos C4 huyendo hacia zonas civiles densamente pobladas."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(215.4f, -805.2f, 30.8f),
                new Vector3(-280.4f, -780.2f, 30.5f),
                new Vector3(380.4f, -650.2f, 28.5f),
                new Vector3(-1380.4f, -480.2f, 30.5f),
                new Vector3(-750.4f, -70.2f, 36.5f),
                new Vector3(-60.4f, -1100.2f, 26.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("EMPEROR", SpawnPoint, 0f, false, 1.1f);
            CreatePursuitDriver("g_m_m_armgoon_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            StartVehicleBombCountdown(180);
            Game.DisplaySubtitle("~r~[ALERTA COCHE BOMBA] ¡Detengan el vehículo antes de que detone en el núcleo urbano!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
