using System;
using Rage;
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

            menu.AddItem(itemDni);
            menu.AddItem(itemMatricula);
            menu.AddItem(itemAntecedentes);
            menu.AddItem(itemRequisitorias);
            menu.AddItem(itemSanciones);
            menu.AddItem(itemAtestado);

            // Crear submenús manualmente
            UIMenu submenuSanciones = new UIMenu("SANCIONES DGT", "~y~Infracciones de trafico");
            pool.Add(submenuSanciones);
            menu.BindMenuToItem(submenuSanciones, itemSanciones);

            UIMenu submenuAtestados = new UIMenu("ATESTADO PENAL", "~r~Delitos del Codigo Penal");
            pool.Add(submenuAtestados);
            menu.BindMenuToItem(submenuAtestados, itemAtestado);

            // Poblar submenu de sanciones
            foreach (Charge charge in SpanishPenalCode.GetCharges())
            {
                if (charge.Gravedad == "Leve" || charge.Gravedad == "Grave" || charge.Gravedad == "Muy Grave")
                {
                    UIMenuItem cItem = new UIMenuItem("Art. " + charge.Articulo + " - " + charge.Descripcion, "Sancion: " + charge.Gravedad);
                    submenuSanciones.AddItem(cItem);
                }
            }

            // Poblar submenu de delitos
            foreach (Charge charge in SpanishPenalCode.GetCharges())
            {
                if (charge.Gravedad == "Delito")
                {
                    UIMenuItem cItem = new UIMenuItem("Art. " + charge.Articulo + " - " + charge.Descripcion, "Pena: " + charge.PenaMinima + " a " + charge.PenaMaxima);
                    submenuAtestados.AddItem(cItem);
                }
            }

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
