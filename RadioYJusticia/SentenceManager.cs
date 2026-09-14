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
            // 1. Tráfico y Seguridad Vial (Arts. 379 a 385 CP)
            catalog.Add(new PenalCrimeDefinition(
                "Art. 379.2 CP",
                "Alcoholemia positiva penal",
                "3 a 6 meses de prision o multa de 6 a 12 meses + retirada de carnet de 1 a 4 años",
                "4 meses de prision sustituida por multa de 720 euros y 1 año y 6 meses de privacion del derecho a conducir",
                "6 meses de prision efectiva en centro penitenciario, multa de 1.440 euros y 3 años y 6 meses de retirada de carnet"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 379.1 CP",
                "Exceso grave de velocidad",
                "3 a 6 meses de prision o multa de 6 a 12 meses + retirada de carnet de 1 a 4 años",
                "Multa de 900 euros (6 meses a 5€/dia) y 1 año y 1 dia de retirada de permiso de conducir",
                "5 meses de prision, multa de 1.800 euros y 3 años de retirada de carnet"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 380 CP",
                "Conduccion temeraria",
                "6 meses a 2 años de prision + retirada de carnet de 1 a 6 años",
                "1 año de prision condicional y 2 años de retirada de permiso de conducir",
                "2 años de prision de obligado cumplimiento y 5 años de retirada de carnet"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 381 CP",
                "Conduccion kamikaze o manifiesto desprecio a la vida",
                "2 a 5 años de prision + multa de 12 a 24 meses + retirada carnet 6 a 10 años",
                "3 años de prision efectiva, multa de 2.400 euros, 6 años de retirada de carnet y comiso de vehiculo",
                "5 años de prision (limite maximo legal), 10 años de retirada de carnet y comiso definitivo del vehiculo"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 383 CP",
                "Negativa a someterse a pruebas de alcoholemia o drogas",
                "6 meses a 1 año de prision + retirada de carnet de 1 a 4 años",
                "8 meses de prision y 1 año y 6 meses de privacion del derecho a conducir",
                "1 año de prision efectiva con agravante de desobediencia y 3 años de retirada de carnet"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 384 CP",
                "Conduccion sin permiso o perdida de puntos",
                "3 a 6 meses de prision o multa de 12 a 24 meses o 31 a 90 dias TBC",
                "Multa de 1.440 euros (12 meses a 4€/dia) y 40 dias de trabajos en beneficio de la comunidad",
                "6 meses de prision de cumplimiento efectivo por quebrantamiento continuado"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 385 CP",
                "Creacion de grave riesgo para la circulacion",
                "6 meses a 2 años de prision o multa de 12 a 24 meses y TBC",
                "1 año de prision sustituida por multa de 1.200 euros y reparacion integra del daño vial",
                "2 años de prision efectiva y responsabilidad civil por los daños ocasionados"
            ));

            // 2. Orden Público y Autoridad (Arts. 550, 556, 557, 564 CP)
            catalog.Add(new PenalCrimeDefinition(
                "Art. 550 CP",
                "Atentado a la autoridad",
                "1 a 4 años de prision y multa de 3 a 6 meses",
                "2 años de prision e inhabilitacion especial para el sufragio pasivo",
                "3 años y 9 meses de prision firme con aplicacion de la agravante de reincidencia (Art. 22.8 CP)"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 556 CP",
                "Resistencia y desobediencia grave",
                "3 meses a 1 año de prision o multa de 6 a 18 meses",
                "6 meses de prision sustituidos por trabajos en beneficio de la comunidad (TBC)",
                "1 año de prision firme por reincidencia reiterada"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 557 CP",
                "Desordenes publicos graves",
                "6 meses a 3 años de prision",
                "1 año y 6 meses de prision y prohibicion de acudir a concentraciones",
                "3 años de prision firme y pago solidario de los desperfectos urbanos causados"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 564 CP",
                "Tenencia ilicita de armas",
                "1 a 3 años de prision",
                "1 año y 6 meses de prision y comiso definitivo del arma intervenida",
                "2 años y 10 meses de prision de cumplimiento efectivo"
            ));

            // 3. Delitos contra las Personas (Arts. 138, 139, 147, 148, 163, 169 CP)
            catalog.Add(new PenalCrimeDefinition(
                "Art. 138 CP",
                "Homicidio",
                "10 a 15 años de prision",
                "12 años y 6 meses de prision mayor y 10 años de libertad vigilada",
                "15 años de prision mayor (limite maximo legal), 10 años de alejamiento e indemnizacion de 250.000 euros"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 139 CP",
                "Asesinato cualificado",
                "15 a 25 años de prision",
                "20 años de prision mayor e indemnizacion a herederos",
                "25 años de prision de cumplimiento integro y privacion de patria potestad"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 147 CP",
                "Lesiones que requieren tratamiento medico o quirurgico",
                "3 meses a 3 años de prision o multa de 6 a 12 meses",
                "1 año de prision y pago de las secuelas medicas peritadas",
                "2 años y 6 meses de prision efectiva e indemnizacion forzosa"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 148 CP",
                "Lesiones agravadas con empleo de armas",
                "2 a 5 años de prision",
                "3 años de prision y 5 años de prohibicion de aproximacion a la victima",
                "4 años y 10 meses de prision firme con agravante de reincidencia"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 163 CP",
                "Detencion ilegal y secuestro",
                "4 a 6 años de prision (secuestro 6 a 10 años)",
                "5 años de prision ordinaria y orden de proteccion a la victima",
                "8 años de prision mayor y 10 años de libertad vigilada posterior"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 169 CP",
                "Amenazas graves",
                "1 a 5 años de prision",
                "1 año y 6 meses de prision y 3 años de alejamiento",
                "3 años y 6 meses de prision de obligado cumplimiento"
            ));

            // 4. Violencia de Género y Doméstica (UFAM - Arts. 153, 468 CP)
            catalog.Add(new PenalCrimeDefinition(
                "Art. 153 CP",
                "Violencia de genero o en el ambito domestico",
                "6 meses a 1 año de prision o trabajos en beneficio de la comunidad",
                "9 meses de prision, 2 años de orden de alejamiento y privacion del derecho a armas",
                "1 año de prision efectiva, 5 años de alejamiento controlado por dispositivo telematico"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 468 CP",
                "Quebrantamiento de condena o de orden de alejamiento",
                "6 meses a 1 año de prision",
                "8 meses de prision efectiva sin posibilidad de suspension por reincidencia de cautelar",
                "1 año de prision de cumplimiento integro en modulo de regimen cerrado"
            ));

            // 5. Delitos Patrimoniales (Arts. 234, 237, 242, 244, 263 CP)
            catalog.Add(new PenalCrimeDefinition(
                "Art. 242 CP",
                "Robo con violencia o intimidacion",
                "2 a 5 años de prision",
                "2 años y 6 meses de prision y prohibicion de aproximacion a la victima por 4 años",
                "4 años y 6 meses de prision firme en regimen ordinario y responsabilidad civil integra"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 234 CP",
                "Hurto patrimonial",
                "6 a 18 meses de prision",
                "8 meses de prision o multa de 720 euros si se restituyen los efectos",
                "1 año y 4 meses de prision efectiva por multirreincidencia patrimonial"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 244 CP",
                "Robo y hurto de uso de vehiculo",
                "31 a 90 dias TBC o 1 a 2 años de prision",
                "1 año de prision e indemnizacion al propietario por desperfectos",
                "2 años de prision de cumplimiento integro"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 263 CP",
                "Daños a la propiedad o mobiliario urbano",
                "Multa de 6 a 24 meses o prision de 1 a 3 años si son graves",
                "Multa de 1.200 euros y pago pericial del valor del bien dañado",
                "1 año y 8 meses de prision y abono subsidiario de costas"
            ));

            // 6. Salud Pública y Tráfico de Drogas (Art. 368 CP)
            catalog.Add(new PenalCrimeDefinition(
                "Art. 368 CP",
                "Trafico de drogas que causan grave daño (cocaina/heroina/sinteticas)",
                "3 a 6 años de prision + multa del tanto al triplo",
                "3 años y 6 meses de prision con multa de 15.000 euros",
                "5 años y 6 meses de prision firme, multa de 45.000 euros y comiso de sustancias y vehiculos"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 368 CP (Cannabis)",
                "Trafico de sustancias que no causan grave daño (cannabis/hachis)",
                "1 a 3 años de prision + multa del tanto al duplo",
                "1 año y 6 meses de prision condicional y multa de 3.000 euros",
                "2 años y 9 meses de prision efectiva y comiso de plantacion e instalaciones"
            ));

            // 7. Delitos Ambientales y Protección Animal (SEPRONA - Arts. 325, 335, 340 bis, 351 CP)
            catalog.Add(new PenalCrimeDefinition(
                "Art. 325 CP",
                "Delito ecologico por vertidos toxicos industriales",
                "6 meses a 2 años de prision + multa e inhabilitacion especial",
                "1 año de prision, multa de 18.000 euros y restauracion del equilibrio ecologico",
                "2 años de prision efectiva, multa de 54.000 euros y clausura de instalacion fabril"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 335 CP",
                "Caza furtiva y pesca ilegal",
                "Multa de 8 a 24 meses e inhabilitacion para cazar o pescar de 2 a 5 años",
                "Multa de 2.400 euros, 3 años de retirada de licencia de caza y comiso de armas",
                "Multa de 4.800 euros, 5 años de inhabilitacion absoluta cinegetica y comiso de vehiculo"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 340 bis CP",
                "Maltrato animal domestico con lesiones graves o muerte",
                "3 a 18 meses de prision e inhabilitacion para tenencia de animales de 1 a 4 años",
                "9 meses de prision, inhabilitacion para tener animales por 2 años e indemnizacion clinica",
                "18 meses de prision de cumplimiento integro y 4 años de prohibicion de tenencia"
            ));

            catalog.Add(new PenalCrimeDefinition(
                "Art. 351 CP",
                "Incendio forestal con peligro para la vida humana",
                "10 a 20 años de prision y multa de 12 a 24 meses",
                "12 años de prision mayor y abono integro de los costes de extincion",
                "18 años de prision mayor con aplicacion estricta del limite legal y costas procesales"
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
