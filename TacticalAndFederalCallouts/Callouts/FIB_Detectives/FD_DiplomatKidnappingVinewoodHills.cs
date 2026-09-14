using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_DiplomatKidnappingVinewoodHills", CalloutProbability.High)]
    public class FD_DiplomatKidnappingVinewoodHills : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_DiplomatKidnappingVinewoodHills"; } }
        public override string DisplayName { get { return "Operacion FIB: Rescate de Consul Extranjero en Vinewood Hills"; } }
        public override string DispatchSubtitle { get { return "Comando de secuestradores asalta villa de lujo y exige transferencia millonaria de criptoactivos."; } }
        public override int ThreatLevel { get { return 5; } }

        private Ped _diplomat;
        private Vehicle _escapeCar;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-450.4f, 750.2f, 185.5f), new Vector3(-780.2f, 950.4f, 215.2f), new Vector3(-120.5f, 650.4f, 175.2f), new Vector3(250.4f, 850.2f, 205.5f), new Vector3(-620.2f, 820.5f, 195.4f), new Vector3(150.2f, 720.4f, 182.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _escapeCar = SpawnVehicle("EXEMPLAR", SpawnPoint.Around(12f), 90f);
            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("s_m_y_blackops_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 85), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 90);
            }

            _diplomat = SpawnPed("a_m_m_business_01", SpawnPoint.Around(2f), 0f);
            if (_diplomat != null)
            {
                _diplomat.Tasks.Cower(-1);
                AttachBlipToHostage(_diplomat, System.Drawing.Color.Green, 0.6f);
                Hostages.Add(_diplomat);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB TACTICAL] ¡Asalto a la villa en marcha! Aseguren la piscina exterior y protejan al cónsul.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 18000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Los secuestradores intentan meter al cónsul en el maletero del deportivo!");
                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && _escapeCar != null && _escapeCar.Exists())
                {
                    TriggerVehicleEscape(Suspects[0], _escapeCar, SpawnPoint.Around(200f));
                }
            }
        
        }
    }
}
