using System;
using Rage;
using LSPD_First_Response.Mod.API;
using TacticalOps.Core;
using TacticalOps.Scene;
using TacticalOps.Injuries;
using TacticalOps.CQB;
using TacticalOps.Negotiation;

namespace TacticalOps
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("TacticalOps: Plugin cargado correctamente.");
        }

        public override void Finally()
        {
            try
            {
                SceneObjectPlacer.RemoveAllObjects();
                StackFormation.DisbandStack();
                InjuryManager.ResetAll();
                NegotiationManager.EndNegotiation();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps: Excepcion en Finally: " + ex.Message);
            }

            Game.LogTrivial("TacticalOps: Plugin descargado.");
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
            {
                try
                {
                    ConfigManager.Load();

                    if (!ConfigManager.ModEnabled)
                    {
                        Game.LogTrivial("TacticalOps: Mod deshabilitado en archivo INI.");
                        return;
                    }

                    // GameFiber para menu de Gestion de Escenas (tecla Home)
                    GameFiber.StartNew(delegate
                    {
                        SceneMenu.Process();
                    });

                    // GameFiber para controlador de trafico perimetral
                    GameFiber.StartNew(delegate
                    {
                        TrafficController.Process();
                    });

                    // GameFiber para menu de Primeros Auxilios / Botiquin IFAK (tecla End)
                    GameFiber.StartNew(delegate
                    {
                        FirstAidKit.Process();
                    });

                    // GameFiber para menu de Entrada Tactica CQB (tecla PageDown)
                    GameFiber.StartNew(delegate
                    {
                        TacticalEntryMenu.Process();
                    });

                    // GameFiber para InjuryManager (monitoreo de salud y sangrado del jugador)
                    GameFiber.StartNew(delegate
                    {
                        InjuryManager.Process();
                    });

                    // GameFiber para sistema de Negociacion tactica y rehenes
                    GameFiber.StartNew(delegate
                    {
                        NegotiationManager.Process();
                    });

                    Game.DisplayNotification(
                        "3dtextures",
                        "mpgroundlogo_cops",
                        "~b~TacticalOps",
                        "~g~Servicio Activo",
                        "~w~Modulos tácticos operativos:~n~~y~Escenas (Inicio), IFAK (Fin), CQB (AvPág), Negociación~w~."
                    );

                    Game.LogTrivial("TacticalOps: Todos los GameFibers y modulos iniciados correctamente.");
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalOps: ERROR en inicializacion de servicio: " + ex.Message);
                }
            }
        }
    }
}
