using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_GasStationShootout", CalloutProbability.High)]
    public class SA_GasStationShootout : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_GasStationShootout"; } }
        public override string DisplayName { get { return "Asalto Urbano: Tiroteo en Gasolinera con Riesgo de Deflagracion"; } }
        public override string DispatchSubtitle { get { return "Sospechosos atrincherados junto a los surtidores abriendo fuego cruzado."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-70.4f, -1760.2f, 29.5f),
                new Vector3(265.4f, -1260.2f, 29.2f),
                new Vector3(1180.4f, -330.2f, 69.2f),
                new Vector3(620.4f, 268.2f, 103.2f),
                new Vector3(2580.4f, 360.2f, 108.5f),
                new Vector3(-2095.4f, -320.2f, 13.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("SULTAN", SpawnPoint.Around(7f), 0f);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_m_armgoon_01", SpawnPoint.Around(2.5f + (i * 2.2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 70);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[PELIGRO DE INCENDIO] ¡Tiroteo en gasolinera! Balas perdidas pueden hacer estallar los depósitos.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
