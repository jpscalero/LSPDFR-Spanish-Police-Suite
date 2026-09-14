using System;
using System.Collections.Generic;

namespace SistemaPolicialIntegral.MDT
{
    public class Charge
    {
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public string PenaMinima { get; set; }
        public string PenaMaxima { get; set; }
        public string Gravedad { get; set; }
    }

    public static class SpanishPenalCode
    {
        private static List<Charge> charges;

        public static List<Charge> GetCharges()
        {
            if (charges == null)
            {
                charges = new List<Charge>();
                
                // Tráfico (Infracciones)
                charges.Add(new Charge { Articulo = "65.4a LSV", Descripcion = "Exceso de velocidad", PenaMinima = "100€", PenaMaxima = "600€", Gravedad = "Grave" });
                charges.Add(new Charge { Articulo = "65.5a LSV", Descripcion = "Conducir bajo efectos del alcohol", PenaMinima = "500€", PenaMaxima = "1000€", Gravedad = "Muy Grave" });
                
                // Tráfico (Delitos)
                charges.Add(new Charge { Articulo = "379.1", Descripcion = "Exceso de velocidad (Delito)", PenaMinima = "3 meses prisión", PenaMaxima = "6 meses prisión", Gravedad = "Delito" });
                charges.Add(new Charge { Articulo = "379.2", Descripcion = "Alcoholemia positiva (Delito)", PenaMinima = "3 meses prisión", PenaMaxima = "6 meses prisión", Gravedad = "Delito" });
                charges.Add(new Charge { Articulo = "380", Descripcion = "Conducción temeraria", PenaMinima = "6 meses prisión", PenaMaxima = "2 años prisión", Gravedad = "Delito" });
                charges.Add(new Charge { Articulo = "383", Descripcion = "Negativa a someterse a pruebas", PenaMinima = "6 meses prisión", PenaMaxima = "1 año prisión", Gravedad = "Delito" });
                charges.Add(new Charge { Articulo = "384", Descripcion = "Conducir sin permiso", PenaMinima = "3 meses prisión", PenaMaxima = "6 meses prisión", Gravedad = "Delito" });

                // Orden Público
                charges.Add(new Charge { Articulo = "550", Descripcion = "Atentado contra agente de autoridad", PenaMinima = "1 año prisión", PenaMaxima = "4 años prisión", Gravedad = "Delito" });
                charges.Add(new Charge { Articulo = "556", Descripcion = "Resistencia y desobediencia", PenaMinima = "3 meses prisión", PenaMaxima = "1 año prisión", Gravedad = "Delito" });

                // Delitos Graves
                charges.Add(new Charge { Articulo = "368", Descripcion = "Tráfico de drogas", PenaMinima = "3 años prisión", PenaMaxima = "6 años prisión", Gravedad = "Delito" });
                charges.Add(new Charge { Articulo = "564", Descripcion = "Tenencia ilícita de armas", PenaMinima = "1 año prisión", PenaMaxima = "3 años prisión", Gravedad = "Delito" });
                charges.Add(new Charge { Articulo = "237", Descripcion = "Robo con violencia", PenaMinima = "2 años prisión", PenaMaxima = "5 años prisión", Gravedad = "Delito" });
                charges.Add(new Charge { Articulo = "138", Descripcion = "Homicidio", PenaMinima = "10 años prisión", PenaMaxima = "15 años prisión", Gravedad = "Delito" });
            }
            return charges;
        }
    }
}
