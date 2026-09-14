using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_RooftopSniperHeist", CalloutProbability.High)]
    public class SN_RooftopSniperHeist : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_RooftopSniperHeist"; } }
        public override string DisplayName { get { return "Operacion SWAT: Nido de Francotiradores en Azotea de Rascacielos"; } }
        public override string DispatchSubtitle { get { return "Tiradores de precision cubren robo millonario desde lo alto de una grua de construccion."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-140.2f, -950.4f, 268.5f), new Vector3(-65.5f, -800.2f, 245.2f), new Vector3(-130.4f, -630.5f, 168.2f), new Vector3(-25.2f, -720.4f, 180.5f), new Vector3(120.4f, -880.2f, 155.4f), new Vector3(-75.2f, -1020.5f, 210.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            CreateRoofSniper("s_m_y_blackops_01", SpawnPoint + new Vector3(0f, 0f, 2f), (WeaponHash)Game.GetHashKey("WEAPON_HEAVYSNIPER"));
            CreateRoofSniper("s_m_y_blackops_01", SpawnPoint + new Vector3(6f, -4f, 2f), (WeaponHash)Game.GetHashKey("WEAPON_SNIPERRIFLE"));

            for (int i = 0; i < 2; i++)
            {
                CreateTacticalSuspect("g_m_m_armgoon_01", SpawnPoint.Around(5f + (i * 3f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), 90);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Tiradores de élite en la azotea! No asomen la cabeza sin cobertura.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 25000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Un helicóptero de extracción furtivo sobrevuela el helipuerto para recoger a los tiradores!");
                Vehicle heli = SpawnVehicle("FROGGER", SpawnPoint + new Vector3(0f, 0f, 35f), 0f, false);
            }
        
        }
    }
}
