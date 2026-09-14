using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_HostileHelicopterSupportChase", CalloutProbability.High)]
    public class PC_HostileHelicopterSupportChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_HostileHelicopterSupportChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Fuga Terrestre con Helicopteco de Apoyo Hostil"; } }
        public override string DispatchSubtitle { get { return "Todoterreno de escape con helicoptero Buzzard hostil abriendo fuego desde el aire."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1680.4f, 3280.2f, 41.1f),
                new Vector3(2130.4f, 4780.2f, 41.0f),
                new Vector3(-970.5f, -3000.2f, 13.9f),
                new Vector3(2500.4f, 4150.2f, 38.5f),
                new Vector3(1200.4f, 2700.2f, 38.0f),
                new Vector3(-1250.4f, -3010.5f, 13.9f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("SCHAFTER", SpawnPoint, 0f, true, 1.3f);
            CreatePursuitDriver("g_m_m_armgoon_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));

            Vehicle heli = SpawnVehicle("BUZZARD", new Vector3(SpawnPoint.X, SpawnPoint.Y, SpawnPoint.Z + 45f), 0f, false);
            if (heli != null)
            {
                heli.IsEngineOn = true;
                Rage.Native.NativeFunction.Natives.SET_HELI_BLADES_FULL_SPEED(heli);

                Ped pilot = SpawnPed("s_m_y_blackops_01", heli.Position, heli.Heading);
                if (pilot != null)
                {
                    Rage.Native.NativeFunction.Natives.SET_PED_INTO_VEHICLE(pilot, heli, -1);
                    pilot.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_MICROSMG")).Ammo = -1;
                    ConfigurePedCombat(pilot, ConfigManager.SuspectAccuracy, 100, false);
                    AttachBlipToSuspect(pilot, System.Drawing.Color.Red, 0.7f);
                    Suspects.Add(pilot);
                }

                Ped gunner = SpawnPed("s_m_y_blackops_01", heli.Position, heli.Heading);
                if (gunner != null)
                {
                    Rage.Native.NativeFunction.Natives.SET_PED_INTO_VEHICLE(gunner, heli, 0);
                    gunner.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_COMBATMG")).Ammo = -1;
                    AttachBlipToSuspect(gunner, System.Drawing.Color.DarkRed, 0.65f);
                    Suspects.Add(gunner);
                    PassengerSetup ps = new PassengerSetup { PassengerPed = gunner, Weapon = (WeaponHash)Game.GetHashKey("WEAPON_COMBATMG"), Accuracy = 80 };
                    PassengerSetups.Add(ps);
                }
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Ped player = Game.LocalPlayer.Character;
            if (player != null && Suspects.Count > 1)
            {
                Ped pilot = Suspects[1];
                if (pilot != null && pilot.Exists() && pilot.IsInAnyVehicle(false))
                {
                    PursuitConfigurator.ConfigureHeliPilot(pilot, pilot.CurrentVehicle, player);
                }
            }

            Game.DisplaySubtitle("~r~[APOYO AÉREO HOSTIL] ¡Helicóptero Buzzard sobrevolando el convoy y abriendo fuego!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
