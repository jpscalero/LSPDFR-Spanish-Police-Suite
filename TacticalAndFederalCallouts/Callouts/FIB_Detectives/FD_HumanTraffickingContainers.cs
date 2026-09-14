using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_HumanTraffickingContainers", CalloutProbability.High)]
    public class FD_HumanTraffickingContainers : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_HumanTraffickingContainers"; } }
        public override string DisplayName { get { return "Operacion FIB: Rescate de Victimas de Trata en Contenedores"; } }
        public override string DispatchSubtitle { get { return "Contenedor frigorifico sellado con personas retenidas ilegalmente en el muelle de carga."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(150.4f, -2850.2f, 6.0f), new Vector3(450.2f, -3100.5f, 6.0f), new Vector3(-200.5f, -2750.2f, 6.0f), new Vector3(320.4f, -2950.2f, 6.0f), new Vector3(-120.2f, -2650.5f, 6.0f), new Vector3(580.4f, -3250.2f, 6.0f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("g_m_y_korean_01", SpawnPoint.Around(4f + (i * 2.8f)), (float)(i * 85), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 80);
            }

            for (int h = 0; h < 2; h++)
            {
                Ped victim = SpawnPed("a_f_y_eastsa_01", SpawnPoint.Around(1.5f + (h * 1.8f)), 0f);
                if (victim != null)
                {
                    victim.Tasks.Cower(-1);
                    AttachBlipToHostage(victim, System.Drawing.Color.Green, 0.55f);
                    Hostages.Add(victim);
                }
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB DERECHOS HUMANOS] ¡Contenedores bajo vigilancia! Neutralicen a los traficantes y liberen a las víctimas.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 16000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Llega una furgoneta de apoyo de los traficantes por el muelle!");
                SpawnHostileReinforcementVehicle("BURRITO", "g_m_y_korean_01", (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 3, SpawnPoint.Around(40f), SpawnPoint);
            }
        
        }
    }
}
