using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_ArmedEscortConvoyRam", CalloutProbability.High)]
    public class PC_ArmedEscortConvoyRam : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_ArmedEscortConvoyRam"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Convoy de Escolta Armada con Embestida Violenta"; } }
        public override string DispatchSubtitle { get { return "Vehiculo de fuga escoltado por todoterreno armado que embiste deliberadamente a las patrullas."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1200.4f, -1450.2f, 35.0f),
                new Vector3(-450.2f, -1650.4f, 18.5f),
                new Vector3(480.2f, -1350.4f, 29.2f),
                new Vector3(810.4f, -1800.2f, 29.0f),
                new Vector3(-1050.2f, -850.4f, 19.2f),
                new Vector3(1350.2f, -600.4f, 74.0f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("SCHAFTER", SpawnPoint, 0f, true, 1.2f);
            CreatePursuitDriver("g_m_m_armgoon_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
            CreatePursuitPassenger("g_m_m_armgoon_01", SuspectVehicle, 0, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 80);

            Vehicle escortVeh = SpawnVehicle("BALLER", SpawnPoint.Around(14f), 0f);
            if (escortVeh != null)
            {
                PursuitConfigurator.ReinforceVehicle(escortVeh, true, 1.4f);
                Ped escortDriver = SpawnPed("g_m_m_armgoon_01", escortVeh.Position, escortVeh.Heading);
                if (escortDriver != null)
                {
                    Rage.Native.NativeFunction.Natives.SET_PED_INTO_VEHICLE(escortDriver, escortVeh, -1);
                    escortDriver.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_MICROSMG")).Ammo = -1;
                    ConfigurePedCombat(escortDriver, ConfigManager.SuspectAccuracy, 100, false);
                    AttachBlipToSuspect(escortDriver, System.Drawing.Color.DarkRed, 0.65f);
                    Suspects.Add(escortDriver);
                    SecondaryDrivers.Add(escortDriver);
                }
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            if (SecondaryDrivers.Count > 0)
            {
                Ped escort = SecondaryDrivers[0];
                if (escort != null && escort.Exists() && escort.IsInAnyVehicle(false) && SuspectVehicle != null)
                {
                    PursuitConfigurator.ConfigureEscortDriver(escort, escort.CurrentVehicle, SuspectVehicle, 45f);
                }
            }

            Game.DisplaySubtitle("~r~[CONVOY ARMADO] ¡El todoterreno de escolta está embistiendo activamente a los vehículos policiales!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
