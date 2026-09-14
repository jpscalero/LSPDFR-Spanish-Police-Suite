using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_EmbassySiege", CalloutProbability.High)]
    public class SN_EmbassySiege : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_EmbassySiege"; } }
        public override string DisplayName { get { return "Operacion SWAT: Asalto a Complejo Diplomatico"; } }
        public override string DispatchSubtitle { get { return "Comando extremista asalta consulado extranjero y toma a diplomaticos como escudo."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-1600.4f, -420.2f, 40.5f), new Vector3(-680.2f, 320.5f, 83.2f), new Vector3(-1350.5f, -750.4f, 22.4f), new Vector3(-2020.4f, -450.2f, 12.5f), new Vector3(-1050.2f, -1450.4f, 5.2f), new Vector3(-850.4f, -220.5f, 38.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            SpawnVehicle("COG55", SpawnPoint.Around(6f), 45f);
            SpawnVehicle("DUBSTA", SpawnPoint.Around(12f), 180f);

            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("s_m_y_blackops_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 75), (WeaponHash)Game.GetHashKey("WEAPON_COMBATPDW"), 100);
            }

            Ped diplomat = CreateTacticalHostage("a_m_m_business_01", SpawnPoint.Around(2f), 0f);
            if (diplomat != null)
            {
                diplomat.Tasks.Cower(-1);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Embajada bajo ataque! Lancen granadas de humo y aseguren a los diplomáticos.");
            TacticalBreachManager.DeployTearGas(SpawnPoint.Around(4f), Suspects);
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 25000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Un segundo comando extremista llega por el patio trasero en todoterreno!");
                SpawnHostileReinforcementVehicle("DUBSTA", "s_m_y_blackops_01", (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 3, SpawnPoint.Around(40f), SpawnPoint);
            }
        
        }
    }
}
