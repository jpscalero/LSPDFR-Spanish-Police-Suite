using System;
using System.Globalization;

namespace SistemaPolicialIntegral.Census
{
    public static class DniModulo23
    {
        private static readonly string TABLA_LETRAS = "TRWAGMYFPDXBNJZSQVHLCKE";
        private static Random sharedRandom = new Random();

        public static char CalcularLetra(int numero)
        {
            return TABLA_LETRAS[numero % 23];
        }

        public static string GenerarDniAleatorio()
        {
            int numero = sharedRandom.Next(10000000, 99999999);
            char letra = CalcularLetra(numero);
            return string.Format("{0}{1}", numero, letra);
        }

        public static bool ValidarDni(string dni)
        {
            if (string.IsNullOrEmpty(dni) || dni.Length != 9) return false;
            string numPart = dni.Substring(0, 8);
            char letraPart = dni[8];

            for (int i = 0; i < numPart.Length; i++)
            {
                if (numPart[i] < '0' || numPart[i] > '9') return false;
            }

            int num;
            if (int.TryParse(numPart, NumberStyles.None, CultureInfo.InvariantCulture, out num))
            {
                return CalcularLetra(num) == char.ToUpper(letraPart);
            }
            return false;
        }
    }
}
