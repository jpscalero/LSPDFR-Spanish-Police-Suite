using System;
using Rage;
using LSPD_First_Response.Mod.API;
using TacticalAndFederalCallouts.Callouts.SWAT_NOOSE;
using TacticalAndFederalCallouts.Callouts.FIB_Detectives;

namespace TacticalAndFederalCallouts.Core
{
    /// <summary>
    /// Registro central de llamadas para Tactical & Federal Callouts (42 llamadas).
    /// </summary>
    public static class CalloutRegistry
    {
        public static int RegisterAllCallouts()
        {
            int registered = 0;

            // Registro de 21 Llamadas SWAT / NOOSE
            if (ConfigManager.IsCalloutEnabled("SN_HostageRescueComplex"))
            {
                Functions.RegisterCallout(typeof(SN_HostageRescueComplex));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_CarBombCell"))
            {
                Functions.RegisterCallout(typeof(SN_CarBombCell));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_RooftopSniperHeist"))
            {
                Functions.RegisterCallout(typeof(SN_RooftopSniperHeist));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_NoKnockRaid"))
            {
                Functions.RegisterCallout(typeof(SN_NoKnockRaid));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_PrisonRiot"))
            {
                Functions.RegisterCallout(typeof(SN_PrisonRiot));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_BusHijacking"))
            {
                Functions.RegisterCallout(typeof(SN_BusHijacking));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_AirportHangarSiege"))
            {
                Functions.RegisterCallout(typeof(SN_AirportHangarSiege));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_YachtAssault"))
            {
                Functions.RegisterCallout(typeof(SN_YachtAssault));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_EmbassySiege"))
            {
                Functions.RegisterCallout(typeof(SN_EmbassySiege));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_PrisonConvoyAmbush"))
            {
                Functions.RegisterCallout(typeof(SN_PrisonConvoyAmbush));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_TrainHijacking"))
            {
                Functions.RegisterCallout(typeof(SN_TrainHijacking));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_HospitalSiege"))
            {
                Functions.RegisterCallout(typeof(SN_HospitalSiege));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_UndergroundBunkerRaid"))
            {
                Functions.RegisterCallout(typeof(SN_UndergroundBunkerRaid));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_NuclearPlantSiege"))
            {
                Functions.RegisterCallout(typeof(SN_NuclearPlantSiege));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_StadiumSiege"))
            {
                Functions.RegisterCallout(typeof(SN_StadiumSiege));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_SkyscraperSiege"))
            {
                Functions.RegisterCallout(typeof(SN_SkyscraperSiege));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_MilitaryConvoyAmbush"))
            {
                Functions.RegisterCallout(typeof(SN_MilitaryConvoyAmbush));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_MountChiliadCompound"))
            {
                Functions.RegisterCallout(typeof(SN_MountChiliadCompound));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_OilRigSiege"))
            {
                Functions.RegisterCallout(typeof(SN_OilRigSiege));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_CommTowerHostage"))
            {
                Functions.RegisterCallout(typeof(SN_CommTowerHostage));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SN_HouseToHouseManhunt"))
            {
                Functions.RegisterCallout(typeof(SN_HouseToHouseManhunt));
                registered++;
            }

            // Registro de 21 Llamadas FIB / Detectives
            if (ConfigManager.IsCalloutEnabled("FD_ControlledDeliveryDocksExplosion"))
            {
                Functions.RegisterCallout(typeof(FD_ControlledDeliveryDocksExplosion));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_HomicideSceneInvestigation"))
            {
                Functions.RegisterCallout(typeof(FD_HomicideSceneInvestigation));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_MafiaExtortionBackroom"))
            {
                Functions.RegisterCallout(typeof(FD_MafiaExtortionBackroom));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_MoneyLaunderingMansion"))
            {
                Functions.RegisterCallout(typeof(FD_MoneyLaunderingMansion));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_HumanTraffickingContainers"))
            {
                Functions.RegisterCallout(typeof(FD_HumanTraffickingContainers));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_CorruptPoliceCommander"))
            {
                Functions.RegisterCallout(typeof(FD_CorruptPoliceCommander));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_ForeignSpyClassifiedData"))
            {
                Functions.RegisterCallout(typeof(FD_ForeignSpyClassifiedData));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_FentanylSuperlab"))
            {
                Functions.RegisterCallout(typeof(FD_FentanylSuperlab));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_RansomwareServerFarm"))
            {
                Functions.RegisterCallout(typeof(FD_RansomwareServerFarm));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_MissileSmuggling"))
            {
                Functions.RegisterCallout(typeof(FD_MissileSmuggling));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_FakePassportPrintShop"))
            {
                Functions.RegisterCallout(typeof(FD_FakePassportPrintShop));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_ProtectedWitnessAmbush"))
            {
                Functions.RegisterCallout(typeof(FD_ProtectedWitnessAmbush));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_UndergroundCasino"))
            {
                Functions.RegisterCallout(typeof(FD_UndergroundCasino));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_OrganTraffickingClinic"))
            {
                Functions.RegisterCallout(typeof(FD_OrganTraffickingClinic));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_BlownCoverAgentExtraction"))
            {
                Functions.RegisterCallout(typeof(FD_BlownCoverAgentExtraction));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_DiamondCasinoHeistResponse"))
            {
                Functions.RegisterCallout(typeof(FD_DiamondCasinoHeistResponse));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_RocketLauncherC4Trafficking"))
            {
                Functions.RegisterCallout(typeof(FD_RocketLauncherC4Trafficking));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_DiplomatKidnappingVinewoodHills"))
            {
                Functions.RegisterCallout(typeof(FD_DiplomatKidnappingVinewoodHills));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_HitmenCellLuxuryHotel"))
            {
                Functions.RegisterCallout(typeof(FD_HitmenCellLuxuryHotel));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_WaterReservoirSabotage"))
            {
                Functions.RegisterCallout(typeof(FD_WaterReservoirSabotage));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("FD_UndergroundArtAuction"))
            {
                Functions.RegisterCallout(typeof(FD_UndergroundArtAuction));
                registered++;
            }

            Game.LogTrivial(string.Format("TacticalAndFederalCallouts: {0} de 42 llamadas registradas en el despachador.", registered));
            return registered;
        }
    }
}
