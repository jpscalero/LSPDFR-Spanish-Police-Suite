using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_NuclearPlantSiege", CalloutProbability.High)]
    public class SN_NuclearPlantSiege : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_NuclearPlantSiege"; } }
        public override string DisplayName { get { return "Operacion SWAT: Sabotaje a Planta Energetica Palmer-Taylor"; } }
        public override string DispatchSubtitle { get { return "Comando paramilitar intenta detonar lineas de refrigeracion nuclear criticas."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(2730.5f, 1530.2f, 24.5f), new Vector3(2820.4f, 1680.5f, 24.5f), new Vector3(2680.2f, 1620.4f, 24.5f), new Vector3(2750.4f, 1420.2f, 24.5f), new Vector3(2890.2f, 1520.4f, 24.5f), new Vector3(2650.5f, 1720.2f, 24.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 5; i++)
            {
                CreateTacticalSuspect("s_m_m_chemsec_01", SpawnPoint.Around(3.5f + (i * 2.8f)), (float)(i * 75), (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Alarma en la planta energética! Desactiven la bomba de sobrepresión en la turbina.");
            TriggerScriptAlarm("PORT_OF_LS_CONTAINER_ALARM");
            StartBombCountdown(SpawnPoint.Around(2f), 75, "prop_ld_can_01");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 18000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~y~[SWAT] ¡Fuga de gas a alta presión activada por los saboteadores! Cuidado con los vapores.");
                TriggerEnvironmentalHazard(SpawnPoint.Around(5f), "core", "ent_sht_steam", 12f, 25000);
            }
        
        }
    }
}
