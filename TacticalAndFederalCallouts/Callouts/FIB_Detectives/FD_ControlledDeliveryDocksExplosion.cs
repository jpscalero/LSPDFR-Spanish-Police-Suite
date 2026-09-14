using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.FIB_Detectives
{
    [CalloutInfo("FD_ControlledDeliveryDocksExplosion", CalloutProbability.High)]
    public class FD_ControlledDeliveryDocksExplosion : BaseFederalCallout
    {
        public override string CalloutIdentifier { get { return "FD_ControlledDeliveryDocksExplosion"; } }
        public override string DisplayName { get { return "Operacion FIB: Entrega Controlada con Trampa Explosiva en Muelles"; } }
        public override string DispatchSubtitle { get { return "Entrega vigilada de contenedor de contrabando en la terminal de carga de Terminal Island."; } }
        public override int ThreatLevel { get { return 5; } }

        private Vehicle _boat;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(150.2f, -2900.5f, 5.9f), new Vector3(350.4f, -3050.2f, 5.9f), new Vector3(-150.5f, -2800.4f, 5.9f), new Vector3(550.2f, -3150.5f, 5.9f), new Vector3(-280.4f, -2700.2f, 5.9f), new Vector3(420.5f, -2950.4f, 5.9f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _boat = SpawnVehicle("SPEEDER", SpawnPoint + new Vector3(0f, 15f, -4f), 180f);
            for (int i = 0; i < 4; i++)
            {
                CreateFederalTarget("s_m_m_chemsec_01", SpawnPoint.Around(3f + (i * 2.8f)), (float)(i * 85), (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), 90);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~b~[FIB] ¡Operación de entrega en marcha! Identifíquense y aseguren el contenedor.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 14000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[FIB] ¡Trampa explosiva en el contenedor! El cabecilla corre hacia la lancha rápida en el muelle.");
                TriggerExplosion(SpawnPoint.Around(5f), 2, 2.5f);
                TriggerEnvironmentalHazard(SpawnPoint.Around(5f), "core", "ent_sht_smoke", 10f, 20000);

                if (Suspects.Count > 0 && Suspects[0].Exists() && Suspects[0].IsAlive && _boat != null && _boat.Exists())
                {
                    TriggerVehicleEscape(Suspects[0], _boat, SpawnPoint + new Vector3(0f, 250f, 0f));
                }
            }
        
        }
    }
}
