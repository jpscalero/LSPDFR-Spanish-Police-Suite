using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_NightclubShootout", CalloutProbability.High)]
    public class SA_NightclubShootout : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_NightclubShootout"; } }
        public override string DisplayName { get { return "Asalto Urbano: Tiroteo y Estampida en Club Nocturno"; } }
        public override string DispatchSubtitle { get { return "Pelea con armas de fuego en puerta de discoteca con multitud en panico."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(125.4f, -1280.2f, 29.2f),
                new Vector3(-1380.4f, -600.2f, 30.5f),
                new Vector3(350.4f, -980.2f, 29.5f),
                new Vector3(-550.4f, -200.2f, 38.5f),
                new Vector3(750.4f, -750.2f, 26.5f),
                new Vector3(-900.4f, -350.2f, 38.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("ORACLE", SpawnPoint.Around(9f), 0f);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_m_armgoon_01", SpawnPoint.Around(2.5f + (i * 2.2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 60);
            }

            // autoCower = false para permitir estampida y evacuacion realista
            for (int c = 0; c < 3; c++)
            {
                CreateVictim("a_f_y_clubcust_01", SpawnPoint.Around(4f + (c * 2f)), 0f, false);
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

            Game.DisplaySubtitle("~r~[TIROTEO EN DISCOTECA] ¡Civiles huyendo en estampida! Fuego de precision requerido.");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();
        }
    }
}
