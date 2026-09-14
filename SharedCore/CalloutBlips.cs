using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using LSPD_First_Response.Mod.API;

namespace PoliceSuite.Common
{
    public static class CalloutBlips
    {
        public static Blip AttachBlip(Entity entity, Color color, float scale)
        {
            if (entity == null || !entity.Exists()) return null;
            try
            {
                Blip b = entity.AttachBlip();
                b.Color = color;
                b.Scale = scale;
                return b;
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CalloutBlips.AttachBlip: " + ex.Message);
                return null;
            }
        }

        public static void UpdatePedBlips(Dictionary<Ped, Blip> blipDict, bool checkArrested)
        {
            if (blipDict == null || blipDict.Count == 0) return;

            List<Ped> deadOrRemoved = new List<Ped>();
            foreach (KeyValuePair<Ped, Blip> pair in blipDict)
            {
                Ped p = pair.Key;
                Blip b = pair.Value;
                bool shouldRemove = (p == null || !p.Exists() || !p.IsAlive);
                if (!shouldRemove && checkArrested && Functions.IsPedArrested(p))
                {
                    shouldRemove = true;
                }

                if (shouldRemove)
                {
                    if (b != null && b.Exists())
                    {
                        b.Delete();
                    }
                    deadOrRemoved.Add(p);
                }
            }

            for (int i = 0; i < deadOrRemoved.Count; i++)
            {
                blipDict.Remove(deadOrRemoved[i]);
            }
        }
    }
}
