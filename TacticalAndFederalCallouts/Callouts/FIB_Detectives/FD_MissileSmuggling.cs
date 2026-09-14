using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_MissileSmuggling", CalloutProbability.High)]
    public class FD_MissileSmuggling : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_MissileSmuggling"; } }
        public override string DisplayName { get { return "Operacion FIB: Trafico de Misiles Tierra-Aire en Pista Aerea"; } }
        public override string DispatchSubtitle { get { return "Cargamento de tecnologia armamentistica pesada siendo cargado en furgon en Sandy Shores."; } }
        public override int ThreatLevel { get { return 5; } }

        private Vehicle _missileTruck;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1730.5f, 3280.2f, 41.1f), new Vector3(2135.4f, 4780.2f, 41.0f), new Vector3(-1100.2f, -2880.4f, 13.9f), new Vector3(1690.4f, 3250.2f, 41.1f), new Vector3(2100.2f, 4750.5f, 41.0f), new Vector3(-1250.5f, -3020.2f, 13.9f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _missileTruck = SpawnVehicle("MULE", SpawnPoint, 90f);
            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("s_m_m_chemsec_01", SpawnPoint.Around(4f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_COMBATMG"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB SEGURIDAD NACIONAL] ¡Cargamento de misiles en pista! Impidan la salida del camión de transporte.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 16000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡El camión cargado con los componentes de misiles acelera por la pista!");
                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && _missileTruck != null && _missileTruck.Exists())
                {
                    TriggerVehicleEscape(Suspects[0], _missileTruck, SpawnPoint + new Vector3(150f, 0f, 0f));
                }
            }
        
        }
    }
}
