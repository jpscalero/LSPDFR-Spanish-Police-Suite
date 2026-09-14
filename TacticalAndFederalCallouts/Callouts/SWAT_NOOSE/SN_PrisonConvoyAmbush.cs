using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_PrisonConvoyAmbush", CalloutProbability.High)]
    public class SN_PrisonConvoyAmbush : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_PrisonConvoyAmbush"; } }
        public override string DisplayName { get { return "Operacion SWAT: Asalto a Furgon Penitenciario en Autovia"; } }
        public override string DispatchSubtitle { get { return "Furgon blindado de reclusos de maxima seguridad emboscado y volcado por sicarios."; } }
        public override int ThreatLevel { get { return 5; } }

        private Ped _vipInmate;
        private int _scenarioStage = 0;
        private uint _stageTimer = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1750.2f, 2600.4f, 45.5f), new Vector3(2450.4f, 3120.2f, 48.5f), new Vector3(-420.5f, 3850.4f, 40.2f), new Vector3(1200.2f, 1850.5f, 52.1f), new Vector3(-1050.4f, 2800.2f, 22.4f), new Vector3(2100.2f, 2150.5f, 54.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            Vehicle bus = SpawnVehicle("pbus", SpawnPoint, 30f);
            if (bus != null) bus.EngineHealth = 0f;

            SpawnVehicle("BURRITO", SpawnPoint.Around(8f), 120f);

            // Guardias penitenciarios heridos
            for (int g = 0; g < 2; g++)
            {
                Ped guard = SpawnPed("s_m_m_armoured_01", SpawnPoint.Around(2f + (g * 2f)), 0f);
                if (guard != null)
                {
                    guard.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_PUMPSHOTGUN")).Ammo = -1;
                    ConfigurePedCombat(guard, 75, 80, true, 1);
                    guard.BlockPermanentEvents = false;
                    FriendlyPeds.Add(guard);
                }
            }

            // Sicarios asaltantes
            for (int s = 0; s < 4; s++)
            {
                CreateTacticalSuspect("g_m_m_armgoon_01", SpawnPoint.Around(6f + (s * 3f)), (float)(s * 85), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 90);
            }

            // Recluso VIP
            _vipInmate = SpawnPed("s_m_y_prismuscl_01", SpawnPoint.Around(1f), 0f);
            if (_vipInmate != null)
            {
                _vipInmate.Tasks.Cower(-1);
                AttachBlipToSuspect(_vipInmate, System.Drawing.Color.DarkRed, 0.7f);
                Suspects.Add(_vipInmate);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Emboscada en curso al furgón penitenciario! Los sicarios intentan liberar al cabecilla.");
            _stageTimer = Game.GameTime;
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();
            uint elapsed = Game.GameTime - _stageTimer;

            if (_scenarioStage == 0 && elapsed > 16000)
            {
                _scenarioStage = 1;
                Game.DisplaySubtitle("~r~[SWAT] ¡El recluso cabecilla corre hacia el todoterreno de fuga! ¡Deténganlo!");
                if (_vipInmate != null && _vipInmate.Exists() && _vipInmate.IsAlive && Vehicles.Count > 1 && Vehicles[1].Exists())
                {
                    TriggerVehicleEscape(_vipInmate, Vehicles[1], SpawnPoint.Around(200f));
                }
            }
        
        }
    }
}
