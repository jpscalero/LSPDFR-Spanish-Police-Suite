using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_AirportHangarSiege", CalloutProbability.High)]
    public class SN_AirportHangarSiege : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_AirportHangarSiege"; } }
        public override string DisplayName { get { return "Operacion SWAT: Asedio a Hangar en Aeropuerto"; } }
        public override string DispatchSubtitle { get { return "Mercenarios armados fuertemente pertrechados intentan fuga en aeronave privada."; } }
        public override int ThreatLevel { get { return 5; } }

        private Vehicle _escapePlane;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-978.5f, -2998.2f, 13.9f), new Vector3(-1260.4f, -3012.8f, 13.9f), new Vector3(1735.2f, 3280.4f, 41.1f), new Vector3(2135.8f, 4780.2f, 41.0f), new Vector3(-1610.2f, -3140.5f, 13.9f), new Vector3(1700.1f, 3250.0f, 41.1f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _escapePlane = SpawnVehicle("SHAMAL", SpawnPoint, 90f);
            SpawnVehicle("DUBSTA", SpawnPoint.Around(14f), 0f);

            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("s_m_m_chemsec_01", SpawnPoint.Around(4f + (i * 2.5f)), (float)(i * 70), (WeaponHash)Game.GetHashKey("WEAPON_SPECIALCARBINE"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Los mercenarios están cargando la aeronave! Impidan el despegue.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 18000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Llegan refuerzos mercenarios y el avión inicia rodaje en pista!");
                SpawnHostileReinforcementVehicle("GRANGER", "s_m_m_chemsec_01", (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), 3, SpawnPoint.Around(45f), SpawnPoint);

                if (_escapePlane != null && _escapePlane.Exists())
                {
                    _escapePlane.IsEngineOn = true;
                    if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive)
                    {
                        TriggerVehicleEscape(Suspects[0], _escapePlane, SpawnPoint + new Vector3(120f, 0f, 0f));
                    }
                }
            }
        
        }
    }
}
