using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;

namespace PeligroExtremoCallouts.Core
{
    public class PassengerSetup
    {
        public Ped PassengerPed { get; set; }
        public WeaponHash Weapon { get; set; }
        public int Accuracy { get; set; }
    }

    /// <summary>
    /// Especializacion para persecuciones vehiculares extremas y peligrosas.
    /// Administra el ciclo de vida del LHandle de persecucion de LSPDFR,
    /// refuerzos en carretera, maniobras evasivas avanzadas, conduccion temeraria,
    /// fuego en marcha (drive-by) sin sobreescritura de armas, deteccion de vehiculos inmovilizados
    /// con evacuacion y combate a pie (bailout), y persecuciones nauticas/aereas.
    /// </summary>
    public abstract class BasePursuitCallout : BaseCallout
    {
        protected LHandle Pursuit { get; set; }
        protected bool PursuitCreated { get; set; }

        protected Vehicle SuspectVehicle { get; set; }
        protected Ped Driver { get; set; }
        protected readonly List<PassengerSetup> PassengerSetups = new List<PassengerSetup>();
        protected readonly List<Ped> SecondaryDrivers = new List<Ped>();

        protected bool IsNauticalOrAerial { get; set; }
        protected bool VehicleBombActive { get; set; }
        protected int VehicleBombSecondsRemaining { get; set; }
        protected bool UseCustomDriverConfig { get; set; }

        private int _vehicleStoppedTicks = 0;
        private bool _bailoutTriggered = false;

        protected BasePursuitCallout()
        {
            PursuitCreated = false;
            IsNauticalOrAerial = false;
            VehicleBombActive = false;
            UseCustomDriverConfig = false;
        }

        protected override void OnPlayerArrival()
        {
            Game.DisplaySubtitle("~r~[PERSECUCION INICIADA]~w~ " + DisplayName + ". El vehiculo sospechoso emprende la huida.");
            StartPursuit();
        }

        protected virtual void StartPursuit()
        {
            if (PursuitCreated) return;

            try
            {
                if (!IsNauticalOrAerial)
                {
                    Pursuit = Functions.CreatePursuit();
                }

                if (Driver != null && Driver.Exists())
                {
                    Driver.BlockPermanentEvents = false;
                    Driver.KeepTasks = true;

                    if (!IsNauticalOrAerial)
                    {
                        Functions.AddPedToPursuit(Pursuit, Driver);
                    }
                    if (!UseCustomDriverConfig && ConfigManager.AggressiveDrivingAI && SuspectVehicle != null && SuspectVehicle.Exists())
                    {
                        PursuitConfigurator.ConfigureAggressiveDriver(Driver, SuspectVehicle);
                    }
                }

                // Conductores secundarios (motos adicionales, quads, vehiculos de escolta)
                for (int d = 0; d < SecondaryDrivers.Count; d++)
                {
                    Ped secDriver = SecondaryDrivers[d];
                    if (secDriver != null && secDriver.Exists() && secDriver.IsAlive)
                    {
                        secDriver.BlockPermanentEvents = false;
                        secDriver.KeepTasks = true;

                        if (!IsNauticalOrAerial)
                        {
                            Functions.AddPedToPursuit(Pursuit, secDriver);
                        }
                    }
                }

                Ped player = Game.LocalPlayer.Character;
                for (int i = 0; i < PassengerSetups.Count; i++)
                {
                    PassengerSetup ps = PassengerSetups[i];
                    Ped p = ps.PassengerPed;
                    if (p != null && p.Exists() && p.IsAlive)
                    {
                        p.BlockPermanentEvents = false;
                        p.KeepTasks = true;

                        if (!IsNauticalOrAerial)
                        {
                            Functions.AddPedToPursuit(Pursuit, p);
                        }
                        if (ConfigManager.EnableDriveByShooters)
                        {
                            PursuitConfigurator.ConfigureDriveByShooter(p, player, ps.Weapon, ps.Accuracy);
                        }
                    }
                }

                if (!IsNauticalOrAerial)
                {
                    Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    Functions.SetPursuitTacticsEnabled(Pursuit, true);

                    if (ThreatLevel >= 4)
                    {
                        Functions.SetPursuitLethalForceForced(Pursuit, true);
                    }

                    if (ConfigManager.AutoBackup)
                    {
                        DispatchHelper.RequestPursuitBackup(SpawnPoint, ThreatLevel >= 5, ThreatLevel >= 4);
                    }
                }

                PursuitCreated = true;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts: Error al iniciar persecucion: " + ex.Message);
            }
        }

        protected override void OnScenarioTick()
        {
            if (!PursuitCreated) return;

            Ped player = Game.LocalPlayer.Character;
            PursuitRealismManager.ProcessVehiclePursuitTick(SuspectVehicle, Driver, player);
            MonitorDisabledVehicleAndBailout();
        }

