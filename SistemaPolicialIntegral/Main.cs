using System;
using Rage;
using LSPD_First_Response.Mod.API;
using SistemaPolicialIntegral.Core;
using SistemaPolicialIntegral.Intervention;
using SistemaPolicialIntegral.MDT;
using SistemaPolicialIntegral.Backup;
using SistemaPolicialIntegral.Ballistics;
using SistemaPolicialIntegral.Census;

namespace SistemaPolicialIntegral
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            try
            {
                Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
                Game.LogTrivial("SistemaPolicialIntegral: Plugin cargado correctamente.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("SistemaPolicialIntegral: Error critico en Initialize: " + ex.Message);
            }
        }

        public override void Finally()
        {
            try
            {
                Game.LogTrivial("SistemaPolicialIntegral: Plugin descargado.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("SistemaPolicialIntegral: Error en Finally: " + ex.Message);
            }
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
                        Game.LogTrivial("SistemaPolicialIntegral: Mod deshabilitado en INI.");
                        return;
                    }

                    // Módulo 1: Menú de Intervención Policial (F9)
                    GameFiber.StartNew(delegate
                    {
                        InterventionMenu.Process();
                    });

                    // Módulo 2: Terminal SITEL / DGT (F10)
                    GameFiber.StartNew(delegate
                    {
                        SitelComputerMenu.Process();
                    });

                    // Módulo 3: Despliegue SIRDEE — Refuerzos (Insert)
                    GameFiber.StartNew(delegate
                    {
                        new SirdeeBackupMenu();
                    });

                    // Módulo 4: Balística — Taser NMI (monitoreo continuo)
                    GameFiber.StartNew(delegate
                    {
                        new TaserNmiController();
                    });

                    // Módulo 4: Balística — Spray Pimienta (monitoreo continuo)
                    GameFiber.StartNew(delegate
                    {
                        new PepperSprayController();
                    });

                    // Módulo 4: Balística — Placas Balísticas (monitoreo continuo)
                    GameFiber.StartNew(delegate
                    {
                        new BallisticArmorManager();
                    });

                    // Módulo 6: Censo Español — Hook de identidades (monitoreo continuo)
                    GameFiber.StartNew(delegate
                    {
                        PedHooker.StartHookingLoop();
                    });

                    Game.DisplayNotification(
                        "3dtextures",
                        "mpgroundlogo_cops",
                        "~b~Sistema Policial Integral",
                        "~g~Servicio Activo",
                        "~w~Todos los módulos cargados:~n~~y~Intervención, SITEL, SIRDEE, Balística, Censo~w~."
                    );

                    Game.LogTrivial("SistemaPolicialIntegral: Todos los módulos inicializados correctamente.");
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("SistemaPolicialIntegral: ERROR al inicializar: " + ex.Message);
                }
            }
        }
    }
}
