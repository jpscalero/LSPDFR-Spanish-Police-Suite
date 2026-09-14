using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using TacticalOps.Core;

namespace TacticalOps.Scene
{
    public static class SceneObjectPlacer
    {
        private static List<Rage.Object> placedObjects = new List<Rage.Object>();

        public static List<Rage.Object> PlacedObjects
        {
            get { return placedObjects; }
        }

        public static int ObjectCount
        {
            get { return placedObjects.Count; }
        }

        public static bool HasObjects
        {
            get { return placedObjects.Count > 0; }
        }

        public static void PlaceObject(string modelName)
        {
            try
            {
                // Limpiar objetos nulos o eliminados previamente
                CleanInvalidObjects();

                if (placedObjects.Count >= ConfigManager.MaxProps)
                {
                    Game.DisplayNotification("~r~Limite alcanzado (" + ConfigManager.MaxProps + " props).~w~ Retira material antes de colocar mas.");
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "ERROR", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                    return;
                }

                Ped player = Game.LocalPlayer.Character;
                if (player == null || !player.Exists() || player.IsDead)
                {
                    return;
                }

                // Posicion frente al jugador
                Vector3 playerPos = player.Position;
                Vector3 forward = player.ForwardVector;
                Vector3 spawnPos = playerPos + (forward * 1.5f);

                float groundZ = 0.0f;
                if (NativeFunction.Natives.GET_GROUND_Z_FOR_3D_COORD<bool>(spawnPos.X, spawnPos.Y, spawnPos.Z + 2.0f, out groundZ, false))
                {
                    spawnPos = new Vector3(spawnPos.X, spawnPos.Y, groundZ);
                }

                // Animacion de agacharse / colocar
                player.Tasks.PlayAnimation("pickup_object", "pickup_low", 4.0f, AnimationFlags.None);

                Model propModel = new Model(modelName);
                propModel.LoadAndWait();

                Rage.Object prop = new Rage.Object(propModel, spawnPos, player.Heading);

                if (prop != null && prop.Exists())
                {
                    prop.IsPositionFrozen = true;
                    NativeFunction.Natives.PLACE_OBJECT_ON_GROUND_OR_OBJECT_PROPERLY(prop);

                    // Si es bengala, activar efecto de iluminacion/particulas
                    if (string.Equals(modelName, "prop_flare_01", StringComparison.OrdinalIgnoreCase))
                    {
                        NativeFunction.Natives.SET_OBJECT_PHYSICS_PARAMS(prop, 10.0f, 1.0f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
                    }

                    placedObjects.Add(prop);

                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                    Game.DisplayNotification("~b~Material colocado:~w~ " + modelName + " (~y~" + placedObjects.Count + "/" + ConfigManager.MaxProps + "~w~)");
                    Game.LogTrivial("TacticalOps: Objeto colocado: " + modelName + " en " + spawnPos.ToString());
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps: Error al colocar objeto " + modelName + ": " + ex.Message);
            }
        }

        public static void RemoveAllObjects()
        {
            try
            {
                int count = 0;
                for (int i = 0; i < placedObjects.Count; i++)
                {
                    Rage.Object obj = placedObjects[i];
                    if (obj != null && obj.Exists())
                    {
                        obj.Delete();
                        count++;
                    }
                }

                placedObjects.Clear();

                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists() && player.IsAlive)
                {
                    player.Tasks.PlayAnimation("pickup_object", "pickup_low", 3.0f, AnimationFlags.None);
                }

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CANCEL", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                Game.DisplayNotification("~g~Retirada completada:~w~ Se han recogido ~y~" + count + "~w~ objetos de escena.");
                Game.LogTrivial("TacticalOps: Todos los objetos de escena han sido eliminados. Total: " + count);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalOps: Error al retirar objetos de escena: " + ex.Message);
            }
        }

        private static void CleanInvalidObjects()
        {
            for (int i = placedObjects.Count - 1; i >= 0; i--)
            {
                if (placedObjects[i] == null || !placedObjects[i].Exists())
                {
                    placedObjects.RemoveAt(i);
                }
            }
        }
    }
}
