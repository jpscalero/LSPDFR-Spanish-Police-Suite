using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_DarkWarehouseShootout", CalloutProbability.High)]
    public class SA_DarkWarehouseShootout : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_DarkWarehouseShootout"; } }
        public override string DisplayName { get { return "Asalto Urbano: Emboscada en Almacen a Oscuras"; } }
        public override string DispatchSubtitle { get { return "Contrabandistas usando linternas tácticas en penumbra interior."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1450.4f, -2050.2f, 52.0f),
                new Vector3(1050.2f, -2400.4f, 30.5f),
                new Vector3(150.4f, -3150.2f, 5.8f),
                new Vector3(-580.4f, -1780.2f, 22.5f),
                new Vector3(720.4f, -1080.2f, 21.5f),
                new Vector3(-1050.2f, -550.4f, 24.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("BURRITO", SpawnPoint.Around(10f), 0f);

            for (int i = 0; i < 4; i++)
            {
                Ped s = CreateAssaultSuspect("g_m_m_armgoon_01", SpawnPoint.Around(3f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_PUMPSHOTGUN"), 80);
                if (s != null && s.Exists())
                {
                    Rage.Native.NativeFunction.Natives.GIVE_WEAPON_COMPONENT_TO_PED(s, Game.GetHashKey("WEAPON_PUMPSHOTGUN"), Game.GetHashKey("COMPONENT_AT_AR_FLSH"));
                    Rage.Native.NativeFunction.Natives.SET_FLASH_LIGHT_ENABLED(s, true);
                }
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            PlayPtfx("core", "ent_sht_smoke", SpawnPoint, 2.0f);
            Game.DisplaySubtitle("~r~[PENUMBRA INTERIOR] ¡Contrabandistas armados con escopetas entre las estanterías!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
