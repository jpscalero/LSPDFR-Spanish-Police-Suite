using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_FlammableTankerTruckChase", CalloutProbability.High)]
    public class PC_FlammableTankerTruckChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_FlammableTankerTruckChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Camion Cisterna de Combustible Altamente Inflamable"; } }
        public override string DispatchSubtitle { get { return "Camion cisterna cargado de gasolina huye a gran velocidad con riesgo de explosion masiva."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(2600.4f, 1500.2f, 24.5f),
                new Vector3(1850.4f, 2600.2f, 45.5f),
                new Vector3(-2095.4f, -320.2f, 13.2f),
                new Vector3(620.4f, 268.2f, 103.2f),
                new Vector3(2580.4f, 360.2f, 108.5f),
                new Vector3(-70.4f, -1760.2f, 29.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("PHANTOM", SpawnPoint, 0f, true, 1.2f);
            CreatePursuitDriver("g_m_m_armgoon_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[PELIGRO EXPLOSIVO] ¡Cisterna de combustible presurizada! Fuego prohibido al tanque.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
