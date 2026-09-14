using System;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;
using SistemaPolicialIntegral.Core;
using SistemaPolicialIntegral.Census;

namespace SistemaPolicialIntegral.Intervention
{
    public static class DniVerification
    {
        public static void VerifyIdentity(Ped ped)
        {
            if (ped == null || !ped.Exists()) return;

            Persona persona = Functions.GetPersonaForPed(ped);
            if (persona == null)
            {
                Game.DisplayNotification("~r~No se ha podido obtener la identidad de esta persona.");
                return;
            }

            // Buscar identidad española del censo local o generar DNI basado en nombre
            SpanishIdentity identity = null;
            if (PedHooker.PedIdentities.ContainsKey(ped.Handle))
            {
                identity = PedHooker.PedIdentities[ped.Handle];
            }

            string dni;
            if (identity != null)
            {
                dni = identity.DniNumero;
            }
            else
            {
                dni = GenerarDniFalso(persona.FullName);
            }

            string nombreCompleto = persona.FullName;
            string fechaNacimiento = persona.Birthday.ToShortDateString();
            bool tieneAntecedentes = persona.Wanted;

            string textoAntecedentes = tieneAntecedentes ? "~r~SI (Orden Judicial)" : "~g~NO";

            string msg = string.Format("~b~MINISTERIO DEL INTERIOR~n~~w~Nombre: {0}~n~DNI: {1}~n~Nacimiento: {2}~n~Antecedentes: {3}",
                nombreCompleto, dni, fechaNacimiento, textoAntecedentes);

            PoliceSuite.Common.SpanishVoiceDispatchEngine.AnnounceDniCheck(dni, nombreCompleto);
            Game.DisplayNotification(msg);
        }

        private static string GenerarDniFalso(string nombre)
        {
            Random r = new Random(nombre.GetHashCode());
            int num = r.Next(10000000, 99999999);
            string letras = "TRWAGMYFPDXBNJZSQVHLCKE";
            char letra = letras[num % 23];
            return num.ToString() + letra;
        }
    }
}
