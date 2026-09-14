using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_NoKnockRaid", CalloutProbability.High)]
    public class SN_NoKnockRaid : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_NoKnockRaid"; } }
        public override string DisplayName { get { return "Operacion SWAT: Entrada Forzada No-Knock en Madrugada"; } }
        public override string DispatchSubtitle { get { return "Orden de registro de alto riesgo sobre piso franco del cartel fuertemente vigilado."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(325.4f, -210.5f, 54.2f), new Vector3(-1380.2f, -590.4f, 30.2f), new Vector3(120.5f, -1480.2f, 29.5f), new Vector3(-820.4f, -1075.2f, 11.2f), new Vector3(980.2f, -1820.4f, 31.2f), new Vector3(-35.4f, -1100.2f, 38.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("g_m_y_mexgoon_01", SpawnPoint.Around(2.5f + (i * 2f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_SAWNOFFSHOTGUN"), 80);
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT NO-KNOCK] ¡3... 2... 1... BRECHA! ¡Carga detonada y aturdidora al interior!");
            TacticalBreachManager.ExecuteBreachingCharge(SpawnPoint, Suspects);
            DetonateFlashbang(SpawnPoint);
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();

            if (_scenarioStage == 0)
            {
                _scenarioStage = 1;
                // El tirador de fondo intenta destruir fardos de droga
                if (Suspects.Count > 1 && Suspects[1].Exists() && Suspects[1].IsAlive)
                {
                    TriggerEnvironmentalHazard(SpawnPoint.Around(4f), "core", "ent_sht_smoke", 6f, 15000);
                }
            }
        
        }
    }
}
