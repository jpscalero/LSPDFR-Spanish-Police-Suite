using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using PoliceLifeSystem.Core;
using PoliceSuite.Common;

namespace PoliceLifeSystem.Career
{
    /// <summary>
    /// Programa homenajes periódicos de carrera en Mission Row y conserva la
    /// invitación pendiente hasta que el agente active la zona de celebración.
    /// </summary>
    public static class CelebrationManager
    {
        private const int MinimumOfficers = 5;
        private const float SpawnDistance = 75.0f;
        private const float CleanupDistance = 180.0f;
        private const float ActivationRadius = 2.5f;
        private const float MarkerDrawDistance = 55.0f;
        private const string OfficerModel = "s_m_y_cop_01";
        private const string CheerScenario = "WORLD_HUMAN_CHEERING";

        private static readonly Vector3 StationCenter = new Vector3(425.1f, -979.5f, 30.7f);
        private static readonly Vector3[] OfficerSpawns = new Vector3[]
        {
            new Vector3(419.0f, -980.5f, 30.7f),
            new Vector3(421.5f, -984.5f, 30.7f),
            new Vector3(425.5f, -985.5f, 30.7f),
            new Vector3(429.2f, -983.3f, 30.7f),
            new Vector3(431.0f, -979.5f, 30.7f)
        };

        private static readonly List<Ped> Officers = new List<Ped>();
        private static bool _running;
        private static bool _announcementShownThisSession;
        private static bool _activationKeyWasDown;
        private static bool _celebrationInProgress;
        private static uint _lastSpawnAttempt;
        private static uint _lastPromptTime;

        public static DateTime LastCelebrationDate { get; set; }
        public static bool IsPending { get; set; }

        static CelebrationManager()
        {
            LastCelebrationDate = DateTime.MinValue;
            IsPending = false;
        }

        public static void ResetPersistentState()
        {
            LastCelebrationDate = DateTime.MinValue;
            IsPending = false;
        }

        public static void Start()
        {
            if (_running) return;

            _running = true;
            GameFiber.StartNew(delegate
            {
                try
                {
                    if (LastCelebrationDate == DateTime.MinValue)
                    {
                        LastCelebrationDate = World.DateTime;
                        SaveManager.Save();
                    }

                    if (IsPending) AnnounceInvitation();

                    while (_running)
                    {
                        try
                        {
                            DateTime gameDate = World.DateTime;
                            if (!IsPending && CelebrationScheduleRules.IsDue(
                                LastCelebrationDate,
                                gameDate,
                                ConfigManager.CelebrationIntervalDays))
                            {
                                IsPending = true;
                                SaveManager.Save();
                                AnnounceInvitation();
                            }

                            if (IsPending) ProcessPendingInvitation();
                        }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("PoliceLifeSystem.CelebrationManager loop error: " + ex.Message);
                        }

                        GameFiber.Sleep(0);
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("PoliceLifeSystem.CelebrationManager fatal error: " + ex.Message);
                }
                finally
                {
                    try { CleanupOfficers(); }
                    catch (Exception ex) { Game.LogTrivial("Celebration cleanup error: " + ex.Message); }
                    _running = false;
                }
            }, "PoliceLifeSystem Career Celebration");
        }

        public static void Stop()
        {
            _running = false;
        }

        private static void AnnounceInvitation()
        {
            if (_announcementShownThisSession) return;
            _announcementShownThisSession = true;

            const string radioMessage = "Sala 091 a todas las unidades: la Comisaría Central espera al agente de servicio para celebrar su esfuerzo y el trabajo realizado. Le esperan al menos cinco compañeros.";
            const string notificationMessage = "La Comisaría Central de Mission Row te espera para celebrar tu esfuerzo. Acércate a la zona azul cuando quieras comenzar el homenaje.";

            try
            {
                Game.DisplayNotification(
                    "3dtextures",
                    "mpgroundlogo_cops",
                    "~b~COMISARÍA CENTRAL",
                    "~y~Reconocimiento al servicio",
                    notificationMessage
                );
                Game.DisplaySubtitle("~b~[SALA 091 / CENTRAL]~w~ " + radioMessage, 7000);
                SpanishVoiceDispatchEngine.SpeakDispatch(radioMessage);
                Game.LogTrivial("PoliceLifeSystem: Invitación al homenaje emitida por radio y notificación.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error anunciando el homenaje: " + ex.Message);
            }
        }

