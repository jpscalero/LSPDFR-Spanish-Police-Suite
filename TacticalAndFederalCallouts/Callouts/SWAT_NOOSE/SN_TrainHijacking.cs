using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_TrainHijacking", CalloutProbability.High)]
    public class SN_TrainHijacking : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_TrainHijacking"; } }
        public override string DisplayName { get { return "Operacion SWAT: Tren de Mercancias Secuestrado en Viaducto"; } }
        public override string DispatchSubtitle { get { return "Comando intercepta tren de carga pesada sobre el viaducto de Raton Canyon con explosivos."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(660.2f, 820.4f, 195.2f), new Vector3(-550.4f, 4450.2f, 25.5f), new Vector3(2600.2f, 3850.4f, 35.2f), new Vector3(1850.4f, 4650.2f, 36.8f), new Vector3(-950.2f, 5450.4f, 38.2f), new Vector3(1250.4f, 2950.2f, 42.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            Vehicle moto1 = SpawnVehicle("SANCHEZ", SpawnPoint.Around(4f), 0f);
            Vehicle moto2 = SpawnVehicle("SANCHEZ", SpawnPoint.Around(8f), 0f);

            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("s_m_y_blackops_01", SpawnPoint.Around(3f + (i * 3f)), (float)(i * 85), (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), 100);
            }

            Ped engineer = CreateTacticalHostage("s_m_y_construct_02", SpawnPoint.Around(2f), 0f);
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Asalto al tren sobre el viaducto! Cuidado con las alturas y rescaten al maquinista.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 18000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Los secuestradores despliegan humo y dos de ellos intentan huir en motos por las vías!");
                TacticalBreachManager.DeployTearGas(SpawnPoint.Around(3f), Suspects);

                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && Vehicles.Count > 0 && Vehicles[0].Exists())
                {
                    TriggerVehicleEscape(Suspects[0], Vehicles[0], SpawnPoint.Around(200f));
                }
            }
        
        }
    }
}
