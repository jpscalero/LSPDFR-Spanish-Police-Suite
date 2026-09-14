using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using Rage;
using Rage.Native;

namespace PoliceSuite.Common
{
    /// <summary>
    /// Motor de locucion y despacho 100% en Espanol para la Suite Policial Espanola.
    /// Emite transmisiones autenticas de Sala 091 / SIRDEE y ordenes de viva voz de los agentes
    /// utilizando sintesis de voz nativa en espanol de Windows con microfonia PTT de radiopatrulla.
    /// </summary>
    public static class SpanishVoiceDispatchEngine
    {
        private static SpeechSynthesizer _synth;
        private static bool _initialized = false;
        private static bool _hasSpanishVoice = false;
        private static readonly object _lock = new object();

        public static void Initialize()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                try
                {
                    _synth = new SpeechSynthesizer();
                    _synth.SetOutputToDefaultAudioDevice();
                    _synth.Volume = 100;
                    _synth.Rate = 1; // Ritmo agil policial

                    // Busqueda de voz en espanol instalada en Windows
                    foreach (InstalledVoice voice in _synth.GetInstalledVoices())
                    {
                        if (!voice.Enabled) continue;

                        string culture = voice.VoiceInfo.Culture.Name.ToLower();
                        string name = voice.VoiceInfo.Name.ToLower();

                        if (culture.StartsWith("es") || name.Contains("helena") || name.Contains("pablo") || name.Contains("laura") || name.Contains("spanish"))
                        {
                            _synth.SelectVoice(voice.VoiceInfo.Name);
                            _hasSpanishVoice = true;
                            Game.LogTrivial("SpanishVoiceDispatchEngine: Voz en espanol seleccionada -> " + voice.VoiceInfo.Name);
                            break;
                        }
                    }

                    if (!_hasSpanishVoice)
                    {
                        Game.LogTrivial("SpanishVoiceDispatchEngine: No se encontro voz es-ES especifica. Usando voz por defecto.");
                    }

                    _initialized = true;
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("SpanishVoiceDispatchEngine.Initialize error: " + ex.Message);
                }
            }
        }

        public static void SpeakDispatch(string mensaje)
        {
            if (string.IsNullOrEmpty(mensaje)) return;

            GameFiber.StartNew(delegate
            {
                try
                {
                    Initialize();
                    if (_synth == null) return;

                    // Tono de radio SIRDEE / 091
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                    GameFiber.Sleep(150);

                    lock (_lock)
                    {
                        _synth.Rate = 0; // Despacho claro y oficial
                        _synth.SpeakAsync(mensaje);
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SpanishVoiceDispatchEngine.SpeakDispatch error: " + ex.Message);
                }
            });
        }

        public static void SpeakOfficer(string orden)
        {
            if (string.IsNullOrEmpty(orden)) return;

            GameFiber.StartNew(delegate
            {
                try
                {
                    Initialize();
                    if (_synth == null) return;

                    // Chirrido PTT de apertura de microfonia
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Event_Start_Text", "GTAO_FM_Events_Soundset", false);
                    GameFiber.Sleep(100);

                    lock (_lock)
                    {
                        _synth.Rate = 1; // Orden viva y contundente
                        _synth.SpeakAsync(orden);
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SpanishVoiceDispatchEngine.SpeakOfficer error: " + ex.Message);
                }
            });
        }

        // ==========================================
        // LOCUCIONES TECNICAS ESTANDARIZADAS EN ESPANOL
        // ==========================================

        public static void AnnounceTrafficStop()
        {
            SpeakOfficer("Policia Nacional. Apague el motor del vehiculo y baje la ventanilla despacio.");
        }

        public static void AnnounceOrderDriverOut()
        {
            SpeakOfficer("Salga del vehiculo con las manos a la vista y sin hacer movimientos bruscos.");
        }

        public static void AnnounceArrest()
        {
            SpeakOfficer("Queda usted detenido por un presunto delito. Ponga las manos en la espalda.");
        }

        public static void AnnounceFrisk()
        {
            SpeakOfficer("Procediendo al cacheo superficial de seguridad por prevencion. Permanezca inmovil.");
        }

        public static void AnnounceAlcoholTest()
        {
            SpeakOfficer("Sople de forma constante en el etilometro Drager hasta que cese la senal acustica.");
        }

        public static void AnnounceDrugTest()
        {
            SpeakOfficer("Abra la boca para la toma de muestra de saliva del control de sustancias.");
        }

        public static void AnnounceRights()
        {
            SpeakOfficer("Tiene derecho a guardar silencio, a no declarar contra si mismo y a solicitar asistencia letrada.");
        }

        public static void AnnounceEscort()
        {
            SpeakOfficer("Acompaneme hacia el vehiculo policial despacio.");
        }

        public static void AnnounceSitCurb()
        {
            SpeakOfficer("Tome asiento en el bordillo y mantenga la calma.");
        }

        public static void AnnounceKneel()
        {
            SpeakOfficer("Coloquese de rodillas con las manos en la nuca inmediatamente.");
        }

        public static void AnnounceBackupRequested(string unitName)
        {
            SpeakDispatch("Sala 091 a indicativos en sector: requerida presencia de unidad " + unitName + " en Codigo 3.");
        }

        public static void AnnounceCode4()
        {
            SpeakDispatch("Sala 091 a todas las unidades: situacion asegurada y bajo control. Codigo 4.");
        }

        public static void AnnouncePanicAlert()
        {
            SpeakDispatch("Alerta Roja 10-99. Agente en peligro extremo. Todas las patrullas acudan de urgencia en Codigo 3.");
        }

        public static void AnnounceSpikesDeployed()
        {
            SpeakOfficer("Atencion patrullas, banda de pinchos desplegada en la calzada.");
        }

        public static void AnnounceRoadblockDeployed()
        {
            SpeakDispatch("Corte de via tactico establecido con barrera policial y unidades GEO.");
        }

        public static void AnnounceAmbientEmergencyResponse()
        {
            SpeakDispatch("Sala 091 a patrulla en servicio: acudan de urgencia en Codigo 3 a requerimiento ciudadano.");
        }

        public static void AnnounceDniCheck(string dni, string nombre)
        {
            SpeakDispatch("Sala 091 verificando antecedentes en SITEL para " + nombre + " con documento " + dni);
        }

        public static void AnnouncePlateCheck(string matricula, string modelo)
        {
            SpeakDispatch("Sala 091 consultando base de datos DGT para el vehiculo " + modelo + " matricula " + matricula);
        }

        public static void AnnounceCourtDecision(string resolucion)
        {
            SpeakDispatch("Resolucion judicial notificada por el Juzgado de Guardia: " + resolucion);
        }

        public static void AnnounceCalloutDispatched(string calloutName)
        {
            SpeakDispatch("Sala 091 a todas las unidades. Aviso prioritario: " + calloutName + ". Acudan con precaucion.");
        }
    }
}
