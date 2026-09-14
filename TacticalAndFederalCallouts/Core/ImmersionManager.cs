using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;

namespace TacticalAndFederalCallouts.Core
{
    /// <summary>
    /// Modulo de inmersión policial, comunicaciones de radio con claves 10, fotografía forense y servicios periciales.
    /// </summary>
    public static class ImmersionManager
    {
        public static void PlayPoliceRadio(string callCode, Vector3 pos)
        {
            try
            {
                // Reproducir clave 10 con sonido de escáner policial nativo de LSPDFR
                Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS " + callCode + " IN_OR_ON_POSITION", pos);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("ImmersionManager.PlayPoliceRadio: " + ex.Message);
            }
        }

        public static void TakeForensicPhoto(Ped player, Vector3 evidencePos, string evidenceName)
        {
            try
            {
                // Animacion de fotografia pericial
                NativeFunction.Natives.REQUEST_ANIM_DICT("amb@world_human_paparazzi@male@idle_a");
                GameFiber.StartNew(delegate
                {
                    int wait = 0;
                    while (!NativeFunction.Natives.HAS_ANIM_DICT_LOADED<bool>("amb@world_human_paparazzi@male@idle_a") && wait < 20)
                    {
                        GameFiber.Yield();
                        wait++;
                    }

                    if (player != null && player.Exists())
                    {
                        NativeFunction.Natives.TASK_PLAY_ANIM(player, "amb@world_human_paparazzi@male@idle_a", "idle_a", 8.0f, -8.0f, 2200, 0, 0f, false, false, false);
                    }

                    GameFiber.Sleep(800);
                    // Flash fotográfico
                    NativeFunction.Natives.ADD_EXPLOSION(evidencePos.X, evidencePos.Y, evidencePos.Z + 0.3f, 25, 0.05f, false, false, 0.0f);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~FIB Forense", "~y~Fotografía Documentada", "Prueba catalogada en cadena de custodia: " + evidenceName);
                });
            }
            catch (Exception ex)
            {
                Game.LogTrivial("ImmersionManager.TakeForensicPhoto: " + ex.Message);
            }
        }

        public static void RequestCrimeSceneServices(Vector3 position, bool hasCasualties)
        {
            try
            {
                if (hasCasualties)
                {
                    // Despacho de forense / medicina legal y ambulancia
                    Functions.RequestBackup(position, LSPD_First_Response.EBackupResponseType.Code2, LSPD_First_Response.EBackupUnitType.Ambulance);
                    Game.DisplaySubtitle("~g~[CODIGO 4]~w~ Escena asegurada. Servicios forenses y sanitarios en camino.");
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("ImmersionManager.RequestCrimeSceneServices: " + ex.Message);
            }
        }
    }
}
