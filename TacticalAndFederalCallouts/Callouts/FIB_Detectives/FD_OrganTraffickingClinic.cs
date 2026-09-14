using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_OrganTraffickingClinic", CalloutProbability.High)]
    public class FD_OrganTraffickingClinic : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_OrganTraffickingClinic"; } }
        public override string DisplayName { get { return "Operacion FIB: Clinica Clandestina de Trafico de Organos"; } }
        public override string DispatchSubtitle { get { return "Sotano con quirofano ilegal y donantes secuestrados para trasplantes en el mercado negro."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(450.4f, -1650.2f, 29.5f), new Vector3(-650.2f, -1450.4f, 10.5f), new Vector3(880.5f, -2150.2f, 30.5f), new Vector3(-350.2f, -1200.4f, 38.5f), new Vector3(120.4f, -1850.2f, 26.5f), new Vector3(-1050.5f, -1400.2f, 5.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 3; i++)
            {
                CreateFederalTarget("s_m_m_doctor_01", SpawnPoint.Around(2.5f + (i * 2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_PISTOL50"), 70);
            }

            Ped patient = SpawnPed("a_m_m_tramp_01", SpawnPoint.Around(1.5f), 0f);
            if (patient != null)
            {
                patient.Tasks.Cower(-1);
                AttachBlipToHostage(patient, System.Drawing.Color.Green, 0.6f);
                Hostages.Add(patient);
            }

            AddEvidenceItem("prop_box_ammo01a", SpawnPoint.Around(2.8f), "Nevera termica de preservacion de organos");
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB] ¡Brecha en la clínica clandestina! Reduzcan a los cirujanos armados y rescaten al paciente.");
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();

            if (_scenarioStage == 0 && Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Uno de los traficantes intenta huir con la nevera de órganos hacia el callejón!");
            }
        
        }
    }
}
