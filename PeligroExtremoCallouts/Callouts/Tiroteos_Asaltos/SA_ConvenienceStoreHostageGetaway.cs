using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_ConvenienceStoreHostageGetaway", CalloutProbability.High)]
    public class SA_ConvenienceStoreHostageGetaway : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_ConvenienceStoreHostageGetaway"; } }
        public override string DisplayName { get { return "Asalto Urbano: Atraco a Tienda 24/7 con Huida en Vehiculo"; } }
        public override string DispatchSubtitle { get { return "Atraco en curso con rehen; vehiculo de fuga esperando en la acera."; } }
        public override int ThreatLevel { get { return 4; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(25.4f, -1345.2f, 29.5f),
                new Vector3(-705.4f, -915.2f, 19.2f),
                new Vector3(1165.2f, -325.4f, 69.2f),
                new Vector3(-1820.4f, 790.2f, 138.2f),
                new Vector3(545.2f, 2670.4f, 42.1f),
                new Vector3(1960.4f, 3740.2f, 32.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            GetawayVehicle = SpawnVehicle("FELON", SpawnPoint.Around(15f), 0f);

            CreateVictim("mp_m_shopkeep_01", SpawnPoint, 0f, true);

            for (int i = 0; i < 3; i++)
            {
                CreateAssaultSuspect("g_m_y_ballasout_01", SpawnPoint.Around(2f + (i * 2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 60);
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[DISPAROS] ¡Atracadores saliendo del establecimiento hacia el vehiculo de huida!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

            if (!GetawayTriggered && GetawayVehicle != null && GetawayVehicle.Exists())
            {
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Position.DistanceTo(SpawnPoint) < 25f && Suspects.Count > 0)
                {
                    GetawayTriggered = true;
                    LSPD_First_Response.Mod.API.LHandle pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit();

                    if (Suspects[0] != null && Suspects[0].Exists() && Suspects[0].IsAlive)
                    {
                        Suspects[0].Tasks.EnterVehicle(GetawayVehicle, -1);
                        LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(pursuit, Suspects[0]);
                    }

                    for (int i = 1; i < Suspects.Count; i++)
                    {
                        if (Suspects[i] != null && Suspects[i].Exists() && Suspects[i].IsAlive)
                        {
                            Suspects[i].Tasks.EnterVehicle(GetawayVehicle, i - 1);
                            // Agregar a TODOS los sospechosos a la persecucion de LSPDFR
                            LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(pursuit, Suspects[i]);
                        }
                    }

                    LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                }
            }
        }
    }
}
