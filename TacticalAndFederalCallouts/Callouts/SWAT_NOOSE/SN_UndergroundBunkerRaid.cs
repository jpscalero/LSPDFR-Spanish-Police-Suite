using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_UndergroundBunkerRaid", CalloutProbability.High)]
    public class SN_UndergroundBunkerRaid : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_UndergroundBunkerRaid"; } }
        public override string DisplayName { get { return "Operacion SWAT: Incursion en Bunker Subterraneo"; } }
        public override string DispatchSubtitle { get { return "Almacen militar ilegal en los tuneles de servicio subterráneos de la ciudad."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-595.4f, -280.2f, 50.5f), new Vector3(350.2f, -1620.4f, -40.5f), new Vector3(-45.2f, -650.4f, 15.5f), new Vector3(450.4f, -800.2f, -12.5f), new Vector3(-1050.2f, -450.4f, -30.5f), new Vector3(150.4f, -3150.2f, -10.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("s_m_m_chemsec_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTSHOTGUN"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Incursión subterránea! Visibilidad reducida. Enciendan linternas tácticas y avancen.");
            DetonateFlashbang(SpawnPoint.Around(3f));
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 18000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Un blindado enemigo fuertemente armado contraataca por el túnel lateral!");
                Ped juggernaut = CreateTacticalSuspect("s_m_y_blackops_01", SpawnPoint.Around(15f), 0f, (WeaponHash)Game.GetHashKey("WEAPON_COMBATMG"), 150);
            }
        
        }
    }
}
