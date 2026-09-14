using System;
using Rage;
using Rage.Native;
using PoliceLifeSystem.Core;
using PoliceLifeSystem.Career;
using PoliceLifeSystem.Stats;

namespace PoliceLifeSystem.Documents
{
    public static class ActaAlcoholemia
    {
        public static void Generar()
        {
            if (Game.LocalPlayer.Character == null || !Game.LocalPlayer.Character.Exists()) return;

            DateTime now = DateTime.Now;
            string streetName = World.GetStreetName(Game.LocalPlayer.Character.Position);
            string rankTitle = RankManager.GetRankTitle(RankManager.CurrentRank);
            int tip = 64000 + (now.Millisecond % 8000);

            // Simulación reglamentaria de tasas
            Random rnd = new Random();
            int scenario = rnd.Next(0, 3);
            float tasa1, tasa2;
            string calificacion, consecuencia;
            bool esPenal;

            if (scenario == 0)
            {
                tasa1 = 0.38f + (float)(rnd.NextDouble() * 0.12);
                tasa2 = tasa1 - 0.03f;
                calificacion = "Infracción Administrativa Grave (Art. 14 Ley de Seguridad Vial)";
                consecuencia = "Multa de 500€ y detracción de 4 puntos del permiso.";
                esPenal = false;
            }
            else if (scenario == 1)
            {
                tasa1 = 0.68f + (float)(rnd.NextDouble() * 0.25);
                tasa2 = tasa1 - 0.04f;
                calificacion = "DELITO PENAL CONTRA LA SEGURIDAD VIAL (Art. 379.2 Código Penal)";
                consecuencia = "Detención y puesta a disposición judicial. Inmovilización cautelar.";
                esPenal = true;
            }
            else
            {
                tasa1 = 0.52f + (float)(rnd.NextDouble() * 0.07);
                tasa2 = tasa1 - 0.02f;
                calificacion = "Infracción Administrativa Muy Grave (Art. 14 Ley de Seguridad Vial)";
                consecuencia = "Multa de 1000€ y detracción de 6 puntos del permiso.";
                esPenal = false;
            }

            string numActa = string.Format("ACTA-ALC-{0:yyyyMMdd}-{0:HHmmss}", now);
            string filename = string.Format("ActaAlcoholemia_{0:yyyyMMdd_HHmmss}.txt", now);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("           DIRECCIÓN GENERAL DE LA POLICÍA - SEGURIDAD Y TRÁFICO               ");
            sb.AppendLine("         ACTA OFICIAL DE PRUEBAS DE DETECCIÓN ALCOHÓLICA EN AIRE ESPIRADO       ");
            sb.AppendLine("================================================================================");
            sb.AppendLine();
            sb.AppendLine("NÚMERO DE ACTA: " + numActa);
            sb.AppendLine("FECHA Y HORA: " + now.ToString("dd/MM/yyyy HH:mm:ss"));
            sb.AppendLine("LUGAR DE LA PRUEBA: " + streetName + " (Control Preventivo de Seguridad)");
            sb.AppendLine("AGENTE ACTUANTE: TIP " + tip + " | Rango: " + rankTitle);
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("I. EQUIPO EVÍDENCIAL DE MEDICIÓN HOMOLOGADO");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("Marca y Modelo: Dräger Alcotest 7510 Evidencial");
            sb.AppendLine("Número de Serie Oficial: ES-84920-CNP");
            sb.AppendLine("Certificado Metrológico Español: CEM-2026-V8 (Vigente y calibrado)");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("II. RESULTADOS DE LAS MEDICIONES");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine(string.Format("Primera prueba reglamentaria (Hora {0:HH:mm}): {1:F2} mg/l en aire espirado.", now.AddMinutes(-10), tasa1));
            sb.AppendLine(string.Format("Segunda prueba de contraste (Hora {0:HH:mm}): {1:F2} mg/l en aire espirado.", now, tasa2));
            sb.AppendLine("Intervalo transcurrido entre pruebas: 10 minutos reglamentarios.");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("III. CALIFICACIÓN JURÍDICA Y EFECTOS");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("Calificación: " + calificacion);
            sb.AppendLine("Medidas adoptadas: " + consecuencia);
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("IV. DILIGENCIA DE INFORMACIÓN DE DERECHOS AL CONDUCTOR");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("Se informa expresamente al interesado del derecho que le asiste a contrastar los");
            sb.AppendLine("resultados obtenidos mediante análisis de sangre, orina u análogos en centro médico");
            sb.AppendLine("oficial, advirtiéndose de que de resultar positivo el importe de los análisis correrá");
            sb.AppendLine("por su cuenta.");
            sb.AppendLine();
            sb.AppendLine("Firmado: El Agente Operativo (TIP " + tip + ") - " + rankTitle);

            DocumentGenerator.ExportDocument(filename, sb.ToString());

            if (esPenal)
            {
                CrimeTracker.RecordCrime("Alcoholemia Delictiva (>0.60 mg/l)", Game.LocalPlayer.Character.Position, "Detenido / Vía Penal");
                PerformanceStats.AddArrest();
                XPSystem.AddXP(70, "Instrucción de Alcoholemia Penal");
            }
            else
            {
                CrimeTracker.RecordCrime("Alcoholemia Administrativa", Game.LocalPlayer.Character.Position, "Denunciado / Vía Administrativa");
                PerformanceStats.AddCitation();
                XPSystem.AddXP(50, "Acta de Alcoholemia Administrativa");
            }

            string notifBody = string.Format(
                "~w~Tasa 1ª: ~y~{0:F2} mg/l~w~ | Tasa 2ª: ~y~{1:F2} mg/l~w~~n~Estado: {2}~n~Lugar: ~b~{3}~w~",
                tasa1,
                tasa2,
                esPenal ? "~r~DELITO PENAL (>0.60)" : "~o~INFRACCIÓN ADM.",
                streetName
            );

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~ACTA DE ALCOHOLEMIA", "~g~Prueba Etilométrica Dräger", notifBody);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Beep_Green", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false);
        }
    }
}
