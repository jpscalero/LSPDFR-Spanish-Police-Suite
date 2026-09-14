using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_ProtectedWitnessAmbush", CalloutProbability.High)]
    public class FD_ProtectedWitnessAmbush : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_ProtectedWitnessAmbush"; } }
        public override string DisplayName { get { return "Operacion FIB: Emboscada a Testigo Protegido en Autopista"; } }
        public override string DispatchSubtitle { get { return "Sicarios atacan sedan federal blindado para rematar a testigo clave del cartel."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1850.4f, 2600.5f, 45.6f), new Vector3(2450.2f, 3100.8f, 48.2f), new Vector3(-420.5f, 3850.4f, 40.5f), new Vector3(1200.4f, 1850.4f, 52.4f), new Vector3(-1050.2f, 2800.5f, 22.5f), new Vector3(2100.5f, 2150.4f, 54.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            Vehicle fedCar = SpawnVehicle("STANIER", SpawnPoint, 0f);
            if (fedCar != null) fedCar.EngineHealth = 0f;

            Ped witness = SpawnPed("a_m_m_business_01", SpawnPoint.Around(2f), 0f);
            if (witness != null)
            {
                witness.Tasks.Cower(-1);
                AttachBlipToHostage(witness, System.Drawing.Color.Green, 0.6f);
                Hostages.Add(witness);
            }

            CreateUndercoverAgent("s_m_m_fiboffice_01", SpawnPoint.Around(1.5f), 0f, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL50"));

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("g_m_m_armgoon_01", SpawnPoint.Around(6f + (i * 3f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 80);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[FIB] ¡Fuego sobre el testigo protegido! Establezcan un perímetro de defensa.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 15000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Segunda oleada de sicarios del cartel llega en un todoterreno blindado!");
                SpawnHostileReinforcementVehicle("GRANGER", "g_m_m_armgoon_01", (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 3, SpawnPoint.Around(50f), SpawnPoint);
            }
        
        }
    }
}
