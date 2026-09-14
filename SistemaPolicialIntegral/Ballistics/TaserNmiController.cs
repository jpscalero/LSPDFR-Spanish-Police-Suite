using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Ballistics
{
    public class TaserNmiController
    {
        private Random rng;

        public TaserNmiController()
        {
            rng = new Random();
            GameFiber.StartNew(delegate
            {
                try
                {
                    ProcessNMI();
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.TaserNmiController fiber error: " + ex.Message);
                }
            });
        }

        private void ProcessNMI()
        {
            uint stunGunHash = 0x3656C8C1; // WEAPON_STUNGUN

            while (true)
            {
                GameFiber.Yield();

                try
                {
                    Ped[] peds = World.GetAllPeds();
                    if (peds == null) continue;
                    List<Ped> hitPeds = new List<Ped>();

                    foreach (Ped ped in peds)
                    {
                        if (ped != null && ped.Exists() && ped.IsAlive)
                        {
                            bool hit = NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON<bool>(ped, stunGunHash, 0);
                            if (hit)
                            {
                                NativeFunction.Natives.CLEAR_ENTITY_LAST_WEAPON_DAMAGE(ped);
                                hitPeds.Add(ped);
                            }
                        }
                    }

                    foreach (Ped target in hitPeds)
                    {
                        if (target == null || !target.Exists() || !target.IsAlive) continue;
                        Ped targetPed = target;
                        GameFiber.StartNew(delegate
                        {
                            try
                            {
                                ApplyNMIEffect(targetPed);
                            }
                            catch (System.Threading.ThreadAbortException) { }
                            catch (Exception ex)
                            {
                                Game.LogTrivial("SPI.TaserNmiController.ApplyNMIEffect fiber error: " + ex.Message);
                            }
                        });
                    }
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.TaserNmiController loop error: " + ex.Message);
                }
            }
        }

        private void ApplyNMIEffect(Ped ped)
        {
            try
            {
                bool isPlayer = (ped == Game.LocalPlayer.Character);
                int duration = ConfigManager.TaserNmiDuration;

                if (isPlayer)
                {
                    NativeFunction.Natives.SET_RADAR_ZOOM(0);
                    NativeFunction.Natives.SHAKE_GAMEPLAY_CAM("HAND_SHAKE", 2.0f);
                    NativeFunction.Natives.ANIMPOSTFX_PLAY("RaceTurbo", 5000, false);
                }

                for (int ciclo = 0; ciclo < duration * 5; ciclo++)
                {
                    if (ped != null && ped.Exists() && ped.IsAlive)
                    {
                        NativeFunction.Natives.SET_PED_TO_RAGDOLL(ped, 400, 400, 0, true, true, false);
                        
                        float fx = (float)(rng.NextDouble() * 2.0 - 1.0) * 3f;
                        float fy = (float)(rng.NextDouble() * 2.0 - 1.0) * 3f;
                        float fz = (float)(rng.NextDouble() * 0.5) * 2f;
                        
                        NativeFunction.Natives.APPLY_FORCE_TO_ENTITY(ped, 1, fx, fy, fz, 0f, 0f, 0f, 0, false, true, true, false, true);
                        
                        if (ped.Health < 10)
                        {
                            ped.Health = 10;
                        }
                    }
                    GameFiber.Sleep(200);
                }

                if (isPlayer)
                {
                    NativeFunction.Natives.STOP_GAMEPLAY_CAM_SHAKING(true);
                }
            }
            catch (System.Threading.ThreadAbortException) { }
            catch (Exception ex)
            {
                Game.LogTrivial("SPI.TaserNmiController ApplyNMIEffect error: " + ex.Message);
            }
        }
    }
}
