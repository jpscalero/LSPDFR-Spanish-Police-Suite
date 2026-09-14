using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_JewelryStoreSmokeBombs", CalloutProbability.High)]
    public class SA_JewelryStoreSmokeBombs : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_JewelryStoreSmokeBombs"; } }
        public override string DisplayName { get { return "Asalto Urbano: Atraco a Joyeria con Bombas de Humo"; } }
        public override string DispatchSubtitle { get { return "Ladrones desplegando botes de humo para huir con bolsas de diamantes."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-625.4f, -235.2f, 38.0f),
                new Vector3(-630.2f, -240.4f, 38.0f),
                new Vector3(-615.4f, -225.2f, 38.0f),
                new Vector3(-635.4f, -220.2f, 38.0f),
                new Vector3(-610.2f, -245.4f, 38.0f),
                new Vector3(-640.4f, -230.2f, 38.0f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("BATI", SpawnPoint.Around(6f), 0f);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_m_armgoon_01", SpawnPoint.Around(2f + (i * 2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 80);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            PlayPtfx("core", "ent_sht_smoke", SpawnPoint, 3.0f);
            Game.DisplaySubtitle("~r~[HUMO DENSO] ¡Botes de humo desplegados! Los ladrones intentan alcanzar las motocicletas.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
