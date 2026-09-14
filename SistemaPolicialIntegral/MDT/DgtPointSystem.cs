using System;
using System.Collections.Generic;
using Rage;
using LSPD_First_Response.Mod.API;

namespace SistemaPolicialIntegral.MDT
{
    public static class DgtPointSystem
    {
        private static Dictionary<string, int> puntosConductores = new Dictionary<string, int>();

        public static int GetPoints(Ped ped)
        {
            if (ped == null || !ped.Exists()) return 12;

            var persona = Functions.GetPersonaForPed(ped);
            if (persona == null) return 12;

            string id = persona.FullName;

            if (!puntosConductores.ContainsKey(id))
            {
                Random r = new Random(id.GetHashCode());
                int puntos = r.Next(8, 16); // 8, 12 o 15
                if (puntos > 12 && puntos < 15) puntos = 12;
                puntosConductores.Add(id, puntos);
            }

            return puntosConductores[id];
        }

        public static void SubtractPoints(Ped ped, int pointsToSubtract)
        {
            if (ped == null || !ped.Exists()) return;

            var persona = Functions.GetPersonaForPed(ped);
            if (persona == null) return;

            string id = persona.FullName;
            int currentPoints = GetPoints(ped);
            
            int newPoints = currentPoints - pointsToSubtract;
            if (newPoints < 0) newPoints = 0;
            
            puntosConductores[id] = newPoints;
            
            if (newPoints == 0)
            {
                Game.DisplayNotification("~r~ALERTA DGT: ~w~El conductor ha perdido la vigencia de su permiso (0 puntos).");
            }
        }
    }
}
