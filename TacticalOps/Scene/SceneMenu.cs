using System;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using TacticalOps.Core;

namespace TacticalOps.Scene
{
    public static class SceneMenu
    {
        private static MenuPool pool;
        private static UIMenu menu;

        public static void Process()
        {
            try
            {
                pool = new MenuPool();
                menu = new UIMenu("GESTION DE ESCENAS", "~b~PERIMETRO Y BALIZAMIENTO VIAL");
                pool.Add(menu);

                UIMenuItem itemCono = new UIMenuItem("Colocar Cono Reflectante", "Despliega un cono de señalizacion vial a 1.5 metros delante del agente.");
                UIMenuItem itemBarreraObra = new UIMenuItem("Colocar Valla de Contencion", "Despliega una valla amarilla de corte peatonal y control de accesos.");
                UIMenuItem itemBarreraJersey = new UIMenuItem("Colocar Barrera de Hormigon", "Despliega una barrera rigida antialunizaje y proteccion perimetral.");
                UIMenuItem itemBengala = new UIMenuItem("Desplegar Bengala Luminosa", "Enciende y deposita en el suelo una bengala quimica de emergencia.");
                UIMenuItem itemDesvio = new UIMenuItem("Activar Desvio de Trafico", "Frena y desvia el trafico rodado en un radio de 25 metros de la escena.");
                UIMenuItem itemRetirar = new UIMenuItem("~r~Retirar Todo el Material", "Limpia y elimina todos los conos, vallas y bengalas colocados.");

                menu.AddItem(itemCono);
                menu.AddItem(itemBarreraObra);
                menu.AddItem(itemBarreraJersey);
                menu.AddItem(itemBengala);
                menu.AddItem(itemDesvio);
                menu.AddItem(itemRetirar);

                menu.OnItemSelect += delegate(UIMenu sender, UIMenuItem selectedItem, int index)
                {
                    try
                    {
                        if (selectedItem == itemCono)
                        {
                            SceneObjectPlacer.PlaceObject("prop_roadcone02a");
                        }
                        else if (selectedItem == itemBarreraObra)
                        {
                            SceneObjectPlacer.PlaceObject("prop_barrier_work06a");
                        }
                        else if (selectedItem == itemBarreraJersey)
                        {
                            SceneObjectPlacer.PlaceObject("prop_barricade_01a");
                        }
                        else if (selectedItem == itemBengala)
                        {
                            SceneObjectPlacer.PlaceObject("prop_flare_01");
                        }
                        else if (selectedItem == itemDesvio)
                        {
                            TrafficController.IsDiversionActive = !TrafficController.IsDiversionActive;
                            if (TrafficController.IsDiversionActive)
                            {
                                itemDesvio.Text = "Desactivar Desvio de Trafico";
                                Game.DisplayNotification("~g~Desvio de trafico activado.~w~ Densidad reducida y perimetro acordonado.");
                                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                            }
                            else
                            {
                                itemDesvio.Text = "Activar Desvio de Trafico";
                                Game.DisplayNotification("~y~Desvio de trafico desactivado.~w~ Circulacion normal restablecida.");
                                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CANCEL", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                            }
                        }
                        else if (selectedItem == itemRetirar)
                        {
                            SceneObjectPlacer.RemoveAllObjects();
                            TrafficController.IsDiversionActive = false;
                            itemDesvio.Text = "Activar Desvio de Trafico";
                        }
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("TacticalOps.SceneMenu.OnItemSelect: " + ex.Message);
                    }
                };

                Game.LogTrivial("TacticalOps: SceneMenu inicializado.");

                while (true)
                {
                    GameFiber.Yield();

                    try
                    {
                        if (Game.IsKeyDown(ConfigManager.SceneKey))
                        {
                            if (!pool.IsAnyMenuOpen())
                            {
                                menu.Visible = true;
                            }
                            else
                            {
                                pool.CloseAllMenus();
                            }
                        }

                        pool.ProcessMenus();
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("TacticalOps.SceneMenu.Process loop: " + ex.Message);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException) { }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps.SceneMenu.Process fatal: " + ex.Message);
            }
        }
    }
}
