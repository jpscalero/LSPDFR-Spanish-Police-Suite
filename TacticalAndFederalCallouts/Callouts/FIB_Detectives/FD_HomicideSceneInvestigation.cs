using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_HomicideSceneInvestigation", CalloutProbability.High)]
    public class FD_HomicideSceneInvestigation : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_HomicideSceneInvestigation"; } }
        public override string DisplayName { get { return "Operacion FIB: Investigacion Forense con Asesino en las Proximidades"; } }
        public override string DispatchSubtitle { get { return "Hallazgo de cadaver con impactos de bala. El ejecutor vigila el perimetro para rematar cabos sueltos."; } }
        public override int ThreatLevel { get { return 4; } }

        private Ped _killer;
        private Vehicle _killerMoto;
        private int _scenarioStage = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(120.4f, -1450.2f, 29.5f), new Vector3(-850.2f, -1200.4f, 6.2f), new Vector3(450.4f, -1850.2f, 28.5f), new Vector3(-1350.2f, -850.4f, 15.5f), new Vector3(780.4f, -2150.2f, 29.5f), new Vector3(-45.2f, -1650.4f, 29.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            // Victima fallecida
            Ped corpse = SpawnPed("a_m_m_business_01", SpawnPoint, 0f);
            if (corpse != null)
            {
                corpse.Kill();
            }

            // Pistas forenses
            AddEvidenceItem("prop_casing_01", SpawnPoint.Around(1.5f), "Casquillo de bala 9mm");
            AddEvidenceItem("prop_w_me_knife_01", SpawnPoint.Around(2.5f), "Arma blanca ensangrentada");

            // Asesino observando desde esquina
            _killerMoto = SpawnVehicle("PCJ", SpawnPoint.Around(22f), 0f);
            _killer = CreateFederalTarget("g_m_y_famca_01", SpawnPoint.Around(20f), 0f, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL50"), 70);
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB FORENSE] Escena del crimen localizada. Recojan las pruebas balísticas con ~INPUT_CONTEXT~.");
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();

            if (_scenarioStage == 0 && EvidenceCollectedCount >= 1)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡El sospechoso observaba la escena y emprende la huida en una motocicleta!");
                if (_killer != null && _killer.Exists() && _killer.IsAlive && _killerMoto != null && _killerMoto.Exists())
                {
                    TriggerVehicleEscape(_killer, _killerMoto, SpawnPoint.Around(250f));
                }
            }
        
        }
    }
}
