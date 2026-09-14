using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_FakePassportPrintShop", CalloutProbability.High)]
    public class FD_FakePassportPrintShop : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_FakePassportPrintShop"; } }
        public override string DisplayName { get { return "Operacion FIB: Imprenta Clandestina de Documentacion y Pasaportes"; } }
        public override string DispatchSubtitle { get { return "Red de falsificacion internacional con matrices oficiales y sellos consulares robados."; } }
        public override int ThreatLevel { get { return 4; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(450.2f, -1820.4f, 28.5f), new Vector3(1120.4f, -1450.2f, 34.5f), new Vector3(-850.5f, -1250.4f, 5.8f), new Vector3(750.2f, -850.5f, 24.5f), new Vector3(-350.4f, -1450.2f, 30.5f), new Vector3(250.5f, -2050.4f, 18.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 3; i++)
            {
                CreateFederalTarget("g_m_y_korean_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 60);
            }

            AddEvidenceItem("prop_cs_dvd_case", SpawnPoint.Around(2f), "Pasaportes falsificados de la UE");
            AddEvidenceItem("prop_novel_01", SpawnPoint.Around(3.5f), "Matrices y sellos consulares");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB FRAUDE] ¡Orden federal de allanamiento! Reduzcan a los falsificadores y aseguren las pruebas.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 15000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Los sospechosos encendieron el horno industrial para quemar los pasaportes!");
                TriggerEnvironmentalHazard(SpawnPoint.Around(2f), "core", "ent_sht_flame", 6f, 18000);
            }
        
        }
    }
}
