using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_AmmunationRobbery", CalloutProbability.High)]
    public class SA_AmmunationRobbery : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_AmmunationRobbery"; } }
        public override string DisplayName { get { return "Asalto Urbano: Atraco a Armeria Ammunation"; } }
        public override string DispatchSubtitle { get { return "Asaltantes armados intentan saquear vitrinas y arsenal con rehenes."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(250.4f, -50.2f, 69.8f),
                new Vector3(840.4f, -1030.2f, 28.2f),
                new Vector3(-660.4f, -935.2f, 21.8f),
                new Vector3(-1305.4f, -390.2f, 36.5f),
                new Vector3(1690.4f, 3755.2f, 34.5f),
                new Vector3(-330.4f, 6080.2f, 31.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("SANDKING", SpawnPoint.Around(16f), 0f);
            CreateVictim("s_m_m_ammucountry", SpawnPoint, 0f);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_y_blackops_01", SpawnPoint.Around(2.5f + (i * 2.2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 100);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[ATRACO A ARMERÍA] ¡Asaltantes con chalecos balísticos pesados robando el arsenal!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
