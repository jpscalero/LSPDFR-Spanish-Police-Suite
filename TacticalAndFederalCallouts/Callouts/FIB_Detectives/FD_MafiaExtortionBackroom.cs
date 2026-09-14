using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_MafiaExtortionBackroom", CalloutProbability.High)]
    public class FD_MafiaExtortionBackroom : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_MafiaExtortionBackroom"; } }
        public override string DisplayName { get { return "Operacion FIB: Extorsion y Coacciones de la Mafia en Trastienda"; } }
        public override string DispatchSubtitle { get { return "Capos mafiosos agrediendo a comerciante local en la trastienda para exigir el pago de mordidas."; } }
        public override int ThreatLevel { get { return 4; } }

        private int _scenarioStage = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-550.4f, 280.2f, 82.5f), new Vector3(-1350.2f, -600.4f, 29.5f), new Vector3(450.4f, -980.2f, 29.5f), new Vector3(-820.2f, -1050.4f, 12.5f), new Vector3(120.4f, -1420.2f, 29.5f), new Vector3(-350.2f, -1450.4f, 29.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 3; i++)
            {
                CreateFederalTarget("g_m_m_armgoon_01", SpawnPoint.Around(2.5f + (i * 2.2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_REVOLVER"), 80);
            }

            Ped merchant = SpawnPed("a_m_m_farmer_01", SpawnPoint.Around(1f), 0f);
            if (merchant != null)
            {
                merchant.Tasks.Cower(-1);
                AttachBlipToHostage(merchant, System.Drawing.Color.Green, 0.55f);
                Hostages.Add(merchant);
            }

            AddEvidenceItem("prop_cd_box_01", SpawnPoint.Around(2.2f), "Libro de cobros de extorsiones");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB CRIMEN ORGANIZADO] ¡Entrada a la trastienda! Alto al fuego y protejan al comerciante.");
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();

            if (_scenarioStage == 0 && Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive)
            {
                _scenarioStage = 1;
                // El sospechoso intenta quemar el libro
                TriggerEnvironmentalHazard(SpawnPoint.Around(2.2f), "core", "ent_sht_flame", 4f, 15000);
            }
        
        }
    }
}
