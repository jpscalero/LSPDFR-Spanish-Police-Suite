using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;

namespace TacticalOps.CQB
{
    public static class StackFormation
    {
        private static List<Ped> stackedOfficers = new List<Ped>();

        public static List<Ped> StackedOfficers
        {
            get { return stackedOfficers; }
        }

        public static bool IsStackFormed
        {
            get { return stackedOfficers.Count > 0; }
        }

        public static void FormStack()
        {
            try
            {
                Ped player = Game.LocalPlayer.Character;
                if (player == null || !player.Exists() || player.IsDead)
                {
                    return;
                }

                CleanOfficersList();

                // Buscar agentes de policia cercanos en un radio de 20 metros
                Entity[] nearbyEntities = World.GetEntities(player.Position, 20.0f, GetEntitiesFlags.ConsiderAllPeds);
                List<Ped> nearbyCops = new List<Ped>();

                if (nearbyEntities != null)
                {
                    for (int i = 0; i < nearbyEntities.Length; i++)
                    {
                        Ped p = nearbyEntities[i] as Ped;
                        if (p != null && p.Exists() && p.IsAlive && p != player)
                        {
                            uint pedType = NativeFunction.Natives.GET_PED_TYPE<uint>(p);
                            if (pedType == 6 || pedType == 27 || p.RelationshipGroup == "COP")
                            {
                                nearbyCops.Add(p);
                            }
                        }
                    }
                }

                // Si no hay agentes en el area, desplegar un binomio tactico GEO/SWAT
                if (nearbyCops.Count == 0)
                {
                    Model swatModel = new Model("s_m_y_swat_01");
                    swatModel.LoadAndWait();

                    for (int s = 1; s <= 2; s++)
                    {
                        Vector3 spawnPos = player.Position - (player.ForwardVector * (1.5f * s));
                        Ped swat = new Ped(swatModel, spawnPos, player.Heading);
                        if (swat != null && swat.Exists())
                        {
                            swat.RelationshipGroup = "COP";
                            swat.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 180, true);
                            nearbyCops.Add(swat);
                        }
                    }
                    Game.DisplayNotification("~b~Refuerzos Tacticos GEO~w~ desplegados para formar columna.");
                }

                stackedOfficers.Clear();
                int maxInStack = Math.Min(4, nearbyCops.Count);

                Vector3 rear = -player.ForwardVector;
                Vector3 right = player.RightVector;

                for (int i = 0; i < maxInStack; i++)
                {
                    Ped cop = nearbyCops[i];
                    if (cop != null && cop.Exists() && cop.IsAlive)
                    {
                        cop.BlockPermanentEvents = true;

                        // Equipar arma larga si no tiene
                        if (cop.Inventory != null)
                        {
                            cop.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 120, true);
                        }

                        // Calcular posicion escalonada en fila india (stack de pared)
                        float offsetDistance = 1.2f + (i * 1.0f);
                        Vector3 targetPos = player.Position + (rear * offsetDistance) + (right * 0.45f);

                        float groundZ = 0.0f;
                        if (NativeFunction.Natives.GET_GROUND_Z_FOR_3D_COORD<bool>(targetPos.X, targetPos.Y, targetPos.Z + 2.0f, out groundZ, false))
                        {
                            targetPos = new Vector3(targetPos.X, targetPos.Y, groundZ);
                        }

                        // Mover al operador a su posicion de stack
                        NativeFunction.Natives.TASK_GO_STRAIGHT_TO_COORD(cop, targetPos.X, targetPos.Y, targetPos.Z, 2.0f, -1, player.Heading, 0.2f);
                        cop.Heading = player.Heading;

                        // Posicion de apunte tactico hacia el frente
                        Vector3 aimTarget = player.Position + (player.ForwardVector * 10.0f);
                        cop.Tasks.AimWeaponAt(aimTarget, -1);

                        stackedOfficers.Add(cop);
                    }
                }

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                Game.DisplayNotification("~b~Stack Tactico CQB formado:~w~ " + stackedOfficers.Count + " operadores preparados en brecha.");
                Game.LogTrivial("TacticalOps: Stack formado con " + stackedOfficers.Count + " agentes.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps: Error al formar stack tactico: " + ex.Message);
            }
        }

        public static void DisbandStack()
        {
            try
            {
                for (int i = 0; i < stackedOfficers.Count; i++)
                {
                    Ped cop = stackedOfficers[i];
                    if (cop != null && cop.Exists() && cop.IsAlive)
                    {
                        cop.BlockPermanentEvents = false;
                        cop.Tasks.Clear();
                        cop.Dismiss();
                    }
                }

                stackedOfficers.Clear();
                Game.DisplayNotification("~y~Columna de asalto disuelta.~w~ Operadores vuelven a patrulla.");
                Game.LogTrivial("TacticalOps: Stack táctico disuelto.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps: Error al disolver stack: " + ex.Message);
            }
        }

        private static void CleanOfficersList()
        {
            for (int i = stackedOfficers.Count - 1; i >= 0; i--)
            {
                if (stackedOfficers[i] == null || !stackedOfficers[i].Exists() || stackedOfficers[i].IsDead)
                {
                    stackedOfficers.RemoveAt(i);
                }
            }
        }
    }
}
