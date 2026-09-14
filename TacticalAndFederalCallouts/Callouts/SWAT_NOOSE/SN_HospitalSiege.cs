using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_HospitalSiege", CalloutProbability.High)]
    public class SN_HospitalSiege : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_HospitalSiege"; } }
        public override string DisplayName { get { return "Operacion SWAT: Asedio a Planta Quirurgica Hospitalaria"; } }
        public override string DispatchSubtitle { get { return "Sicarios asaltan hospital para asesinar a testigo protegido ingresado en quirófano."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(300.5f, -1400.2f, 32.5f), new Vector3(360.4f, -585.2f, 28.8f), new Vector3(-450.2f, -340.5f, 34.5f), new Vector3(-875.4f, -305.2f, 39.5f), new Vector3(1840.5f, 3670.2f, 33.8f), new Vector3(-245.2f, 6315.4f, 32.4f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("g_m_m_armgoon_01", SpawnPoint.Around(3f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTSHOTGUN"), 90);
            }

            for (int h = 0; h < 2; h++)
            {
                Ped medic = CreateTacticalHostage("s_m_m_doctor_01", SpawnPoint.Around(1.5f + (h * 2f)), 0f);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Fuego en el ala quirúrgica! Despejen sala por sala protegiendo al personal médico.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 15000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~y~[SWAT] ¡Los sicarios incendiaron suministros médicos para cubrir su retirada!");
                TriggerEnvironmentalHazard(SpawnPoint.Around(3f), "core", "ent_sht_smoke", 10f, 20000);
            }
        
        }
    }
}
