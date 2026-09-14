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
                try
                {
                    while (true)
                    {
                        GameFiber.Yield();
                        GameFiber.Sleep(2000);

                        try
                        {
                            if (Game.LocalPlayer.Character == null || !Game.LocalPlayer.Character.Exists()) continue;

                            Ped[] allPeds = World.GetAllPeds();
                            if (allPeds == null) continue;

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
                        catch (System.Threading.ThreadAbortException) { break; }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SPI Census loop error: " + ex.Message);
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.PedHooker fatal fiber error: " + ex.Message);
                }
            });
        }
    }
}
