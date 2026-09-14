using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_GruppeSechsArmoredTruckHeist", CalloutProbability.High)]
    public class SA_GruppeSechsArmoredTruckHeist : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_GruppeSechsArmoredTruckHeist"; } }
        public override string DisplayName { get { return "Asalto Urbano: Asalto Armado a Camion Blindado Gruppe Sechs"; } }
        public override string DispatchSubtitle { get { return "Comando con armas pesadas intercepta camion de caudales en calle comercial."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-620.4f, -700.2f, 31.5f),
                new Vector3(15.4f, -650.2f, 33.2f),
                new Vector3(320.4f, -800.2f, 29.5f),
                new Vector3(-1380.4f, -480.2f, 33.5f),
                new Vector3(1150.4f, -780.2f, 57.5f),
                new Vector3(-255.4f, -920.2f, 32.2f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            SpawnVehicle("STOCKADE", SpawnPoint, 0f);

            Ped gGuard = CreateAlliedOfficer("s_m_m_armoured_01", SpawnPoint.Around(1.8f), 0f, (WeaponHash)Game.GetHashKey("WEAPON_PUMPSHOTGUN"));
            if (gGuard != null && gGuard.Exists())
            {
                gGuard.BlockPermanentEvents = false;
            }

            for (int i = 0; i < 4; i++)
            {
                CreateAssaultSuspect("g_m_y_blackops_01", SpawnPoint.Around(5f + (i * 2.5f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), 100);
            }
        }

        protected override void OnStagingCompleted()
        {
            base.OnStagingCompleted();

            if (FriendlyPeds.Count > 0 && Suspects.Count > 0)
            {
                Ped guard = FriendlyPeds[0];
                if (guard != null && guard.Exists())
                {
                    guard.Tasks.FightAgainst(Suspects[0]);
                }
                for (int i = 0; i < Suspects.Count; i++)
                {
                    Ped s = Suspects[i];
                    if (s != null && s.Exists() && guard != null)
                    {
                        s.Tasks.FightAgainst(guard);
                    }
                }
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[ATRACO A BLINDADO] ¡Asaltantes con fusiles pesados intentando reventar las puertas traseras!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
