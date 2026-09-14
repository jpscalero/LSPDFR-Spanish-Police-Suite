using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_UltralightKartsTunnelChase", CalloutProbability.High)]
    public class PC_UltralightKartsTunnelChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_UltralightKartsTunnelChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Karts y Quads Ligeros en Red de Tuneles"; } }
        public override string DispatchSubtitle { get { return "Vehiculos ultraligeros de alta maniobrabilidad cruzando canales y tuneles estrechos."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-590.2f, -1780.5f, 22.8f),
                new Vector3(350.4f, -1450.2f, 18.5f),
                new Vector3(-850.4f, -150.2f, 22.5f),
                new Vector3(720.5f, -1080.2f, 21.5f),
                new Vector3(-1050.4f, -550.2f, 24.5f),
                new Vector3(-250.8f, -1920.4f, 20.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            CreatePursuitVehicle("BLAZER", SpawnPoint, 0f, false, 1.4f);
            CreatePursuitDriver("g_m_y_ballasout_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"));

            Vehicle quad2 = SpawnVehicle("BLAZER", SpawnPoint.Around(4f), 0f);
            if (quad2 != null)
            {
                Ped q2Driver = SpawnPed("g_m_y_ballasout_01", quad2.Position, quad2.Heading);
                if (q2Driver != null)
                {
                    Rage.Native.NativeFunction.Natives.SET_PED_INTO_VEHICLE(q2Driver, quad2, -1);
                    q2Driver.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_MICROSMG")).Ammo = -1;
                    ConfigurePedCombat(q2Driver, ConfigManager.SuspectAccuracy, 60, false);
                    AttachBlipToSuspect(q2Driver, System.Drawing.Color.DarkRed, 0.6f);
                    Suspects.Add(q2Driver);
                    SecondaryDrivers.Add(q2Driver);
                }
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            if (PursuitCreated && SecondaryDrivers.Count > 0)
            {
                Ped q2 = SecondaryDrivers[0];
                if (q2 != null && q2.Exists() && q2.IsInAnyVehicle(false))
                {
                    PursuitConfigurator.ConfigureAggressiveDriver(q2, q2.CurrentVehicle, 45f);
                }
            }

            Game.DisplaySubtitle("~r~[QUIVER DE TÚNELES] ¡Quads ultraligeros maniobrando en túneles estrechos!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
