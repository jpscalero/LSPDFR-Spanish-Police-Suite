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
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("AlternativeCallouts: Plugin cargado satisfactoriamente.");
        }

        public override void Finally()
        {
            Game.LogTrivial("AlternativeCallouts: Plugin descargado.");
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
            {
                GameFiber.StartNew(delegate
                {
                    AlternativeConfigManager.LoadConfig();
                    CalloutRegistry.RegisterAllCallouts();

                    Game.DisplayNotification(
                        "3dtextures", "mpgroundlogo_cops",
                        "~b~Llamadas Alternativas~s~",
                        "~g~Cargado y Activo~s~",
                        "~w~25 llamadas con ~y~minimo 5 desenlaces alternativos~w~ cada una listas para despacho."
                    );
                });
            }
        }
    }
}
