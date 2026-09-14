using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_ArmoredRammingTruck", CalloutProbability.High)]
    public class PC_ArmoredRammingTruck : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_ArmoredRammingTruck"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Camion Pesado Blindado con Embestida Frontal"; } }
        public override string DispatchSubtitle { get { return "Tractocamion blindado embiste frontalmente a las patrullas bloqueando calzadas."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1680.4f, 2600.2f, 45.5f),
                new Vector3(2450.2f, 3100.4f, 48.2f),
                new Vector3(-420.5f, 3850.2f, 40.5f),
                new Vector3(1200.4f, 1850.2f, 52.4f),
                new Vector3(-1050.2f, 2800.4f, 22.5f),
                new Vector3(2100.5f, 2150.8f, 54.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            UseCustomDriverConfig = true;
            CreatePursuitVehicle("PHANTOM", SpawnPoint, 0f, true, 1.5f);
            CreatePursuitDriver("g_m_m_armgoon_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"));
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Ped player = Game.LocalPlayer.Character;
            if (Driver != null && Driver.Exists() && SuspectVehicle != null && SuspectVehicle.Exists() && player != null)
            {
                PursuitConfigurator.ConfigureRammingDriver(Driver, SuspectVehicle, player, 48f);
            }

            Game.DisplaySubtitle("~r~[CAMIÓN DE IMPACTO] ¡El tractocamión no frena ante bloqueos! Eviten colisiones frontales directas.");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
