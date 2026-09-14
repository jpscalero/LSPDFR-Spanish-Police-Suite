using System;
using System.Collections.Generic;

namespace SistemaPolicialIntegral.MDT
{
    public class Charge
    {
        public string Categoria { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public string PenaMinima { get; set; }
        public string PenaMaxima { get; set; }
        public string Gravedad { get; set; } // "Grave", "Muy Grave", "Delito", "Delito Grave", "Infraccion Leve"
        public int MultaEuros { get; set; }
        public int PuntosDGT { get; set; }
        public string FundamentoLegal { get; set; }
        public string AccionOperativa { get; set; }
    }

    public static class SpanishPenalCode
    {
        private static List<Charge> charges;

        public static List<Charge> GetCharges()
        {
            if (charges == null)
            {
                charges = new List<Charge>();

                // =========================================================
                // 1. SEGURIDAD CIUDADANA (LEY ORGÁNICA 4/2015 - LOSC)
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Seguridad Ciudadana",
                    Articulo = "36.6 LOSC",
                    Descripcion = "Desobediencia o resistencia a la autoridad sin violencia",
                    PenaMinima = "601€ multa",
                    PenaMaxima = "30.000€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 601,
                    FundamentoLegal = "Art. 36.6 Ley Orgánica 4/2015 de Protección de la Seguridad Ciudadana",
                    AccionOperativa = "Acta de denuncia administrativa e inmovilización o identificación"
                });

                charges.Add(new Charge
                {
                    Categoria = "Seguridad Ciudadana",
                    Articulo = "37.4 LOSC",
                    Descripcion = "Falta de respeto o insulto a miembro de FCS",
                    PenaMinima = "100€ multa",
                    PenaMaxima = "600€ multa",
                    Gravedad = "Infraccion Leve",
                    MultaEuros = 100,
                    FundamentoLegal = "Art. 37.4 Ley Orgánica 4/2015",
                    AccionOperativa = "Acta de sanción administrativa sin detención"
                });

                charges.Add(new Charge
                {
                    Categoria = "Seguridad Ciudadana",
                    Articulo = "36.16 LOSC",
                    Descripcion = "Consumo o tenencia ilicita de drogas en via publica",
                    PenaMinima = "601€ multa",
                    PenaMaxima = "10.400€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 601,
                    FundamentoLegal = "Art. 36.16 Ley Orgánica 4/2015",
                    AccionOperativa = "Incautación de sustancias y remisión de acta a Delegación de Gobierno"
                });

                charges.Add(new Charge
                {
                    Categoria = "Seguridad Ciudadana",
                    Articulo = "36.10 LOSC",
                    Descripcion = "Porte o exhibicion de armas blancas o prohibidas",
                    PenaMinima = "601€ multa",
                    PenaMaxima = "30.000€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 601,
                    FundamentoLegal = "Art. 36.10 Ley Orgánica 4/2015",
                    AccionOperativa = "Comiso inmediato del arma blanca y boletín de denuncia"
                });

