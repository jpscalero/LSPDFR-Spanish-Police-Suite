using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_HostageRescueComplex", CalloutProbability.High)]
    public class SN_HostageRescueComplex : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_HostageRescueComplex"; } }
        public override string DisplayName { get { return "Operacion SWAT: Rescate de Rehenes en Nave Logistica"; } }
        public override string DispatchSubtitle { get { return "Comando de secuestradores mantiene rehenes atados en sala blindada bajo amenaza de ejecucion."; } }
        public override int ThreatLevel { get { return 5; } }

        private int _scenarioStage = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1010.5f, -2530.2f, 28.3f), new Vector3(880.4f, -2180.5f, 30.5f), new Vector3(150.2f, -3200.4f, 5.8f), new Vector3(-350.5f, -2600.2f, 6.2f), new Vector3(720.4f, -1950.2f, 29.5f), new Vector3(-550.2f, -2450.4f, 13.5f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            for (int i = 0; i < 4; i++)
            {
                CreateTacticalSuspect("s_m_y_blackops_01", SpawnPoint.Around(4f + (i * 2.5f)), (float)(i * 90), (WeaponHash)Game.GetHashKey("WEAPON_COMBATMG"), 100);
            }

            Ped h1 = CreateTacticalHostage("a_f_y_business_01", SpawnPoint.Around(2f), 0f);
            Ped h2 = CreateTacticalHostage("a_m_y_business_02", SpawnPoint.Around(3.5f), 180f);
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Rehenes en peligro crítico! Brecha táctica inminente con aturdidoras.");
            DetonateFlashbang(SpawnPoint.Around(4f));

            if (Suspects.Count > 0 && Hostages.Count > 0)
            {
                StartHostageExecutionCountdown(45, Suspects[0], Hostages[0], "¡Neutralicen al líder antes de que abra fuego sobre los civiles!");
            }
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();

            if (_scenarioStage == 0)
            {
                bool hostilesNeutralized = true;
                for (int i = 0; i < Suspects.Count; i++)
                {
                    if (Suspects[i] != null && Suspects[i].Exists() && Suspects[i].IsAlive) { hostilesNeutralized = false; break; }
                }

                if (hostilesNeutralized)
                {
                    _scenarioStage = 1;
                    Game.DisplaySubtitle("~g~[SWAT] ¡Secuestradores neutralizados! Evacuando rehenes a la zona segura.");
                    Vector3 safeCoord = SpawnPoint.Around(30f);
                    for (int h = 0; h < Hostages.Count; h++)
                    {
                        if (Hostages[h] != null && Hostages[h].Exists() && Hostages[h].IsAlive)
                        {
                            ExtractHostageToSafety(Hostages[h], safeCoord);
                        }
                    }
                }
            }
        
        }
    }
}
