using System;
using System.Collections.Generic;
using Rage;
using LSPD_First_Response.Mod.API;

namespace AlternativeCallouts.Core
{
    public static class CalloutRegistry
    {
        private static readonly List<Type> RegisteredCallouts = new List<Type>();

        public static void RegisterAllCallouts()
        {
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_DisputaVecinalRuido));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_HurtoCentroComercial));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_OkupacionVivienda));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_ControlAlcoholemiaDrogas));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_PeleaDiscoteca));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_IntentoSuicidioPuente));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_FalsoPolicia));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_MaletinSospechosoMetro));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_PersonaDesorientadaDemencia));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.PatrullaUrbana.AL_SeguimientoMotoRobada));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.TraficoAutovias.AL_AccidenteMultipleAutovia));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.TraficoAutovias.AL_ConductorKamikaze));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.TraficoAutovias.AL_PerdidaCargaCamion));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.TraficoAutovias.AL_ControlVelocidadRadar));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.TraficoAutovias.AL_VehiculoAveriadoCurva));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.TraficoAutovias.AL_CarreraIlegalPoligono));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.TraficoAutovias.AL_TransporteAnimalesIlegal));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.TraficoAutovias.AL_ConductorAgresivoRoadRage));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.RuralSeprona.AL_CazaFurtivaParque));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.RuralSeprona.AL_IncendioForestalSospechoso));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.RuralSeprona.AL_RoboGanadoFinca));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.RuralSeprona.AL_SenderistaExtraviado));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.RuralSeprona.AL_VertidoResiduosToxicos));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.RuralSeprona.AL_PlantacionMarihuanaExterior));
            RegisterCallout(typeof(AlternativeCallouts.Callouts.RuralSeprona.AL_PescadoresIlegalesFluviales));
            Game.LogTrivial("AlternativeCallouts: Se han registrado con exito " + RegisteredCallouts.Count + " llamadas alternativas de 5 ramas.");
        }

        private static void RegisterCallout(Type calloutType)
        {
            try
            {
                Functions.RegisterCallout(calloutType);
                RegisteredCallouts.Add(calloutType);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("AlternativeCallouts: Error al registrar llamada " + calloutType.Name + ": " + ex.Message);
            }
        }
    }
}
