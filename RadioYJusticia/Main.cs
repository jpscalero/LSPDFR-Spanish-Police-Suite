using System;
using Rage;
using LSPD_First_Response.Mod.API;
using RadioYJusticia.Core;
using RadioYJusticia.Radio;
using RadioYJusticia.Justice;

namespace RadioYJusticia
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            try
            {
                Functions.OnOnDutyStateChanged += OnDutyHandler;
                Game.LogTrivial("RadioYJusticia: Plugin inicializado con exito.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error critico en Initialize: " + ex.Message);
            }
        }

        public override void Finally()
        {
            try
            {
                ArrestHistoryDB.Save();
                Game.LogTrivial("RadioYJusticia: Base de datos de arrestos guardada correctamente.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error al guardar base de datos al descargar: " + ex.Message);
            }

            Game.LogTrivial("RadioYJusticia: Plugin descargado.");
        }

        private static void OnDutyHandler(bool onDuty)
        {
            if (onDuty)
            {
                try
                {
                    ConfigManager.Load();
                    ArrestHistoryDB.Load();

                    if (!ConfigManager.ModEnabled)
                    {
                        Game.LogTrivial("RadioYJusticia: Mod deshabilitado en configuracion INI.");
                        return;
                    }

                    // GameFiber para Menú de Radio SIRDEE (NumPad *)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            RadioMenu.Process();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("RadioYJusticia: Error critico en fibra de RadioMenu: " + ex.Message);
                        }
                    });

                    // GameFiber para Botón de Pánico 10-99 (NumPad -)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            PanicButton.Process();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("RadioYJusticia: Error critico en fibra de PanicButton: " + ex.Message);
                        }
                    });

                    // GameFiber para Radio Ambiental de fondo (chatter aleatorio)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            AmbientRadioChatter.Process();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("RadioYJusticia: Error critico en fibra de AmbientRadioChatter: " + ex.Message);
                        }
                    });

                    // GameFiber para CustodyManager (monitoreo de detenidos, 72h legales y juicios)
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            CustodyManager.Process();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("RadioYJusticia: Error critico en fibra de CustodyManager: " + ex.Message);
                        }
                    });

                    Game.DisplayNotification(
                        "3dtextures",
                        "mpgroundlogo_cops",
                        "~b~Radio y Justicia (SIRDEE)",
                        "~g~Servicio Activo",
                        "~w~Radio SIRDEE (~y~NumPad *~w~), Boton Panico (~r~NumPad -~w~) y Gestion Procesal 72h operativos."
                    );

                    Game.LogTrivial("RadioYJusticia: Todos los modulos (SIRDEE, Panico, Chatter y Custodia) iniciados correctamente.");
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia: ERROR critico al inicializar modulos en OnDuty: " + ex.Message);
                }
            }
        }
    }
}
