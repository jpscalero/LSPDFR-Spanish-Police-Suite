using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_StadiumSiege", CalloutProbability.High)]
    public class SN_StadiumSiege : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_StadiumSiege"; } }
        public override string DisplayName { get { return "Operacion SWAT: Asedio y Furgon con Bomba en Maze Bank Arena"; } }
        public override string DispatchSubtitle { get { return "Celula terrorista atrinchera un furgon cargado con ANFO en los muelles de carga del estadio."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-250.4f, -1950.2f, 27.5f), new Vector3(-320.2f, -1880.5f, 27.5f), new Vector3(-210.5f, -1820.4f, 27.5f), new Vector3(-280.2f, -1750.5f, 27.5f), new Vector3(-180.4f, -1910.2f, 27.5f), new Vector3(-350.5f, -1820.2f, 27.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            SpawnVehicle("BOXVILLE", SpawnPoint, 0f);
            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("g_m_m_armgoon_01", SpawnPoint.Around(3f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 90);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Muelles del estadio bloqueados! Detengan al comando terrorista y desactiven el explosivo del furgón.");
            StartBombCountdown(SpawnPoint.Around(2f), 85, "prop_ld_can_01");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 22000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Llega un todoterreno de apoyo con ametralladores pesados por la rampa sur!");
                SpawnHostileReinforcementVehicle("DUBSTA", "g_m_m_armgoon_01", (WeaponHash)Game.GetHashKey("WEAPON_COMBATMG"), 2, SpawnPoint.Around(45f), SpawnPoint);
            }
        
        }
    }
}
