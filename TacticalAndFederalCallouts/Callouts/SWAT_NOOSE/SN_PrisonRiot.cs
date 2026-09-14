using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_PrisonRiot", CalloutProbability.High)]
    public class SN_PrisonRiot : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_PrisonRiot"; } }
        public override string DisplayName { get { return "Operacion SWAT: Motin Armado en Prision de Bolingbroke"; } }
        public override string DispatchSubtitle { get { return "Centenares de reclusos armados desbordan el patio y tienen a funcionarios de prisiones acorralados."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1690.4f, 2565.2f, 45.5f), new Vector3(1750.2f, 2520.4f, 45.5f), new Vector3(1620.5f, 2510.2f, 45.5f), new Vector3(1700.2f, 2460.5f, 45.5f), new Vector3(1780.4f, 2580.2f, 45.5f), new Vector3(1640.2f, 2570.5f, 45.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            // Guardias tomados como rehenes
            for (int g = 0; g < 2; g++)
            {
                Ped guard = CreateTacticalHostage("s_m_m_armoured_01", SpawnPoint.Around(2f + (g * 1.5f)), 0f);
            }

            // Presos amotinados
            for (int i = 0; i < 5; i++)
            {
                CreateTacticalSuspect("s_m_y_prismuscl_01", SpawnPoint.Around(4f + (i * 2.5f)), (float)(i * 72), (WeaponHash)Game.GetHashKey("WEAPON_BAT"), 80);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Motín en Bolingbroke! Sirenas de emergencia activadas. Desplieguen gas lacrimógeno sobre el patio.");
            TriggerScriptAlarm("PRISON_ALARMS");
            TacticalBreachManager.DeployTearGas(SpawnPoint.Around(3f), Suspects);
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 18000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Presos del pabellón B entran al patio armados con cócteles molotov!");
                TriggerEnvironmentalHazard(SpawnPoint.Around(5f), "core", "ent_sht_flame", 8f, 20000);

                for (int i = 0; i < 2; i++)
                {
                    CreateTacticalSuspect("s_m_y_prisoner_01", SpawnPoint.Around(12f + (i * 3f)), 0f, (WeaponHash)Game.GetHashKey("WEAPON_MOLOTOV"), 60);
                }
            }
        
        }
    }
}
