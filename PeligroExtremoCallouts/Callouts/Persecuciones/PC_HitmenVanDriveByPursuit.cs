using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_HitmenVanDriveByPursuit", CalloutProbability.High)]
    public class PC_HitmenVanDriveByPursuit : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_HitmenVanDriveByPursuit"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Furgon de Sicarios con Fuego de Rafaga (>4 Ocupantes)"; } }
        public override string DispatchSubtitle { get { return "Furgon con comando de sicarios (>4 ocupantes) abriendo fuego continuo por las ventanillas."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(450.2f, -1680.5f, 35.4f),
                new Vector3(-520.8f, -410.2f, 35.1f),
                new Vector3(1250.4f, -1850.2f, 42.5f),
                new Vector3(-1250.4f, -1150.2f, 7.5f),
                new Vector3(1680.4f, 3280.2f, 41.1f),
                new Vector3(-2150.4f, 3850.2f, 32.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("GRANGER", SpawnPoint, 0f, true, 1.3f);

            CreatePursuitDriver("g_m_y_ballasout_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));

            // Armas con compatibilidad total de drive-by en asientos
            CreatePursuitPassenger("g_m_y_ballasout_01", SuspectVehicle, 0, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 100);
            CreatePursuitPassenger("g_m_y_ballaeast_01", SuspectVehicle, 1, (WeaponHash)Game.GetHashKey("WEAPON_MACHINEPISTOL"), 100);
            CreatePursuitPassenger("g_m_y_ballaeast_01", SuspectVehicle, 2, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 100);
            CreatePursuitPassenger("g_m_m_armgoon_01", SuspectVehicle, 3, (WeaponHash)Game.GetHashKey("WEAPON_COMBATPISTOL"), 80);
            CreatePursuitPassenger("g_m_m_armgoon_01", SuspectVehicle, 4, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 80);
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[PERSECUCION EXTREMA] ¡Furgon con 6 ocupantes abriendo fuego de rafaga por todas las ventanillas!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();
        }
    }
}
