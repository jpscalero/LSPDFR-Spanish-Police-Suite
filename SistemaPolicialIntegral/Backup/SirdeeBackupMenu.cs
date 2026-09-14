using System;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SistemaPolicialIntegral.Core;
using SistemaPolicialIntegral.Services;

namespace SistemaPolicialIntegral.Backup
{
    public class SirdeeBackupMenu
    {
        private MenuPool pool;
        private UIMenu menu;

        public SirdeeBackupMenu()
        {
            pool = new MenuPool();
            menu = new UIMenu("~b~COMUNICACIÓN SIRDEE", "~w~Centro de Mando y Control");
            pool.Add(menu);

            UIMenuItem itemZeta = new UIMenuItem("Patrulla ZETA (091/062)", "1 coche patrulla + 2 agentes uniformados");
            UIMenuItem itemTrafico = new UIMenuItem("Atestados Tráfico ATGC", "1 todoterreno Guardia Civil tráfico + 2 agentes");
            UIMenuItem itemUpr = new UIMenuItem("UPR Antidisturbios", "1 furgón + 4 agentes con escudo/defensa");
            UIMenuItem itemGeo = new UIMenuItem("GEO / UEI Táctico", "1 furgón blindado + 4 operadores de élite con fusiles");
            UIMenuItem itemK9 = new UIMenuItem("K9 Guías Caninos", "1 coche + 1 agente + 1 pastor alemán");
            UIMenuItem itemHelicoptero = new UIMenuItem("Helicóptero Cóndor", "1 helicóptero policial con foco");
            UIMenuItem itemGrua = new UIMenuItem("Grúa Municipal", "Llama a Grúa Municipal");
            UIMenuItem itemAmbulancia = new UIMenuItem("Ambulancia SAMUR 061", "Llama a Ambulancia SAMUR");
            UIMenuItem itemCientifica = new UIMenuItem("Policía Científica", "Llama a Policía Científica");

            UIMenuItem itemSpikes = new UIMenuItem("Desplegar Banda de Clavos", "Lanza una banda de clavos punzante frente a ti.");
            UIMenuItem itemRemoveSpikes = new UIMenuItem("Retirar Banda de Clavos", "Retira todas las bandas de clavos colocadas.");
            UIMenuItem itemRoadblock = new UIMenuItem("Bloqueo de Carretera Táctico", "Establece un corte de vía con patrullas cruzadas y agentes GEO.");
            UIMenuItem itemClearRoadblock = new UIMenuItem("Levantar Bloqueo de Carretera", "Desmantela el control y corte de carretera activo.");
            UIMenuItem itemPit = new UIMenuItem("Ejecutar Maniobra PIT", "Aplica un impulso de desestabilización en el eje trasero del fugitivo.");
            UIMenuItem itemBoxing = new UIMenuItem("Coordinar Maniobra de Caja", "Ordena a los patrulleros cercar y encerrar el vehículo en fuga.");
            UIMenuItem itemDismissBackup = new UIMenuItem("~g~Despedir / Cancelar Apoyos (Codigo 4)", "Libera a todas las unidades de apoyo y borra sus marcas del radar.");

            menu.AddItem(itemZeta);
            menu.AddItem(itemTrafico);
            menu.AddItem(itemUpr);
            menu.AddItem(itemGeo);
            menu.AddItem(itemK9);
            menu.AddItem(itemHelicoptero);
            menu.AddItem(itemGrua);
            menu.AddItem(itemAmbulancia);
            menu.AddItem(itemCientifica);
            menu.AddItem(itemSpikes);
            menu.AddItem(itemRemoveSpikes);
            menu.AddItem(itemRoadblock);
            menu.AddItem(itemClearRoadblock);
            menu.AddItem(itemPit);
            menu.AddItem(itemBoxing);
            menu.AddItem(itemDismissBackup);

            menu.OnItemSelect += OnItemSelect;

            GameFiber.StartNew(delegate
            {
                try
                {
                    ProcessMenu();
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.SirdeeBackupMenu ProcessMenu fiber error: " + ex.Message);
                }
            });
        }

        private void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            try
            {
                menu.Visible = false;

                if (index == 9)
                {
                    SpikeStripManager.DeploySpikeStrips();
                    PoliceSuite.Common.SpanishVoiceDispatchEngine.AnnounceSpikesDeployed();
                    return;
                }
                if (index == 10)
                {
                    SpikeStripManager.RemoveSpikeStrips();
                    PoliceSuite.Common.SpanishVoiceDispatchEngine.SpeakOfficer("Bandas de pinchos retiradas de la calzada.");
                    return;
                }
                if (index == 11)
                {
                    RoadblockManager.RequestTacticalRoadblock();
                    return;
                }
                if (index == 12)
                {
                    RoadblockManager.ClearRoadblock();
                    PoliceSuite.Common.SpanishVoiceDispatchEngine.SpeakOfficer("Bloqueo de carretera levantado. Circulación restablecida.");
                    return;
                }
                if (index == 13)
                {
                    PursuitTacticsManager.PerformPitManeuver();
                    return;
                }
                if (index == 14)
                {
                    PursuitTacticsManager.OrderBoxingIn();
                    return;
                }
                if (index == 15)
                {
                    BackupLifecycleManager.DismissAllBackup();
                    return;
                }

                AudioEngine.PlaySirdeeOpen();
                string unidad = selectedItem.Text;
                int eta = new Random().Next(10, 25);
                Game.DisplaySubtitle("~b~[SIRDEE]~w~ Solicitando " + unidad + "... ETA: " + eta + " segundos.", 5000);

                GameFiber.StartNew(delegate
                {
                    try
                    {
                        GameFiber.Sleep(eta * 1000);

                        if (index == 0) UnitSpawner.SpawnPatrullaZeta();
                        else if (index == 1) UnitSpawner.SpawnATGCTrafico();
                        else if (index == 2) UnitSpawner.SpawnUPRAntidisturbios();
                        else if (index == 3) UnitSpawner.SpawnGEOTactico();
                        else if (index == 4) UnitSpawner.SpawnK9Canino();
                        else if (index == 5) UnitSpawner.SpawnHelicopteroCondor();
                        else if (index == 6)
                        {
                            Vehicle closest = FindNearbyVehicle(10f);
                            if (closest != null && closest.Exists()) MunicipalFlatbedTow.RequestTow(closest);
                            else Game.DisplayNotification("~r~No hay vehiculo cercano para la grua.");
                        }
                        else if (index == 7) SamurAmbulanceService.RequestAmbulance(Game.LocalPlayer.Character.Position);
                        else if (index == 8) ForensicJudicialVan.RequestForensicUnit(Game.LocalPlayer.Character.Position);

                        AudioEngine.PlaySirdeeClose();
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("SPI.SirdeeBackupMenu.OnItemSelect fiber error: " + ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                Game.LogTrivial("SPI.SirdeeBackupMenu OnItemSelect error: " + ex.Message);
            }
        }

        private void ProcessMenu()
        {
            while (true)
            {
                GameFiber.Yield();

                try
                {
                    pool.ProcessMenus();
                    
                    if (Game.IsKeyDown(ConfigManager.BackupKey))
                    {
                        menu.Visible = !menu.Visible;
                    }
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.SirdeeBackupMenu loop error: " + ex.Message);
                }
            }
        }
        private static Vehicle FindNearbyVehicle(float radius)
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
