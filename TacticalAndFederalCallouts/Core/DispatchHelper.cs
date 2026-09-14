using System;
using Rage;
using LSPD_First_Response.Mod.API;

namespace TacticalAndFederalCallouts.Core
{
    /// <summary>
    /// Utilidades de comunicacion con el despachador, notificaciones y solicitud de refuerzos.
    /// </summary>
    public static class DispatchHelper
    {
        public static void SendDispatchNotification(string calloutName, string stateSubtitle, string messageBody)
        {
            try
            {
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~" + calloutName, "~y~" + stateSubtitle, messageBody);
            }
            catch
            {
                Game.DisplaySubtitle("~b~[" + calloutName + "]~w~ " + stateSubtitle);
            }
        }

        public static void RequestTacticalBackup(Vector3 position, bool swat = true, bool air = false)
        {
            try
            {
                if (swat)
                {
                    Functions.RequestBackup(position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);
                }
                else
                {
                    Functions.RequestBackup(position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                }

                if (air)
                {
                    Functions.RequestBackup(position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.AirUnit);
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts: Error solicitando refuerzos: " + ex.Message);
            }
        }

        public static void PlayDispatchAudio(string audioString, Vector3 position)
        {
            try
            {
                Functions.PlayScannerAudioUsingPosition(audioString, position);
            }
            catch { }
        }
    }
}
