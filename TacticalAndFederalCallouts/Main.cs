using System;
using System.Reflection;
using Rage;
using LSPD_First_Response.Mod.API;
using TacticalAndFederalCallouts.Core;

namespace TacticalAndFederalCallouts
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            try
            {
                Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
                Game.LogTrivial("TacticalAndFederalCallouts: Plugin cargado y a la espera de entrada en servicio.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts: Error critico en Initialize: " + ex.Message);
            }
        }

        public override void Finally()
        {
            try
            {
                Game.LogTrivial("TacticalAndFederalCallouts: Plugin descargado correctamente.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts: Error en Finally: " + ex.Message);
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
                        Game.LogTrivial("TacticalAndFederalCallouts: Mod deshabilitado en el archivo INI.");
                        return;
                    }

                    int count = CalloutRegistry.RegisterAllCallouts();

                    Game.DisplayNotification(
                        "3dtextures",
                        "mpgroundlogo_cops",
                        "~b~Tactical & Federal Ops",
                        "~g~Servicio Activo",
                        string.Format("~w~Modulo cargado con exito. ~y~{0} misiones SWAT y FIB~w~ activas.", count)
                    );
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalAndFederalCallouts: Error al entrar en servicio: " + ex.Message);
                }
            }
        }
    }
}
