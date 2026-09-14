using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_DeadEndAlleyShootout", CalloutProbability.High)]
    public class SA_DeadEndAlleyShootout : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_DeadEndAlleyShootout"; } }
        public override string DisplayName { get { return "Asalto Urbano: Tiroteo Desesperado en Callejon Sin Salida"; } }
        public override string DispatchSubtitle { get { return "Sospechosos acorralados abren fuego a quemarropa sin intencion de rendirse."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(128.4f, -1650.2f, 29.5f),
                new Vector3(-1050.4f, -480.2f, 36.8f),
                new Vector3(380.2f, -800.4f, 29.2f),
                new Vector3(-520.4f, -1220.5f, 18.5f),
                new Vector3(810.2f, -2150.4f, 29.5f),
                new Vector3(1150.4f, -420.5f, 67.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("MANANA", SpawnPoint, 0f);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_y_ballasout_01", SpawnPoint.Around(2f + (i * 2.2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 70);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[CALLEJÓN SIN SALIDA] ¡Sujetos acorralados disparando a discreción!");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
