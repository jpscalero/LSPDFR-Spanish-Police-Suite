using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_MoneyLaunderingMansion", CalloutProbability.High)]
    public class FD_MoneyLaunderingMansion : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_MoneyLaunderingMansion"; } }
        public override string DisplayName { get { return "Operacion FIB: Lavado de Capitales en Mansion de Rockford Hills"; } }
        public override string DispatchSubtitle { get { return "Contables del cartel triturando documentos financieros mientras la seguridad privada repele al FIB."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-780.4f, 450.2f, 105.5f), new Vector3(-1050.2f, 520.4f, 102.5f), new Vector3(-550.5f, 620.2f, 120.5f), new Vector3(-1250.4f, 480.2f, 95.5f), new Vector3(-680.2f, 550.5f, 110.2f), new Vector3(-920.5f, 410.4f, 98.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("s_m_y_blackops_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 85), (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), 90);
            }

            AddEvidenceItem("prop_security_case_01", SpawnPoint.Around(2f), "Maletin con millones en efectivo");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB DELITOS ECONOMICOS] ¡Allanen la mansión! Eviten el borrado de las cuentas bancarias off-shore.");
            StartDataWipeCountdown(SpawnPoint.Around(2f), 65);
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 18000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Guardias de seguridad privada de élite acuden desde la entrada principal!");
                SpawnHostileReinforcementVehicle("DUBSTA", "s_m_y_blackops_01", (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 2, SpawnPoint.Around(45f), SpawnPoint);
            }
        
        }
    }
}