                // =========================================================
                // 2. TRÁFICO - INFRACCIONES ADMINISTRATIVAS (LSV / DGT)
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "65.4a LSV",
                    Descripcion = "Exceso de velocidad reglamentaria",
                    PenaMinima = "100€",
                    PenaMaxima = "600€",
                    Gravedad = "Grave",
                    MultaEuros = 300,
                    PuntosDGT = 2,
                    FundamentoLegal = "Art. 52 Ley de Seguridad Vial / Reglamento General de Circulación",
                    AccionOperativa = "Boletín de denuncia por cinemómetro y detracción de puntos"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "20 RGC / LSV",
                    Descripcion = "Alcoholemia positiva administrativa (0.25 - 0.50 mg/l)",
                    PenaMinima = "500€ multa",
                    PenaMaxima = "500€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 500,
                    PuntosDGT = 4,
                    FundamentoLegal = "Art. 20 RGC y Art. 14 LSV",
                    AccionOperativa = "Inmovilización de vehículo y boletín DGT"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "20 RGC / LSV",
                    Descripcion = "Alcoholemia positiva administrativa (>0.50 mg/l o reincidente)",
                    PenaMinima = "1000€ multa",
                    PenaMaxima = "1000€ multa",
                    Gravedad = "Muy Grave",
                    MultaEuros = 1000,
                    PuntosDGT = 6,
                    FundamentoLegal = "Art. 20 RGC y Art. 80 LSV",
                    AccionOperativa = "Inmovilización cautelar de vehículo y retirada de 6 puntos"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "14 LSV",
                    Descripcion = "Presencia de drogas en el organismo (DrugTest positivo)",
                    PenaMinima = "1000€ multa",
                    PenaMaxima = "1000€ multa",
                    Gravedad = "Muy Grave",
                    MultaEuros = 1000,
                    PuntosDGT = 6,
                    FundamentoLegal = "Art. 14 Ley de Seguridad Vial",
                    AccionOperativa = "Inmovilización de vehículo y prueba de contraste en laboratorio"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "76.g LSV",
                    Descripcion = "Conducir utilizando el telefono movil manualmente",
                    PenaMinima = "200€ multa",
                    PenaMaxima = "200€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 200,
                    PuntosDGT = 6,
                    FundamentoLegal = "Art. 76.g Ley de Tráfico y Seguridad Vial",
                    AccionOperativa = "Boletín de sanción y detracción de 6 puntos"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "76.h LSV",
                    Descripcion = "No utilizar cinturon de seguridad o dispositivo de retencion",
                    PenaMinima = "200€ multa",
                    PenaMaxima = "200€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 200,
                    PuntosDGT = 4,
                    FundamentoLegal = "Art. 76.h Ley de Tráfico y Seguridad Vial",
                    AccionOperativa = "Boletín de sanción y advertencia de seguridad"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "76.c LSV",
                    Descripcion = "No respetar la luz roja del semaforo o señal de STOP",
                    PenaMinima = "200€ multa",
                    PenaMaxima = "200€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 200,
                    PuntosDGT = 4,
                    FundamentoLegal = "Art. 76.c Ley de Tráfico y Seguridad Vial",
                    AccionOperativa = "Boletín de sanción DGT"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "76.m LSV",
                    Descripcion = "Conduccion negligente creando peligro para otros usuarios",
                    PenaMinima = "200€ multa",
                    PenaMaxima = "200€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 200,
                    PuntosDGT = 0,
                    FundamentoLegal = "Art. 76.m Ley de Tráfico",
                    AccionOperativa = "Boletín de sanción administrativa"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "76.o LSV",
                    Descripcion = "Circular con vehiculo sin haber pasado la ITV reglamentaria",
                    PenaMinima = "200€ multa",
                    PenaMaxima = "200€ multa",
                    Gravedad = "Grave",
                    MultaEuros = 200,
                    FundamentoLegal = "Art. 76.o Ley de Tráfico",
                    AccionOperativa = "Boletín de sanción y plazo de 10 días para subsanación"
                });

                charges.Add(new Charge
                {
                    Categoria = "Tráfico DGT",
                    Articulo = "2 SOA",
                    Descripcion = "Circular sin seguro de responsabilidad civil obligatorio",
                    PenaMinima = "1500€ multa",
                    PenaMaxima = "1500€ multa",
                    Gravedad = "Muy Grave",
                    MultaEuros = 1500,
                    FundamentoLegal = "Ley sobre Responsabilidad Civil y Seguro en la Circulación",
                    AccionOperativa = "Inmovilización y retirada inmediata con grúa municipal"
                });

