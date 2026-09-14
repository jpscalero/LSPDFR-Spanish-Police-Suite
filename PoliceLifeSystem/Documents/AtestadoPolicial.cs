using System;
using Rage;
using Rage.Native;
using PoliceLifeSystem.Core;
using PoliceLifeSystem.Career;
using PoliceLifeSystem.Stats;

namespace PoliceLifeSystem.Documents
{
    public static class AtestadoPolicial
    {
        public static void Generar()
        {
            if (Game.LocalPlayer.Character == null || !Game.LocalPlayer.Character.Exists()) return;

            DateTime now = DateTime.Now;
            string numDiligencias = string.Format("CNP-{0:yyyyMMdd}-{0:HHmmss}", now);
            string streetName = World.GetStreetName(Game.LocalPlayer.Character.Position);
            string rankTitle = RankManager.GetRankTitle(RankManager.CurrentRank);
            int tip = 78000 + (now.Millisecond % 9000);

            string filename = string.Format("Atestado_{0:yyyyMMdd_HHmmss}.txt", now);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("                 CUERPO NACIONAL DE POLICÍA - JEFATURA SUPERIOR                ");
            sb.AppendLine("                   COMISARÍA DE POLICÍA JUDICIAL Y SEGURIDAD                   ");
            sb.AppendLine("================================================================================");
            sb.AppendLine();
            sb.AppendLine("DILIGENCIAS PREVIAS NÚMERO: " + numDiligencias);
            sb.AppendLine("FECHA Y HORA DE INSTRUCCIÓN: " + now.ToString("dd/MM/yyyy HH:mm:ss"));
            sb.AppendLine("LUGAR DE LOS HECHOS: " + streetName + " (Sector Operativo Central)");
            sb.AppendLine("AGENTE INSTRUCTOR: TIP " + tip + " | Categoría: " + rankTitle);
            sb.AppendLine("SECRETARIO EN FUNCIONES: Agente de Apoyo Técnico Policial");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("I. DILIGENCIA DE INICIO Y EXPOSICIÓN DE HECHOS");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("Que siendo la fecha y hora indicadas, la dotación policial actuante, debidamente");
            sb.AppendLine("uniformada y en vehículo oficial de servicio, se personó en la vía pública arriba");
            sb.AppendLine("reseñada tras observar una alteración grave del orden público y presunta actividad");
            sb.AppendLine("ilícita flagrante.");
            sb.AppendLine();
            sb.AppendLine("Al proceder a la identificación del autor o autores, los mismos mostraron una actitud");
            sb.AppendLine("hostil y refractaria a los mandatos legítimos de la fuerza actuante, procediéndose");
            sb.AppendLine("a su contención física, inmovilización y aplicación de los elementos de seguridad");
            sb.AppendLine("reglamentarios (grilletes) con pleno respeto a la proporcionalidad de la fuerza.");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("II. DILIGENCIA DE LECTURA DE DERECHOS (ART. 520 L.E.CRIM.)");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("Se hace constar expresamente que a los encausados se les dio oportuna lectura");
            sb.AppendLine("de los derechos fundamentales que asisten a toda persona investigada o detenida:");
            sb.AppendLine("- Derecho a guardar silencio y a no declarar en su contra.");
            sb.AppendLine("- Derecho a no confesarse culpable.");
            sb.AppendLine("- Derecho a designar abogado de libre elección o ser asistido de oficio.");
            sb.AppendLine("- Derecho a que se comunique el hecho y lugar de detención a persona que designe.");
            sb.AppendLine("- Derecho a ser asistido por facultativo médico oficial.");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("III. CALIFICACIÓN PENAL PRELIMINAR");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("Los hechos descritos pudieran resultar constitutivos de:");
            sb.AppendLine("1. Delito de Atentado contra la Autoridad y sus Agentes (Art. 550 del Código Penal).");
            sb.AppendLine("2. Delito de Resistencia y Desobediencia Grave (Art. 556 del Código Penal).");
            sb.AppendLine("3. Delitos contra la Seguridad Colectiva y Vial (Art. 379 y concordantes del CP).");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("IV. DESTINO Y REMISIÓN");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("El presente atestado se remite en formato fehaciente al Iltmo. Juzgado de Instrucción");
            sb.AppendLine("en funciones de guardia, quedando copia debidamente custodiada en el archivo de la Comisaría.");
            sb.AppendLine();
            sb.AppendLine("Y para que conste y surta los efectos oportunos, se expide y firma el presente en fecha ut supra.");
            sb.AppendLine("Fdo.: El Instructor (TIP " + tip + ") - " + rankTitle);

            DocumentGenerator.ExportDocument(filename, sb.ToString());

            // Actualizar estadísticas y tracker
            CrimeTracker.RecordCrime("Atestado Penal Instruido", Game.LocalPlayer.Character.Position, "Detenido / Remitido a Juzgado");

            // Otorgar XP
            XPSystem.AddXP(75, "Instrucción de Atestado Penal");

            // Mostrar notificación en pantalla
            string notif = string.Format(
                "~w~Diligencias: ~y~{0}~w~~n~Lugar: ~b~{1}~w~~n~Instructor: ~b~{2}~w~~n~Destino: ~g~Juzgado de Guardia~w~",
                numDiligencias,
                streetName,
                rankTitle
            );

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~ATESTADO POLICIAL CNP", "~g~Diligencias Judiciales", notif);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Event_Start_Text", "GTAO_FM_Events_Soundset", false);
        }
    }
}
