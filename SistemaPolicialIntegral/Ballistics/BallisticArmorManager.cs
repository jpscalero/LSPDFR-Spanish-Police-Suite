using System;
using Rage;
using Rage.Native;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.Ballistics
{
    public class BallisticArmorManager
    {
        private int currentArmorHP;

        public BallisticArmorManager()
        {
            currentArmorHP = ConfigManager.ArmorPlateHP;
            GameFiber.StartNew(delegate
            {
                try
                {
                    ProcessArmor();
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.BallisticArmorManager fiber error: " + ex.Message);
                }
            });
        }

        private void ProcessArmor()
        {
            int lastHealth = 0;
            if (Game.LocalPlayer.Character != null && Game.LocalPlayer.Character.Exists())
            {
                lastHealth = Game.LocalPlayer.Character.Health;
            }

            while (true)
            {
                GameFiber.Yield();

                try
                {
                    if (Game.LocalPlayer.Character != null && Game.LocalPlayer.Character.Exists() && Game.LocalPlayer.Character.IsAlive)
                    {
                        int currentHealth = Game.LocalPlayer.Character.Health;

                        if (currentHealth < lastHealth)
                        {
                            int damage = lastHealth - currentHealth;
                            
                            if (currentArmorHP > 0)
                            {
                                if (currentArmorHP >= damage)
                                {
                                    currentArmorHP -= damage;
                                    Game.LocalPlayer.Character.Health = lastHealth; // Absorb damage
                                }
                                else
                                {
                                    int remainingDamage = damage - currentArmorHP;
                                    currentArmorHP = 0;
                                    Game.LocalPlayer.Character.Health = lastHealth - remainingDamage;
                                    Game.DisplaySubtitle("~r~¡Chaleco dañado! Placas balísticas agotadas.", 3000);
                                }
                            }
                        }

                        lastHealth = Game.LocalPlayer.Character.Health;
                    }
                }
                catch (System.Threading.ThreadAbortException) { break; }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.BallisticArmorManager loop error: " + ex.Message);
                }
            }
        }

        public void ReloadArmor()
        {
            currentArmorHP = ConfigManager.ArmorPlateHP;
            Game.DisplaySubtitle("~g~Placas balísticas reemplazadas.", 3000);
        }
    }
}
