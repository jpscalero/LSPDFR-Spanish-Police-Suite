using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using PoliceSuite.Common;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Services
{
    public enum EAmbientServiceType
    {
        PolicePatrol,
        SamurAmbulance,
        HearseJudicial,
        FireBrigade,
        MunicipalTow
    }

    /// <summary>
    /// Sistema integral de trafico ambiental en respuesta de emergencia y servicios publicos.
    /// Simula de forma continua el paso de coches patrulla en Codigo 3, ambulancias SAMUR/061,
    /// coches funebres del tanatorio/forense, camiones de bomberos y gruas de trafico municipal,
    /// con locuciones de radio 100% en espanol y erradicacion total de blips atascados o fugas de memoria.
    /// </summary>
    public static class AmbientEmergencyPatrolSystem
    {
        private static bool _isRunning = false;
        private static DateTime _lastServiceTime = DateTime.MinValue;
        private static readonly Random _rnd = new Random();

        private static readonly string[] _policeModels = new string[] { "police", "police2", "police3", "sheriff", "sheriff2" };
        private static readonly string[] _ambulanceModels = new string[] { "ambulance" };
        private static readonly string[] _hearseModels = new string[] { "romero", "policet", "speedo" };
        private static readonly string[] _fireModels = new string[] { "firetruk" };
        private static readonly string[] _towModels = new string[] { "flatbed", "towtruck", "towtruck2" };

        public static void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            _lastServiceTime = DateTime.Now.AddSeconds(12); // Primer despliegue rapido al patrullar

            GameFiber.StartNew(delegate
            {
                try
                {
                    Game.LogTrivial("SPI: AmbientEmergencyPatrolSystem (Policia, SAMUR, Funebres, Bomberos, Gruas) iniciado.");
                    Loop();
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("AmbientEmergencyPatrolSystem.Start error: " + ex.Message);
                }
            });
        }

        private static void Loop()
        {
            while (_isRunning)
            {
                GameFiber.Sleep(5000);

                try
                {
                    if (!ConfigManager.AmbientServicesEnabled) continue;

                    Ped player = Game.LocalPlayer.Character;
                    if (player == null || !player.Exists() || !player.IsAlive) continue;

                    int minSec = ConfigManager.AmbientIntervalMin > 10 ? ConfigManager.AmbientIntervalMin : 45;
                    int maxSec = ConfigManager.AmbientIntervalMax >= minSec ? ConfigManager.AmbientIntervalMax : 75;
                    int targetInterval = _rnd.Next(minSec, maxSec + 1);

                    double elapsed = (DateTime.Now - _lastServiceTime).TotalSeconds;
                    if (elapsed < targetInterval) continue;

                    _lastServiceTime = DateTime.Now;
                    EAmbientServiceType serviceType = SelectNextServiceType();
                    SpawnAmbientService(player, serviceType);
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("AmbientEmergencyPatrolSystem.Loop cycle error: " + ex.Message);
                }
            }
        }

        private static EAmbientServiceType SelectNextServiceType()
        {
            List<EAmbientServiceType> allowed = new List<EAmbientServiceType>();

            if (ConfigManager.AmbientPoliceEnabled) allowed.Add(EAmbientServiceType.PolicePatrol);
            if (ConfigManager.AmbientAmbulanceEnabled) allowed.Add(EAmbientServiceType.SamurAmbulance);
            if (ConfigManager.AmbientHearseEnabled) allowed.Add(EAmbientServiceType.HearseJudicial);
            if (ConfigManager.AmbientFireEnabled) allowed.Add(EAmbientServiceType.FireBrigade);
            if (ConfigManager.AmbientTowEnabled) allowed.Add(EAmbientServiceType.MunicipalTow);

            if (allowed.Count == 0) return EAmbientServiceType.PolicePatrol;

            // Seleccion ponderada de realismo
            int roll = _rnd.Next(100);
            if (roll < 35 && allowed.Contains(EAmbientServiceType.PolicePatrol))
                return EAmbientServiceType.PolicePatrol;
            if (roll < 60 && allowed.Contains(EAmbientServiceType.SamurAmbulance))
                return EAmbientServiceType.SamurAmbulance;
            if (roll < 75 && allowed.Contains(EAmbientServiceType.HearseJudicial))
                return EAmbientServiceType.HearseJudicial;
            if (roll < 90 && allowed.Contains(EAmbientServiceType.FireBrigade))
                return EAmbientServiceType.FireBrigade;
            if (allowed.Contains(EAmbientServiceType.MunicipalTow))
                return EAmbientServiceType.MunicipalTow;

            return allowed[_rnd.Next(allowed.Count)];
        }

        private static void SpawnAmbientService(Ped player, EAmbientServiceType type)
        {
            GameFiber.StartNew(delegate
            {
                Vehicle veh = null;
                Ped driver = null;
                Ped passenger = null;
                Blip blip = null;

                try
                {
                    Vector3 playerPos = player.Position;
                    Vector3 forward = player.ForwardVector;
                    float lateralOffset = (float)(_rnd.NextDouble() * 60 - 30);

                    // Buscar posicion en carretera a unos 115m del jugador
                    Vector3 spawnCandidate = playerPos - (forward * 115f) + (player.RightVector * lateralOffset);
                    Vector3 spawnPos = World.GetNextPositionOnStreet(spawnCandidate);

                    // Destino hacia adelante atravesando el area en la via (380m)
                    Vector3 targetCandidate = playerPos + (forward * 390f) + (player.RightVector * lateralOffset);
                    Vector3 targetPos = World.GetNextPositionOnStreet(targetCandidate);

                    if (spawnPos.DistanceTo(playerPos) < 40f || spawnPos.DistanceTo(targetPos) < 90f) return;

                    string modelName = GetModelForService(type);
                    veh = new Vehicle(modelName, spawnPos);
                    if (veh == null || !veh.Exists()) return;

                    veh.IsPersistent = true;

                    // Configurar comportamiento, sirenas y tripulacion segun servicio
                    string driverPedModel = "s_m_y_cop_01";
                    string passengerPedModel = null;
                    float cruiseSpeed = 34.0f;
                    int drivingStyle = 786603; // Codigo 3 evasivo / prioridad maxima
                    Color blipColor = Color.FromArgb(0, 100, 255);
                    string blipName = "Patrulla ZETA";
                    string subtitleText = "";

                    switch (type)
                    {
                        case EAmbientServiceType.PolicePatrol:
                            veh.IsSirenOn = true;
                            veh.IsSirenSilent = false;
                            driverPedModel = _rnd.Next(2) == 0 ? "s_m_y_cop_01" : "s_m_y_sheriff_01";
                            passengerPedModel = driverPedModel;
                            cruiseSpeed = 34.0f;
                            drivingStyle = 786603;
                            blipColor = Color.FromArgb(0, 100, 255);
                            blipName = "Patrulla Policial (Codigo 3)";
                            subtitleText = "~b~[SALA 091]~w~ Patrulla ZETA respondiendo en Codigo 3 a aviso de seguridad ciudadana.";
                            SpanishVoiceDispatchEngine.AnnounceAmbientPolicePatrol();
                            break;

                        case EAmbientServiceType.SamurAmbulance:
                            veh.IsSirenOn = true;
                            veh.IsSirenSilent = false;
                            driverPedModel = "s_m_m_paramedic_01";
                            passengerPedModel = "s_m_m_paramedic_01";
                            cruiseSpeed = 32.0f;
                            drivingStyle = 786603;
                            blipColor = Color.FromArgb(255, 140, 0); // Naranja sanitario
                            blipName = "Ambulancia SAMUR / 061";
                            subtitleText = "~g~[SAMUR 061]~w~ Soporte Vital Avanzado acudiendo en Codigo 3 por emergencia medica grave.";
                            SpanishVoiceDispatchEngine.AnnounceAmbientAmbulance();
                            break;

                        case EAmbientServiceType.HearseJudicial:
                            veh.IsSirenOn = false;
                            veh.IsSirenSilent = true;
                            if (modelName.Equals("romero", StringComparison.OrdinalIgnoreCase))
                            {
                                NativeFunction.Natives.SET_VEHICLE_COLOURS(veh, 0, 0); // Pintura negra clasica solemne
                            }
                            driverPedModel = "s_m_m_doctor_01";
                            passengerPedModel = null;
                            cruiseSpeed = 16.0f;
                            drivingStyle = 786468; // Trafico normal solemne respetando carril
                            blipColor = Color.FromArgb(148, 0, 211); // Purpura forense/funerario
                            blipName = "Coche Funebre / Forense Judicial";
                            subtitleText = "~p~[SERVICIOS FUNEBRES]~w~ Furgon judicial en comitiva solemne hacia dependencias del tanatorio.";
                            SpanishVoiceDispatchEngine.AnnounceAmbientHearse();
                            break;

                        case EAmbientServiceType.FireBrigade:
                            veh.IsSirenOn = true;
                            veh.IsSirenSilent = false;
                            driverPedModel = "s_m_y_fireman_01";
                            passengerPedModel = "s_m_y_fireman_01";
                            cruiseSpeed = 28.0f;
                            drivingStyle = 786603;
                            blipColor = Color.FromArgb(255, 45, 0); // Rojo bomberos
                            blipName = "Bomba Urbana Bomberos";
                            subtitleText = "~r~[BOMBEROS]~w~ Bomba Urbana Pesada acudiendo en Codigo 3 con sirena activa.";
                            SpanishVoiceDispatchEngine.AnnounceAmbientFireTruck();
                            break;

                        case EAmbientServiceType.MunicipalTow:
                            veh.IsSirenOn = false;
                            driverPedModel = "s_m_y_autopep_01";
                            passengerPedModel = null;
                            cruiseSpeed = 18.0f;
                            drivingStyle = 786468;
                            blipColor = Color.FromArgb(255, 204, 0); // Amarillo grua
                            blipName = "Grua de Trafico Municipal";
                            subtitleText = "~y~[GRUA MUNICIPAL]~w~ Asistencia vial municipal en ruta para retirada y despeje de calzada.";
                            SpanishVoiceDispatchEngine.AnnounceAmbientTowTruck();
                            break;
                    }

                    // Creacion y asiento del conductor
                    driver = new Ped(driverPedModel, spawnPos, 0f);
                    if (driver != null && driver.Exists())
                    {
                        driver.IsPersistent = true;
                        driver.WarpIntoVehicle(veh, -1);
                        if (type == EAmbientServiceType.PolicePatrol)
                        {
                            driver.RelationshipGroup = "COP";
                            NativeFunction.Natives.SET_PED_AS_COP(driver, true);
                        }

                        NativeFunction.Natives.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE(
                            driver, veh, targetPos.X, targetPos.Y, targetPos.Z, cruiseSpeed, drivingStyle, 15.0f
                        );
                    }

                    // Creacion de acompanante si aplica
                    if (!string.IsNullOrEmpty(passengerPedModel))
                    {
                        passenger = new Ped(passengerPedModel, spawnPos, 0f);
                        if (passenger != null && passenger.Exists())
                        {
                            passenger.IsPersistent = true;
                            passenger.WarpIntoVehicle(veh, 0);
                            if (type == EAmbientServiceType.PolicePatrol)
                            {
                                passenger.RelationshipGroup = "COP";
                                NativeFunction.Natives.SET_PED_AS_COP(passenger, true);
                            }
                        }
                    }

                    // Blip tactico representativo en el mapa
                    try
                    {
                        blip = veh.AttachBlip();
                        if (blip != null && blip.Exists())
                        {
                            blip.Color = blipColor;
                            blip.Scale = 0.65f;
                            blip.Name = blipName;
                        }
                    }
                    catch (Exception exB)
                    {
                        Game.LogTrivial("AmbientEmergencyPatrolSystem: Error asignando blip: " + exB.Message);
                    }

                    if (!string.IsNullOrEmpty(subtitleText))
                    {
                        Game.DisplaySubtitle(subtitleText, 4500);
                    }

                    // Bucle de monitorizacion y autodismiss
                    uint startTime = Game.GameTime;
                    while (veh != null && veh.Exists() && (Game.GameTime - startTime < 45000))
                    {
                        GameFiber.Sleep(2000);

                        if (player != null && player.Exists() && veh.Position.DistanceTo(player.Position) > 195f && (Game.GameTime - startTime > 12000))
                        {
                            break;
                        }

                        if (veh.Position.DistanceTo(targetPos) < 25f)
                        {
                            break;
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("AmbientEmergencyPatrolSystem.SpawnAmbientService error: " + ex.Message);
                }
                finally
                {
                    // Erradicacion garantizada de blips y entidades huerfanas
                    try
                    {
                        if (blip != null && blip.Exists())
                        {
                            blip.DisableRoute();
                            blip.Delete();
                            blip = null;
                        }
                    }
                    catch { }

                    try
                    {
                        if (driver != null && driver.Exists())
                        {
                            driver.BlockPermanentEvents = false;
                            driver.IsPersistent = false;
                            driver.Dismiss();
                        }
                    }
                    catch { }

                    try
                    {
                        if (passenger != null && passenger.Exists())
                        {
                            passenger.BlockPermanentEvents = false;
                            passenger.IsPersistent = false;
                            passenger.Dismiss();
                        }
                    }
                    catch { }

                    try
                    {
                        if (veh != null && veh.Exists())
                        {
                            veh.IsPersistent = false;
                            veh.Dismiss();
                        }
                    }
                    catch { }
                }
            });
        }

        private static string GetModelForService(EAmbientServiceType type)
        {
            switch (type)
            {
                case EAmbientServiceType.PolicePatrol:
                    return _policeModels[_rnd.Next(_policeModels.Length)];
                case EAmbientServiceType.SamurAmbulance:
                    return _ambulanceModels[_rnd.Next(_ambulanceModels.Length)];
                case EAmbientServiceType.HearseJudicial:
                    return _hearseModels[_rnd.Next(_hearseModels.Length)];
                case EAmbientServiceType.FireBrigade:
                    return _fireModels[_rnd.Next(_fireModels.Length)];
                case EAmbientServiceType.MunicipalTow:
                    return _towModels[_rnd.Next(_towModels.Length)];
                default:
                    return "police";
            }
        }
    }
}
