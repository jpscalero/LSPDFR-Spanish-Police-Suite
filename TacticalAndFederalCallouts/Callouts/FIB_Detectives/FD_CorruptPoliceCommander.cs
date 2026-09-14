using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_CorruptPoliceCommander", CalloutProbability.High)]
    public class FD_CorruptPoliceCommander : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_CorruptPoliceCommander"; } }
        public override string DisplayName { get { return "Operacion FIB: Detencion de Comandante Policial Corrupto"; } }
        public override string DispatchSubtitle { get { return "Comandante bajo investigacion interna reunido con enlaces del cartel en aparcamiento subterraneo."; } }
        public override int ThreatLevel { get { return 5; } }

        private Ped _commander;
        private Vehicle _cmdCar;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-450.4f, -320.2f, 34.5f), new Vector3(220.2f, -800.5f, 30.5f), new Vector3(-1050.5f, -450.4f, 36.5f), new Vector3(450.4f, -1020.2f, 28.5f), new Vector3(-750.2f, -650.4f, 30.2f), new Vector3(150.4f, -600.2f, 40.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _cmdCar = SpawnVehicle("POLICE4", SpawnPoint, 90f);
            _commander = CreateFederalTarget("s_m_y_cop_01", SpawnPoint.Around(2f), 0f, (WeaponHash)Game.GetHashKey("WEAPON_COMBATPISTOL"), 100);

            for (int i = 0; i < 3; i++)
            {
                CreateFederalTarget("s_m_y_swat_01", SpawnPoint.Around(4f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_PUMPSHOTGUN"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB ASUNTOS INTERNOS] ¡Comandante, arroje el arma! Tenemos orden judicial de arresto.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 12000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡El comandante corrupto ordenó a su escolta abrir fuego e intenta huir en el coche patrulla!");
                if (_commander != null && _commander.Exists() && _commander.IsAlive && _cmdCar != null && _cmdCar.Exists())
                {
                    TriggerVehicleEscape(_commander, _cmdCar, SpawnPoint.Around(200f));
                }
            }
        
        }
    }
}
