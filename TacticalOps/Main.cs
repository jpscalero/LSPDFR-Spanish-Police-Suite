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
            try
            {
                Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
                Game.LogTrivial("TacticalOps: Plugin cargado correctamente.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps: Error critico en Initialize: " + ex.Message);
            }
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
                        try { SceneMenu.Process(); }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex) { Game.LogTrivial("TacticalOps: Error en SceneMenu: " + ex.Message); }
                    });

                    // GameFiber para controlador de trafico perimetral
                    GameFiber.StartNew(delegate
                    {
                        try { TrafficController.Process(); }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex) { Game.LogTrivial("TacticalOps: Error en TrafficController: " + ex.Message); }
                    });

                    // GameFiber para menu de Primeros Auxilios / Botiquin IFAK (tecla End)
                    GameFiber.StartNew(delegate
                    {
                        try { FirstAidKit.Process(); }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex) { Game.LogTrivial("TacticalOps: Error en FirstAidKit: " + ex.Message); }
                    });

                    // GameFiber para menu de Entrada Tactica CQB (tecla PageDown)
                    GameFiber.StartNew(delegate
                    {
                        try { TacticalEntryMenu.Process(); }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex) { Game.LogTrivial("TacticalOps: Error en TacticalEntryMenu: " + ex.Message); }
                    });

                    // GameFiber para InjuryManager (monitoreo de salud y sangrado del jugador)
                    GameFiber.StartNew(delegate
                    {
                        try { InjuryManager.Process(); }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex) { Game.LogTrivial("TacticalOps: Error en InjuryManager: " + ex.Message); }
                    });

                    // GameFiber para sistema de Negociacion tactica y rehenes
                    GameFiber.StartNew(delegate
                    {
                        try { NegotiationManager.Process(); }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex) { Game.LogTrivial("TacticalOps: Error en NegotiationManager: " + ex.Message); }
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
