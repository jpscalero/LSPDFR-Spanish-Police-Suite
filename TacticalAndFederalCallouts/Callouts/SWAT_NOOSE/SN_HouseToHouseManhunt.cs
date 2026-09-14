using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_HouseToHouseManhunt", CalloutProbability.High)]
    public class SN_HouseToHouseManhunt : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_HouseToHouseManhunt"; } }
        public override string DisplayName { get { return "Operacion SWAT: Batida Urbana Casa por Casa"; } }
        public override string DispatchSubtitle { get { return "Tiradores activos huyen por patios residenciales abriendo fuego sobre las patrullas."; } }
        public override int ThreatLevel { get { return 4; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-1150.2f, -1520.4f, 4.5f), new Vector3(320.5f, -1980.2f, 24.2f), new Vector3(1350.4f, -1650.2f, 52.4f), new Vector3(-20.5f, -1850.4f, 25.5f), new Vector3(850.2f, -2300.5f, 30.2f), new Vector3(-950.4f, -1200.2f, 5.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 3; i++)
            {
                CreateTacticalSuspect("g_m_y_famca_01", SpawnPoint.Around(4f + (i * 3.5f)), (float)(i * 110), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 70);
            }
            SpawnVehicle("ASTEROPE", SpawnPoint.Around(12f), 90f);
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Tiradores activos en el vecindario! Establezcan cerrojo perimetral y avancen.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 16000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Un tirador salta la valla e intenta robar un turismo aparcado para escapar!");
                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && Vehicles.Count > 0 && Vehicles[0].Exists())
                {
                    TriggerVehicleEscape(Suspects[0], Vehicles[0], SpawnPoint.Around(200f));
                }
            }
        
        }
    }
}
