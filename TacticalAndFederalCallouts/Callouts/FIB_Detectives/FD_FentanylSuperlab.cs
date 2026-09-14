using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_FentanylSuperlab", CalloutProbability.High)]
    public class FD_FentanylSuperlab : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_FentanylSuperlab"; } }
        public override string DisplayName { get { return "Operacion FIB: Desmantelamiento de Superlaboratorio de Fentanilo"; } }
        public override string DispatchSubtitle { get { return "Laboratorio quimico a gran escala con precursores letales y quimicos armados del cartel."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1050.5f, -2500.2f, 28.5f), new Vector3(720.4f, -2300.5f, 23.5f), new Vector3(1450.2f, -1650.4f, 65.5f), new Vector3(-620.5f, -1650.2f, 25.5f), new Vector3(250.4f, -3100.5f, 5.8f), new Vector3(880.2f, -2950.4f, 5.8f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("g_m_y_mexgoon_01", SpawnPoint.Around(3f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTSHOTGUN"), 80);
            }

            AddEvidenceItem("prop_barrel_02a", SpawnPoint.Around(2f), "Precursores quimicos de fentanilo");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB NARCOTICOS] ¡Redada en superlaboratorio! Precaución extrema con las sustancias volátiles.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 14000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~y~[FIB] ¡Fuga de vapores químicos tóxicos por impacto de bala en los reactores!");
                TriggerEnvironmentalHazard(SpawnPoint.Around(3f), "core", "ent_sht_smoke", 10f, 22000);
            }
        
        }
    }
}
