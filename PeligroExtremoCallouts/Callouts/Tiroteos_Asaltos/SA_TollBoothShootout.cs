using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_TollBoothShootout", CalloutProbability.High)]
    public class SA_TollBoothShootout : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_TollBoothShootout"; } }
        public override string DisplayName { get { return "Asalto Urbano: Emboscada Armada en Peaje de Autopista"; } }
        public override string DispatchSubtitle { get { return "Tiradores atrincherados en cabinas de peaje disparando a vehiculos policiales."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-1055.2f, -862.5f, 5.0f), // Palomino / Del Perro Freeway Toll
                new Vector3(-1012.4f, -2700.5f, 13.8f), // LSIA Terminal Barrier
                new Vector3(1048.5f, -2975.2f, 5.9f), // Port of LS Barrier
                new Vector3(380.5f, -2730.4f, 6.0f), // Elysian Island Gate
                new Vector3(2745.2f, 1490.4f, 24.5f), // Palmer-Taylor Security Gate
                new Vector3(-2300.5f, 3240.2f, 32.8f) // Zancudo Barrier Gate
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("SCHAFTER", SpawnPoint.Around(8f), 0f);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_y_blackops_01", SpawnPoint.Around(2.5f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), 90);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[FUEGO EN PEAJE] ¡Tiradores parapetados en las cabinas de peaje! Despejen los carriles.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
