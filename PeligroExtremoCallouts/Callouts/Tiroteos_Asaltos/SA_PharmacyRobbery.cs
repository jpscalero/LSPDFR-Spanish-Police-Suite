using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_PharmacyRobbery", CalloutProbability.High)]
    public class SA_PharmacyRobbery : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_PharmacyRobbery"; } }
        public override string DisplayName { get { return "Asalto Urbano: Atraco a Farmacia por Sustancias Controladas"; } }
        public override string DispatchSubtitle { get { return "Toxicomanos armados exigen medicamentos de especial control bajo amenaza."; } }
        public override int ThreatLevel { get { return 3; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-170.4f, 6380.2f, 31.5f),
                new Vector3(1700.4f, 4920.2f, 42.0f),
                new Vector3(380.4f, 325.2f, 103.0f),
                new Vector3(-70.4f, -1530.2f, 34.0f),
                new Vector3(1150.4f, -1520.2f, 35.0f),
                new Vector3(305.4f, -1435.2f, 29.8f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("ASEA", SpawnPoint.Around(7f), 0f);
            CreateVictim("s_m_m_doctor_01", SpawnPoint, 0f);

            for (int i = 0; i < 2; i++)
            {
                CreateAssaultSuspect("g_m_y_mexgang_01", SpawnPoint.Around(2f + (i * 2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"), 50);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[ATRACO A FARMACIA] ¡Sujetos armados exigiendo estupefacientes! Reduzcan la amenaza.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
