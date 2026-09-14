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
                        try
                        {
                            InterventionMenu.Process();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SPI.InterventionMenu fatal fiber error: " + ex.Message);
                        }
                    });

                    // Módulo 2: Terminal SITEL / DGT (F10)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            SitelComputerMenu.Process();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SPI.SitelComputerMenu fatal fiber error: " + ex.Message);
                        }
                    });

                    // Módulo 3: Despliegue SIRDEE — Refuerzos (Insert)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            new SirdeeBackupMenu();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SPI.SirdeeBackupMenu fatal fiber error: " + ex.Message);
                        }
                    });

                    // Módulo 4: Balística — Taser NMI (monitoreo continuo)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            new TaserNmiController();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SPI.TaserNmiController fatal fiber error: " + ex.Message);
                        }
                    });

                    // Módulo 4: Balística — Spray Pimienta (monitoreo continuo)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            new PepperSprayController();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SPI.PepperSprayController fatal fiber error: " + ex.Message);
                        }
                    });

                    // Módulo 4: Balística — Placas Balísticas (monitoreo continuo)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            new BallisticArmorManager();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SPI.BallisticArmorManager fatal fiber error: " + ex.Message);
                        }
                    });

                    // Módulo 6: Censo Español — Hook de identidades (monitoreo continuo)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            PedHooker.StartHookingLoop();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("SPI.PedHooker fatal fiber error: " + ex.Message);
                        }
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
