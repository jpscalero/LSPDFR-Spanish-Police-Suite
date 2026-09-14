using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos
{
    [CalloutInfo("SA_TrafficStopPointBlankAmbush", CalloutProbability.High)]
    public class SA_TrafficStopPointBlankAmbush : BaseAssaultCallout
    {
        public override string CalloutIdentifier { get { return "SA_TrafficStopPointBlankAmbush"; } }
        public override string DisplayName { get { return "Asalto Urbano: Emboscada a Bocajarro en Parada de Trafico"; } }
        public override string DispatchSubtitle { get { return "Conductor finge buscar papeles y abre fuego con escopeta recortada."; } }
        public override int ThreatLevel { get { return 5; } }

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(250.4f, -1200.2f, 38.5f),
                new Vector3(-210.4f, -1020.2f, 30.5f),
                new Vector3(430.4f, -650.2f, 28.5f),
                new Vector3(1150.4f, -420.2f, 67.2f),
                new Vector3(-800.4f, -150.2f, 37.5f),
                new Vector3(1690.4f, 3280.2f, 41.1f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            // Distancia de activacion realista (16 metros) para activar al aproximarse a pie
            EngagementDistance = 16f;
            Vehicle primo = SpawnVehicle("PRIMO", SpawnPoint, 0f);

            Ped shooter = CreateAssaultSuspect("g_m_y_ballaeast_01", SpawnPoint, 0f, (WeaponHash)Game.GetHashKey("WEAPON_SAWNOFFSHOTGUN"), 80);
            Ped passenger = CreateAssaultSuspect("g_m_y_ballasout_01", SpawnPoint, 0f, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"), 60);

            if (primo != null && primo.Exists())
            {
                if (shooter != null) shooter.WarpIntoVehicle(primo, -1);
                if (passenger != null) passenger.WarpIntoVehicle(primo, 0);
            }
        }

        protected override void OnPlayerArrival()
        {
            base.OnPlayerArrival();

            Ped player = Game.LocalPlayer.Character;
            Game.DisplaySubtitle("~r~[¡EMBOSCADA A BOCAJARRO!] ¡El sospechoso abre fuego sorpresivo con recortada!");

            // Secuencia de descenso del vehiculo con GameFiber para evitar colision de tareas nativas
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists())
                {
                    s.BlockPermanentEvents = false;
                    s.KeepTasks = true;

                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            if (s != null && s.Exists() && s.IsAlive)
                            {
                                s.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut | LeaveVehicleFlags.LeaveDoorOpen);
                                GameFiber.Sleep(1200);
                                if (s != null && s.Exists() && s.IsAlive && player != null && player.Exists())
                                {
                                    s.Tasks.FightAgainst(player);
                                }
                            }
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SA_TrafficStopPointBlankAmbush.Fiber: " + ex.Message);
                        }
                    });
                }
            }
        }

        protected override void OnScenarioTick()
        {
            base.OnScenarioTick();
        }
    }
}
