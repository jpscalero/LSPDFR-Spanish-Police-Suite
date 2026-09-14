using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_RansomwareServerFarm", CalloutProbability.High)]
    public class FD_RansomwareServerFarm : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_RansomwareServerFarm"; } }
        public override string DisplayName { get { return "Operacion FIB: Granja de Servidores de Ransomware Masivo"; } }
        public override string DispatchSubtitle { get { return "Celula de hackers armados bloqueando infraestructuras criticas y borrando los registros."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(720.5f, -1950.2f, 29.5f), new Vector3(1010.2f, -2530.4f, 28.3f), new Vector3(-350.4f, -2600.2f, 6.2f), new Vector3(880.2f, -2180.5f, 30.5f), new Vector3(150.5f, -3200.2f, 5.8f), new Vector3(-550.2f, -2450.4f, 13.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("s_m_m_chemsec_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_SMG"), 80);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB CIBERDELINCUENCIA] ¡Ataque informático en curso! Abatan a los atacantes y frenen el borrado en el servidor.");
            TriggerScriptAlarm("PORT_OF_LS_CONTAINER_ALARM");
            StartDataWipeCountdown(SpawnPoint.Around(2f), 70);
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 18000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Los hackers activaron granadas térmicas en las bahías de discos!");
                TriggerEnvironmentalHazard(SpawnPoint.Around(2f), "core", "ent_sht_flame", 6f, 20000);
            }
        
        }
    }
}
