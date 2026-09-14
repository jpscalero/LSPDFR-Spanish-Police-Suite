using System;
using Rage;
using LSPD_First_Response.Mod.API;
using PeligroExtremoCallouts.Callouts.Tiroteos_Asaltos;
using PeligroExtremoCallouts.Callouts.Persecuciones;

namespace PeligroExtremoCallouts.Core
{
    /// <summary>
    /// Registro central de llamadas para PeligroExtremoCallouts (38 llamadas: 18 Tiroteos y Asaltos + 20 Persecuciones).
    /// </summary>
    public static class CalloutRegistry
    {
        public static int RegisterAllCallouts()
        {
            int registered = 0;

            // Registro de 18 Llamadas de Tiroteos y Asaltos
            if (ConfigManager.IsCalloutEnabled("SA_ConvenienceStoreHostageGetaway"))
            {
                Functions.RegisterCallout(typeof(SA_ConvenienceStoreHostageGetaway));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_ThreeWayGangWar"))
            {
                Functions.RegisterCallout(typeof(SA_ThreeWayGangWar));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_PatrolAmbush1099"))
            {
                Functions.RegisterCallout(typeof(SA_PatrolAmbush1099));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_NarcoLabFlammableDrums"))
            {
                Functions.RegisterCallout(typeof(SA_NarcoLabFlammableDrums));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_GasStationShootout"))
            {
                Functions.RegisterCallout(typeof(SA_GasStationShootout));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_JewelryStoreSmokeBombs"))
            {
                Functions.RegisterCallout(typeof(SA_JewelryStoreSmokeBombs));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_DarkWarehouseShootout"))
            {
                Functions.RegisterCallout(typeof(SA_DarkWarehouseShootout));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_TrafficStopPointBlankAmbush"))
            {
                Functions.RegisterCallout(typeof(SA_TrafficStopPointBlankAmbush));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_NightclubShootout"))
            {
                Functions.RegisterCallout(typeof(SA_NightclubShootout));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_LiquorStoreRobbery"))
            {
                Functions.RegisterCallout(typeof(SA_LiquorStoreRobbery));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_DeadEndAlleyShootout"))
            {
                Functions.RegisterCallout(typeof(SA_DeadEndAlleyShootout));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_AmmunationRobbery"))
            {
                Functions.RegisterCallout(typeof(SA_AmmunationRobbery));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_DelPerroPierShootout"))
            {
                Functions.RegisterCallout(typeof(SA_DelPerroPierShootout));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_GruppeSechsArmoredTruckHeist"))
            {
                Functions.RegisterCallout(typeof(SA_GruppeSechsArmoredTruckHeist));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_UndergroundParkingShootout"))
            {
                Functions.RegisterCallout(typeof(SA_UndergroundParkingShootout));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_PharmacyRobbery"))
            {
                Functions.RegisterCallout(typeof(SA_PharmacyRobbery));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_CarWashTunnelShootout"))
            {
                Functions.RegisterCallout(typeof(SA_CarWashTunnelShootout));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("SA_TollBoothShootout"))
            {
                Functions.RegisterCallout(typeof(SA_TollBoothShootout));
                registered++;
            }

            // Registro de 20 Llamadas de Persecuciones Peligrosas
            if (ConfigManager.IsCalloutEnabled("PC_HitmenVanDriveByPursuit"))
            {
                Functions.RegisterCallout(typeof(PC_HitmenVanDriveByPursuit));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_SupercarWrongWayPursuit"))
            {
                Functions.RegisterCallout(typeof(PC_SupercarWrongWayPursuit));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_ArmoredRammingTruck"))
            {
                Functions.RegisterCallout(typeof(PC_ArmoredRammingTruck));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_ArmedEscortConvoyRam"))
            {
                Functions.RegisterCallout(typeof(PC_ArmedEscortConvoyRam));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_OffroadMountainRiverChase"))
            {
                Functions.RegisterCallout(typeof(PC_OffroadMountainRiverChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_ShotgunBikerAlleyChase"))
            {
                Functions.RegisterCallout(typeof(PC_ShotgunBikerAlleyChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_CarBombFleeingDowntown"))
            {
                Functions.RegisterCallout(typeof(PC_CarBombFleeingDowntown));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_ShootingPlaneTakeoffAttempt"))
            {
                Functions.RegisterCallout(typeof(PC_ShootingPlaneTakeoffAttempt));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_CoastalSpeedboatChase"))
            {
                Functions.RegisterCallout(typeof(PC_CoastalSpeedboatChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_ArmoredLimoSunroofShooters"))
            {
                Functions.RegisterCallout(typeof(PC_ArmoredLimoSunroofShooters));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_DrugMuleCaltropsChase"))
            {
                Functions.RegisterCallout(typeof(PC_DrugMuleCaltropsChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_HeavyCraneRammingChase"))
            {
                Functions.RegisterCallout(typeof(PC_HeavyCraneRammingChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_OverpoweredMuscleCarChase"))
            {
                Functions.RegisterCallout(typeof(PC_OverpoweredMuscleCarChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_FlammableTankerTruckChase"))
            {
                Functions.RegisterCallout(typeof(PC_FlammableTankerTruckChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_MonsterTruckCrushingChase"))
            {
                Functions.RegisterCallout(typeof(PC_MonsterTruckCrushingChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_StolenTrackedMilitaryVehicleChase"))
            {
                Functions.RegisterCallout(typeof(PC_StolenTrackedMilitaryVehicleChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_UltralightKartsTunnelChase"))
            {
                Functions.RegisterCallout(typeof(PC_UltralightKartsTunnelChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_HostileHelicopterSupportChase"))
            {
                Functions.RegisterCallout(typeof(PC_HostileHelicopterSupportChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_ExcavatorDemolitionChase"))
            {
                Functions.RegisterCallout(typeof(PC_ExcavatorDemolitionChase));
                registered++;
            }
            if (ConfigManager.IsCalloutEnabled("PC_ArmoredSportscarSteelRamChase"))
            {
                Functions.RegisterCallout(typeof(PC_ArmoredSportscarSteelRamChase));
                registered++;
            }

            return registered;
        }
    }
}
