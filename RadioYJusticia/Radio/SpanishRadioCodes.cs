using System;
using System.Collections.Generic;

namespace RadioYJusticia.Radio
{
    public class SpanishRadioCode
    {
        public string Code { get; set; }
        public string Significado { get; set; }
        public string DescripcionEspanola { get; set; }
        public string RespuestaSala { get; set; }

        public SpanishRadioCode(string code, string significado, string descripcion, string respuesta)
        {
            Code = code;
            Significado = significado;
            DescripcionEspanola = descripcion;
            RespuestaSala = respuesta;
        }
    }

    public static class SpanishRadioCodes
    {
        private static Dictionary<string, SpanishRadioCode> codes;

        static SpanishRadioCodes()
        {
            codes = new Dictionary<string, SpanishRadioCode>();

            AddCode(new SpanishRadioCode(
                "10-4",
                "Entendido / Recibido",
                "Confirmacion de mensaje recibido y conforme.",
                "Recibido y conforme indicativo. Prosigamos en canal de grupo."
            ));

            AddCode(new SpanishRadioCode(
                "10-7",
                "Fin de turno / Fuera de servicio",
                "Patrulla finaliza servicio activo o entra en descanso reglamentario.",
                "Anotado 10-7 de la dotacion. Quedan fuera de transmisiones operativas."
            ));

            AddCode(new SpanishRadioCode(
                "10-8",
                "En servicio / Disponible",
                "Unidad operativa en calle y disponible para intervenciones.",
                "Zeta recibido. Queda anotado en servicio y disponible en sector. Buen servicio."
            ));

            AddCode(new SpanishRadioCode(
                "10-15",
                "Detenido custodiado a bordo",
                "Traslado de detenido a dependencias policiales para incoacion de diligencias.",
                "Recibido 10-15. Sala de custodia de comisaria prevenida para recepcion del detenido. Maximo legal 72 horas."
            ));

            AddCode(new SpanishRadioCode(
                "10-20",
                "Posicion actual",
                "Transmision de la ubicacion geografica o via publica de la patrulla.",
                "Posicion registrada en cartografia de Sala. Mantengan patrullaje en su demarcacion."
            ));

            AddCode(new SpanishRadioCode(
                "10-33",
                "Trafico de radio prioritario por emergencia",
                "Peticion de silencio radial absoluto para atencion de suceso grave.",
                "¡Atencion a toda la malla! Silencio de radio absoluto, prioridad 10-33 en curso."
            ));

            AddCode(new SpanishRadioCode(
                "10-38",
                "Parada de trafico preventiva",
                "Interceptacion y control selectivo de vehiculo en via publica.",
                "Recibido 10-38. Control preventivo registrado. Extreme precauciones y notifique matricula."
            ));

            AddCode(new SpanishRadioCode(
                "10-50",
                "Accidente de trafico",
                "Colision o siniestro vial en via urbana o carretera.",
                "Recibido 10-50. Se comisionan servicios de atestados y ambulancia si fuese necesario."
            ));

            AddCode(new SpanishRadioCode(
                "10-99",
                "¡ATENCION! Agente en peligro extremo",
                "Alerta maxima: agente bajo fuego o en riesgo vital inminente.",
                "¡Alerta general! 10-99 confirmado. Todas las dotaciones disponibles acudan en Codigo 3."
            ));
        }

        private static void AddCode(SpanishRadioCode codeObj)
        {
            if (codeObj != null && !codes.ContainsKey(codeObj.Code))
            {
                codes.Add(codeObj.Code, codeObj);
            }
        }

        public static SpanishRadioCode GetCode(string code)
        {
            if (codes != null && codes.ContainsKey(code))
            {
                return codes[code];
            }
            return null;
        }

        public static List<SpanishRadioCode> GetAllCodes()
        {
            List<SpanishRadioCode> list = new List<SpanishRadioCode>();
            if (codes != null)
            {
                foreach (KeyValuePair<string, SpanishRadioCode> pair in codes)
                {
                    list.Add(pair.Value);
                }
            }
            return list;
        }
    }
}
