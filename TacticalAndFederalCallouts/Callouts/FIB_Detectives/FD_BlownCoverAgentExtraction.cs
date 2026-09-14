using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_BlownCoverAgentExtraction", CalloutProbability.High)]
    public class FD_BlownCoverAgentExtraction : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_BlownCoverAgentExtraction"; } }
        public override string DisplayName { get { return "Operacion FIB: Extraccion de Agente Encubierto Delatado"; } }
        public override string DispatchSubtitle { get { return "Agente encubierto del FIB ha sido descubierto en chatarreria y esta acorralado bajo fuego."; } }
        public override int ThreatLevel { get { return 5; } }

        private Ped _agent;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(480.5f, -3200.2f, 6.2f), new Vector3(150.4f, -2200.5f, 6.5f), new Vector3(-450.2f, -1700.4f, 18.5f), new Vector3(950.2f, -1500.5f, 30.5f), new Vector3(2300.4f, 2550.2f, 46.5f), new Vector3(-1150.5f, 2700.2f, 19.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _agent = CreateUndercoverAgent("s_m_m_fiboffice_01", SpawnPoint.Around(2f), 0f, (WeaponHash)Game.GetHashKey("WEAPON_COMBATPISTOL"));

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("g_m_m_armgoon_01", SpawnPoint.Around(6f + (i * 3f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 70);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB] ¡Agente encubierto bajo fuego enemigo! Formen perímetro defensivo y extraigan al agente.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 16000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Llega un sedán con sicarios de la mafia para rematar al agente encubierto!");
                SpawnHostileReinforcementVehicle("SCHAFTER", "g_m_m_armgoon_01", (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 3, SpawnPoint.Around(45f), SpawnPoint);
            }
        
        }
    }
}
