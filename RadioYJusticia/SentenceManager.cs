using System;
using System.Collections.Generic;

namespace RadioYJusticia
{
    public class PenalCrimeDefinition
    {
        public string Articulo { get; set; }
        public string Delito { get; set; }
        public string RangoLegal { get; set; }
        public string PenaEstandar { get; set; }
        public string PenaReincidente { get; set; }

        public PenalCrimeDefinition(string articulo, string delito, string rangoLegal, string penaEstandar, string penaReincidente)
        {
            Articulo = articulo;
            Delito = delito;
            RangoLegal = rangoLegal;
            PenaEstandar = penaEstandar;
            PenaReincidente = penaReincidente;
        }
    }

    public static class SentenceManager
    {
        private static readonly List<PenalCrimeDefinition> catalog = new List<PenalCrimeDefinition>();
        private static readonly Random random = new Random();

        static SentenceManager()
        {
            // 1. Alcoholemia positiva penal
            catalog.Add(new PenalCrimeDefinition(
                "Art. 379.2 CP",
                "Alcoholemia positiva penal",
                "3 a 6 meses de prision o multa de 6 a 12 meses + retirada de carnet de 1 a 4 años",
                "4 meses de prision sustituida por multa de 720 euros y 1 año y 6 meses de privacion del derecho a conducir",
                "6 meses de prision efectiva en centro penitenciario, multa de 1.440 euros y 3 años y 6 meses de retirada de carnet"
            ));

            // 2. Atentado a la autoridad
            catalog.Add(new PenalCrimeDefinition(
                "Art. 550 CP",
                "Atentado a la autoridad",
                "1 a 4 años de prision",
                "2 años de prision e inhabilitacion especial para el sufragio pasivo",
                "3 años y 9 meses de prision firme con aplicacion de la agravante de reincidencia (Art. 22.8 CP)"
            ));

            // 3. Tráfico de drogas
            catalog.Add(new PenalCrimeDefinition(
                "Art. 368 CP",
                "Trafico de drogas",
                "3 a 6 años de prision + multa del tanto al triplo",
                "3 años y 6 meses de prision con multa de 15.000 euros",
                "5 años y 6 meses de prision firme, multa de 45.000 euros y comiso de sustancias y vehiculos"
            ));

            // 4. Robo con violencia
            catalog.Add(new PenalCrimeDefinition(
                "Art. 242 CP",
                "Robo con violencia",
                "2 a 5 años de prision",
                "2 años y 6 meses de prision y prohibicion de aproximacion a la victima por 4 años",
                "4 años y 6 meses de prision firme en régimen ordinario y responsabilidad civil íntegra"
            ));

            // 5. Homicidio
            catalog.Add(new PenalCrimeDefinition(
                "Art. 138 CP",
                "Homicidio",
                "10 a 15 años de prision",
                "12 años y 6 meses de prision mayor y 10 años de libertad vigilada",
                "15 años de prision mayor (limite maximo legal), 10 años de alejamiento e indemnizacion de 250.000 euros"
            ));

            // Delitos complementarios del Código Penal
            catalog.Add(new PenalCrimeDefinition(
                "Art. 380 CP",
                "Conduccion temeraria",
                "6 meses a 2 años de prision + retirada de carnet de 1 a 6 años",
                "1 año de prision y 2 años de retirada de permiso de conducir",
                "2 años de prision de obligado cumplimiento y 5 años de retirada de carnet"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 556 CP",
                "Resistencia y desobediencia grave",
                "3 meses a 1 año de prision o multa de 6 a 18 meses",
                "6 meses de prision sustituidos por trabajos en beneficio de la comunidad (TBC)",
                "1 año de prision firme por reincidencia reiterada"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 564 CP",
                "Tenencia ilicita de armas",
                "1 a 3 años de prision",
                "1 año y 6 meses de prision y comiso definitivo del arma",
                "2 años y 10 meses de prision de cumplimiento efectivo"
            ));
        }

        public static string CalculateSentence(string delito, bool reincidente)
        {
            if (string.IsNullOrEmpty(delito))
            {
                delito = "Atentado a la autoridad (Art. 550 CP)";
            }

            for (int i = 0; i < catalog.Count; i++)
            {
                PenalCrimeDefinition def = catalog[i];
                if (delito.IndexOf(def.Delito, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    delito.IndexOf(def.Articulo, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (reincidente)
                    {
                        return def.PenaReincidente;
                    }
                    else
                    {
                        return def.PenaEstandar;
                    }
                }
            }

            // Fallback genérico aplicando normativa de Código Penal español
            if (reincidente)
            {
                return "3 años de prision efectiva con agravante de reincidencia (Art. 22.8 CP) y costas procesales";
            }
            else
            {
                return "1 año y 6 meses de prision sustituible conforme al Art. 80 CP y multa de 540 euros";
            }
        }

        public static PenalCrimeDefinition GetRandomCrimeDefinition()
        {
            int idx = random.Next(catalog.Count);
            return catalog[idx];
        }

        public static List<PenalCrimeDefinition> GetAllCatalog()
        {
            return new List<PenalCrimeDefinition>(catalog);
        }
    }
}
