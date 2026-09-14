using System;
using System.Collections.Generic;
using Rage;
using RadioYJusticia.Core;

namespace RadioYJusticia.Radio
{
    public static class AmbientRadioChatter
    {
        private static readonly Random random = new Random();
        private static readonly List<string> transmissions = new List<string>();

        static AmbientRadioChatter()
        {
            transmissions.Add("Zeta-4 a Sala: Iniciamos seguimiento discreto sobre turismo sospechoso Seat Leon negro circulando a gran velocidad hacia M-30.");
            transmissions.Add("Patrulla de Trafico A-4 a Central: Accidente por alcance en kilometro 18 sentido entrada. Solicitamos grua y equipo de señalizacion vial.");
            transmissions.Add("Sala 091 a todas las unidades de sector: Aviso de atraco en joyeria de avenida comercial. Dos varones armados con arma corta huyen a pie.");
            transmissions.Add("Indicativo Condor (Helicoptero) a Sala: En visual sobre vehiculo a la fuga por poligono industrial. Marcamos vector de fuga para unidades de tierra.");
            transmissions.Add("Tango-3 a Central: 10-15 con un varon detenido por presunto delito de robo con violencia. Nos dirigimos a Comisaria de Distrito para instruir.");
            transmissions.Add("Sala 091 a indicativo Charlie-2: Acudan en preventivo a calle Mayor por conato de reyerta tumultuaria entre varios jovenes.");
            transmissions.Add("Zeta-10 a Sala: Procedemos a identificacion de dos individuos en actitud vigilante y sospechosa junto a cajero de entidad bancaria.");
            transmissions.Add("Patrulla UPR a Sala: Establecido control estatico de prevencion y seguridad ciudadana en acceso principal de estacion intermodal.");
            transmissions.Add("Sala 091 a Zeta-8: Comprueben requerimiento ciudadano por musica excesivamente alta y posibles molestias vecinales en edificio residencial.");
            transmissions.Add("Zeta-2 a Sala: Solicitamos de urgencia unidad medicalizada SAMUR en Glorieta central por varon desvanecido en la via publica.");
            transmissions.Add("Sala 091 a todas las dotaciones: Localizacion de menor de edad de 13 años extraviado hace dos horas en inmediaciones de zona escolar.");
            transmissions.Add("Charlie-5 a Central: Requisitoria positiva sobre conductor identificado. Consta orden judicial de detencion e ingreso en prision.");
            transmissions.Add("Patrulla GOES a Sala: Dispositivo tactico posicionado en perimetro de inmueble conflictivo. En espera de autorizacion judicial de entrada.");
            transmissions.Add("Zeta-7 a Sala: Positivo en prueba de alcoholemia con tasa de 0.82 mg/l. Conductor investigado por delito contra la seguridad vial.");
            transmissions.Add("Sala 091 a Tango-1: Trasladense a hipermercado por sustraccion continuada en tienda de electronica. Seguridad privada tiene al sujeto retenido.");
            transmissions.Add("Patrulla de Trafico a Central: Camion articulado averiado ocupando carril derecho de autovia. Retenciones importantes, balizamos zona.");
            transmissions.Add("Zeta-12 a Sala: Detonaciones reportadas en sector norte. Solicitamos apoyo inmediato de dotaciones proximas en Codigo 2.");
            transmissions.Add("Sala 091 a indicativos de zona: Varon sospechoso forzando cerraduras de vehiculos en aparcamiento publico subterraneo.");
            transmissions.Add("Charlie-8 a Central: Persona de avanzada edad desorientada asistida en via publica. Trasladamos a domicilio con familiares.");
            transmissions.Add("Sala 091 a Zeta-3: Confirmen si continuan en 10-38 en avenida de Madrid. Entra requerimiento urgente en calle contigua.");
            transmissions.Add("Policia Judicial a Sala: Finalizada inspeccion ocular y recogida de huellas tras allanamiento de morada. Remitimos informe a Juzgado de Guardia.");
            transmissions.Add("Tango-4 a Sala: 10-4 a Sala, dejamos malla de radio despejada para atencion de incidente prioritario.");
            transmissions.Add("Zeta-6 a Central: Incendio de contenedor de residuos con riesgo de propagacion a fachada. Bomberos comisionados por Sala.");
            transmissions.Add("Sala 091 a todas las dotaciones: Desactivada alarma en joyeria de sector centro. Se confirma falsa alarma tecnica por corte de suministro.");
            transmissions.Add("Zeta-1 a Sala: 10-8 disponible en sector centro tras entrega de diligencias y atestado en Juzgado de Guardia.");
            transmissions.Add("Guias Caninos K-9 a Central: Marcaje positivo por sustancia estupefaciente en hueco motor de turismo interceptado.");
            transmissions.Add("Sala 091 a Charlie-4: Acudan de urgencia a vestibulo de metro por individuo muy agresivo amenazando a vigilantes de seguridad.");
            transmissions.Add("Zeta-9 a Sala: Localizado turismo sustraido estacionado sin ocupantes. Preservamos lugar para equipo de Policia Cientifica.");
            transmissions.Add("Patrulla UIP a Sala: Disolucion pacifica de concentracion no comunicada frente a delegacion. Restablecido el trafico rodado.");
            transmissions.Add("Sala 091 a indicativos de noche: Atencion todas las dotaciones, cambio de turno de transmisiones en Sala 091. Mantengan frecuencias de grupo.");
        }

        public static void Process()
        {
            Game.LogTrivial("RadioYJusticia: Bucle de Radio Ambiental iniciado con " + transmissions.Count.ToString() + " mensajes realistas.");

            while (true)
            {
                int baseSeconds = ConfigManager.ChatterIntervalSeconds;
                if (baseSeconds < 60) baseSeconds = 90;

                // Variación aleatoria entre 60 y 120 segundos (o alrededor del valor configurado)
                int delaySeconds = random.Next(Math.Max(60, baseSeconds - 20), Math.Max(120, baseSeconds + 30));
                GameFiber.Sleep(delaySeconds * 1000);

                try
                {
                    Ped player = Game.LocalPlayer.Character;
                    if (player != null && player.Exists() && player.IsAlive)
                    {
                        PlayRandomChatter();
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia: Error en ciclo de Radio Ambiental: " + ex.Message);
                }
            }
        }

        private static void PlayRandomChatter()
        {
            if (transmissions.Count == 0) return;

            int index = random.Next(transmissions.Count);
            string message = transmissions[index];

            DispatchVoice.PlayRadioClick();

            Game.DisplayNotification(
                "3dtextures",
                "mpgroundlogo_cops",
                "~b~SALA 091 / SIRDEE~s~",
                "~y~Trafico de Demarcacion~s~",
                message
            );

            Game.LogTrivial("RadioYJusticia [Chatter Ambiental]: " + message);

            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(4200);
                DispatchVoice.PlayRadioClick();
            });
        }
    }
}
