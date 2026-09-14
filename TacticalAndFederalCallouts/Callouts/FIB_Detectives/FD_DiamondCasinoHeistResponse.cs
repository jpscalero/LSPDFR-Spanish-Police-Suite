using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_DiamondCasinoHeistResponse", CalloutProbability.High)]
    public class FD_DiamondCasinoHeistResponse : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_DiamondCasinoHeistResponse"; } }
        public override string DisplayName { get { return "Operacion FIB: Asalto a Camara Acorazada Diamond Casino"; } }
        public override string DispatchSubtitle { get { return "Banda internacional de atracadores asalta tunel de servicio del casino con explosivos termicos."; } }
        public override int ThreatLevel { get { return 5; } }

        private Vehicle _getawayVan;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(920.5f, 45.2f, 81.2f), new Vector3(980.2f, 15.4f, 80.5f), new Vector3(880.4f, -15.2f, 82.5f), new Vector3(1050.2f, 85.4f, 80.5f), new Vector3(950.4f, -45.2f, 74.5f), new Vector3(890.2f, 75.4f, 83.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _getawayVan = SpawnVehicle("BURRITO", SpawnPoint.Around(10f), 45f);
            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("g_m_m_armgoon_01", SpawnPoint.Around(3f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 90);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB] ¡Atraco al Diamond Casino! Los ladrones salen por la rampa de servicio con los fondos.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 15000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Los atracadores despliegan granadas de humo y tratan de abordar la furgoneta de fuga!");
                TriggerEnvironmentalHazard(SpawnPoint.Around(4f), "core", "ent_sht_smoke", 12f, 20000);

                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && _getawayVan != null && _getawayVan.Exists())
                {
                    TriggerVehicleEscape(Suspects[0], _getawayVan, SpawnPoint.Around(250f));
                }
            }
        
        }
    }
}
