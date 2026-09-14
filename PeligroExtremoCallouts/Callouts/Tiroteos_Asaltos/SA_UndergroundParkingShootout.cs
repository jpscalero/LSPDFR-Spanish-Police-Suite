using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_UndergroundParkingShootout", CalloutProbability.High)]
    public class SA_UndergroundParkingShootout : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_UndergroundParkingShootout"; } }
        public override string DisplayName { get { return "Asalto Urbano: Fuego Cruzado en Parking Subterraneo"; } }
        public override string DispatchSubtitle { get { return "Sospechosos atrincherados entre columnas de hormigon y vehiculos aparcados."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(215.4f, -805.2f, 30.8f),
                new Vector3(-280.4f, -780.2f, 30.5f),
                new Vector3(380.4f, -650.2f, 28.5f),
                new Vector3(-1380.4f, -480.2f, 30.5f),
                new Vector3(-750.4f, -70.2f, 36.5f),
                new Vector3(-60.4f, -1100.2f, 26.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("BUFFALO", SpawnPoint.Around(6f), 0f);

            for (int i = 0; i < 4; i++)
            {
                CreateAssaultSuspect("g_m_m_armgoon_01", SpawnPoint.Around(2.5f + (i * 2.2f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 70);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[COMBATE EN PARKING] ¡Eco de disparos entre columnas! Flanqueen con cuidado.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
