using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_UndergroundCasino", CalloutProbability.High)]
    public class FD_UndergroundCasino : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_UndergroundCasino"; } }
        public override string DisplayName { get { return "Operacion FIB: Casino Clandestino de la Triada"; } }
        public override string DispatchSubtitle { get { return "Local subterraneo con ruletas, mesas de baccarat y blanqueo masivo de dinero ilicito."; } }
        public override int ThreatLevel { get { return 4; } }

        private int _scenarioStage = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-550.2f, -620.4f, 34.5f), new Vector3(450.4f, -950.2f, 28.5f), new Vector3(-1150.2f, -1450.4f, 4.5f), new Vector3(880.4f, -1250.2f, 26.5f), new Vector3(-850.5f, -1050.4f, 12.5f), new Vector3(250.4f, -1820.2f, 26.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("g_m_y_korean_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 70);
            }

            AddEvidenceItem("prop_cash_pile_01", SpawnPoint.Around(1.8f), "Fajos de dinero de las mesas de juego");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB JUEGO ILEGAL] ¡Redada en el casino clandestino! Reduzcan a los matones armados.");
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();

            if (_scenarioStage == 0 && Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡El jefe de sala intenta bloquear la puerta de la caja fuerte!");
            }
        
        }
    }
}
