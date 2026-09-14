using System;
using System.Collections.Generic;
using Rage;

namespace SistemaPolicialIntegral.Intervention
{
    public static class FriskSearchManager
    {
        private static string[] lícitos = new string[] { "Cartera", "Llaves", "Teléfono móvil", "Paquete de tabaco" };
        private static string[] ilícitos = new string[] { "Navaja automática (Art. 563 CP)", "Bolsa con sustancia blanca", "Báscula de precisión", "Billetes de 50€ fraccionados", "Pasamontañas" };
        private static string[] vehiculo = new string[] { "Fardos precintados", "Arma de fuego", "Kit de robo", "Placa de matrícula falsa" };

        public static void FriskPed(Ped suspect)
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    if (suspect == null || !suspect.Exists()) return;
                
                Game.DisplaySubtitle("~b~Agente:~w~ Separe las piernas y ponga las manos donde pueda verlas.", 3000);
                GameFiber.Sleep(3000);
                
                Random rand = new Random();
                int itemsFound = rand.Next(1, 4);
                
                List<string> encontrados = new List<string>();
                
                for(int i = 0; i < itemsFound; i++)
                {
                    if (rand.Next(100) < 30)
                    {
                        encontrados.Add(ilícitos[rand.Next(ilícitos.Length)]);
                    }
                    else
                    {
                        encontrados.Add(lícitos[rand.Next(lícitos.Length)]);
                    }
                }
                
                foreach(string item in encontrados)
                {
                    Game.DisplaySubtitle("~y~Encontrado: ~w~" + item, 2000);
                    GameFiber.Sleep(2500);
                }
                
                Game.DisplayNotification("~b~CACHEO FINALIZADO~n~~w~Se han revisado los bolsillos y pertenencias.");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.FriskPed error: " + ex.Message);
                }
            });
        }

        public static void SearchVehicle(Vehicle vehicle)
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    if (vehicle == null || !vehicle.Exists()) return;
                
                Game.DisplaySubtitle("~y~Registrando vehículo...", 3000);
                GameFiber.Sleep(3000);
                
                Random rand = new Random();
                int itemsFound = rand.Next(1, 3);
                
                List<string> encontrados = new List<string>();
                
                for(int i = 0; i < itemsFound; i++)
                {
                    if (rand.Next(100) < 20)
                    {
                        encontrados.Add(vehiculo[rand.Next(vehiculo.Length)]);
                    }
                    else
                    {
                        encontrados.Add(lícitos[rand.Next(lícitos.Length)]);
                    }
                }
                
                foreach(string item in encontrados)
                {
                    Game.DisplaySubtitle("~y~Maletero: ~w~" + item, 2000);
                    GameFiber.Sleep(2500);
                }
                
                Game.DisplayNotification("~b~REGISTRO DE VEHÍCULO FINALIZADO");
                }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception ex)
                {
                    Game.LogTrivial("SPI.SearchVehicle error: " + ex.Message);
                }
            });
        }
    }
}
