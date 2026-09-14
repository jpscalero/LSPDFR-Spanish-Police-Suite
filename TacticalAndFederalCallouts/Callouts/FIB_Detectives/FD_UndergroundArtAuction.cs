using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_UndergroundArtAuction", CalloutProbability.High)]
    public class FD_UndergroundArtAuction : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_UndergroundArtAuction"; } }
        public override string DisplayName { get { return "Operacion FIB: Subasta Clandestina de Arte Robado"; } }
        public override string DispatchSubtitle { get { return "Obras maestras de museos europeos subastadas en una galeria privada blindada."; } }
        public override int ThreatLevel { get { return 4; } }

        private Vehicle _artVan;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-1250.4f, -480.2f, 33.2f), new Vector3(-680.2f, -220.5f, 38.5f), new Vector3(220.4f, -800.5f, 30.5f), new Vector3(-550.2f, -180.4f, 38.2f), new Vector3(850.5f, -1280.4f, 26.2f), new Vector3(-1350.2f, -850.4f, 15.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _artVan = SpawnVehicle("SPEEDO", SpawnPoint.Around(12f), 0f);
            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("s_m_y_blackops_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 85), (WeaponHash)Game.GetHashKey("WEAPON_COMBATPDW"), 80);
            }

            AddEvidenceItem("prop_box_wood02a", SpawnPoint.Around(2f), "Pintura de valor incalculable");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB PATRIMONIO] ¡Subasta intervenida! Aseguren las obras de arte y neutralicen a la seguridad privada.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 15000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Los guardias intentan cargar el cuadro robado en la furgoneta para huir!");
                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && _artVan != null && _artVan.Exists())
                {
                    TriggerVehicleEscape(Suspects[0], _artVan, SpawnPoint.Around(200f));
                }
            }
        
        }
    }
}
