using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RadioYJusticia.Core;
using RadioYJusticia.Justice;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace RadioYJusticia.Radio
{
    public static class RadioMenu
    {
        private static MenuPool pool;
        private static UIMenu menu;
        private static readonly Random random = new Random();

        public static void Process()
        {
            try
            {
                pool = new MenuPool();
                menu = new UIMenu("~b~RADIO TETRA / SIRDEE~s~", "~y~RED DIGITAL DE COMUNICACIONES CNP / GC");
                pool.Add(menu);

                UIMenuItem item10_8 = new UIMenuItem("Reportar 10-8 (Disponible para servicio)", "Comunica a Sala que la unidad esta operativa en calle.");
                UIMenuItem item10_20 = new UIMenuItem("Reportar 10-20 (Transmitir ubicacion actual)", "Transmite la posicion GPS y demarcacion actual a Sala.");
                UIMenuItem item10_38 = new UIMenuItem("Reportar 10-38 (Control de trafico iniciado)", "Notifica a Central el inicio de una parada preventiva o control.");
                UIMenuItem item10_15 = new UIMenuItem("Reportar 10-15 (Traslado de detenido a comisaria)", "Notifica a Sala el traslado de un detenido e inicia la custodia de 72h.");
                UIMenuItem itemConsultaMatricula = new UIMenuItem("Solicitar Consulta de Matricula a Central", "Solicita telematicamente estado de ITV, seguro y requisitorias del vehiculo mas cercano.");
                UIMenuItem itemConsultaDni = new UIMenuItem("Solicitar Consulta de DNI/Requisitoria a Central", "Verifica antecedentes penales y ordenes de busca y captura del sujeto mas proximo.");
                UIMenuItem item10_7 = new UIMenuItem("Reportar 10-7 (Pausa / Fuera de servicio temporal)", "Notifica cese momentaneo para descanso reglamentario o fin de servicio.");

                menu.AddItem(item10_8);
                menu.AddItem(item10_20);
                menu.AddItem(item10_38);
                menu.AddItem(item10_15);
                menu.AddItem(itemConsultaMatricula);
                menu.AddItem(itemConsultaDni);
                menu.AddItem(item10_7);

                menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
                {
                    DispatchVoice.PlayRadioClick();
                    menu.Visible = false;

                    if (selectedItem == item10_8)
                    {
                        HandleReport10_8();
                    }
                    else if (selectedItem == item10_20)
                    {
                        HandleReport10_20();
                    }
                    else if (selectedItem == item10_38)
                    {
                        HandleReport10_38();
                    }
                    else if (selectedItem == item10_15)
                    {
                        HandleReport10_15();
                    }
                    else if (selectedItem == itemConsultaMatricula)
                    {
                        HandleVehicleCheck();
                    }
                    else if (selectedItem == itemConsultaDni)
                    {
                        HandlePedCheck();
                    }
                    else if (selectedItem == item10_7)
                    {
                        HandleReport10_7();
                    }
                };

                Game.LogTrivial("RadioYJusticia: Menu de Radio SIRDEE registrado correctamente.");

                while (true)
                {
                    GameFiber.Yield();

                    try
                    {
                        if (Game.IsKeyDown(ConfigManager.RadioKey))
                        {
                            menu.Visible = !menu.Visible;
                        }

                        pool.ProcessMenus();
                    }
                    catch (System.Threading.ThreadAbortException) { break; }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("RadioYJusticia.RadioMenu loop error: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia: Error en ejecucion de RadioMenu: " + ex.Message);
            }
        }

        private static void HandleReport10_8()
        {
            DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Sala: 10-8 disponible en sector para cualquier requerimiento.");
            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(2500);
                    DispatchVoice.PlayDispatchMessage("Zeta recibido. Queda anotado en servicio y disponible en sector. Buen servicio.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.HandleReport10_8 fiber error: " + ex.Message);
                }
            });
        }

        private static void HandleReport10_20()
        {
            Ped player = Game.LocalPlayer.Character;
            Vector3 pos = player.Position;
            string street = World.GetStreetName(pos);
            string zone = NativeFunction.Natives.GET_NAME_OF_ZONE<string>(pos.X, pos.Y, pos.Z);

            if (string.IsNullOrEmpty(street)) street = "Demarcacion Sector Centro";
            if (string.IsNullOrEmpty(zone)) zone = "Urbana";

            DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Sala: 10-20 en " + street + " (Sector " + zone + ").");
            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(2500);
                    DispatchVoice.PlayDispatchMessage("Recibido Zeta. Posicion registrada en " + street + " (Demarcacion " + zone + "). Mantengan patrullaje preventivo.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.HandleReport10_20 fiber error: " + ex.Message);
                }
            });
        }

        private static void HandleReport10_38()
        {
            DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Sala: 10-38. Iniciamos parada de trafico y control preventivo.");
            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(2500);
                    DispatchVoice.PlayDispatchMessage("Recibido 10-38. Control preventivo de trafico anotado en su demarcacion. Extreme precauciones y notifique matricula.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.HandleReport10_38 fiber error: " + ex.Message);
                }
            });
        }

        private static void HandleReport10_15()
        {
            Ped player = Game.LocalPlayer.Character;
            Ped suspect = FindClosestArrestedPed(15f);

            DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Sala: 10-15 con detenido custodiado a bordo. Procedemos a traslado a Comisaria de Distrito.");

            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(2500);
                    if (suspect != null && suspect.Exists())
                    {
                        CustodyManager.RegisterDetainee(suspect, null);
                        DispatchVoice.PlayDispatchMessage("Recibido 10-15. Sala de custodia y calabozos de Comisaria prevenidos para recepcion del detenido. Reloj procesal de 72 horas activado.");
                    }
                    else
                    {
                        DispatchVoice.PlayDispatchMessage("Recibido 10-15 preventivo. Comisaria informada de traslado. Diligencias de custodia policial activadas.");
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.HandleReport10_15 fiber error: " + ex.Message);
                }
            });
        }

        private static void HandleVehicleCheck()
        {
            Vehicle veh = FindClosestVehicle(18f);

            if (veh == null || !veh.Exists())
            {
                DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Central: Solicitamos consulta telemática de matricula de vehiculo proximo.");
                GameFiber.StartNew(delegate
                {
                    try
                    {
                        GameFiber.Sleep(2000);
                        DispatchVoice.PlayDispatchMessage("Central a patrulla: No se detecta ningun vehiculo objetivo en las inmediaciones. Aproximese o enfoque al vehiculo.");
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("RadioYJusticia.HandleVehicleCheck fiber error: " + ex.Message);
                    }
                });
                return;
            }

            string matricula = veh.LicensePlate;
            string modelo = veh.Model.Name;

            DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Sala: Consulta de antecedentes para matricula " + matricula + " modelo " + modelo + ".");

            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(3000);

                    int roll = random.Next(100);
                    if (roll < 65)
                    {
                        DispatchVoice.PlayDispatchMessage("SITEL / DGT para matricula " + matricula + " (" + modelo + "): Vehiculo LIMPIO. ITV en vigor, seguro obligatorio contratado y titular sin señalamientos.");
                    }
                    else if (roll < 85)
                    {
                        DispatchVoice.PlayDispatchMessage("¡ATENCION PATRULLA! DGT informa sobre " + matricula + ": ITV CADUCADA y SEGURO SIN VIGENCIA activa en Registro General. Procedan a sancion administrativa.");
                    }
                    else
                    {
                        DispatchVoice.PlayDispatchMessage("¡¡ALERTA MAXIMA POLICIAL!! Matricula " + matricula + " (" + modelo + "): CONSTA SEÑALAMIENTO JUDICIAL POR ROBO / SUSTRACCION. Titular con orden de busca y captura. Procedan con extrema cautela.");
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.HandleVehicleCheck lookup fiber error: " + ex.Message);
                }
            });
        }

        private static void HandlePedCheck()
        {
            Ped targetPed = FindClosestPed(8f);

            if (targetPed == null || !targetPed.Exists())
            {
                DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Central: Solicitamos comprobacion telemática de identidad y requisitorias.");
                GameFiber.StartNew(delegate
                {
                    try
                    {
                        GameFiber.Sleep(2000);
                        DispatchVoice.PlayDispatchMessage("Central informa: No se detecta ningun sospechoso a distancia de identificacion. Aproximese al individuo para lectura de DNI.");
                    }
                    catch (System.Threading.ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("RadioYJusticia.HandlePedCheck fiber error: " + ex.Message);
                    }
                });
                return;
            }

            string nombre = "Sujeto sospechoso";
            bool wantedLspdfr = false;
            try
            {
                Persona persona = Functions.GetPersonaForPed(targetPed);
                if (persona != null)
                {
                    nombre = persona.FullName;
                    wantedLspdfr = persona.Wanted;
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("RadioYJusticia.RadioMenu.OpenFastTrialMenu: " + ex.Message);
            }

            string dni = CustodyManager.GenerarDniEspanol(nombre);
            bool esReincidente = ArrestHistoryDB.IsReoffender(dni);

            DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Sala: Comprobacion en base de Requisitorias para el ciudadano " + nombre + ", portando DNI " + dni + ".");

            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(3000);

                    if (wantedLspdfr || esReincidente || random.Next(100) < 25)
                    {
                        string delitoAnterior = esReincidente ? "antecedentes previos en historial" : "orden de busca y captura de Juzgado de Instruccion";
                        DispatchVoice.PlayDispatchMessage("¡ATENCION PATRULLA! Sujeto " + nombre + " (DNI " + dni + "): REQUISITORIA POSITIVA. Consta " + delitoAnterior + ". Procedan a retencion preventiva con medidas de seguridad.");
                    }
                    else
                    {
                        DispatchVoice.PlayDispatchMessage("Central informa: Ciudadano " + nombre + " (DNI " + dni + "): Sin antecedentes ni requisitorias judiciales pendientes en el sistema.");
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.HandlePedCheck lookup fiber error: " + ex.Message);
                }
            });
        }

        private static void HandleReport10_7()
        {
            DispatchVoice.PlayOfficerMessage("ZETA", "Zeta a Sala: 10-7. Entramos en pausa reglamentaria / fuera de servicio temporal.");
            GameFiber.StartNew(delegate
            {
                try
                {
                    GameFiber.Sleep(2500);
                    DispatchVoice.PlayDispatchMessage("Recibido Zeta. Queda registrado en 10-7 fuera de servicio temporal. Notifique reincorporacion mediante 10-8.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.HandleReport10_7 fiber error: " + ex.Message);
                }
            });
        }

        private static Vehicle FindClosestVehicle(float radius)
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists()) return null;

            if (player.IsInAnyVehicle(false))
            {
                return player.CurrentVehicle;
            }

            Entity[] entities = World.GetEntities(player.Position, radius, GetEntitiesFlags.ConsiderAllVehicles);
            if (entities == null) return null;

            Vehicle closest = null;
            float minDist = float.MaxValue;

            for (int i = 0; i < entities.Length; i++)
            {
                Vehicle v = entities[i] as Vehicle;
                if (v != null && v.Exists())
                {
                    float dist = player.DistanceTo(v);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = v;
                    }
                }
            }

            return closest;
        }

        private static Ped FindClosestPed(float radius)
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists()) return null;

            Entity[] entities = World.GetEntities(player.Position, radius, GetEntitiesFlags.ConsiderHumanPeds);
            if (entities == null) return null;

            Ped closest = null;
            float minDist = float.MaxValue;

            for (int i = 0; i < entities.Length; i++)
            {
                Ped p = entities[i] as Ped;
                if (p != null && p.Exists() && p != player && p.IsHuman && p.IsAlive)
                {
                    float dist = player.DistanceTo(p);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = p;
                    }
                }
            }

            return closest;
        }

        private static Ped FindClosestArrestedPed(float radius)
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists()) return null;

            Entity[] entities = World.GetEntities(player.Position, radius, GetEntitiesFlags.ConsiderHumanPeds);
            if (entities == null) return null;

            for (int i = 0; i < entities.Length; i++)
            {
                Ped p = entities[i] as Ped;
                if (p != null && p.Exists() && p != player && p.IsHuman && p.IsAlive)
                {
                    bool isArrested = false;
                    try
                    {
                        isArrested = Functions.IsPedArrested(p);
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("RadioYJusticia.RadioMenu.GetNearestArrestedPed (IsPedArrested): " + ex.Message);
                    }
                    if (!isArrested)
                    {
                        try
                        {
                            isArrested = NativeFunction.Natives.IS_PED_CUFFED<bool>(p);
                        }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("RadioYJusticia.RadioMenu.GetNearestArrestedPed (IS_PED_CUFFED): " + ex.Message);
                        }
                    }

                    if (isArrested)
                    {
                        return p;
                    }
                }
            }

            return null;
        }
    }
}
