using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace SistemaPolicialIntegral.Census
{
    public static class PedHooker
    {
        private static HashSet<uint> hookedPeds = new HashSet<uint>();
        public static Dictionary<uint, SpanishIdentity> PedIdentities = new Dictionary<uint, SpanishIdentity>();

        public static void StartHookingLoop()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    GameFiber.Yield();
                    GameFiber.Sleep(2000);

                    if (Game.LocalPlayer.Character == null || !Game.LocalPlayer.Character.Exists()) continue;

                    Ped[] allPeds = World.GetAllPeds();
                    foreach (Ped p in allPeds)
                    {
                        if (p != null && p.Exists() && p.DistanceTo(Game.LocalPlayer.Character) < 50f)
                        {
                            if (!hookedPeds.Contains(p.Handle) && p != Game.LocalPlayer.Character)
                            {
                                bool isMale = p.IsMale;
                                SpanishIdentity identity = SpanishIdentityGenerator.GenerateRandomIdentity(isMale);

                                PedIdentities[p.Handle] = identity;
                                hookedPeds.Add(p.Handle);

                                try
                                {
                                    // LSPDFR no expone un constructor publico de Persona con todos los campos.
                                    // Almacenamos la identidad en nuestro diccionario y la mostramos
                                    // a traves de DniVerification y el MDT SITEL.
                                    Game.LogTrivial("SPI Census: Identidad asignada a ped " + p.Handle + ": " + identity.NombreCompleto + " DNI " + identity.DniNumero);
                                }
                                catch (Exception)
                                {
                                    // Fallback silencioso
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
