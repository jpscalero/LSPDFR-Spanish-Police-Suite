using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_MilitaryConvoyAmbush", CalloutProbability.High)]
    public class SN_MilitaryConvoyAmbush : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_MilitaryConvoyAmbush"; } }
        public override string DisplayName { get { return "Operacion SWAT: Emboscada a Convoy Militar en Desfiladero"; } }
        public override string DispatchSubtitle { get { return "Tropa del ejercito emboscada por insurgentes con armamento pesado y RPGs."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-450.4f, 2850.2f, 34.5f), new Vector3(1850.2f, 2650.4f, 45.2f), new Vector3(2550.4f, 3850.2f, 41.5f), new Vector3(-1100.2f, 4250.4f, 30.2f), new Vector3(650.4f, 3050.2f, 40.5f), new Vector3(1450.2f, 4750.4f, 42.1f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            SpawnVehicle("BARRACKS", SpawnPoint, 45f);
            SpawnVehicle("CRUSADER", SpawnPoint.Around(8f), 60f);

            // Soldados aliados emboscados
            for (int a = 0; a < 2; a++)
            {
                Ped soldier = SpawnPed("s_m_m_marine_01", SpawnPoint.Around(2f + (a * 2f)), 0f);
                if (soldier != null)
                {
                    soldier.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE")).Ammo = -1;
                    ConfigurePedCombat(soldier, 75, 100, true, 1);
                    soldier.BlockPermanentEvents = false;
                    FriendlyPeds.Add(soldier);
                }
            }

            // Insurgentes en cota elevada
            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("s_m_y_blackops_01", SpawnPoint.Around(12f + (i * 3.5f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Convoy militar bajo fuego intenso! Apoyen a los soldados y neutralicen a los insurgentes.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 20000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡Camioneta artillada de insurgentes entra en el desfiladero a toda velocidad!");
                SpawnHostileReinforcementVehicle("REBEL", "s_m_y_blackops_01", (WeaponHash)Game.GetHashKey("WEAPON_MG"), 3, SpawnPoint.Around(55f), SpawnPoint);
            }
        
        }
    }
}
