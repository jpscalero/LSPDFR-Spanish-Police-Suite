using System;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using TacticalOps.Core;

namespace TacticalOps.CQB
{
    public static class TacticalEntryMenu
    {
        private static MenuPool pool;
        private static UIMenu menu;

        public static void Process()
        {
            try
            {
                pool = new MenuPool();
                menu = new UIMenu("ENTRADA TACTICA CQB", "~b~PROTOCOLO DE ASALTO CERRADO");
                pool.Add(menu);

                UIMenuItem itemStack = new UIMenuItem("Ordenar Formacion en Stack", "Alinea a los agentes cercanos en columna de asalto pegados a la pared del punto de entrada.");
                UIMenuItem itemAriete = new UIMenuItem("Brecha con Ariete Tactico", "Fuerza y rompe cerraduras mediante impacto mecanico continuo con ariete.");
                UIMenuItem itemEscopeta = new UIMenuItem("Brecha con Escopeta de Asalto", "Destruye bisagras y pasadores disparando cartuchos de brecha balistica.");
                UIMenuItem itemExplosivo = new UIMenuItem("Brecha con Carga Explosiva", "Fija y detona una carga lineal de corte para demoler el acceso al instante.");
                UIMenuItem itemFlashbang = new UIMenuItem("Lanzar Granada Aturdidora (Flashbang)", "Lanza una granada de destello y sonido para cegar y desarmar a los ocupantes.");
                UIMenuItem itemGas = new UIMenuItem("Lanzar Gas Lacrimogeno", "Despliega una cortina de gas CS irritante para forzar la rendicion de amenazas.");
                UIMenuItem itemAsalto = new UIMenuItem("¡Entrada Dinamica! (Asalto)", "Ordena la penetracion inmediata en abanico para despejar sectores y neutralizar amenazas.");

                menu.AddItem(itemStack);
                menu.AddItem(itemAriete);
                menu.AddItem(itemEscopeta);
                menu.AddItem(itemExplosivo);
                menu.AddItem(itemFlashbang);
                menu.AddItem(itemGas);
                menu.AddItem(itemAsalto);

                menu.OnItemSelect += delegate(UIMenu sender, UIMenuItem selectedItem, int index)
                {
                    try
                    {
                        pool.CloseAllMenus();

                        if (selectedItem == itemStack)
                        {
                            StackFormation.FormStack();
                        }
                        else if (selectedItem == itemAriete)
                        {
                            BreachSystem.ExecuteBatteringRam();
                        }
                        else if (selectedItem == itemEscopeta)
                        {
                            BreachSystem.ExecuteShotgunBreach();
                        }
                        else if (selectedItem == itemExplosivo)
                        {
                            BreachSystem.ExecuteExplosiveBreach();
                        }
                        else if (selectedItem == itemFlashbang)
                        {
                            BreachSystem.ThrowFlashbang();
                        }
                        else if (selectedItem == itemGas)
                        {
                            BreachSystem.ThrowTearGas();
                        }
                        else if (selectedItem == itemAsalto)
                        {
                            RoomClearingAI.ExecuteDynamicEntry();
                        }
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("TacticalOps.TacticalEntryMenu.OnItemSelect: " + ex.Message);
                    }
                };

                Game.LogTrivial("TacticalOps: TacticalEntryMenu inicializado.");

                while (true)
                {
                    GameFiber.Yield();

                    try
                    {
                        if (Game.IsKeyDown(ConfigManager.CqbKey))
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
                        Game.LogTrivial("TacticalOps.TacticalEntryMenu.Process loop: " + ex.Message);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException) { }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps.TacticalEntryMenu.Process fatal: " + ex.Message);
            }
        }
    }
}