        private static void ProcessPendingInvitation()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                GameFiber.Sleep(500);
                return;
            }

            float distance = player.Position.DistanceTo(StationCenter);
            if (distance > CleanupDistance)
            {
                CleanupOfficers();
                _activationKeyWasDown = false;
                return;
            }

            if (distance <= SpawnDistance) EnsureOfficers();
            if (distance > MarkerDrawDistance || CountActiveOfficers() < MinimumOfficers)
            {
                _activationKeyWasDown = false;
                return;
            }

            DrawCelebrationZone();
            bool insideZone = distance <= ActivationRadius;
            bool keyDown = Game.IsKeyDown(Keys.E);

            if (insideZone)
            {
                if (Game.GameTime - _lastPromptTime >= 1800)
                {
                    Game.DisplaySubtitle("~b~HOMENAJE POLICIAL~w~ Pulsa ~y~E~w~ para celebrar con tus compañeros.", 1800);
                    _lastPromptTime = Game.GameTime;
                }

                if (keyDown && !_activationKeyWasDown)
                {
                    if (NativeFunction.Natives.IS_PED_IN_ANY_VEHICLE<bool>(player, false))
                    {
                        Game.DisplayNotification("~y~Bájate del vehículo para iniciar el homenaje.");
                    }
                    else
                    {
                        BeginCelebration(player);
                    }
                }
            }

