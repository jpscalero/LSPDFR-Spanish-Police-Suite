using System;
using Rage;
using Rage.Native;
using PoliceLifeSystem.Core;
using PoliceLifeSystem.Career;
using PoliceLifeSystem.Economy;
using PoliceLifeSystem.Stats;

namespace PoliceLifeSystem.Documents
{
    public static class InformeIncidentes
    {
        public static void Generar()
        {
            if (Game.LocalPlayer.Character == null || !Game.LocalPlayer.Character.Exists()) return;

            DateTime now = DateTime.Now;
            int shiftNum = PerformanceStats.ShiftsCompleted + 1;
            string numInforme = string.Format("INF-TURNO-{0:yyyyMMdd}-{1:D3}", now, shiftNum);
            string rankTitle = RankManager.GetRankTitle(RankManager.CurrentRank);
            int tip = 52000 + (now.Millisecond % 5000);
            string streetName = World.GetStreetName(Game.LocalPlayer.Character.Position);

            string filename = string.Format("InformeTurno_{0:yyyyMMdd_HHmmss}.txt", now);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("                 CUERPO NACIONAL DE POLICÍA - PARTE DE NOVEDADES                ");
            sb.AppendLine("                     INFORME OFICIAL DE CIERRE DE TURNO                         ");
            sb.AppendLine("================================================================================");
            sb.AppendLine();
            sb.AppendLine("NÚMERO DE INFORME: " + numInforme);
            sb.AppendLine("FECHA Y HORA DE EMISIÓN: " + now.ToString("dd/MM/yyyy HH:mm:ss"));
            sb.AppendLine("INDICATIVO / DOTACIÓN: Z-24 (Patrulla de Seguridad Ciudadana)");
            sb.AppendLine("RESPONSABLE DEL SERVICIO: TIP " + tip + " | Rango: " + rankTitle);
            sb.AppendLine("ÚLTIMA POSICIÓN REPORTADA: " + streetName);
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("I. ESTADÍSTICAS OPERATIVAS ACUMULADAS EN EL SERVICIO");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("Turnos de servicio completados: " + shiftNum);
            sb.AppendLine("Detenciones y puestas a disposición judicial: " + PerformanceStats.TotalArrests);
            sb.AppendLine("Actas y denuncias de tráfico tramitadas: " + PerformanceStats.TotalCitations);
            sb.AppendLine("Amenazas armadas neutralizadas: " + PerformanceStats.HostilesNeutralized);
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("II. ESTADO FINANCIERO Y DOTACIÓN");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("Saldo en cuenta nómina oficial: " + SalaryManager.CurrentBalance + "€");
            sb.AppendLine("Nivel de experiencia profesional: " + XPSystem.CurrentXP + " XP");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("III. OBSERVACIONES DE LA FUERZA ACTUANTE");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("El turno de servicio transcurrió conforme a las órdenes generales de la Jefatura,");
            sb.AppendLine("manteniéndose el orden público, la seguridad ciudadana y la prevención de ilícitos.");
            sb.AppendLine("Se procede a la entrega del arma reglamentaria y vehículo en las dependencias.");
            sb.AppendLine();
            sb.AppendLine("Firmado: El Funcionario Actuante (TIP " + tip + ") - " + rankTitle);

            DocumentGenerator.ExportDocument(filename, sb.ToString());

            // Registrar turno en estadísticas
            PerformanceStats.AddShift();

            // Otorga XP
            XPSystem.AddXP(100, "Cierre de Turno de Servicio Policial");

            string notifBody = string.Format(
                "~w~Turno Nº: ~b~{0}~w~~n~Detenciones: ~y~{1}~w~ | Denuncias: ~y~{2}~w~~n~Neutralizados: ~r~{3}~w~~n~Saldo final: ~g~{4}€~w~",
                shiftNum,
                PerformanceStats.TotalArrests,
                PerformanceStats.TotalCitations,
                PerformanceStats.HostilesNeutralized,
                SalaryManager.CurrentBalance
            );

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~INFORME DE TURNO CNP", "~g~Servicio Cerrado con Éxito", notifBody);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Event_Message_Purple", "GTAO_FM_Events_Soundset", false);
        }
    }
}