        private void MonitorDisabledVehicleAndBailout()
        {
            if (_bailoutTriggered) return;
            if (IsNauticalOrAerial) return;
            if (SuspectVehicle == null || !SuspectVehicle.Exists()) return;

            bool vehicleImmobilized = false;

            // 1. Vehiculo volcado, incendiado o con motor muerto
            if (SuspectVehicle.IsUpsideDown || !SuspectVehicle.IsDriveable || SuspectVehicle.Health <= 0 || SuspectVehicle.EngineHealth <= 0)
            {
                vehicleImmobilized = true;
            }
            else if (SuspectVehicle.Speed < 1.2f)
            {
                _vehicleStoppedTicks++;
                // A 450ms por tick, 8 ticks corresponden a ~3.6 segundos totalmente detenido
                if (_vehicleStoppedTicks >= 8)
                {
                    vehicleImmobilized = true;
                }
            }
            else
            {
                _vehicleStoppedTicks = 0;
            }

            if (vehicleImmobilized)
            {
                _bailoutTriggered = true;
                TriggerVehicleBailout();
            }
        }

        private void TriggerVehicleBailout()
        {
            Ped player = Game.LocalPlayer.Character;

            // Bailout del conductor si sigue dentro del vehiculo
            if (Driver != null && Driver.Exists() && Driver.IsAlive && !Functions.IsPedArrested(Driver))
            {
                PursuitConfigurator.EnableVehicleBailout(Driver);
                if (Driver.IsInAnyVehicle(false))
                {
                    Driver.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut | LeaveVehicleFlags.LeaveDoorOpen);
                }
                PursuitRealismManager.ExecuteTacticalBailout(Driver, SuspectVehicle, player, ThreatLevel >= 4);
                if (player != null && player.Exists())
                {
                    Driver.Tasks.FightAgainst(player);
                }
            }

            // Bailout de todos los pasajeros armados
            for (int i = 0; i < PassengerSetups.Count; i++)
            {
                Ped p = PassengerSetups[i].PassengerPed;
                if (p != null && p.Exists() && p.IsAlive && !Functions.IsPedArrested(p))
                {
                    PursuitConfigurator.EnableVehicleBailout(p);
                    if (p.IsInAnyVehicle(false))
                    {
                        p.Tasks.LeaveVehicle(LeaveVehicleFlags.BailOut | LeaveVehicleFlags.LeaveDoorOpen);
                    }
                    if (player != null && player.Exists())
                    {
                        p.Tasks.FightAgainst(player);
                    }
                }
            }
        }

