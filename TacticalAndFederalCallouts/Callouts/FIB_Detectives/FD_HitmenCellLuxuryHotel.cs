using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_HitmenCellLuxuryHotel", CalloutProbability.High)]
    public class FD_HitmenCellLuxuryHotel : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_HitmenCellLuxuryHotel"; } }
        public override string DisplayName { get { return "Operacion FIB: Celula de Sicarios en Suite de Lujo"; } }
        public override string DispatchSubtitle { get { return "Sicarios de cartel alojados en hotel de 5 estrellas con arsenal para atentar contra juez federal."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-550.2f, -180.5f, 38.2f), new Vector3(-680.4f, -250.2f, 38.5f), new Vector3(-1380.2f, -480.4f, 33.2f), new Vector3(-850.4f, -420.2f, 37.5f), new Vector3(-320.5f, -120.4f, 39.5f), new Vector3(-1050.2f, -220.5f, 44.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 3; i++)
            {
                CreateFederalTarget("g_m_m_armgoon_01", SpawnPoint.Around(2.5f + (i * 2.2f)), (float)(i * 95), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 90);
            }

            AddEvidenceItem("prop_box_ammo04a", SpawnPoint.Around(3f), "Rifle de francotirador desmontado");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB OPERACIONES] ¡Asalto a la suite! Neutralicen a los sicarios antes de que abran fuego.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 12000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Los sicarios detonan una aturdidora e intentan escapar por la escalera de servicio!");
                PlayPtfx("core", "ent_sht_smoke", SpawnPoint.Around(2f), 2.0f);
            }
        
        }
    }
}
