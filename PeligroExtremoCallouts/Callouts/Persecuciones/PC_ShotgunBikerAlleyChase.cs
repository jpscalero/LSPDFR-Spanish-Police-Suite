using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_ShotgunBikerAlleyChase", CalloutProbability.High)]
    public class PC_ShotgunBikerAlleyChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_ShotgunBikerAlleyChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Peloton de Motociclistas con Escopetas"; } }
        public override string DispatchSubtitle { get { return "Peloton de 3 motocicletas chopper con escopetas huyendo por callejones angostos."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(128.4f, -1650.2f, 29.5f),
                new Vector3(-1050.4f, -480.2f, 36.8f),
                new Vector3(380.2f, -800.4f, 29.2f),
                new Vector3(-520.4f, -1220.5f, 18.5f),
                new Vector3(810.2f, -2150.4f, 29.5f),
                new Vector3(1150.4f, -420.5f, 67.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("DAEMON", SpawnPoint, 0f, false, 1.2f);
            CreatePursuitDriver("g_m_y_armgoon_02", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_SAWNOFFSHOTGUN"));

            // Segunda motocicleta
            Vehicle bike2 = SpawnVehicle("DAEMON", SpawnPoint.Around(4f), 0f);
            if (bike2 != null)
            {
                Ped b2Driver = SpawnPed("g_m_y_armgoon_02", bike2.Position, bike2.Heading);
                if (b2Driver != null)
                {
                    Rage.Native.NativeFunction.Natives.SET_PED_INTO_VEHICLE(b2Driver, bike2, -1);
                    b2Driver.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_SAWNOFFSHOTGUN")).Ammo = -1;
                    ConfigurePedCombat(b2Driver, ConfigManager.SuspectAccuracy, 60, false);
                    AttachBlipToSuspect(b2Driver, System.Drawing.Color.DarkRed, 0.6f);
                    Suspects.Add(b2Driver);
                    SecondaryDrivers.Add(b2Driver);
                }
            }

            // Tercera motocicleta
            Vehicle bike3 = SpawnVehicle("DAEMON", SpawnPoint.Around(7f), 0f);
            if (bike3 != null)
            {
                Ped b3Driver = SpawnPed("g_m_y_armgoon_02", bike3.Position, bike3.Heading);
                if (b3Driver != null)
                {
                    Rage.Native.NativeFunction.Natives.SET_PED_INTO_VEHICLE(b3Driver, bike3, -1);
                    b3Driver.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_SAWNOFFSHOTGUN")).Ammo = -1;
                    ConfigurePedCombat(b3Driver, ConfigManager.SuspectAccuracy, 60, false);
                    AttachBlipToSuspect(b3Driver, System.Drawing.Color.DarkRed, 0.6f);
                    Suspects.Add(b3Driver);
                    SecondaryDrivers.Add(b3Driver);
                }
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            if (PursuitCreated)
            {
                for (int d = 0; d < SecondaryDrivers.Count; d++)
                {
                    Ped sd = SecondaryDrivers[d];
                    if (sd != null && sd.Exists() && sd.IsInAnyVehicle(false))
                    {
                        PursuitConfigurator.ConfigureAggressiveDriver(sd, sd.CurrentVehicle, 42f);
                    }
                }
            }

            Game.DisplaySubtitle("~r~[BIKERS ARMADOS] ¡Pelotón de 3 motocicletas con escopetas recortadas huyendo!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
