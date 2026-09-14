using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Persecuciones
{
    [CalloutInfo("PC_CoastalSpeedboatChase", CalloutProbability.High)]
    public class PC_CoastalSpeedboatChase : BasePursuitCallout
    {
        public override string CalloutIdentifier { get { return "PC_CoastalSpeedboatChase"; } }
        public override string DisplayName { get { return "Persecucion Extrema: Lancha Rapida en Costa Maritima"; } }
        public override string DispatchSubtitle { get { return "Embarcacion de alta velocidad con narcos armados huyendo mar adentro."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-2085.4f, -1020.2f, 1.5f),
                new Vector3(-3420.5f, 950.4f, 1.5f),
                new Vector3(-1605.4f, -1450.8f, 1.5f),
                new Vector3(3120.4f, -350.2f, 1.5f),
                new Vector3(3850.2f, 1200.5f, 1.5f),
                new Vector3(-1850.8f, 2050.2f, 1.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            IsNauticalOrAerial = true;
            CreatePursuitVehicle("TROPIC2", SpawnPoint, 0f, true, 1.5f);
            CreatePursuitDriver("g_m_m_armgoon_01", SuspectVehicle, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"));
            CreatePursuitPassenger("g_m_m_armgoon_01", SuspectVehicle, 0, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 70);
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Ped player = Game.LocalPlayer.Character;
            if (Driver != null && Driver.Exists() && SuspectVehicle != null && SuspectVehicle.Exists() && player != null)
            {
                PursuitConfigurator.ConfigureBoatDriver(Driver, SuspectVehicle, player);
            }

            Game.DisplaySubtitle("~r~[PERSECUCIÓN COSTERA] ¡Lancha rápida mar adentro! Empleen apoyo marítimo de guardacostas.");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
