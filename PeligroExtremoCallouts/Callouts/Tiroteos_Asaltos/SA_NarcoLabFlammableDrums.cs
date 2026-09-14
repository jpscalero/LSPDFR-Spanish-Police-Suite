using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_NarcoLabFlammableDrums", CalloutProbability.High)]
    public class SA_NarcoLabFlammableDrums : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_NarcoLabFlammableDrums"; } }
        public override string DisplayName { get { return "Asalto Urbano: Laboratorio con Tambores Inflamables"; } }
        public override string DispatchSubtitle { get { return "Tiroteo en nave clandestina con presencia de bidones de disolvente explosivo."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1400.4f, -2050.2f, 52.0f),
                new Vector3(2500.5f, 4150.2f, 38.5f),
                new Vector3(-1100.4f, 2700.2f, 19.5f),
                new Vector3(1700.2f, 3500.4f, 36.5f),
                new Vector3(2750.4f, 1550.2f, 24.5f),
                new Vector3(380.2f, 3580.4f, 33.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("BURRITO3", SpawnPoint.Around(8f), 0f);
            SpawnProp("prop_barrel_exp_01a", SpawnPoint.Around(3f), 0f);
            SpawnProp("prop_barrel_exp_01a", SpawnPoint.Around(6f), 45f);

            for (int i = 0; i < 4; i++)
            {
                CreateAssaultSuspect("g_m_y_mexgang_01", SpawnPoint.Around(3f + (i * 2.5f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 70);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            PlayPtfx("core", "ent_sht_smoke", SpawnPoint.Around(3f), 2.5f);
            Game.DisplaySubtitle("~r~[PELIGRO QUÍMICO] ¡Disparos dentro de la nave! Eviten los bidones presurizados.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
