using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_DelPerroPierShootout", CalloutProbability.High)]
    public class SA_DelPerroPierShootout : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_DelPerroPierShootout"; } }
        public override string DisplayName { get { return "Asalto Urbano: Fuego Cruzado en Paseo Maritimo de Del Perro Pier"; } }
        public override string DispatchSubtitle { get { return "Tiradores armados en el muelle provocando panico generalizado entre banistas."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-1650.4f, -1050.2f, 13.0f),
                new Vector3(-1700.2f, -1120.4f, 13.0f),
                new Vector3(-1600.4f, -980.2f, 13.0f),
                new Vector3(-1750.4f, -1180.2f, 13.0f),
                new Vector3(-1550.2f, -920.4f, 13.0f),
                new Vector3(-1680.4f, -1080.2f, 13.0f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("SANCHEZ", SpawnPoint.Around(6f), 0f);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_y_mexgang_01", SpawnPoint.Around(2.5f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 60);
            }

            // autoCower = false para permitir huida activa y estampida
            for (int c = 0; c < 3; c++)
            {
                CreateVictim("a_m_y_beach_01", SpawnPoint.Around(4f + (c * 2.5f)), 0f, false);
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            for (int i = 0; i < Victims.Count; i++)
            {
                Ped v = Victims[i];
                if (v != null && v.Exists() && v.IsAlive)
                {
                    v.BlockPermanentEvents = false;
                    v.KeepTasks = true;
                    v.Tasks.ReactAndFlee(Suspects.Count > 0 ? Suspects[0] : Game.LocalPlayer.Character);
                }
            }

            Game.DisplaySubtitle("~r~[MUELLE DEL PERRO] ¡Banistas en peligro! Precision maxima para evitar bajas civiles.");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();
        }
    }
}
