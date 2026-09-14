using System;
using Rage;
using LSPD_First_Response.Mod.API;
using MisionesEspeciales.CrimenOrg;
using MisionesEspeciales.SEPRONA;
using MisionesEspeciales.Aeropuerto;
using MisionesEspeciales.ViolDom;
using MisionesEspeciales.Eventos;

namespace MisionesEspeciales
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            try
            {
                Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
                Game.LogTrivial("MisionesEspeciales: Plugin inicializado con 27 llamadas policiales de élite.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("MisionesEspeciales: Error critico en Initialize: " + ex.Message);
            }
        }

        public override void Finally()
        {
            try
            {
                Game.LogTrivial("MisionesEspeciales: Plugin descargado.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("MisionesEspeciales: Error en Finally: " + ex.Message);
            }
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
            {
                try
                {
                    // Registro de llamadas de Crimen Organizado (8)
                    Functions.RegisterCallout(typeof(LaboratorioMeta));
                    Functions.RegisterCallout(typeof(AlijoPuerto));
                    Functions.RegisterCallout(typeof(RedTrata));
                    Functions.RegisterCallout(typeof(BlanqueoCapitales));
                    Functions.RegisterCallout(typeof(AjusteCuentas));
                    Functions.RegisterCallout(typeof(SecuestroExpres));
                    Functions.RegisterCallout(typeof(AtentadoTerrorista));
                    Functions.RegisterCallout(typeof(HackeoBancario));

                    // Registro de llamadas de SEPRONA (6)
                    Functions.RegisterCallout(typeof(CazaFurtiva));
                    Functions.RegisterCallout(typeof(VertidoToxico));
                    Functions.RegisterCallout(typeof(IncendioForestal));
                    Functions.RegisterCallout(typeof(MaltratoAnimal));
                    Functions.RegisterCallout(typeof(PescaIlegal));
                    Functions.RegisterCallout(typeof(TraficoEspecies));

                    // Registro de llamadas de Policía Aeroportuaria (5)
                    Functions.RegisterCallout(typeof(AmenazaBombaAeropuerto));
                    Functions.RegisterCallout(typeof(PasajeroConflictivo));
                    Functions.RegisterCallout(typeof(MulaDroga));
                    Functions.RegisterCallout(typeof(DocumentoFalso));
                    Functions.RegisterCallout(typeof(EscoltaVIP));

                    // Registro de llamadas de Violencia Doméstica UFAM (4)
                    Functions.RegisterCallout(typeof(DenunciaMaltrato));
                    Functions.RegisterCallout(typeof(Quebrantamiento));
                    Functions.RegisterCallout(typeof(DesaparicionMenor));
                    Functions.RegisterCallout(typeof(AmenazasConArma));

                    // Registro de llamadas de Eventos Masivos (4)
                    Functions.RegisterCallout(typeof(Manifestacion));
                    Functions.RegisterCallout(typeof(PartidoFutbol));
                    Functions.RegisterCallout(typeof(Concierto));
                    Functions.RegisterCallout(typeof(VisitaEstado));

                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~MISIONES ESPECIALES", "~g~SERVICIO ACTIVO", "~w~27 llamadas policiales de élite registradas con éxito.");
                    Game.LogTrivial("MisionesEspeciales: 27 llamadas registradas correctamente en LSPDFR.");
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("MisionesEspeciales: Error al registrar llamadas: " + ex.Message);
                }
            }
        }
    }
}
