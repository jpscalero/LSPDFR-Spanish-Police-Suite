using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_OilRigSiege", CalloutProbability.High)]
    public class SN_OilRigSiege : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_OilRigSiege"; } }
        public override string DisplayName { get { return "Operacion SWAT: Infiltracion y Combate en Refineria Costera"; } }
        public override string DispatchSubtitle { get { return "Toma armada de instalaciones petroliferas. Amenaza de vertido e ignicion catastrofica."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1710.2f, -1620.4f, 112.5f), new Vector3(1750.4f, -1550.2f, 112.5f), new Vector3(1650.2f, -1680.5f, 112.5f), new Vector3(1690.5f, -1490.4f, 112.5f), new Vector3(1780.2f, -1610.2f, 112.5f), new Vector3(1620.4f, -1580.5f, 112.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 5; i++)
            {
                CreateTacticalSuspect("s_m_y_blackops_01", SpawnPoint.Around(4f + (i * 3f)), (float)(i * 70), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Refinería bajo asedio! Combate vertical en las pasarelas metálicas superiores.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 20000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Explosión de tubería de gas en la plataforma! Nube tóxica y fuego activo.");
                TriggerExplosion(SpawnPoint.Around(8f), 2, 2.0f);
                TriggerEnvironmentalHazard(SpawnPoint.Around(8f), "core", "ent_sht_flame", 8f, 20000);
            }
        
        }
    }
}