            _activationKeyWasDown = keyDown;
        }

        private static void EnsureOfficers()
        {
            if (CountActiveOfficers() >= MinimumOfficers) return;
            if (Game.GameTime - _lastSpawnAttempt < 10000) return;

            _lastSpawnAttempt = Game.GameTime;
            CleanupOfficers();

            Model officerModel = new Model(OfficerModel);
            officerModel.LoadAndWait();

            for (int i = 0; i < OfficerSpawns.Length; i++)
            {
                try
                {
                    Ped officer = new Ped(officerModel, OfficerSpawns[i], 180.0f);
                    if (officer == null || !officer.Exists()) continue;

                    officer.IsPersistent = true;
                    officer.BlockPermanentEvents = true;
                    NativeFunction.Natives.SET_ENTITY_AS_MISSION_ENTITY(officer, true, true);
                    NativeFunction.Natives.SET_ENTITY_INVINCIBLE(officer, true);
                    NativeFunction.Natives.SET_PED_AS_COP(officer, true);
                    NativeFunction.Natives.TASK_STAND_STILL(officer, -1);
                    Officers.Add(officer);
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PoliceLifeSystem: No se pudo crear un agente para el homenaje: " + ex.Message);
                }
            }

            if (CountActiveOfficers() < MinimumOfficers)
            {
                CleanupOfficers();
                Game.LogTrivial("PoliceLifeSystem: El homenaje espera hasta poder reunir cinco agentes.");
            }
        }

        private static int CountActiveOfficers()
        {
            int count = 0;
            for (int i = Officers.Count - 1; i >= 0; i--)
            {
                Ped officer = Officers[i];
                if (officer != null && officer.Exists() && officer.IsAlive)
                {
                    count++;
                }
                else
                {
                    if (officer != null && officer.Exists())
                    {
                        try { officer.Delete(); }
                        catch { }
                    }
                    Officers.RemoveAt(i);
                }
            }
            return count;
        }

        private static void DrawCelebrationZone()
        {
            NativeFunction.Natives.DRAW_MARKER(
                1,
                StationCenter.X, StationCenter.Y, StationCenter.Z - 1.0f,
                0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f,
                3.2f, 3.2f, 0.8f,
                0, 125, 255, 155,
                false, false, 2, false, null, null, false
            );
        }

        private static void BeginCelebration(Ped player)
        {
            if (_celebrationInProgress || CountActiveOfficers() < MinimumOfficers) return;
            _celebrationInProgress = true;
            int camera = -1;
            bool cameraRendered = false;
            bool screenFadedOut = false;
            bool celebrationCompleted = false;

            try
            {
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~HOMENAJE POLICIAL", "~y~Comisaría Central", "Tus compañeros celebran el trabajo realizado.");

                for (int i = 0; i < Officers.Count; i++)
                {
                    Ped officer = Officers[i];
                    if (officer != null && officer.Exists() && officer.IsAlive)
                    {
                        NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(officer, CheerScenario, 0, true);
                    }
                }
                NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(player, CheerScenario, 0, true);

                NativeFunction.Natives.DO_SCREEN_FADE_OUT(600);
                screenFadedOut = true;
                GameFiber.Sleep(650);

                camera = NativeFunction.Natives.CREATE_CAM<int>("DEFAULT_SCRIPTED_CAMERA", true);
                if (camera < 0 || !NativeFunction.Natives.DOES_CAM_EXIST<bool>(camera))
                {
                    throw new InvalidOperationException("GTA no creó la cámara de la escena.");
                }

                NativeFunction.Natives.SET_CAM_FOV(camera, 48.0f);
                NativeFunction.Natives.SET_CAM_COORD(camera, StationCenter.X + 9.0f, StationCenter.Y, StationCenter.Z + 4.2f);
                NativeFunction.Natives.POINT_CAM_AT_COORD(camera, StationCenter.X, StationCenter.Y, StationCenter.Z + 1.2f);
                NativeFunction.Natives.SET_CAM_ACTIVE(camera, true);
                NativeFunction.Natives.RENDER_SCRIPT_CAMS(true, true, 900, true, false);
                cameraRendered = true;
                NativeFunction.Natives.DO_SCREEN_FADE_IN(700);
                screenFadedOut = false;

                const int cinematicFrames = 72;
                for (int frame = 0; frame < cinematicFrames && _running && player.Exists() && player.IsAlive; frame++)
                {
                    double angle = (Math.PI * 2.0 * frame) / cinematicFrames;
                    float cameraX = StationCenter.X + (float)Math.Cos(angle) * 9.0f;
                    float cameraY = StationCenter.Y + (float)Math.Sin(angle) * 9.0f;
                    NativeFunction.Natives.SET_CAM_COORD(camera, cameraX, cameraY, StationCenter.Z + 4.2f);
                    NativeFunction.Natives.POINT_CAM_AT_COORD(camera, StationCenter.X, StationCenter.Y, StationCenter.Z + 1.2f);
                    GameFiber.Sleep(100);
                }

                celebrationCompleted = _running && player.Exists() && player.IsAlive;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error en la escena de homenaje: " + ex.Message);
                Game.DisplayNotification("~r~No se pudo completar la escena. El homenaje seguirá pendiente.");
            }
            finally
            {
                try
                {
                    if (cameraRendered)
                    {
                        NativeFunction.Natives.DO_SCREEN_FADE_OUT(350);
                        GameFiber.Sleep(400);
                        NativeFunction.Natives.RENDER_SCRIPT_CAMS(false, true, 500, true, false);
                    }
                    if (camera >= 0 && NativeFunction.Natives.DOES_CAM_EXIST<bool>(camera))
                    {
                        NativeFunction.Natives.SET_CAM_ACTIVE(camera, false);
                        NativeFunction.Natives.DESTROY_CAM(camera, false);
                    }
                    if (player != null && player.Exists())
                    {
                        NativeFunction.Natives.CLEAR_PED_TASKS(player);
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PoliceLifeSystem: Error restaurando la cámara del homenaje: " + ex.Message);
                }
                finally
                {
                    if (screenFadedOut || cameraRendered)
                    {
                        try { NativeFunction.Natives.DO_SCREEN_FADE_IN(500); }
                        catch { }
                    }
                    CleanupOfficers();
                    _celebrationInProgress = false;
                }
            }

            if (celebrationCompleted)
            {
                LastCelebrationDate = World.DateTime;
                IsPending = false;
                _announcementShownThisSession = false;
                SaveManager.Save();
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~g~HOMENAJE COMPLETADO", "~b~Gracias por tu servicio", "La próxima celebración llegará según el intervalo configurado.");
                Game.LogTrivial("PoliceLifeSystem: Homenaje completado en Comisaría Central.");
            }
        }

        private static void CleanupOfficers()
        {
            for (int i = Officers.Count - 1; i >= 0; i--)
            {
                try
                {
                    Ped officer = Officers[i];
                    if (officer != null && officer.Exists())
                    {
                        NativeFunction.Natives.CLEAR_PED_TASKS(officer);
                        NativeFunction.Natives.SET_ENTITY_INVINCIBLE(officer, false);
                        officer.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PoliceLifeSystem: Error eliminando agente del homenaje: " + ex.Message);
                }
            }
            Officers.Clear();
        }
    }
}
