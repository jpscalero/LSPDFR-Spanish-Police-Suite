using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_RocketLauncherC4Trafficking", CalloutProbability.High)]
    public class FD_RocketLauncherC4Trafficking : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_RocketLauncherC4Trafficking"; } }
        public override string DisplayName { get { return "Operacion FIB: Trafico de Lanzacohetes y C4 de Uso Militar"; } }
        public override string DispatchSubtitle { get { return "Arsenal robado de bases del ejercito vendido en un desguace abandonado del desierto."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(2350.4f, 3120.2f, 48.5f), new Vector3(1750.2f, 2620.4f, 45.5f), new Vector3(-420.5f, 3850.2f, 40.2f), new Vector3(1200.2f, 1820.5f, 52.1f), new Vector3(-1050.4f, 2820.2f, 22.4f), new Vector3(2100.2f, 2120.5f, 54.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("g_m_m_armgoon_01", SpawnPoint.Around(3.5f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 90);
            }

            AddEvidenceItem("prop_box_ammo04a", SpawnPoint.Around(2.5f), "Caja sellada con explosivo C4 militar");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB ARMAS] ¡Traficantes fuertemente pertrechados! Cuidado con explosivos pesados entre la chatarra.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 14000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Uno de los traficantes detonó una carga de C4 entre los coches viejos!");
                TriggerExplosion(SpawnPoint.Around(8f), 2, 3.5f);
                TriggerEnvironmentalHazard(SpawnPoint.Around(8f), "core", "ent_sht_flame", 8f, 18000);
            }
        
        }
    }
}
