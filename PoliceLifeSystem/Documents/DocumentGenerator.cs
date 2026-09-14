using System;
using System.IO;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using PoliceLifeSystem.Core;
using PoliceLifeSystem.Economy;

namespace PoliceLifeSystem.Documents
{
    public static class DocumentGenerator
    {
        private const string DocsDirectory = "plugins/LSPDFR/SPI_Data/Documentos";
        private static MenuPool _pool;
        private static UIMenu _menu;

        public static void Process()
        {
            _pool = new MenuPool();
            _menu = new UIMenu("DOCUMENTACIÓN CNP", "~b~ATESTADOS, ACTAS E INFORMES OFICIALES");
            _pool.Add(_menu);

            UIMenuItem itemAtestado = new UIMenuItem("Instruir Atestado Policial", "Redacta un atestado penal por hechos delictivos y lo remite al Juzgado.");
            UIMenuItem itemAlcoholemia = new UIMenuItem("Acta de Detección Alcohólica", "Diligencia oficial de prueba etilométrica con etilómetro Dräger evidencial.");
            UIMenuItem itemInformeTurno = new UIMenuItem("Informe de Fin de Turno", "Genera el informe oficial de novedades y cierre de servicio de patrulla.");
            UIMenuItem itemTaller = new UIMenuItem("Taller de Automoción Policial", "Inspecciona y repara el vehículo patrulla actual deduciendo el coste reglamentario.");

            _menu.AddItem(itemAtestado);
            _menu.AddItem(itemAlcoholemia);
            _menu.AddItem(itemInformeTurno);
            _menu.AddItem(itemTaller);

            _menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);

                if (selectedItem == itemAtestado)
                {
                    AtestadoPolicial.Generar();
                    _menu.Visible = false;
                }
                else if (selectedItem == itemAlcoholemia)
                {
                    ActaAlcoholemia.Generar();
                    _menu.Visible = false;
                }
                else if (selectedItem == itemInformeTurno)
                {
                    InformeIncidentes.Generar();
                    _menu.Visible = false;
                }
                else if (selectedItem == itemTaller)
                {
                    Vehicle veh = null;
                    if (Game.LocalPlayer.Character != null && Game.LocalPlayer.Character.Exists())
                    {
                        veh = Game.LocalPlayer.Character.CurrentVehicle;
                        if (veh == null)
                        {
                            veh = FindClosestVehicle(8f);
                        }
                    }

                    if (veh != null && veh.Exists())
                    {
                        ExpenseTracker.RepairPatrolVehicle(veh);
                    }
                    else
                    {
                        Game.DisplayNotification("~r~No se encuentra ningún vehículo policial cercano para reparar.");
                    }
                    _menu.Visible = false;
                }
            };

            while (true)
            {
                GameFiber.Yield();

                try
                {
                    if (Game.IsKeyDown(ConfigManager.DocsKey))
                    {
                        if (!_menu.Visible)
                        {
                            _menu.Visible = true;
                        }
                        else
                        {
                            _menu.Visible = false;
                        }
                    }

                    _pool.ProcessMenus();
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("PLS.DocumentGenerator loop error: " + ex.Message);
                }
            }
        }

        public static void ExportDocument(string filename, string content)
        {
            try
            {
                if (!Directory.Exists(DocsDirectory))
                {
                    Directory.CreateDirectory(DocsDirectory);
                }

                string filePath = Path.Combine(DocsDirectory, filename);
                File.WriteAllText(filePath, content, System.Text.Encoding.UTF8);

                Game.LogTrivial("PoliceLifeSystem: Documento oficial guardado en " + filePath);

                string notif = string.Format("~g~Documento archivado en SPI_Data/Documentos:~n~~w~{0}", filename);
                Game.DisplayNotification(notif);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error al archivar documento: " + ex.Message);
            }
        }

        private static Vehicle FindClosestVehicle(float radius)
        {
            if (Game.LocalPlayer.Character == null || !Game.LocalPlayer.Character.Exists()) return null;

            Entity[] entities = World.GetEntities(Game.LocalPlayer.Character.Position, radius, GetEntitiesFlags.ConsiderAllVehicles);
            Vehicle closest = null;
            float minDist = float.MaxValue;

            for (int i = 0; i < entities.Length; i++)
            {
                Vehicle v = entities[i] as Vehicle;
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

            return closest;
        }
    }
}
