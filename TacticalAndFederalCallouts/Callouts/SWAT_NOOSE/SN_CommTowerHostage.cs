using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_CommTowerHostage", CalloutProbability.High)]
    public class SN_CommTowerHostage : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_CommTowerHostage"; } }
        public override string DisplayName { get { return "Operacion SWAT: Francotiradores y Rehenes en Antena Repetidora"; } }
        public override string DispatchSubtitle { get { return "Tiradores apostados en la torre de comunicaciones retienen a los tecnicos de guardia."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-750.2f, 1280.4f, 328.2f), new Vector3(760.4f, 1275.2f, 360.5f), new Vector3(2500.5f, -420.2f, 95.4f), new Vector3(-2300.2f, 280.4f, 169.5f), new Vector3(-430.4f, 950.2f, 240.2f), new Vector3(1850.2f, 3680.4f, 40.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            CreateRoofSniper("s_m_m_marine_01", SpawnPoint + new Vector3(0f, 0f, 12f), (WeaponHash)Game.GetHashKey("WEAPON_SNIPERRIFLE"));
            CreateRoofSniper("s_m_m_marine_01", SpawnPoint + new Vector3(5f, -3f, 8f), (WeaponHash)Game.GetHashKey("WEAPON_HEAVYSNIPER"));

            for (int i = 0; i < 3; i++)
            {
                CreateTacticalSuspect("s_m_m_chemsec_01", SpawnPoint.Around(4f + (i * 3f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), 90);
            }

            Ped tech = CreateTacticalHostage("s_m_m_cntrybar_01", SpawnPoint + new Vector3(2f, 2f, 0f), 0f);
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Tiradores en la antena repetidora! Busquen cobertura inmediata y abatan a los francotiradores.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 22000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~y~[SWAT] ¡Llegan refuerzos hostiles en todoterreno para apoyar la toma de la antena!");
                SpawnHostileReinforcementVehicle("CONTENDER", "s_m_m_chemsec_01", (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 2, SpawnPoint.Around(50f), SpawnPoint);
            }
        
        }
    }
}
