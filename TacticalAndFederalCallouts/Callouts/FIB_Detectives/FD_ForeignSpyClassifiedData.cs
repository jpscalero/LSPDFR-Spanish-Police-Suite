using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_ForeignSpyClassifiedData", CalloutProbability.High)]
    public class FD_ForeignSpyClassifiedData : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_ForeignSpyClassifiedData"; } }
        public override string DisplayName { get { return "Operacion FIB: Intercepcion de Espia Extranjero con Datos Clasificados"; } }
        public override string DispatchSubtitle { get { return "Entrega de portatil con secretos de defensa militar en pabellon de observatorio."; } }
        public override int ThreatLevel { get { return 5; } }

        private Ped _spy;
        private Vehicle _diplomaticCar;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-420.5f, 1120.4f, 325.8f), new Vector3(-750.2f, 1280.5f, 328.5f), new Vector3(720.4f, 1250.2f, 360.5f), new Vector3(-2250.4f, 250.2f, 169.5f), new Vector3(1850.2f, 3700.4f, 33.5f), new Vector3(-1350.5f, 4550.2f, 35.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _diplomaticCar = SpawnVehicle("FELON", SpawnPoint.Around(12f), 0f);
            _spy = CreateFederalTarget("s_m_m_highsec_01", SpawnPoint.Around(2f), 0f, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL50"), 90);

            for (int i = 0; i < 3; i++)
            {
                CreateFederalTarget("s_m_y_blackops_01", SpawnPoint.Around(5f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 90);
            }

            AddEvidenceItem("prop_laptop_01a", SpawnPoint.Around(1.5f), "Portatil con planos clasificados");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB CONTRAINTELIGENCIA] ¡Entrega clandestina descubierta! Intercepten al espía extranjero.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 16000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡El agente extranjero intenta alcanzar su coche consular para escapar!");
                if (_spy != null && _spy.Exists() && _spy.IsAlive && _diplomaticCar != null && _diplomaticCar.Exists())
                {
                    TriggerVehicleEscape(_spy, _diplomaticCar, SpawnPoint.Around(250f));
                }
            }
        
        }
    }
}
