using System;
using Rage;
using LSPD_First_Response.Mod.API;
using AlternativeCallouts.Core;

namespace AlternativeCallouts
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            try
            {
                Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
                Game.LogTrivial("AlternativeCallouts: Plugin cargado satisfactoriamente.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts: Error critico en Initialize: " + ex.Message);
            }
        }

        public override void Finally()
        {
            try
            {
                Game.LogTrivial("AlternativeCallouts: Plugin descargado.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts: Error en Finally: " + ex.Message);
            }
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
            {
                GameFiber.StartNew(delegate
                {
                    try
                    {
                        AlternativeConfigManager.LoadConfig();
                        CalloutRegistry.RegisterAllCallouts();

                        Game.DisplayNotification(
                            "3dtextures", "mpgroundlogo_cops",
                            "~b~Llamadas Alternativas~s~",
                            "~g~Cargado y Activo~s~",
                            "~w~25 llamadas con ~y~minimo 5 desenlaces alternativos~w~ cada una listas para despacho."
                        );
                        Game.LogTrivial("AlternativeCallouts: 25 llamadas alternativas registradas correctamente.");
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("AlternativeCallouts: Error al inicializar en OnDuty: " + ex.Message);
                    }
                });
            }
        }
    }
}
