using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_MountChiliadCompound", CalloutProbability.High)]
    public class SN_MountChiliadCompound : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_MountChiliadCompound"; } }
        public override string DisplayName { get { return "Operacion SWAT: Asalto a Fortaleza Montañosa en Monte Chiliad"; } }
        public override string DispatchSubtitle { get { return "Milicia armada atrincherada en complejo fortificado con armas automaticas."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-680.5f, 5800.2f, 17.5f), new Vector3(450.2f, 5550.4f, 780.2f), new Vector3(-1150.4f, 4950.2f, 220.5f), new Vector3(1500.2f, 6450.4f, 35.2f), new Vector3(-380.2f, 4400.5f, 55.4f), new Vector3(2100.4f, 5800.2f, 45.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            SpawnVehicle("BIFTA", SpawnPoint.Around(10f), 0f);
            for (int i = 0; i < 5; i++)
            {
                CreateTacticalSuspect("s_m_m_marine_01", SpawnPoint.Around(4f + (i * 3f)), (float)(i * 70), (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Fortaleza de la milicia localizada! Alarma perimetral activada. Neutralicen a los combatientes.");
            TriggerScriptAlarm("PRISON_ALARMS");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 24000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡El líder de la milicia intenta escapar por las sendas montañosas en un buggy!");
                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && Vehicles.Count > 0 && Vehicles[0].Exists())
                {
                    TriggerVehicleEscape(Suspects[0], Vehicles[0], SpawnPoint.Around(250f));
                }
            }
        
        }
    }
}
