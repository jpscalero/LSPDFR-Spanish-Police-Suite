using System;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SistemaPolicialIntegral.Core;
using LSPD_First_Response.Mod.API;

namespace SistemaPolicialIntegral.Intervention
{
    public static class InterventionMenu
    {
        private static MenuPool pool;
        private static UIMenu menu;
        private static Ped targetPed;

        public static void Process()
        {
            pool = new MenuPool();
            menu = new UIMenu("INTERVENCION POLICIAL", "~b~ACCIONES DE IDENTIFICACION Y CONTROL");
            pool.Add(menu);

            UIMenuItem itemDni = new UIMenuItem("Identificar (DNI/NIE)", "Solicita la documentacion al ciudadano.");
            UIMenuItem itemAlcoholemia = new UIMenuItem("Prueba de Alcoholemia (Drager)", "Realiza la prueba de alcoholemia en aire espirado.");
            UIMenuItem itemDrogas = new UIMenuItem("Test de Drogas Salival", "Realiza un test de estupefacientes (DrugTest 5000).");
            UIMenuItem itemCacheo = new UIMenuItem("Cacheo de Seguridad", "Realiza un cacheo superficial en busca de armas o ilicitos.");
            UIMenuItem itemRegistroVeh = new UIMenuItem("Registro de Vehiculo", "Registra el vehiculo mas cercano.");
            UIMenuItem itemDerechos = new UIMenuItem("Lectura de Derechos", "Informa al detenido de sus derechos (Art. 520 LECrim).");
            UIMenuItem itemDetener = new UIMenuItem("Detener y Engrilletar", "Practica la detencion y pone los grilletes.");
            
            // Nuevas opciones StopThePed 100%
            UIMenuItem itemEscoltar = new UIMenuItem("Agarrar / Soltar del Brazo", "Sujeta al detenido por el brazo para escoltarlo caminando.");
            UIMenuItem itemMeterPatrulla = new UIMenuItem("Introducir en Coche Patrulla", "Sienta al detenido en la parte trasera del patrullero mas cercano.");
            UIMenuItem itemSacarPatrulla = new UIMenuItem("Sacar del Coche Patrulla", "Apea al detenido del coche patrulla.");
            UIMenuItem itemArrodillar = new UIMenuItem("Ordenar Arrodillarse", "Hace que el sospechoso se arrodille con manos a la espalda.");
            UIMenuItem itemSentar = new UIMenuItem("Ordenar Sentarse en Bordillo", "Hace que el sospechoso se siente en el suelo o bordillo.");
            UIMenuItem itemPonerPie = new UIMenuItem("Ordenar Ponerse de Pie", "Hace que el sospechoso se levante.");
            UIMenuItem itemVentanilla = new UIMenuItem("Interactuar en Ventanilla", "Baja la ventanilla del coche mas cercano y solicita papeles.");
            UIMenuItem itemBajarCoche = new UIMenuItem("Ordenar Apearse del Vehiculo", "Ordena al conductor detenido que descienda del vehiculo.");

            UIMenuItem itemLiberar = new UIMenuItem("Dejar ir", "Finaliza la intervencion y permite que se marche.");

            menu.AddItem(itemDni);
            menu.AddItem(itemAlcoholemia);
            menu.AddItem(itemDrogas);
            menu.AddItem(itemCacheo);
            menu.AddItem(itemRegistroVeh);
            menu.AddItem(itemDerechos);
            menu.AddItem(itemDetener);
            menu.AddItem(itemEscoltar);
            menu.AddItem(itemMeterPatrulla);
            menu.AddItem(itemSacarPatrulla);
            menu.AddItem(itemArrodillar);
            menu.AddItem(itemSentar);
            menu.AddItem(itemPonerPie);
            menu.AddItem(itemVentanilla);
            menu.AddItem(itemBajarCoche);
            menu.AddItem(itemLiberar);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (selectedItem == itemDni)
                {
                    if (targetPed != null && targetPed.Exists()) DniVerification.VerifyIdentity(targetPed);
                }
                else if (selectedItem == itemAlcoholemia)
                {
                    if (targetPed != null && targetPed.Exists()) AlcoholimeterDrager.PerformTest(targetPed);
                }
                else if (selectedItem == itemDrogas)
                {
                    if (targetPed != null && targetPed.Exists()) DrugTest5000.PerformTest(targetPed);
                }
                else if (selectedItem == itemCacheo)
                {
                    if (targetPed != null && targetPed.Exists()) FriskSearchManager.FriskPed(targetPed);
                }
                else if (selectedItem == itemRegistroVeh)
                {
                    Vehicle veh = FindClosestVehicle(6f);
                    if (veh != null && veh.Exists()) FriskSearchManager.SearchVehicle(veh);
                    else Game.DisplayNotification("~r~No hay ningun vehiculo lo suficientemente cerca.");
                }
                else if (selectedItem == itemDerechos)
                {
                    if (targetPed != null && targetPed.Exists()) RightsManager.ReadRights(targetPed);
                }
                else if (selectedItem == itemDetener)
                {
                    if (targetPed != null && targetPed.Exists())
                    {
                        NativeFunction.Natives.SET_ENABLE_HANDCUFFS(targetPed, true);
                        targetPed.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                        Game.DisplayNotification("~b~Sujeto engrilletado.");
                    }
                }
                else if (selectedItem == itemEscoltar)
                {
                    if (targetPed != null && targetPed.Exists())
                    {
                        PedControlManager.ToggleGrabPed(targetPed);
                    }
                    else if (PedControlManager.IsEscorting)
                    {
                        PedControlManager.ReleaseGrabbedPed();
                    }
                }
                else if (selectedItem == itemMeterPatrulla)
                {
                    Vehicle veh = FindClosestVehicle(8f);
                    Ped p = PedControlManager.GrabbedPed != null ? PedControlManager.GrabbedPed : targetPed;
                    if (p != null && p.Exists() && veh != null && veh.Exists())
                    {
                        PedControlManager.PutInVehicle(p, veh);
                    }
                    else
                    {
                        Game.DisplayNotification("~r~Se requiere un detenido y un vehiculo patrulla cercano.");
                    }
                }
                else if (selectedItem == itemSacarPatrulla)
                {
                    Vehicle veh = FindClosestVehicle(8f);
                    if (veh != null && veh.Exists())
                    {
                        PedControlManager.RemoveFromVehicle(veh);
                    }
                    else
                    {
                        Game.DisplayNotification("~r~No hay patrullero cerca.");
                    }
                }
                else if (selectedItem == itemArrodillar)
                {
                    if (targetPed != null && targetPed.Exists()) PedControlManager.MakePedKneel(targetPed);
                }
                else if (selectedItem == itemSentar)
                {
                    if (targetPed != null && targetPed.Exists()) PedControlManager.MakePedSitOnCurb(targetPed);
                }
                else if (selectedItem == itemPonerPie)
                {
                    if (targetPed != null && targetPed.Exists()) PedControlManager.MakePedStandUp(targetPed);
                }
                else if (selectedItem == itemVentanilla)
                {
                    Vehicle veh = FindClosestVehicle(5f);
                    if (veh != null && veh.Exists()) TrafficStopInteraction.InteractWithDriver(veh);
                    else Game.DisplayNotification("~r~No hay ningun vehiculo detenido cerca.");
                }
                else if (selectedItem == itemBajarCoche)
                {
                    Vehicle veh = FindClosestVehicle(5f);
                    if (veh != null && veh.Exists()) TrafficStopInteraction.OrderDriverExit(veh);
                }
                else if (selectedItem == itemLiberar)
                {
                    if (targetPed != null && targetPed.Exists())
                    {
                        if (PedControlManager.IsEscorting && PedControlManager.GrabbedPed == targetPed)
                        {
                            PedControlManager.ReleaseGrabbedPed();
                        }
                        NativeFunction.Natives.SET_ENABLE_HANDCUFFS(targetPed, false);
                        targetPed.Tasks.Clear();
                        targetPed.Dismiss();
                        Game.DisplayNotification("~g~Sujeto liberado.");
                    }
                }
                menu.Visible = false;
            };

