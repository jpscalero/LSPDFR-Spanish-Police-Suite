using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SistemaPolicialIntegral.Core;
using LSPD_First_Response.Mod.API;

namespace SistemaPolicialIntegral.MDT
{
    public static class SitelComputerMenu
    {
        private static MenuPool pool;
        private static UIMenu menu;

        public static void Process()
        {
            pool = new MenuPool();
            menu = new UIMenu("TERMINAL SITEL / DGT", "~b~CONSULTAS BASES DE DATOS POLICIALES");
            pool.Add(menu);

            UIMenuItem itemDni = new UIMenuItem("Consulta por DNI/NIE", "Busca datos del ciudadano mas cercano.");
            UIMenuItem itemMatricula = new UIMenuItem("Consulta por Matricula", "Busca datos del vehiculo mas cercano.");
            UIMenuItem itemAntecedentes = new UIMenuItem("Antecedentes Penales", "Consulta el registro de penados y rebeldes.");
            UIMenuItem itemRequisitorias = new UIMenuItem("Requisitorias Judiciales", "Consulta ordenes de busqueda y captura.");
            UIMenuItem itemSanciones = new UIMenuItem("Imponer Sancion de Trafico", "Acceso al cuadro de sanciones de la DGT.");
            UIMenuItem itemAtestado = new UIMenuItem("Redactar Atestado Penal", "Registro de delitos (Codigo Penal).");
            UIMenuItem itemManual = new UIMenuItem("📖 Manual Jurídico Policial", "Consulta interactiva del compendio legal y protocolos policiales.");

            menu.AddItem(itemDni);
            menu.AddItem(itemMatricula);
            menu.AddItem(itemAntecedentes);
            menu.AddItem(itemRequisitorias);
            menu.AddItem(itemSanciones);
            menu.AddItem(itemAtestado);
            menu.AddItem(itemManual);

            // Submenú de Sanciones DGT
            UIMenu submenuSanciones = new UIMenu("SANCIONES DGT", "~y~Infracciones de trafico y seguridad vial");
            pool.Add(submenuSanciones);
            menu.BindMenuToItem(submenuSanciones, itemSanciones);

            // Submenú de Delitos / Atestado Penal
            UIMenu submenuAtestados = new UIMenu("ATESTADO PENAL", "~r~Delitos del Codigo Penal");
            pool.Add(submenuAtestados);
            menu.BindMenuToItem(submenuAtestados, itemAtestado);

            // Submenú del Manual Jurídico Oficial
            UIMenu submenuManual = ManualJuridicoMenu.CreateMenu(pool);
            menu.BindMenuToItem(submenuManual, itemManual);

            // Poblar submenú de sanciones
            List<Charge> listaSanciones = new List<Charge>();
            foreach (Charge charge in SpanishPenalCode.GetCharges())
            {
                if (charge.Gravedad == "Leve" || charge.Gravedad == "Infraccion Leve" || charge.Gravedad == "Grave" || charge.Gravedad == "Muy Grave")
                {
                    listaSanciones.Add(charge);
                    string infoPuntos = charge.PuntosDGT > 0 ? " | -" + charge.PuntosDGT + " pts" : "";
                    string desc = charge.MultaEuros > 0 ? charge.MultaEuros + "€" + infoPuntos : charge.Gravedad;
                    UIMenuItem cItem = new UIMenuItem("Art. " + charge.Articulo + " - " + charge.Descripcion, "Sanción: " + desc);
                    submenuSanciones.AddItem(cItem);
                }
            }

            // Poblar submenú de delitos
            List<Charge> listaDelitos = new List<Charge>();
            foreach (Charge charge in SpanishPenalCode.GetCharges())
            {
                if (charge.Gravedad == "Delito" || charge.Gravedad == "Delito Grave")
                {
                    listaDelitos.Add(charge);
                    UIMenuItem cItem = new UIMenuItem("Art. " + charge.Articulo + " - " + charge.Descripcion, "Pena: " + charge.PenaMinima + " a " + charge.PenaMaxima);
                    submenuAtestados.AddItem(cItem);
                }
            }

            // Manejador de selección de Sanciones DGT
            submenuSanciones.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (index >= 0 && index < listaSanciones.Count)
                {
                    Charge sel = listaSanciones[index];
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);

                    string pts = sel.PuntosDGT > 0 ? " (" + sel.PuntosDGT + " puntos DGT)" : "";
                    string msg = string.Format("~w~Infracción: ~y~{0}~w~~n~Artículo: ~r~{1}~w~~n~Importe: ~g~{2} euros{3}~n~Instrucción: ~w~{4}",
                        sel.Descripcion, sel.Articulo, sel.MultaEuros, pts, sel.AccionOperativa);

                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~DGT - BOLETÍN TRAMITADO", "~g~Sanción Administrativa", msg);
                    Game.DisplaySubtitle("~b~[DGT LSV] ~w~Sanción notificada: Art. " + sel.Articulo + " (" + sel.MultaEuros + "€)", 4000);
                    Game.LogTrivial("SITEL: Boletín de denuncia impuesto por Art. " + sel.Articulo + " (" + sel.Descripcion + ")");
                }
            };

