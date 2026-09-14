using System;
using Rage;
using LSPD_First_Response.Mod.API;

namespace SistemaPolicialIntegral.MDT
{
    public static class JudicialWarrants
    {
        private static string[] tiposOrden = new string[] 
        { 
            "Orden de busca y captura", 
            "Requisitoria por delito de quebrantamiento", 
            "Orden de ingreso en prisión" 
        };

        private static string[] juzgados = new string[]
        {
            "Madrid", "Barcelona", "Valencia", "Sevilla", "Zaragoza", "Málaga"
        };

        public static void CheckWarrants(Ped ped)
        {
            if (ped == null || !ped.Exists()) return;
            
            var persona = Functions.GetPersonaForPed(ped);
            if (persona == null) return;

            Random r = new Random(persona.FullName.GetHashCode() + DateTime.Now.Minute);
            
            if (r.Next(100) < 20) // 20% probabilidad
            {
                string tipo = tiposOrden[r.Next(tiposOrden.Length)];
                string juzgado = juzgados[r.Next(juzgados.Length)];
                int numJuzgado = r.Next(1, 10);
                
                string msg = string.Format("~r~ALERTA JUDICIAL:~n~~w~El ciudadano {0} tiene una orden pendiente:~n~{1}~n~Emitida por: Juzgado de Instrucción nº {2} de {3}",
                    persona.FullName, tipo, numJuzgado, juzgado);
                
                Game.DisplayNotification(msg);
            }
            else
            {
                Game.DisplayNotification("~g~SIN REQUISITORIAS:~n~~w~El ciudadano " + persona.FullName + " no tiene causas pendientes.");
            }
        }
    }
}
