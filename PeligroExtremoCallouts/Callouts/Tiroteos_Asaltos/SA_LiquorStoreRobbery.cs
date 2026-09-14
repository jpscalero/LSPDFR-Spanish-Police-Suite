using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_LiquorStoreRobbery", CalloutProbability.High)]
    public class SA_LiquorStoreRobbery : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_LiquorStoreRobbery"; } }
        public override string DisplayName { get { return "Asalto Urbano: Atraco a Mano Armada en Licoreria"; } }
        public override string DispatchSubtitle { get { return "Asaltantes encañonan al encargado tras el mostrador de la tienda."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1390.4f, 3600.2f, 34.5f),
                new Vector3(-1220.4f, -910.2f, 12.5f),
                new Vector3(1135.4f, -980.2f, 46.5f),
                new Vector3(-2960.4f, 390.2f, 15.0f),
                new Vector3(-1480.4f, -380.2f, 40.2f),
                new Vector3(1165.4f, 2710.2f, 38.0f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("SPEEDO", SpawnPoint.Around(8f), 0f);
            CreateVictim("a_m_m_salton_01", SpawnPoint, 0f);

            for (int i = 0; i < 2; i++)
            {
                CreateAssaultSuspect("g_m_y_mexgang_01", SpawnPoint.Around(2f + (i * 2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_COMBATPISTOL"), 60);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[ASALTO EN CURSO] ¡Asaltantes armados en el interior! Protejan al encargado.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
