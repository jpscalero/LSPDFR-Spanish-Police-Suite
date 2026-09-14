using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts.Callouts.SWAT_NOOSE
{
    [CalloutInfo("SN_BusHijacking", CalloutProbability.High)]
    public class SN_BusHijacking : BaseTacticalCallout
    {
        public override string CalloutIdentifier { get { return "SN_BusHijacking"; } }
        public override string DisplayName { get { return "Operacion SWAT: Secuestro de Autobus Urbano en Marcha"; } }
        public override string DispatchSubtitle { get { return "Comando armado ha tomado un autobus con civiles como rehenes a bordo."; } }
        public override int ThreatLevel { get { return 5; } }

        private Vehicle _bus;
        private int _scenarioStage = 0;
        
        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(420.5f, -650.2f, 28.5f), new Vector3(1150.2f, -480.6f, 65.2f), new Vector3(-210.4f, -1650.2f, 34.8f), new Vector3(2500.1f, 2750.4f, 41.5f), new Vector3(-1050.8f, -850.4f, 19.2f), new Vector3(380.2f, -1880.5f, 23.4f)
            };

            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {

            _bus = SpawnVehicle("BUS", SpawnPoint, 0f);
            if (_bus != null)
            {
                Ped driver = SpawnPed("g_m_m_armgoon_01", _bus.Position, _bus.Heading);
                if (driver != null)
                {
                    driver.WarpIntoVehicle(_bus, -1);
                    driver.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_PISTOL50")).Ammo = -1;
                    ConfigurePedCombat(driver, ConfigManager.SuspectAccuracy, 80, true, 2);
                    AttachBlipToSuspect(driver, System.Drawing.Color.Red, 0.7f);
                    Suspects.Add(driver);
                    Rage.Native.NativeFunction.Natives.TASK_VEHICLE_DRIVE_WANDER(driver, _bus, 12f, 786603);
                }

                for (int p = 0; p < 2; p++)
                {
                    Ped gunner = SpawnPed("g_m_m_armgoon_01", _bus.Position, _bus.Heading);
                    if (gunner != null)
                    {
                        gunner.WarpIntoVehicle(_bus, p);
                        gunner.Inventory.Weapons.Add((WeaponHash)Game.GetHashKey("WEAPON_MICROSMG")).Ammo = -1;
                        ConfigurePedCombat(gunner, ConfigManager.SuspectAccuracy, 80, true, 2);
                        AttachBlipToSuspect(gunner, System.Drawing.Color.DarkRed, 0.6f);
                        Suspects.Add(gunner);
                    }
                }

                for (int h = 1; h <= 3; h++)
                {
                    Ped civ = SpawnPed("a_m_y_stbla_01", _bus.Position, _bus.Heading);
                    if (civ != null)
                    {
                        civ.WarpIntoVehicle(_bus, h + 2);
                        civ.Tasks.Cower(-1);
                        AttachBlipToHostage(civ, System.Drawing.Color.Green, 0.5f);
                        Hostages.Add(civ);
                    }
                }
            }
        
        }

        protected override void OnPlayerArrival()
        {

            base.OnPlayerArrival();
            Game.DisplaySubtitle("~r~[SWAT] ¡Inmovilicen el autobús con fuego de precisión a las ruedas! ¡Cuidado con los rehenes!");
        
        }

        protected override void OnScenarioTick()
        {

            base.OnScenarioTick();

            if (_scenarioStage == 0 && _bus != null && _bus.Exists())
            {
                if (_bus.Speed < 3.0f || _bus.EngineHealth < 400f)
                {
                    _scenarioStage = 1;
                    Game.DisplaySubtitle("~r~[SWAT] ¡El autobús se ha detenido! Los secuestradores toman posiciones de fuego.");

                    for (int i = 0; i < Suspects.Count; i++)
                    {
                        Ped s = Suspects[i];
                        if (s != null && s.Exists() && s.IsAlive && s.IsInVehicle(_bus, false))
                        {
                            s.BlockPermanentEvents = false;
                            s.KeepTasks = true;
                            s.Tasks.LeaveVehicle(_bus, LeaveVehicleFlags.None);
                        }
                    }

                    if (Suspects.Count > 0 && Hostages.Count > 0 && Suspects[0].IsAlive && Hostages[0].IsAlive)
                    {
                        StartHostageExecutionCountdown(45, Suspects[0], Hostages[0], "¡Neutralicen al secuestrador antes de que dispare!");
                    }
                }
            }
            else if (_scenarioStage == 1)
            {
                bool hostilesDead = true;
                for (int i = 0; i < Suspects.Count; i++)
                {
                    if (Suspects[i] != null && Suspects[i].Exists() && Suspects[i].IsAlive) { hostilesDead = false; break; }
                }

                if (hostilesDead && _scenarioStage == 1)
                {
                    _scenarioStage = 2;
                    Game.DisplaySubtitle("~g~[SWAT] ¡Autobús despejado! Evacuando rehenes hacia la ambulancia.");
                    Vector3 safeCoord = SpawnPoint.Around(25f);
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
