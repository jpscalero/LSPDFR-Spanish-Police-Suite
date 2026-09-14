using System;
using System.Reflection;
using Rage;
using LSPD_First_Response.Mod.API;
using PeligroExtremoCallouts.Core;

namespace PeligroExtremoCallouts
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("PeligroExtremoCallouts: Plugin cargado y a la espera de entrada en servicio.");
        }

        public override void Finally()
        {
            Game.LogTrivial("PeligroExtremoCallouts: Plugin descargado correctamente.");
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
                        Game.LogTrivial("PeligroExtremoCallouts: Mod deshabilitado en el archivo INI.");
                        return;
                    }

                    int count = CalloutRegistry.RegisterAllCallouts();

                    Game.DisplayNotification(
                        "3dtextures",
                        "mpgroundlogo_cops",
                        "~r~Peligro Extremo Callouts",
                        "~g~Servicio Activo",
                        string.Format("~w~Modulo cargado con exito. ~y~{0} misiones de tiroteos y persecuciones~w~ activas.", count)
                    );
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PeligroExtremoCallouts: Error al entrar en servicio: " + ex.Message);
                }
            }
        }
    }
}
