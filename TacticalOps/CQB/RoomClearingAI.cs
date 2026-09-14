using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;

namespace TacticalOps.CQB
{
    public static class RoomClearingAI
    {
        public static void ExecuteDynamicEntry()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists() || player.IsDead)
            {
                return;
            }

            GameFiber.StartNew(delegate
            {
                try
                {
                    if (!StackFormation.IsStackFormed)
                    {
                        StackFormation.FormStack();
                        GameFiber.Sleep(600);
                    }

                    List<Ped> officers = StackFormation.StackedOfficers;
                    if (officers.Count == 0)
                    {
                        Game.DisplayNotification("~r~No hay agentes en formación para iniciar la entrada dinámica.");
                        return;
                    }

                    Game.DisplaySubtitle("~b~Lider de Asalto:~w~ «¡¡ENTRADA DINAMICA, VAMOS VAMOS!! ¡¡POLICIA AL SUELO!!»", 3500);
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);

                    Vector3 playerPos = player.Position;
                    Vector3 forward = player.ForwardVector;
                    Vector3 right = player.RightVector;

                    // Vectores de dispersion en abanico
                    Vector3 p1Left = playerPos + (forward * 5.0f) - (right * 3.0f);
                    Vector3 p2Center = playerPos + (forward * 8.0f);
                    Vector3 p3Right = playerPos + (forward * 5.0f) + (right * 3.0f);
                    Vector3 p4Rear = playerPos + (forward * 2.5f);

                    Vector3[] targetPoints = new Vector3[] { p1Left, p2Center, p3Right, p4Rear };

                    for (int i = 0; i < officers.Count; i++)
                    {
                        Ped cop = officers[i];
                        if (cop != null && cop.Exists() && cop.IsAlive)
                        {
                            cop.BlockPermanentEvents = true;

                            Vector3 target = targetPoints[i % targetPoints.Length];
                            float groundZ = 0.0f;
                            if (NativeFunction.Natives.GET_GROUND_Z_FOR_3D_COORD<bool>(target.X, target.Y, target.Z + 2.0f, out groundZ, false))
                            {
                                target = new Vector3(target.X, target.Y, groundZ);
                            }

                            Vector3 aimPoint = target + (forward * 4.0f);

                            // Mover al operador apuntando a su sector de responsabilidad
                            NativeFunction.Natives.TASK_GO_TO_COORD_WHILE_AIMING_AT_COORD(
                                cop,
                                target.X, target.Y, target.Z,
                                aimPoint.X, aimPoint.Y, aimPoint.Z,
                                2.5f,
                                false,
                                0.5f, 0.5f,
                                true,
                                0,
                                false,
                                0
                            );
                        }
                    }

                    GameFiber.Sleep(2000);

                    // Escanear amenazas armadas en el area (radio 20m)
                    Entity[] nearbyEntities = World.GetEntities(playerPos + (forward * 4.0f), 18.0f, GetEntitiesFlags.ConsiderAllPeds);
                    Ped primaryHostile = null;

                    if (nearbyEntities != null)
                    {
                        for (int i = 0; i < nearbyEntities.Length; i++)
                        {
                            Ped suspect = nearbyEntities[i] as Ped;
                            if (suspect != null && suspect.Exists() && suspect.IsAlive && suspect != player)
                            {
                                uint pedType = NativeFunction.Natives.GET_PED_TYPE<uint>(suspect);
                                if (pedType != 6 && pedType != 27 && suspect.RelationshipGroup != "COP")
                                {
                                    uint weaponHash = NativeFunction.Natives.GET_SELECTED_PED_WEAPON<uint>(suspect);
                                    if (weaponHash != 0 && weaponHash != Game.GetHashKey("WEAPON_UNARMED"))
                                    {
                                        primaryHostile = suspect;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (primaryHostile != null && primaryHostile.Exists())
                    {
                        Game.DisplaySubtitle("~r~¡¡CONTACTO ARMADO DENTRO!! ¡¡FUEGO DE RESPUESTA!!~w~", 3500);

                        for (int i = 0; i < officers.Count; i++)
                        {
                            Ped cop = officers[i];
                            if (cop != null && cop.Exists() && cop.IsAlive)
                            {
                                cop.Tasks.FightAgainst(primaryHostile);
                            }
                        }
                    }
                    else
                    {
                        Game.DisplaySubtitle("~g~Operador GEO:~w~ «¡¡HABITACION DESPEJADA Y ASEGURADA!!»", 3000);
                    }

                    Game.DisplayNotification("~g~¡Entrada Dinamica Completada!~w~ Vectores de tiro y angulos cubiertos.");
                    Game.LogTrivial("TacticalOps: Entrada dinamica CQB ejecutada con exito.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalOps.RoomClearingAI.ExecuteDynamicEntry: " + ex.Message);
                }
            });
        }
    }
}
