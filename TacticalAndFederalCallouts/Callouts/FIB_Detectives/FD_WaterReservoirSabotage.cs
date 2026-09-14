using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_WaterReservoirSabotage", CalloutProbability.High)]
    public class FD_WaterReservoirSabotage : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_WaterReservoirSabotage"; } }
        public override string DisplayName { get { return "Operacion FIB: Sabotaje Biologico a la Presa de Agua Potable"; } }
        public override string DispatchSubtitle { get { return "Celula terrorista vertiendo patogenos quimicos en las turbinas del embalse principal."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1650.5f, 20.2f, 165.5f), new Vector3(1710.2f, 85.4f, 165.5f), new Vector3(1580.4f, -45.2f, 165.5f), new Vector3(1690.2f, 120.5f, 165.5f), new Vector3(1620.5f, -95.2f, 165.5f), new Vector3(1750.4f, 45.2f, 165.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("s_m_y_blackops_01", SpawnPoint.Around(3.5f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 90);
            }

            AddEvidenceItem("prop_barrel_02a", SpawnPoint.Around(2f), "Bidon con toxinas biologicas");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB BIOTERRORISMO] ¡Alerta en la presa! Neutralicen a los saboteadores antes de que contaminen la toma principal.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 15000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~y~[FIB] ¡Válvula de presión saboteada! Nube de vapor químico en la estación de bombeo.");
                TriggerEnvironmentalHazard(SpawnPoint.Around(3f), "core", "ent_sht_steam", 10f, 22000);
            }
        
        }
    }
}
