using System;
using Rage;
using Rage.Native;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Ballistics
{
    public class PepperSprayController
    {
        public PepperSprayController()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    ProcessPepperSpray();
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.PepperSprayController fiber error: " + ex.Message);
                }
            });
        }

        private void ProcessPepperSpray()
        {
            uint extHash = 0x060EC506; // WEAPON_FIREEXTINGUISHER as proxy

            while (true)
            {
                GameFiber.Yield();

                try
                {
                    Ped[] peds = World.GetAllPeds();
                    if (peds == null) continue;

                    foreach (Ped ped in peds)
                    {
                        if (ped != null && ped.Exists() && ped.IsAlive)
                        {
                            bool hit = NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON<bool>(ped, extHash, 0);
                            if (hit)
                            {
                                NativeFunction.Natives.CLEAR_ENTITY_LAST_WEAPON_DAMAGE(ped);
                                Ped targetPed = ped;
                                GameFiber.StartNew(delegate
                                {
                                    try
                                    {
                                        ApplyPepperEffect(targetPed);
                                    }
                                    catch (System.Threading.ThreadAbortException) { }
                                    catch (Exception ex)
                                    {
                                        Game.LogTrivial("SPI.PepperSprayController.ApplyPepperEffect fiber error: " + ex.Message);
                                    }
                                });
                            }
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.PepperSprayController loop error: " + ex.Message);
                }
            }
        }

        private void ApplyPepperEffect(Ped ped)
        {
            try
            {
                bool isPlayer = (ped == Game.LocalPlayer.Character);

                if (isPlayer)
                {
                    NativeFunction.Natives.ANIMPOSTFX_PLAY("Dont_tazeme_bro", 8000, false);
                }

                if (ped != null && ped.Exists() && ped.IsAlive)
                {
                    bool animStarted = SafeAnimation.Play(ped, "missfbi5ig_22", "coughing", 8f, AnimationFlags.Loop);
                    if (!animStarted)
                    {
                        ped.Tasks.Cower(8000);
                    }
                    NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 46, false); // No attack
                    
                    for (int i = 0; i < 8; i++)
                    {
                        Game.DisplaySubtitle("*tose violentamente*", 1000);
                        GameFiber.Sleep(1000);
                        if (ped == null || !ped.Exists() || !ped.IsAlive) break;
                    }
                    
                    if (ped != null && ped.Exists() && ped.IsAlive)
                    {
                        ped.Tasks.Clear();
                        NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 46, true);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException) { }
            catch (Exception ex)
            {
                Game.LogTrivial("SPI.PepperSprayController ApplyPepperEffect error: " + ex.Message);
            }
        }
    }
}
