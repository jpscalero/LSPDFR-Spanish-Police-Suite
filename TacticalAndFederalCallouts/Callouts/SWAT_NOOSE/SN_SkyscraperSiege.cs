using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_SkyscraperSiege", CalloutProbability.High)]
    public class SN_SkyscraperSiege : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_SkyscraperSiege"; } }
        public override string DisplayName { get { return "Operacion SWAT: Asedio Vertical en Rascacielos Mile High Club"; } }
        public override string DispatchSubtitle { get { return "Comando paramilitar toma el piso superior de una torre en obras amenazando con volar la grua."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-145.5f, -955.2f, 268.5f), new Vector3(-68.2f, -805.4f, 245.2f), new Vector3(-135.2f, -635.4f, 168.2f), new Vector3(-30.5f, -725.2f, 180.5f), new Vector3(115.2f, -885.4f, 155.4f), new Vector3(-80.5f, -1025.2f, 210.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("s_m_y_blackops_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 85), (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 100);
            }

            for (int h = 0; h < 2; h++)
            {
                Ped worker = CreateTacticalHostage("s_m_y_construct_01", SpawnPoint.Around(2f + (h * 2f)), 0f);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Combate en la cota superior del rascacielos! Neutralicen a los hostiles entre los andamios.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 20000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~y~[SWAT] ¡Cargas explosivas en los anclajes de la grúa! Desactiven el detonador antes de que colapse.");
                StartBombCountdown(SpawnPoint.Around(4f), 60, "prop_c4_final_sb");
            }
        
        }
    }
}
