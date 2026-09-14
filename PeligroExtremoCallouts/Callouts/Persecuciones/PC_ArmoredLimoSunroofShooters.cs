using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_ArmoredLimoSunroofShooters", CalloutProbability.High)]
    public class PC_ArmoredLimoSunroofShooters : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_ArmoredLimoSunroofShooters"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Limusina Blindada con Tiradores por Techo Solar"; } }
        public override string DispatchSubtitle { get { return "Limusina blindada de alta masa con tiradores asomados por el techo practicable."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-250.4f, -920.2f, 32.2f),
                new Vector3(-1380.4f, -480.2f, 33.5f),
                new Vector3(-550.2f, -200.4f, 38.5f),
                new Vector3(-1085.4f, -250.2f, 37.8f),
                new Vector3(-160.4f, -605.2f, 35.5f),
                new Vector3(-810.2f, -180.4f, 37.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("STRETCH", SpawnPoint, 0f, true, 1.4f);
            CreatePursuitDriver("g_m_m_armgoon_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));

            // Armas compatibles con animaciones nativas de drive-by en asientos de vehiculo
            CreatePursuitPassenger("g_m_m_armgoon_01", SuspectVehicle, 0, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 100);
            CreatePursuitPassenger("g_m_m_armgoon_01", SuspectVehicle, 1, (WeaponHash)Game.GetHashKey("WEAPON_MACHINEPISTOL"), 100);
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[LIMUSINA BLINDADA] ¡Gran resistencia a maniobras PIT y fuego desde el techo practicable!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();
        }
    }
}
