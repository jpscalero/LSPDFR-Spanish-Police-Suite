using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_CarBombCell", CalloutProbability.High)]
    public class SN_CarBombCell : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_CarBombCell"; } }
        public override string DisplayName { get { return "Operacion SWAT: Celula Terrorista con Coche Bomba"; } }
        public override string DispatchSubtitle { get { return "Yihadistas fuertemente armados custodian furgoneta cargada con explosivos militares."; } }
        public override int ThreatLevel { get { return 5; } }

        private Vehicle _bombVan;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(145.2f, -1045.4f, 29.3f), new Vector3(-550.4f, -200.2f, 38.2f), new Vector3(220.4f, -800.5f, 30.5f), new Vector3(-1350.2f, -480.4f, 33.2f), new Vector3(850.5f, -1280.4f, 26.2f), new Vector3(-45.2f, -1450.4f, 31.4f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _bombVan = SpawnVehicle("RUMPO", SpawnPoint, 0f);
            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("g_m_m_armgoon_01", SpawnPoint.Around(3f + (i * 2.8f)), (float)(i * 85), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Coche bomba detectado! Neutralicen a los terroristas y desactiven el detonador.");
            StartBombCountdown(SpawnPoint.Around(2f), 80, "prop_c4_final_sb");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 20000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Un terrorista intenta huir en moto para activar el detonador a distancia!");
                Vehicle moto = SpawnVehicle("SANCHEZ", SpawnPoint.Around(12f), 0f);
                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && moto != null)
                {
                    TriggerVehicleEscape(Suspects[0], moto, SpawnPoint.Around(150f));
                }
            }
        
        }
    }
}
