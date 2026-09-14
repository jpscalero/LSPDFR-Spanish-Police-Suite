using System;
using Rage;
using LSPD_First_Response.Mod.API;

namespace PeligroExtremoCallouts.Core
{
    /// <summary>
    /// Utilidades de comunicacion con el despachador, notificaciones y solicitud de refuerzos para PeligroExtremoCallouts.
    /// </summary>
    public static class DispatchHelper
    {
        public static void SendDispatchNotification(string calloutName, string stateSubtitle, string messageBody)
        {
            try
            {
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~" + calloutName, "~y~" + stateSubtitle, messageBody);
            }
            catch
            {
                Game.DisplaySubtitle("~r~[" + calloutName + "]~w~ " + stateSubtitle);
            }
        }

        public static void RequestCode3Backup(Vector3 position, bool swat = false, bool air = false)
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
                Game.LogTrivial("PeligroExtremoCallouts: Error solicitando refuerzos Code 3: " + ex.Message);
            }
        }

        public static void RequestPursuitBackup(Vector3 position, bool swat = false, bool air = false)
        {
            try
            {
                if (swat)
                {
                    Functions.RequestBackup(position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                }
                else
                {
                    Functions.RequestBackup(position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                }

                if (air)
                {
                    Functions.RequestBackup(position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PeligroExtremoCallouts: Error solicitando refuerzos de persecucion: " + ex.Message);
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