                // =========================================================
                // 3. SEGURIDAD VIAL - DELITOS PENALES (CÓDIGO PENAL)
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Delitos Tráfico",
                    Articulo = "379.1",
                    Descripcion = "Exceso grave de velocidad (>60 km/h urbana / >80 km/h autovia)",
                    PenaMinima = "3 meses prisión",
                    PenaMaxima = "6 meses prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 379.1 Código Penal (Privación de carnet de 1 a 4 años)",
                    AccionOperativa = "Detención o imputación, citación para Juicio Rápido y retirada de carnet"
                });

                charges.Add(new Charge
                {
                    Categoria = "Delitos Tráfico",
                    Articulo = "379.2",
                    Descripcion = "Alcoholemia positiva penal (>0.60 mg/l) o drogas tóxicas",
                    PenaMinima = "3 meses prisión",
                    PenaMaxima = "6 meses prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 379.2 Código Penal (Multa 6 a 12 meses + retirada de carnet 1 a 4 años)",
                    AccionOperativa = "Detención obligatoria, lectura derechos Art. 520 y atestado judicial"
                });

                charges.Add(new Charge
                {
                    Categoria = "Delitos Tráfico",
                    Articulo = "380",
                    Descripcion = "Conduccion temeraria manifiesta con peligro para la vida",
                    PenaMinima = "6 meses prisión",
                    PenaMaxima = "2 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 380 Código Penal (Privación de carnet de 1 a 6 años)",
                    AccionOperativa = "Detención in situ, inmovilización del coche y atestado penal"
                });

                charges.Add(new Charge
                {
                    Categoria = "Delitos Tráfico",
                    Articulo = "381",
                    Descripcion = "Conduccion con manifiesto desprecio a la vida ajena (Kamikaze)",
                    PenaMinima = "2 años prisión",
                    PenaMaxima = "5 años prisión",
                    Gravedad = "Delito Grave",
                    FundamentoLegal = "Art. 381 Código Penal (Comiso de vehículo y retirada de carnet 6 a 10 años)",
                    AccionOperativa = "Bloqueo táctico en V, clavos, detención inmediata y prisión provisional"
                });

                charges.Add(new Charge
                {
                    Categoria = "Delitos Tráfico",
                    Articulo = "383",
                    Descripcion = "Negativa taxativa a someterse a pruebas de alcohol o drogas",
                    PenaMinima = "6 meses prisión",
                    PenaMaxima = "1 año prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 383 Código Penal (Privación de carnet de 1 a 4 años)",
                    AccionOperativa = "Detención instantánea por delito contra la seguridad vial"
                });

                charges.Add(new Charge
                {
                    Categoria = "Delitos Tráfico",
                    Articulo = "384",
                    Descripcion = "Conduccion sin permiso por perdida de puntos o privacion judicial",
                    PenaMinima = "3 meses prisión",
                    PenaMaxima = "6 meses prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 384 Código Penal o multa de 12 a 24 meses",
                    AccionOperativa = "Inmovilización del coche e imputación para comparecencia judicial"
                });

                charges.Add(new Charge
                {
                    Categoria = "Delitos Tráfico",
                    Articulo = "385",
                    Descripcion = "Creacion de grave riesgo para la circulacion en la calzada",
                    PenaMinima = "6 meses prisión",
                    PenaMaxima = "2 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 385 Código Penal (Obstáculos o derrames en calzada)",
                    AccionOperativa = "Aseguramiento de calzada y atestado por riesgo catastrófico vial"
                });

                // =========================================================
                // 4. DELITOS CONTRA EL ORDEN PÚBLICO Y LA AUTORIDAD
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Orden Público",
                    Articulo = "550",
                    Descripcion = "Atentado contra agentes de la autoridad (agresion fisica)",
                    PenaMinima = "1 año prisión",
                    PenaMaxima = "4 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 550 Código Penal y multa de 3 a 6 meses",
                    AccionOperativa = "Uso proporcional de medios intermedios (Taser/Spray), grilletes e IFAK"
                });

                charges.Add(new Charge
                {
                    Categoria = "Orden Público",
                    Articulo = "556",
                    Descripcion = "Resistencia y desobediencia grave a la autoridad",
                    PenaMinima = "3 meses prisión",
                    PenaMaxima = "1 año prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 556 Código Penal o multa de 6 a 18 meses",
                    AccionOperativa = "Detención forzosa y lectura formal de derechos Art. 520 LECrim"
                });

                charges.Add(new Charge
                {
                    Categoria = "Orden Público",
                    Articulo = "557",
                    Descripcion = "Desordenes publicos graves con violencia o pillaje",
                    PenaMinima = "6 meses prisión",
                    PenaMaxima = "3 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 557 Código Penal",
                    AccionOperativa = "Intervención de unidades UPR / UIP y dispersión táctica"
                });

                charges.Add(new Charge
                {
                    Categoria = "Orden Público",
                    Articulo = "564",
                    Descripcion = "Tenencia ilicita de armas de fuego",
                    PenaMinima = "1 año prisión",
                    PenaMaxima = "3 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 564 Código Penal (1-2 años armas cortas, 2-3 sin número)",
                    AccionOperativa = "Comiso bajo cadena de custodia y traslado a Policía Científica"
                });

                // =========================================================
                // 5. DELITOS CONTRA LAS PERSONAS Y LIBERTAD
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Contra las Personas",
                    Articulo = "138",
                    Descripcion = "Homicidio doloso",
                    PenaMinima = "10 años prisión",
                    PenaMaxima = "15 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 138 Código Penal (Audiencia Provincial)",
                    AccionOperativa = "Preservación de escena de homicidio, SAMUR y Policía Científica"
                });

                charges.Add(new Charge
                {
                    Categoria = "Contra las Personas",
                    Articulo = "139",
                    Descripcion = "Asesinato cualificado (alevosia, ensañamiento, recompensa)",
                    PenaMinima = "15 años prisión",
                    PenaMaxima = "25 años prisión",
                    Gravedad = "Delito Grave",
                    FundamentoLegal = "Art. 139 Código Penal",
                    AccionOperativa = "Acordonamiento perimetral total y diligencias judiciales urgentes"
                });

                charges.Add(new Charge
                {
                    Categoria = "Contra las Personas",
                    Articulo = "147",
                    Descripcion = "Lesiones con necesidad de tratamiento quirurgico o medico",
                    PenaMinima = "3 meses prisión",
                    PenaMaxima = "3 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 147 Código Penal o multa de 6 a 12 meses",
                    AccionOperativa = "Primeros auxilios IFAK, traslado hospitalario y detención del agresor"
                });

                charges.Add(new Charge
                {
                    Categoria = "Contra las Personas",
                    Articulo = "148",
                    Descripcion = "Lesiones agravadas con empleo de armas o medios peligrosos",
                    PenaMinima = "2 años prisión",
                    PenaMaxima = "5 años prisión",
                    Gravedad = "Delito Grave",
                    FundamentoLegal = "Art. 148 Código Penal",
                    AccionOperativa = "Detención armada inmediata y aseguramiento de la víctima"
                });

                charges.Add(new Charge
                {
                    Categoria = "Contra las Personas",
                    Articulo = "163",
                    Descripcion = "Detencion ilegal y secuestro de personas",
                    PenaMinima = "4 años prisión",
                    PenaMaxima = "6 años prisión",
                    Gravedad = "Delito Grave",
                    FundamentoLegal = "Art. 163 Código Penal (Secuestro: 6 a 10 años)",
                    AccionOperativa = "Activación de GEO, negociación y asalto táctico CQB coordinado"
                });

                charges.Add(new Charge
                {
                    Categoria = "Contra las Personas",
                    Articulo = "169",
                    Descripcion = "Amenazas graves de mal constitutivo de delito",
                    PenaMinima = "1 año prisión",
                    PenaMaxima = "5 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 169 Código Penal",
                    AccionOperativa = "Protección cautelar de la víctima y detención del autor"
                });

                // =========================================================
                // 6. VIOLENCIA DE GÉNERO Y FAMILIAR (UFAM)
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Violencia Género UFAM",
                    Articulo = "153",
                    Descripcion = "Maltrato en el ambito de la violencia de genero o domestica",
                    PenaMinima = "6 meses prisión",
                    PenaMaxima = "1 año prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 153 Código Penal y privación de tenencia de armas",
                    AccionOperativa = "Detención preceptiva in situ, traslado UFAM y protección policial"
                });

                charges.Add(new Charge
                {
                    Categoria = "Violencia Género UFAM",
                    Articulo = "468",
                    Descripcion = "Quebrantamiento de condena o de orden judicial de alejamiento",
                    PenaMinima = "6 meses prisión",
                    PenaMaxima = "1 año prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 468.2 Código Penal",
                    AccionOperativa = "Detención forzosa inmediata in fraganti y parte urgente al juzgado"
                });

                // =========================================================
                // 7. DELITOS CONTRA EL PATRIMONIO
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Patrimonio",
                    Articulo = "234",
                    Descripcion = "Hurto patrimonial (>400€ valor de lo sustraido)",
                    PenaMinima = "6 meses prisión",
                    PenaMaxima = "18 meses prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 234 Código Penal",
                    AccionOperativa = "Recuperación de efectos hurtados y entrega a su legítimo dueño"
                });

                charges.Add(new Charge
                {
                    Categoria = "Patrimonio",
                    Articulo = "237",
                    Descripcion = "Robo con violencia o intimidacion en las personas",
                    PenaMinima = "2 años prisión",
                    PenaMaxima = "5 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 237 y 242 Código Penal",
                    AccionOperativa = "Detención armada, cacheo exhaustivo de seguridad y atestado penal"
                });

                charges.Add(new Charge
                {
                    Categoria = "Patrimonio",
                    Articulo = "244",
                    Descripcion = "Robo y hurto de uso de vehiculo a motor",
                    PenaMinima = "31 días TBC",
                    PenaMaxima = "1 año prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 244 Código Penal",
                    AccionOperativa = "Interceptación de vehículo, maniobra PIT/Caja y detención"
                });

                charges.Add(new Charge
                {
                    Categoria = "Patrimonio",
                    Articulo = "263",
                    Descripcion = "Daños dolosos en propiedad ajena o bienes publicos",
                    PenaMinima = "Multa 6 meses",
                    PenaMaxima = "Prisión 3 años",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 263 Código Penal",
                    AccionOperativa = "Inspección ocular fotográfica y valoración pericial de daños"
                });

                // =========================================================
                // 8. SALUD PÚBLICA Y TRÁFICO DE DROGAS
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Salud Pública",
                    Articulo = "368",
                    Descripcion = "Trafico de drogas (sustancias que causan grave daño a la salud)",
                    PenaMinima = "3 años prisión",
                    PenaMaxima = "6 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 368 CP (Cocaina, heroina, metanfetamina) + multa triplo",
                    AccionOperativa = "Pesaje judicial, precinto de sustancias y custodia en comisaría"
                });

                charges.Add(new Charge
                {
                    Categoria = "Salud Pública",
                    Articulo = "368",
                    Descripcion = "Trafico de drogas (sustancias que no causan grave daño)",
                    PenaMinima = "1 año prisión",
                    PenaMaxima = "3 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 368 CP (Cannabis, hachis) + multa duplo",
                    AccionOperativa = "Decomiso de fardos o plantas y atestado penal antidroga"
                });

                // =========================================================
                // 9. MEDIO AMBIENTE Y PROTECCIÓN ANIMAL (SEPRONA)
                // =========================================================
                charges.Add(new Charge
                {
                    Categoria = "Medio Ambiente SEPRONA",
                    Articulo = "325",
                    Descripcion = "Delito ecologico por vertidos o emisiones toxicas industriales",
                    PenaMinima = "6 meses prisión",
                    PenaMaxima = "2 años prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 325 Código Penal",
                    AccionOperativa = "Toma de muestras por SEPRONA y precinto preventivo de instalaciones"
                });

                charges.Add(new Charge
                {
                    Categoria = "Medio Ambiente SEPRONA",
                    Articulo = "335",
                    Descripcion = "Caza furtiva o pesca no autorizada de fauna protegida",
                    PenaMinima = "Multa 8 meses",
                    PenaMaxima = "Multa 24 meses",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 335 Código Penal e inhabilitación especial de 2 a 5 años",
                    AccionOperativa = "Comiso de armas de caza, artes prohibidas y piezas cobradas"
                });

                charges.Add(new Charge
                {
                    Categoria = "Medio Ambiente SEPRONA",
                    Articulo = "340 bis",
                    Descripcion = "Maltrato animal domestico causando lesiones graves o muerte",
                    PenaMinima = "3 meses prisión",
                    PenaMaxima = "18 meses prisión",
                    Gravedad = "Delito",
                    FundamentoLegal = "Art. 340 bis Código Penal e inhabilitación para tenencia de animales",
                    AccionOperativa = "Rescate veterinario de animales e imputación penal del responsable"
                });

                charges.Add(new Charge
                {
                    Categoria = "Medio Ambiente SEPRONA",
                    Articulo = "351",
                    Descripcion = "Incendio forestal provocando peligro para la vida humana",
                    PenaMinima = "10 años prisión",
                    PenaMaxima = "20 años prisión",
                    Gravedad = "Delito Grave",
                    FundamentoLegal = "Art. 351 Código Penal",
                    AccionOperativa = "Evacuación civil, apoyo a bomberos y detención urgente del pirómano"
                });
            }
            return charges;
        }

        public static Charge FindByArticle(string articulo)
        {
            List<Charge> all = GetCharges();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].Articulo.IndexOf(articulo, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return all[i];
                }
            }
            return null;
        }
    }
}