            // Manejador de selección de Atestados Penales
            submenuAtestados.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (index >= 0 && index < listaDelitos.Count)
                {
                    Charge sel = listaDelitos[index];
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);

                    string msg = string.Format("~w~Delito: ~y~{0}~w~~n~Tipificación: ~r~Art. {1} CP~w~~n~Pena legal: ~r~{2} a {3}~w~~n~Protocolo: ~b~{4}",
                        sel.Descripcion, sel.Articulo, sel.PenaMinima, sel.PenaMaxima, sel.AccionOperativa);

                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~ATESTADO JUDICIAL CNP", "~y~Diligencias Penales", msg);
                    Game.DisplaySubtitle("~r~[CÓDIGO PENAL] ~w~Delito imputado: Art. " + sel.Articulo + " CP - " + sel.Descripcion, 4000);
                    Game.LogTrivial("SITEL: Atestado penal iniciado por delito Art. " + sel.Articulo + " CP (" + sel.Descripcion + ")");
                }
            };

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                AudioEngine.PlayMenuSelect();
                Ped targetPed = GetClosestPed();

                if (selectedItem == itemDni)
                {
                    if (targetPed != null) SistemaPolicialIntegral.Intervention.DniVerification.VerifyIdentity(targetPed);
                    else Game.DisplayNotification("~r~Nadie cerca para consultar.");
                }
                else if (selectedItem == itemMatricula)
                {
                    Vehicle targetVeh = GetClosestVehicle();
                    if (targetVeh != null)
                    {
                        Game.DisplayNotification("~b~DGT - VEHICULO~n~~w~Matricula: " + targetVeh.LicensePlate + "~n~Modelo: " + targetVeh.Model.Name + "~n~Seguro: ~g~VIGENTE~n~~w~ITV: ~g~FAVORABLE");
                    }
                    else Game.DisplayNotification("~r~No hay vehiculo cerca.");
                }
                else if (selectedItem == itemAntecedentes)
                {
                    if (targetPed != null)
                    {
                        var persona = Functions.GetPersonaForPed(targetPed);
                        string ant = (persona != null && persona.Wanted) ? "~r~TIENE ANTECEDENTES" : "~g~LIMPIO";
                        string nombre = persona != null ? persona.FullName : "Desconocido";
                        Game.DisplayNotification("~b~REGISTRO CENTRAL~n~~w~Ciudadano: " + nombre + "~n~Estado: " + ant);
                    }
                }
                else if (selectedItem == itemRequisitorias)
                {
                    if (targetPed != null) JudicialWarrants.CheckWarrants(targetPed);
                }
            };

            while (true)
            {
                GameFiber.Yield();

                try
                {
                    if (Game.IsKeyDown(ConfigManager.MdtKey))
                    {
                        if (!menu.Visible)
                        {
                            menu.Visible = true;
                        }
                        else
                        {
                            menu.Visible = false;
                        }
                    }

                    pool.ProcessMenus();
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.SitelComputerMenu loop error: " + ex.Message);
                }
            }
        }

        private static Ped GetClosestPed()
        {
            Ped[] peds = Game.LocalPlayer.Character.GetNearbyPeds(10);
            Ped targetPed = null;
            float closestDist = 9999f;
            foreach (Ped p in peds)
            {
                if (p != null && p.Exists() && p != Game.LocalPlayer.Character && p.IsHuman)
                {
                    float dist = Game.LocalPlayer.Character.DistanceTo(p);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        targetPed = p;
                    }
                }
            }
            return targetPed;
        }

        private static Vehicle GetClosestVehicle()
        {
            Entity[] entities = World.GetEntities(Game.LocalPlayer.Character.Position, 10f, GetEntitiesFlags.ConsiderAllVehicles);
            Vehicle closest = null;
            float minDist = float.MaxValue;
            if (entities != null)
            {
                foreach (Entity e in entities)
                {
                    Vehicle v = e as Vehicle;
                    if (v != null && v.Exists())
                    {
                        float d = Game.LocalPlayer.Character.DistanceTo(v);
                        if (d < minDist)
                        {
                            minDist = d;
                            closest = v;
                        }
                    }
                }
            }
            return closest;
        }
    }
}
