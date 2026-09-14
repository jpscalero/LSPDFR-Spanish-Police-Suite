using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_PatrolAmbush1099", CalloutProbability.High)]
    public class SA_PatrolAmbush1099 : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_PatrolAmbush1099"; } }
        public override string DisplayName { get { return "Asalto Urbano: Emboscada Mortal 10-99 a Patrulla Policial"; } }
        public override string DispatchSubtitle { get { return "Oficial bajo fuego intenso pidiendo auxilio desesperado tras ser acorralado."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(480.2f, -1350.4f, 29.2f),
                new Vector3(-450.2f, -1650.4f, 18.5f),
                new Vector3(1200.4f, -1450.2f, 35.0f),
                new Vector3(810.4f, -1800.2f, 29.0f),
                new Vector3(-1050.2f, -850.4f, 19.2f),
                new Vector3(1350.2f, -600.4f, 74.0f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            Vehicle copCar = SpawnVehicle("POLICE", SpawnPoint, 0f);
            if (copCar != null)
            {
                copCar.EngineHealth = 0f;
                copCar.IsSirenOn = true;
                copCar.IsSirenSilent = false;
            }

            Ped heroOfficer = CreateAlliedOfficer("s_m_y_cop_01", SpawnPoint.Around(1.8f), 180f, (WeaponHash)Game.GetHashKey("WEAPON_COMBATPISTOL"));
            if (heroOfficer != null && heroOfficer.Exists())
            {
                heroOfficer.BlockPermanentEvents = false;
            }

            for (int i = 0; i < 4; i++)
            {
                CreateAssaultSuspect("g_m_y_mexgang_01", SpawnPoint.Around(6f + (i * 2.8f)), (float)(i * 80), (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), 80);
            }
        }

        protected override void OnStagingCompleted()
        {
            base.OnStagingCompleted();

            if (FriendlyPeds.Count > 0 && Suspects.Count > 0)
            {
                Ped hero = FriendlyPeds[0];
                if (hero != null && hero.Exists())
                {
                    hero.Tasks.FightAgainst(Suspects[0]);
                }
                for (int i = 0; i < Suspects.Count; i++)
                {
                    Ped s = Suspects[i];
                    if (s != null && s.Exists() && hero != null)
                    {
                        s.Tasks.FightAgainst(hero);
                    }
                }
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Game.DisplaySubtitle("~r~[10-99 OFICIAL BAJO FUEGO] ¡Respalden al oficial emboscado antes de que sea abatido!");
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();

        }
    }
}
