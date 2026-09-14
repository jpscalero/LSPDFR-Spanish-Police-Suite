using System;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using PoliceLifeSystem.Core;

namespace PoliceLifeSystem.Economy
{
    public static class ShopMenu
    {
        public static UIMenu CreateShopMenu(MenuPool pool)
        {
            UIMenu shopMenu = new UIMenu("ARMERÍA Y EQUIPO", "~b~TIENDA TÁCTICA POLICIAL (CNP)");
            pool.Add(shopMenu);

            UIMenuItem itemChaleco = new UIMenuItem("Chaleco Balístico Reforzado (250€)", "Otorga protección balística completa (100% Blindaje).");
            UIMenuItem itemLinterna = new UIMenuItem("Linterna Táctica Policial (50€)", "Linterna LED de alta intensidad para intervenciones con baja visibilidad.");
            UIMenuItem itemMunicion = new UIMenuItem("Caja de Munición de Reserva (80€)", "Caja de cartuchos 9mm Luger y 5.56 OTAN reglamentarios.");
            UIMenuItem itemBotiquin = new UIMenuItem("Kit Médico de Emergencia (120€)", "Botiquín táctico IFAK. Restaura la salud completa del agente.");

            shopMenu.AddItem(itemChaleco);
            shopMenu.AddItem(itemLinterna);
            shopMenu.AddItem(itemMunicion);
            shopMenu.AddItem(itemBotiquin);

            shopMenu.OnItemSelect += delegate (UIMenu sender, UIMenuItem selectedItem, int index)
            {
                if (Game.LocalPlayer.Character == null || !Game.LocalPlayer.Character.Exists()) return;

                if (selectedItem == itemChaleco)
                {
                    const int precio = 250;
                    if (SalaryManager.CurrentBalance >= precio)
                    {
                        SalaryManager.DeductMoney(precio);
                        Game.LocalPlayer.Character.Armor = 100;
                        NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", false);
                        Game.DisplayNotification("~g~COMPRA REALIZADA:~n~~w~Chaleco balístico reforzado equipado.~n~Blindaje al ~b~100%~w~.");
                    }
                    else
                    {
                        MostrarErrorFondos(precio);
                    }
                }
                else if (selectedItem == itemLinterna)
                {
                    const int precio = 50;
                    if (SalaryManager.CurrentBalance >= precio)
                    {
                        SalaryManager.DeductMoney(precio);
                        uint flashHash = (uint)Game.GetHashKey("WEAPON_FLASHLIGHT");
                        NativeFunction.Natives.GIVE_WEAPON_TO_PED(Game.LocalPlayer.Character, flashHash, 1, false, true);
                        NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", false);
                        Game.DisplayNotification("~g~COMPRA REALIZADA:~n~~w~Linterna táctica añadida a tu inventario de dotación.");
                    }
                    else
                    {
                        MostrarErrorFondos(precio);
                    }
                }
                else if (selectedItem == itemMunicion)
                {
                    const int precio = 80;
                    if (SalaryManager.CurrentBalance >= precio)
                    {
                        SalaryManager.DeductMoney(precio);
                        uint pistolHash = (uint)Game.GetHashKey("WEAPON_COMBATPISTOL");
                        uint carbineHash = (uint)Game.GetHashKey("WEAPON_CARBINERIFLE");
                        uint shotgunHash = (uint)Game.GetHashKey("WEAPON_PUMPSHOTGUN");

                        NativeFunction.Natives.GIVE_WEAPON_TO_PED(Game.LocalPlayer.Character, pistolHash, 150, false, false);
                        NativeFunction.Natives.GIVE_WEAPON_TO_PED(Game.LocalPlayer.Character, carbineHash, 240, false, false);
                        NativeFunction.Natives.GIVE_WEAPON_TO_PED(Game.LocalPlayer.Character, shotgunHash, 50, false, false);

                        NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", false);
                        Game.DisplayNotification("~g~COMPRA REALIZADA:~n~~w~Munición de dotación recargada por completo.");
                    }
                    else
                    {
                        MostrarErrorFondos(precio);
                    }
                }
                else if (selectedItem == itemBotiquin)
                {
                    const int precio = 120;
                    if (SalaryManager.CurrentBalance >= precio)
                    {
                        SalaryManager.DeductMoney(precio);
                        Game.LocalPlayer.Character.Health = Game.LocalPlayer.Character.MaxHealth;
                        NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", false);
                        Game.DisplayNotification("~g~COMPRA REALIZADA:~n~~w~Kit médico aplicado. Salud restaurada al ~g~100%~w~.");
                    }
                    else
                    {
                        MostrarErrorFondos(precio);
                    }
                }
            };

            return shopMenu;
        }

        private static void MostrarErrorFondos(int coste)
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "ERROR", "HUD_AMMO_SHOP_SOUNDSET", false);
            string msg = string.Format("~r~Fondos insuficientes.~w~~n~Necesitas ~y~{0}€~w~ y dispones de ~y~{1}€~w~ en tu cuenta.", coste, SalaryManager.CurrentBalance);
            Game.DisplayNotification(msg);
        }
    }
}
