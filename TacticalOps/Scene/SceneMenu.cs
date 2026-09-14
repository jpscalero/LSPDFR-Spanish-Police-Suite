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
            pool = new MenuPool();
            menu = new UIMenu("GESTION DE ESCENAS", "~b~BALIZAMIENTO Y CONTROL DE TRAFICO");
            pool.Add(menu);

            UIMenuItem itemCono = new UIMenuItem("Colocar Cono de Trafico", "Despliega un cono reflectante de senalizacion vial (prop_roadcone02a).");
            UIMenuItem itemBarreraObra = new UIMenuItem("Colocar Barrera de Obra", "Despliega una valla movil de obra con bandas reflectantes (prop_barrier_work06a).");
            UIMenuItem itemBarreraJersey = new UIMenuItem("Colocar Barrera New Jersey", "Despliega una barrera de hormigon New Jersey para cortes pesados (prop_barricade_01a).");
            UIMenuItem itemBengala = new UIMenuItem("Colocar Bengala de Emergencia", "Enciende y coloca una bengala luminosa de advertencia nocturna (prop_flare_01).");
            UIMenuItem itemDesvio = new UIMenuItem("Activar Desvio de Trafico", "Reduce la velocidad del trafico local y obliga a los vehiculos a frenar o desviarse.");
            UIMenuItem itemRetirar = new UIMenuItem("Retirar Todo el Material", "Limpia y retira todos los conos, barreras y senales desplegadas en la zona.");

            menu.AddItem(itemCono);
            menu.AddItem(itemBarreraObra);
            menu.AddItem(itemBarreraJersey);
            menu.AddItem(itemBengala);
            menu.AddItem(itemDesvio);
            menu.AddItem(itemRetirar);

            menu.OnItemSelect += delegate(UIMenu sender, UIMenuItem selectedItem, int index)
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
            };

            Game.LogTrivial("TacticalOps: SceneMenu inicializado.");

            while (true)
            {
                GameFiber.Yield();

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
        }
    }
}
