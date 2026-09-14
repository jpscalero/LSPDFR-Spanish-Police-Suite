using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_YachtAssault", CalloutProbability.High)]
    public class SN_YachtAssault : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_YachtAssault"; } }
        public override string DisplayName { get { return "Operacion SWAT: Asalto Aeronaval a Megayate"; } }
        public override string DispatchSubtitle { get { return "Piratas mercenarios armados toman megayate privado frente a las costas de Pacific Bluffs."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-2080.5f, -1020.2f, 8.5f), new Vector3(-2950.4f, 450.2f, 8.5f), new Vector3(-3400.2f, 950.4f, 8.5f), new Vector3(-1850.4f, -1550.2f, 8.5f), new Vector3(-2350.2f, -750.4f, 8.5f), new Vector3(-3100.5f, -120.2f, 8.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            Vehicle jetSki = SpawnVehicle("SEASHARK", SpawnPoint.Around(8f), 0f);
            for (int i = 0; i < 5; i++)
            {
                CreateTacticalSuspect("s_m_y_blackops_01", SpawnPoint.Around(3.5f + (i * 2.8f)), (float)(i * 70), (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 100);
            }

            Ped captain = CreateTacticalHostage("s_m_m_armgoon_01", SpawnPoint.Around(2f), 0f);
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT NAVAL] ¡Asalto al megayate en marcha! Despejen las cubiertas y aseguren el puente de mando.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 20000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡El cabecilla mercenario intenta escapar en la moto acuática de popa!");
                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && Vehicles.Count > 0 && Vehicles[0].Exists())
                {
                    TriggerVehicleEscape(Suspects[0], Vehicles[0], SpawnPoint.Around(250f));
                }
            }
        
        }
    }
}
