using System;
using Rage;
using Rage.Native;

namespace TacticalOps.Negotiation
{
    public static class NegotiationManager
    {
        private static bool isNegotiating;
        private static Ped suspectPed;
        private static Ped hostagePed;

        public static bool IsNegotiating
        {
            get { return isNegotiating; }
            set { isNegotiating = value; }
        }

        public static Ped SuspectPed
        {
            get { return suspectPed; }
            set { suspectPed = value; }
        }

        public static Ped HostagePed
        {
            get { return hostagePed; }
            set { hostagePed = value; }
        }

        static NegotiationManager()
        {
            isNegotiating = false;
            suspectPed = null;
            hostagePed = null;
        }

        public static void StartNegotiation(Ped suspect, Ped hostage)
        {
            if (suspect == null || !suspect.Exists() || suspect.IsDead)
            {
                Game.DisplayNotification("~r~No se puede iniciar la negociacion: sospechoso no valido.");
                return;
            }

            suspectPed = suspect;
            hostagePed = hostage;
            isNegotiating = true;

            // Bloquear eventos del sospechoso para que mantenga posicion
            suspectPed.BlockPermanentEvents = true;
            suspectPed.Tasks.Clear();

            Ped player = Game.LocalPlayer.Character;
            if (hostagePed != null && hostagePed.Exists() && hostagePed.IsAlive)
            {
                hostagePed.BlockPermanentEvents = true;
                suspectPed.Tasks.AimWeaponAt(hostagePed, -1);
            }
            else
            {
                suspectPed.Tasks.AimWeaponAt(player, -1);
            }

            // Inicializar tension al 50%
            TensionSystem.Initialize(50.0f);

            // Iniciar arbol de dialogos
            DialogueTree.Start(suspectPed, hostagePed);

            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET", false);
            Game.DisplayNotification("~b~Negociacion de Rehenes Activa~w~~n~Canal de megafono abierto. Calibra tus respuestas.");
            Game.LogTrivial("TacticalOps: Negociacion iniciada con sospechoso.");
        }

        public static void TriggerHostileAction()
        {
            isNegotiating = false;
            DialogueTree.Close();

            Game.DisplaySubtitle("~r~¡EL SOSPECHOSO HA PERDIDO EL CONTROL Y ABRE FUEGO!~w~", 5000);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Bed", "WastedSounds", false);

            if (suspectPed != null && suspectPed.Exists() && suspectPed.IsAlive)
            {
                suspectPed.BlockPermanentEvents = false;

                if (hostagePed != null && hostagePed.Exists() && hostagePed.IsAlive)
                {
                    NativeFunction.Natives.TASK_SHOOT_AT_ENTITY(suspectPed, hostagePed, -1, 0);
                }
                else
                {
                    suspectPed.Tasks.FightAgainst(Game.LocalPlayer.Character);
                }
            }

            Game.DisplayNotification("~r~¡Negociacion Fracasada!~w~ El sospechoso ha roto el alto el fuego.");
            Game.LogTrivial("TacticalOps: Negociacion fracasada por tension maxima.");
        }

        public static void TriggerSurrender()
        {
            isNegotiating = false;
            DialogueTree.Close();

            Game.DisplaySubtitle("~g~Sospechoso:~w~ «¡Esta bien, me rindo! ¡No dispareis por favor!»", 5000);

            if (suspectPed != null && suspectPed.Exists() && suspectPed.IsAlive)
            {
                // Soltar arma
                uint weaponHash = NativeFunction.Natives.GET_SELECTED_PED_WEAPON<uint>(suspectPed);
                if (weaponHash != 0 && weaponHash != Game.GetHashKey("WEAPON_UNARMED"))
                {
                    NativeFunction.Natives.SET_PED_DROPS_INVENTORY_WEAPON(suspectPed, weaponHash, 0.0f, 0.0f, 0.0f, 0);
                }

                suspectPed.Tasks.Clear();
                NativeFunction.Natives.CLEAR_PED_TASKS_IMMEDIATELY(suspectPed);

                // Manos arriba y de rodillas
                suspectPed.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                NativeFunction.Natives.SET_ENABLE_HANDCUFFS(suspectPed, true);
                suspectPed.BlockPermanentEvents = true;
            }

            if (hostagePed != null && hostagePed.Exists() && hostagePed.IsAlive)
            {
                hostagePed.BlockPermanentEvents = false;
                hostagePed.Tasks.ReactAndFlee(suspectPed);
            }

            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "MEDAL_UP", "HUD_MINI_GAME_SOUNDSET", false);
            Game.DisplayNotification("~g~¡Negociacion Exitosa!~w~ El sujeto ha depuesto las armas y se entrega.");
            Game.LogTrivial("TacticalOps: Negociacion resuelta pacificamente.");
        }

        public static void EndNegotiation()
        {
            isNegotiating = false;
            TensionSystem.IsActive = false;
            DialogueTree.Close();

            if (suspectPed != null && suspectPed.Exists())
            {
                suspectPed.BlockPermanentEvents = false;
            }

            if (hostagePed != null && hostagePed.Exists())
            {
                hostagePed.BlockPermanentEvents = false;
            }

            Game.LogTrivial("TacticalOps: Negociacion finalizada.");
        }

        public static void Process()
        {
            try
            {
                Game.LogTrivial("TacticalOps: NegotiationManager loop iniciado.");

                while (true)
                {
                    GameFiber.Yield();

                    try
                    {
                        if (isNegotiating)
                        {
                            if (suspectPed == null || !suspectPed.Exists() || suspectPed.IsDead)
                            {
                                EndNegotiation();
                                Game.DisplayNotification("~y~Negociacion interrumpida:~w~ El sospechoso ha sido neutralizado o no existe.");
                                continue;
                            }

                            TensionSystem.Update();
                            DialogueTree.Process();
                        }
                        else
                        {
                            GameFiber.Sleep(200);
                        }
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("TacticalOps.NegotiationManager.Process loop: " + ex.Message);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException) { }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps.NegotiationManager.Process fatal: " + ex.Message);
            }
        }
    }
}
