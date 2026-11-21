using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using LSPD_First_Response.Mod.API;
using PoliceLifeSystem.Core;
using PoliceLifeSystem.Career;
using PoliceLifeSystem.Economy;
using PoliceLifeSystem.Stats;
using PoliceLifeSystem.Documents;

namespace PoliceLifeSystem
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            try
            {
                Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
                Game.LogTrivial("PoliceLifeSystem: Plugin registrado en LSPDFR.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error critico en Initialize: " + ex.Message);
            }
        }

        public override void Finally()
        {
            try
            {
                CelebrationManager.Stop();
                SaveManager.Save();
                Game.LogTrivial("PoliceLifeSystem: Plugin descargado y datos guardados correctamente.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("PoliceLifeSystem: Error en Finally: " + ex.Message);
            }
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (!onDuty)
            {
                CelebrationManager.Stop();
                return;
            }

            if (onDuty)
            {
                try
                {
                    ConfigManager.Load();

                    if (!ConfigManager.ModEnabled)
                    {
                        Game.LogTrivial("PoliceLifeSystem: Mod deshabilitado en configuración INI.");
                        return;
                    }

                    SaveManager.Load();
                    CrimeTracker.LoadHistory();
                    CelebrationManager.Start();

                    // Loop de salario y gastos periódicos
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            while (true)
                            {
                                // En GTA V, 1 minuto de juego equivale a 2000 milisegundos reales (48 min juego = 96 seg reales)
                                int intervalMs = ConfigManager.SalaryIntervalMinutes * 2000;
                                if (intervalMs < 10000) intervalMs = 10000;

                                GameFiber.Sleep(intervalMs);

                                try
                                {
                                    SalaryManager.ProcessSalaryPayment();
                                    ExpenseTracker.ProcessExpenses();
                                }
                                catch (Exception ex)
                                {
                                    Game.LogTrivial("PoliceLifeSystem: Error en ciclo de salario/gastos: " + ex.Message);
                                }
                            }
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("PoliceLifeSystem: Error critico en fibra de salario: " + ex.Message);
                        }
                    });

                    // Loop para escuchar tecla Add (NumPad +) y abrir Menú General
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            ProcessGeneralMenu();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("PoliceLifeSystem: Error critico en fibra de ProcessGeneralMenu: " + ex.Message);
                        }
                    });

                    // Loop para escuchar tecla Divide (NumPad /) y abrir Menú de Documentos Policiales
                    GameFiber.StartNew(delegate
                    {
                        try
                        {
                            DocumentGenerator.Process();
                        }
                        catch (System.Threading.ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("PoliceLifeSystem: Error critico en fibra de DocumentGenerator: " + ex.Message);
                        }
                    });

                    // Notificación en pantalla de carga completa
                    Game.DisplayNotification(
                        "3dtextures",
                        "mpgroundlogo_cops",
                        "~b~Police Life System",
                        "~g~Servicio Activo (CNP)",
                        "~w~Sistema de vida policial cargado.~n~[~y~NumPad +~w~] Carrera y Tienda~n~[~y~NumPad /~w~] Atestados y Actas"
                    );

                    Game.LogTrivial("PoliceLifeSystem: Todos los módulos inicializados con éxito.");
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("PoliceLifeSystem: ERROR al inicializar servicio: " + ex.Message);
                }
            }
        }

        private static void ProcessGeneralMenu()
        {
            MenuPool pool = new MenuPool();
            UIMenu mainMenu = new UIMenu("POLICE LIFE SYSTEM", "~b~SISTEMA DE VIDA POLICIAL (CNP)");
            pool.Add(mainMenu);

            // 1. Submenú de Carrera y Hoja de Servicio
            UIMenu carreraMenu = new UIMenu("HOJA DE SERVICIO", "~b~CARRERA Y RANGO EN EL CNP");
            pool.Add(carreraMenu);
            UIMenuItem itemMenuCarrera = new UIMenuItem("Hoja de Servicio y Carrera", "Consulta tu rango, experiencia acumulada y condecoraciones.");
            mainMenu.AddItem(itemMenuCarrera);
            mainMenu.BindMenuToItem(carreraMenu, itemMenuCarrera);

            UIMenuItem itemVerRango = new UIMenuItem("Consultar Rango y Categoría", "Muestra información de tu rango actual en el Cuerpo Nacional de Policía.");
            UIMenuItem itemVerXp = new UIMenuItem("Consultar Experiencia (XP)", "Muestra los puntos de experiencia y los necesarios para el siguiente ascenso.");
            UIMenuItem itemComprobarAscenso = new UIMenuItem("Verificar Ascenso Policial", "Comprueba si cumples los requisitos de mérito para ascender.");
            UIMenuItem itemVerMedallas = new UIMenuItem("Medallero y Condecoraciones", "Muestra las condecoraciones al mérito policial obtenidas.");

            carreraMenu.AddItem(itemVerRango);
            carreraMenu.AddItem(itemVerXp);
            carreraMenu.AddItem(itemComprobarAscenso);
            carreraMenu.AddItem(itemVerMedallas);

            carreraMenu.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);

                if (selectedItem == itemVerRango)
                {
                    int rank = RankManager.CurrentRank;
                    string title = RankManager.GetRankTitle(rank);
                    int reqXp = RankManager.GetRequiredXP(rank);
                    string msg = string.Format("~w~Categoría: ~b~{0}~w~~n~Nivel: ~y~{1}/6~w~~n~XP Base de Rango: ~g~{2} XP~w~", title, rank, reqXp);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~ESCALAFÓN POLICIAL", "~g~Rango Vigente", msg);
                }
                else if (selectedItem == itemVerXp)
                {
                    int rank = RankManager.CurrentRank;
                    int curXp = XPSystem.CurrentXP;
                    int nextXp = (rank < 6) ? RankManager.GetRequiredXP(rank + 1) : curXp;
                    int faltan = Math.Max(0, nextXp - curXp);
                    string msg = string.Format("~w~XP Actual: ~g~{0} XP~w~~n~Siguiente Rango: ~y~{1} XP~w~~n~Puntos restantes: ~o~{2} XP~w~", curXp, nextXp, faltan);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~EXPERIENCIA POLICIAL", "~y~Puntos de Mérito", msg);
                }
                else if (selectedItem == itemComprobarAscenso)
                {
                    RankManager.CheckRankUp();
                    string rankTitle = RankManager.GetRankTitle(RankManager.CurrentRank);
                    Game.DisplayNotification(string.Format("~b~Rango verificado: ~w~{0}", rankTitle));
                }
                else if (selectedItem == itemVerMedallas)
                {
                    List<MedalSystem.MedalEntry> medals = MedalSystem.Medals;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = 0; i < medals.Count; i++)
                    {
                        MedalSystem.MedalEntry m = medals[i];
                        sb.Append(m.Desbloqueada ? "~g~[OK] " : "~c~[--] ");
                        sb.Append(m.Nombre);
                        sb.Append("~n~");
                    }
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~MEDALLERO CNP", "~w~Condecoraciones", sb.ToString());
                }
            };

            // 2. Submenú de Tienda de Equipamiento Táctico
            UIMenu shopMenu = ShopMenu.CreateShopMenu(pool);
            UIMenuItem itemMenuTienda = new UIMenuItem("Armería y Equipamiento Táctico", "Compra chaleco reforzado, linterna, munición y botiquín.");
            mainMenu.AddItem(itemMenuTienda);
            mainMenu.BindMenuToItem(shopMenu, itemMenuTienda);

            // 3. Submenú de Estadísticas y Rendimiento
            UIMenu statsMenu = new UIMenu("ESTADÍSTICAS", "~b~RENDIMIENTO Y DELINCUENCIA");
            pool.Add(statsMenu);
            UIMenuItem itemMenuStats = new UIMenuItem("Expediente y Estadísticas Operativas", "Consulta detenciones, denuncias, neutralizaciones y mapa de calor.");
            mainMenu.AddItem(itemMenuStats);
            mainMenu.BindMenuToItem(statsMenu, itemMenuStats);

            UIMenuItem itemVerStats = new UIMenuItem("Ver Expediente Completo", "Muestra en pantalla un resumen detallado de tus intervenciones.");
            UIMenuItem itemHeatMap = new UIMenuItem("Desplegar Mapa de Calor de Delitos (30s)", "Muestra en el radar zonas rojas, amarillas y verdes de actividad delictiva.");

            statsMenu.AddItem(itemVerStats);
            statsMenu.AddItem(itemHeatMap);

            statsMenu.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);

                if (selectedItem == itemVerStats)
                {
                    PerformanceStats.ShowStatsNotification();
                }
                else if (selectedItem == itemHeatMap)
                {
                    CrimeHeatMap.ShowHeatMap();
                    statsMenu.Visible = false;
                    mainMenu.Visible = false;
                }
            };

            // 4. Opciones directas en Menú Principal
            UIMenuItem itemCobrarNomina = new UIMenuItem("Cobrar Nómina de Turno", "Liquida la nómina reglamentaria correspondiente a tu rango actual.");
            UIMenuItem itemGuardar = new UIMenuItem("Guardar Progreso Manualmente", "Guarda el expediente de carrera y economía en disco inmediatamente.");

            mainMenu.AddItem(itemCobrarNomina);
            mainMenu.AddItem(itemGuardar);

            mainMenu.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);

                if (selectedItem == itemCobrarNomina)
                {
                    SalaryManager.ProcessSalaryPayment();
                    mainMenu.Visible = false;
                }
                else if (selectedItem == itemGuardar)
                {
                    SaveManager.Save();
                    Game.DisplayNotification("~g~Progreso policial guardado con éxito en disco.");
                }
            };

            while (true)
            {
                GameFiber.Yield();

                try
                {
                    if (Game.IsKeyDown(ConfigManager.MenuKey))
                    {
                        if (!mainMenu.Visible && !carreraMenu.Visible && !shopMenu.Visible && !statsMenu.Visible)
                        {
                            mainMenu.Visible = true;
                        }
                        else
                        {
                            mainMenu.Visible = false;
                            carreraMenu.Visible = false;
                            shopMenu.Visible = false;
                            statsMenu.Visible = false;
                        }
                    }

                    pool.ProcessMenus();
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("PLS.ProcessGeneralMenu loop error: " + ex.Message);
                }
            }
        }
    }
}
