using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_CarWashTunnelShootout", CalloutProbability.High)]
    public class SA_CarWashTunnelShootout : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_CarWashTunnelShootout"; } }
        public override string DisplayName { get { return "Asalto Urbano: Emboscada Armada en Tunel de Autolavado"; } }
        public override string DispatchSubtitle { get { return "Tiradores emboscados en la salida del tunel de lavado de vehiculos."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(55.4f, -1390.2f, 29.2f),
                new Vector3(170.4f, -1710.2f, 29.2f),
                new Vector3(-700.4f, -920.2f, 19.2f),
                new Vector3(-1400.4f, -650.2f, 28.5f),
                new Vector3(1150.4f, -780.2f, 57.5f),
                new Vector3(-350.2f, -1520.4f, 27.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("BALLER", SpawnPoint, 0f);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_y_mexgang_01", SpawnPoint.Around(2.2f + (i * 2.2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_PUMPSHOTGUN"), 70);
            }
        
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            PlayPtfx("core", "ent_sht_smoke", SpawnPoint, 2.0f);
            Game.DisplaySubtitle("~r~[AUTOLAVADO] ¡Chorros de vapor y agua reduciendo visibilidad! Abran fuego.");
        
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