        protected override void CheckCompletionCriteria()
        {
            if (!PursuitCreated) return;

            try
            {
                if (!IsNauticalOrAerial && Functions.IsPursuitStillRunning(Pursuit))
                {
                    return;
                }
            }
            catch { }

            // Verificacion de estado de sospechosos con rango amplio anti-desaparicion
            bool anySuspectActive = false;
            Ped player = Game.LocalPlayer.Character;
            Vector3 playerPos = (player != null && player.Exists()) ? player.Position : SpawnPoint;

            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    // Evitar despawn abrupto en persecucion en autopista (distancia amplia: 650m)
                    if (s.Position.DistanceTo(SpawnPoint) > 700f && s.Position.DistanceTo(playerPos) > 650f)
                    {
                        if (SuspectBlips.ContainsKey(s) && SuspectBlips[s] != null && SuspectBlips[s].Exists())
                        {
                            SuspectBlips[s].Delete();
                        }
                        s.Dismiss();
                        continue;
                    }
                    anySuspectActive = true;
                    break;
                }
            }

            if (!anySuspectActive)
            {
                NotifyCode4();
            }
        }

        public override void End()
        {
            try
            {
                if (PursuitCreated && !IsNauticalOrAerial && Functions.IsPursuitStillRunning(Pursuit))
                {
                    Functions.ForceEndPursuit(Pursuit);
                }
            }
            catch { }

            base.End();
            PassengerSetups.Clear();
            SecondaryDrivers.Clear();
            PursuitCreated = false;
            VehicleBombActive = false;
            _bailoutTriggered = false;
            _vehicleStoppedTicks = 0;
        }

        // ---------------- Helpers de Persecucion ----------------

        protected Vehicle CreatePursuitVehicle(string model, Vector3 pos, float heading, bool reinforced = false, float powerMultiplier = 1.0f)
        {
            SuspectVehicle = SpawnVehicle(model, pos, heading);
            if (SuspectVehicle != null)
            {
                PursuitConfigurator.ReinforceVehicle(SuspectVehicle, reinforced, powerMultiplier);
            }
            return SuspectVehicle;
        }

        protected Ped CreatePursuitDriver(string model, Vehicle vehicle, WeaponHash weapon)
        {
            if (vehicle == null || !vehicle.Exists()) return null;

            Ped driver = SpawnPed(model, vehicle.Position, vehicle.Heading);
            if (driver != null)
            {
                NativeFunction.Natives.SET_PED_INTO_VEHICLE(driver, vehicle, -1);
                driver.Inventory.Weapons.Add(weapon).Ammo = -1;
                driver.BlockPermanentEvents = false;
                driver.KeepTasks = true;
                ConfigurePedCombat(driver, ConfigManager.SuspectAccuracy, 50, false);
                AttachBlipToSuspect(driver, Color.Red, 0.65f);
                Suspects.Add(driver);
                Driver = driver;
            }
            return Driver;
        }

        protected Ped CreatePursuitPassenger(string model, Vehicle vehicle, int seatIndex, WeaponHash weapon, int armor = 50)
        {
            if (vehicle == null || !vehicle.Exists()) return null;

            Ped p = SpawnPed(model, vehicle.Position, vehicle.Heading);
            if (p != null)
            {
                NativeFunction.Natives.SET_PED_INTO_VEHICLE(p, vehicle, seatIndex);
                p.Inventory.Weapons.Add(weapon).Ammo = -1;
                p.BlockPermanentEvents = false;
                p.KeepTasks = true;
                ConfigurePedCombat(p, ConfigManager.SuspectAccuracy, armor, false);
                AttachBlipToSuspect(p, Color.DarkRed, 0.60f);

                PassengerSetup setup = new PassengerSetup();
                setup.PassengerPed = p;
                setup.Weapon = weapon;
                setup.Accuracy = ConfigManager.SuspectAccuracy;
                PassengerSetups.Add(setup);

                Suspects.Add(p);
            }
            return p;
        }

        protected void StartVehicleBombCountdown(int seconds)
        {
            VehicleBombActive = true;
            VehicleBombSecondsRemaining = seconds;

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                while (VehicleBombActive && VehicleBombSecondsRemaining > 0)
                {
                    if (SuspectVehicle == null || !SuspectVehicle.Exists()) break;

                    bool driverNeutralized = (Driver == null || !Driver.Exists() || !Driver.IsAlive || Functions.IsPedArrested(Driver));
                    if (driverNeutralized)
                    {
                        VehicleBombActive = false;
                        Game.DisplaySubtitle("~g~[PERSECUCION] ¡Vehiculo bomba inmovilizado y asegurado!");
                        break;
                    }

                    Game.DisplayHelp(string.Format("~r~VEHICULO BOMBA EN HUIDA: ~w~{0}s antes de detonacion urbana. ¡Deten el vehiculo!", VehicleBombSecondsRemaining));
                    GameFiber.Sleep(1000);
                    VehicleBombSecondsRemaining--;
                }

                if (VehicleBombActive && VehicleBombSecondsRemaining <= 0)
                {
                    if (SuspectVehicle != null && SuspectVehicle.Exists())
                    {
                        TriggerExplosion(SuspectVehicle.Position, 7, 10.0f);
                        Game.DisplaySubtitle("~r~[PERSECUCION] ¡El vehiculo bomba ha detonado!");
                    }
                    VehicleBombActive = false;
                }
            });
            Cleaner.RegisterFiber(fiber);
        }

        protected void DropCaltropsBehindVehicle()
        {
            if (SuspectVehicle == null || !SuspectVehicle.Exists()) return;

            Vector3 dropPos = SuspectVehicle.GetOffsetPosition(new Vector3(0f, -4.5f, -0.5f));
            // Usar prop de pinchos/stinger en lugar de barrera de hormigon para evitar colision
            Rage.Object caltrop = SpawnProp("p_ld_stinger_s", dropPos, SuspectVehicle.Heading);

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                for (int i = 0; i < 30; i++)
                {
                    Ped player = Game.LocalPlayer.Character;
                    if (player != null && player.Exists() && player.IsInAnyVehicle(false))
                    {
                        Vehicle playerVeh = player.CurrentVehicle;
                        if (playerVeh != null && playerVeh.Exists() && playerVeh.Position.DistanceTo(dropPos) < 2.8f)
                        {
                            NativeFunction.Natives.SET_VEHICLE_TYRE_BURST(playerVeh, 0, true, 1000.0f);
                            NativeFunction.Natives.SET_VEHICLE_TYRE_BURST(playerVeh, 1, true, 1000.0f);
                            Game.DisplaySubtitle("~r~¡Pinchaste con abrojos/caltrops en la calzada!");
                            break;
                        }
                    }
                    GameFiber.Sleep(200);
                }
            });
            Cleaner.RegisterFiber(fiber);
        }
    }
}
