using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_ThreeWayGangWar", CalloutProbability.High)]
    public class SA_ThreeWayGangWar : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_ThreeWayGangWar"; } }
        public override string DisplayName { get { return "Asalto Urbano: Guerra Cruzada a Tres Bandas entre Pandillas"; } }
        public override string DispatchSubtitle { get { return "Tiroteo masivo entre Ballas, Families y Vagos en zona urbana cerrada."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(115.4f, -1960.2f, 20.8f),
                new Vector3(320.4f, -2020.5f, 18.2f),
                new Vector3(-150.2f, -1650.4f, 32.5f),
                new Vector3(480.2f, -1880.4f, 26.0f),
                new Vector3(1250.4f, -1720.2f, 51.5f),
                new Vector3(950.4f, -180.2f, 73.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            IsGangWarScenario = true;

            SpawnVehicle("VOODOO", SpawnPoint.Around(6f), 45f);
            SpawnVehicle("BUCCANEER", SpawnPoint.Around(12f), 180f);
            SpawnVehicle("PEYOTE", SpawnPoint.Around(16f), 270f);

            RelationshipGroup ballasGroup = new RelationshipGroup("GANG_BALLAS");
            RelationshipGroup familiesGroup = new RelationshipGroup("GANG_FAMILIES");
            RelationshipGroup vagosGroup = new RelationshipGroup("GANG_VAGOS");
            RelationshipGroup copGroup = new RelationshipGroup("COP");
            RelationshipGroup playerGroup = new RelationshipGroup("PLAYER");

            // Odio mutuo y cruzado entre las 3 bandas
            SetupMutualHate(ballasGroup, familiesGroup);
            SetupMutualHate(ballasGroup, vagosGroup);
            SetupMutualHate(familiesGroup, vagosGroup);

            // Odio mutuo contra la policia y el jugador
            SetupMutualHate(ballasGroup, copGroup);
            SetupMutualHate(familiesGroup, copGroup);
            SetupMutualHate(vagosGroup, copGroup);
            SetupMutualHate(ballasGroup, playerGroup);
            SetupMutualHate(familiesGroup, playerGroup);
            SetupMutualHate(vagosGroup, playerGroup);

            // Ballas
            for (int b = 0; b < 3; b++)
            {
                Ped bPed = CreateAssaultSuspect("g_m_y_ballasout_01", SpawnPoint.Around(2.5f + (b * 2f)), 0f, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), 70);
                if (bPed != null) bPed.RelationshipGroup = ballasGroup;
            }

            // Families
            for (int f = 0; f < 3; f++)
            {
                Ped fPed = CreateAssaultSuspect("g_m_y_famca_01", SpawnPoint.Around(8f + (f * 2.5f)), 180f, (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 70);
                if (fPed != null) fPed.RelationshipGroup = familiesGroup;
            }

            // Vagos
            for (int v = 0; v < 3; v++)
            {
                Ped vPed = CreateAssaultSuspect("g_m_y_salvagoon_01", SpawnPoint.Around(13f + (v * 2.2f)), 90f, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL50"), 70);
                if (vPed != null) vPed.RelationshipGroup = vagosGroup;
            }
        }

        protected override void OnStagingCompleted()
        {
            base.OnStagingCompleted();

            // Desbloquear eventos inmediatamente para que el tiroteo cruzado comience antes de llegar el jugador
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists())
                {
                    s.BlockPermanentEvents = false;
                    s.KeepTasks = true;
                    Rage.Native.NativeFunction.Natives.TASK_COMBAT_HATED_TARGETS_IN_AREA(s, 150f, 0);
                }
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[GUERRA A TRES BANDAS] ¡Ballas, Families y Vagos en fuego cruzado masivo! Busquen cobertura.");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();
        }
    }
}
