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
            GameFiber.StartNew(ProcessPepperSpray);
        }

        private void ProcessPepperSpray()
        {
            uint extHash = 0x060EC506; // WEAPON_FIREEXTINGUISHER as proxy

            while (true)
            {
                GameFiber.Yield();

                Ped[] peds = World.GetAllPeds();

                foreach (Ped ped in peds)
                {
                    if (ped != null && ped.Exists() && ped.IsAlive)
                    {
                        bool hit = NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON<bool>(ped, extHash, 0);
                        if (hit)
                        {
                            NativeFunction.Natives.CLEAR_ENTITY_LAST_WEAPON_DAMAGE(ped);
                            GameFiber.StartNew(delegate { ApplyPepperEffect(ped); });
                        }
                    }
                }
            }
        }

        private void ApplyPepperEffect(Ped ped)
        {
            bool isPlayer = (ped == Game.LocalPlayer.Character);

            if (isPlayer)
            {
                NativeFunction.Natives.ANIMPOSTFX_PLAY("Dont_tazeme_bro", 8000, false);
            }

            if (ped != null && ped.Exists() && ped.IsAlive)
            {
                ped.Tasks.PlayAnimation("move_m@drunk@verydrunk", "idle", 8f, AnimationFlags.Loop);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 46, false); // No attack
                
                for(int i = 0; i < 8; i++)
                {
                    Game.DisplaySubtitle("*tose violentamente*", 1000);
                    GameFiber.Sleep(1000);
                }
                
                if (ped != null && ped.Exists())
                {
                    ped.Tasks.Clear();
                    NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, 46, true);
                }
            }
        }
    }
}