            while (true)
            {
                GameFiber.Yield();

                if (Game.IsKeyDown(ConfigManager.InterventionKey))
                {
                    if (!menu.Visible)
                    {
                        Ped[] peds = Game.LocalPlayer.Character.GetNearbyPeds(10);
                        targetPed = null;
                        float closestDist = 9999f;
                        foreach (Ped p in peds)
                        {
                            if (p != null && p.Exists() && p != Game.LocalPlayer.Character && p.IsHuman)
                            {
                                float dist = Game.LocalPlayer.Character.DistanceTo(p);
                                if (dist < 4f && dist < closestDist)
                                {
                                    closestDist = dist;
                                    targetPed = p;
                                }
                            }
                        }

                        Vehicle nearbyVeh = FindClosestVehicle(5f);

                        // Si hay un ped cerca O un vehiculo cerca O estamos escoltando, abrir menu
                        if (targetPed != null || nearbyVeh != null || PedControlManager.IsEscorting)
                        {
                            menu.Visible = true;
                        }
                        else
                        {
                            Game.DisplayNotification("~y~No hay nadie ni ningun vehiculo lo suficientemente cerca para intervenir.");
                        }
                    }
                    else
                    {
                        menu.Visible = false;
                    }
                }

                pool.ProcessMenus();
            }
        }

        private static Vehicle FindClosestVehicle(float radius)
        {
            Entity[] entities = World.GetEntities(Game.LocalPlayer.Character.Position, radius, GetEntitiesFlags.ConsiderAllVehicles);
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
