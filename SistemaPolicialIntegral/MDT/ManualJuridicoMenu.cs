using System;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SistemaPolicialIntegral.Core;

namespace SistemaPolicialIntegral.MDT
{
    public static class ManualJuridicoMenu
    {
        public static UIMenu CreateMenu(MenuPool pool)
        {
            UIMenu menu = new UIMenu("MANUAL JURÍDICO", "~b~MARCO LEGAL Y DOCTRINA POLICIAL");
            pool.Add(menu);

            // Submenú 1: Marco Constitucional
            UIMenu subConstitucion = new UIMenu("CONSTITUCIÓN Y FCS", "~y~Garantías fundamentales y LO 2/1986");
            pool.Add(subConstitucion);
            UIMenuItem itemConst = new UIMenuItem("TÍTULO I: Constitución y Principios FCS", "Artículos 17, 18 y 104 CE y principios de la LOFCS 2/1986.");
            menu.AddItem(itemConst);
            menu.BindMenuToItem(subConstitucion, itemConst);
            PoblarConstitucion(subConstitucion);

            // Submenú 2: Seguridad Ciudadana
            UIMenu subSegCiudadana = new UIMenu("SEGURIDAD CIUDADANA", "~y~Ley Orgánica 4/2015 (LOSC)");
            pool.Add(subSegCiudadana);
            UIMenuItem itemSeg = new UIMenuItem("TÍTULO II: Seguridad Ciudadana (LO 4/2015)", "Identificaciones, DNI Mod23, cacheos, cortes de via y sanciones.");
            menu.AddItem(itemSeg);
            menu.BindMenuToItem(subSegCiudadana, itemSeg);
            PoblarSegCiudadana(subSegCiudadana);

            // Submenú 3: Detención y LECrim
            UIMenu subDetencion = new UIMenu("DETENCIÓN Y LECRIM", "~y~Garantías procesales y derechos");
            pool.Add(subDetencion);
            UIMenuItem itemDet = new UIMenuItem("TÍTULO III: Detención y Derechos (LECrim)", "Supuestos de detención, lectura de derechos Art. 520 y plazo de 72h.");
            menu.AddItem(itemDet);
            menu.BindMenuToItem(subDetencion, itemDet);
            PoblarDetencion(subDetencion);

            // Submenú 4: Código Penal
            UIMenu subCodigoPenal = new UIMenu("CÓDIGO PENAL", "~r~Tipificación de delitos y penas");
            pool.Add(subCodigoPenal);
            UIMenuItem itemPenal = new UIMenuItem("TÍTULO IV: Código Penal (LO 10/1995)", "Atentado, homicidio, violencia de genero UFAM, drogas y SEPRONA.");
            menu.AddItem(itemPenal);
            menu.BindMenuToItem(subCodigoPenal, itemPenal);
            PoblarCodigoPenal(subCodigoPenal);

            // Submenú 5: Tráfico y Seguridad Vial
            UIMenu subTrafico = new UIMenu("TRÁFICO Y DGT", "~y~Tasas de alcohol, drogas y delitos viales");
            pool.Add(subTrafico);
            UIMenuItem itemTraf = new UIMenuItem("TÍTULO V: Tráfico y Seguridad Vial (DGT)", "Alcoholemia, drogas, exceso de velocidad y delitos Art. 379 a 385 CP.");
            menu.AddItem(itemTraf);
            menu.BindMenuToItem(subTrafico, itemTraf);
            PoblarTrafico(subTrafico);

            // Submenú 6: Uso Gradual de la Fuerza
            UIMenu subFuerza = new UIMenu("USO DE LA FUERZA", "~b~Escala y medios intermedios");
            pool.Add(subFuerza);
            UIMenuItem itemFuerza = new UIMenuItem("TÍTULO VI: Uso Proporcional de la Fuerza", "Escala de 5 niveles, Spray OC, Taser NMI y arma reglamentaria.");
            menu.AddItem(itemFuerza);
            menu.BindMenuToItem(subFuerza, itemFuerza);
            PoblarFuerza(subFuerza);

            // Submenú 7: Protocolos Tácticos CQB e IFAK
            UIMenu subTactico = new UIMenu("TÁCTICA E IFAK", "~r~Asalto CQB y Botiquín MARCH");
            pool.Add(subTactico);
            UIMenuItem itemTact = new UIMenuItem("TÍTULO VII: Protocolos Tácticos y Sanitarios", "Formacion en Stack, brechas, flashbangs y primeros auxilios MARCH.");
            menu.AddItem(itemTact);
            menu.BindMenuToItem(subTactico, itemTact);
            PoblarTactico(subTactico);

            // Submenú 8: Diligencias y Atestados
            UIMenu subAtestados = new UIMenu("ATESTADOS JUDICIALES", "~g~Documentación y diligencias");
            pool.Add(subAtestados);
            UIMenuItem itemAtest = new UIMenuItem("TÍTULO VIII: Redacción de Atestados", "Estructura formal, acta de derechos y archivo en SPI_Data.");
            menu.AddItem(itemAtest);
            menu.BindMenuToItem(subAtestados, itemAtest);
            PoblarAtestados(subAtestados);

            return menu;
        }

        private static void PoblarConstitucion(UIMenu menu)
        {
            UIMenuItem item17 = new UIMenuItem("Art. 17 CE: Libertad y Reloj de 72h", "Plazo máximo de detención preventiva y solicitud de Habeas Corpus.");
            UIMenuItem item18 = new UIMenuItem("Art. 18 CE: Inviolabilidad del Domicilio", "Prohibición de entrada sin orden judicial salvo flagrante delito.");
            UIMenuItem item104 = new UIMenuItem("Art. 104 CE: Misión Constitucional de FCS", "Proteger el libre ejercicio de los derechos y libertades públicas.");
            UIMenuItem itemLofcs = new UIMenuItem("Art. 5 LO 2/1986: Deontología Policial", "Principios de oportunidad, congruencia y estricta proporcionalidad.");

            menu.AddItem(item17);
            menu.AddItem(item18);
            menu.AddItem(item104);
            menu.AddItem(itemLofcs);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem item, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (item == item17)
                {
                    ShowLegalCard("ARTÍCULO 17 CE", "Derecho a la Libertad y Seguridad",
                        "~y~Plazo máximo:~w~ 72 horas de custodia policial preventiva.~n~" +
                        "~y~Garantía:~w~ Lectura formal inmediata de derechos y procedimiento de Habeas Corpus si la detención carece de base legal.");
                }
                else if (item == item18)
                {
                    ShowLegalCard("ARTÍCULO 18 CE", "Inviolabilidad de Domicilio",
                        "~y~Regla general:~w~ Requiere mandamiento judicial o consentimiento del morador.~n~" +
                        "~r~Excepción Operativa:~w~ Flagrante delito o necesidad urgente de socorro (permite asalto táctico CQB inmediato).");
                }
                else if (item == item104)
                {
                    ShowLegalCard("ARTÍCULO 104 CE", "Misión Policial",
                        "~b~Mandato:~w~ Las Fuerzas y Cuerpos de Seguridad garantizarán la seguridad ciudadana bajo la dependencia del Gobierno de la Nación.");
                }
                else if (item == itemLofcs)
                {
                    ShowLegalCard("ART. 5 LO 2/1986", "Principios Básicos de Actuación",
                        "~g~Adecuación, Congruencia y Proporcionalidad.~w~~n~" +
                        "El uso de armas de fuego se reserva para situaciones de riesgo racionalmente grave e inminente para la vida o integridad física.");
                }
            };
        }

        private static void PoblarSegCiudadana(UIMenu menu)
        {
            UIMenuItem item16 = new UIMenuItem("Art. 16: Identificación en Vía Pública", "Requisitos legales y algoritmo DNI Mod23 (Traslado max 6h).");
            UIMenuItem item18 = new UIMenuItem("Art. 18: Registro de Vehículos", "Comprobaciones en vehículos para prevención o incautación de ilícitos.");
            UIMenuItem item20 = new UIMenuItem("Art. 20: Registros Superficiales (Cacheo)", "Cacheo preventivo de personas por agentes del mismo sexo.");
            UIMenuItem item17 = new UIMenuItem("Art. 17: Cortes de Vía y Bloqueos", "Controles de paso, bandas de clavos y bloqueos tácticos en V.");
            UIMenuItem item36_6 = new UIMenuItem("Art. 36.6: Desobediencia / Resistencia", "Infracción grave: Sanción de 601€ a 30.000€.");
            UIMenuItem item37_4 = new UIMenuItem("Art. 37.4: Faltas de Respeto a FCS", "Infracción leve: Sanción de 100€ a 600€ sin detención.");

            menu.AddItem(item16);
            menu.AddItem(item18);
            menu.AddItem(item20);
            menu.AddItem(item17);
            menu.AddItem(item36_6);
            menu.AddItem(item37_4);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem item, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (item == item16)
                {
                    ShowLegalCard("ART. 16 LO 4/2015", "Identificación de Personas",
                        "~y~Procedencia:~w~ Indicios de participación en infracción o prevención.~n~" +
                        "~b~Algoritmo DNI:~w~ Número mod 23 contrastado con tabla de 23 letras.~n~" +
                        "~w~Traslado a dependencia: solo si no es posible la identificación in situ (máx 6h).");
                }
                else if (item == item18)
                {
                    ShowLegalCard("ART. 18 LO 4/2015", "Registro de Vehículos",
                        "~y~Facultad:~w~ Los agentes podrán practicar comprobaciones en el habitáculo y maletero para impedir el porte de armas, explosivos o drogas.");
                }
                else if (item == item20)
                {
                    ShowLegalCard("ART. 20 LO 4/2015", "Cacheo de Seguridad",
                        "~y~Garantías:~w~ Realizado por agente del mismo sexo, respetando la dignidad personal y en lugar reservado si se retiran prendas accesorias.");
                }
                else if (item == item17)
                {
                    ShowLegalCard("ART. 17 LO 4/2015", "Controles y Bloqueos en Vía",
                        "~b~Medios tácticos:~w~ Autoriza el establecimiento de dispositivos estáticos de control, bandas de clavos y bloqueos con unidades policiales.");
                }
                else if (item == item36_6)
                {
                    ShowLegalCard("ART. 36.6 LOSC", "Desobediencia Administrativa",
                        "~r~Infracción Grave.~w~ Negativa a cumplir órdenes legítimas o entorpecimiento sin alcanzar violencia penal. Multa: ~y~601€ a 30.000€~w~.");
                }
                else if (item == item37_4)
                {
                    ShowLegalCard("ART. 37.4 LOSC", "Faltas de Respeto a Policía",
                        "~y~Infracción Leve.~w~ Insultos o menosprecio verbal directo. Procede ~g~acta de denuncia~w~ sin privación de libertad (100€ a 600€).");
                }
            };
        }

        private static void PoblarDetencion(UIMenu menu)
        {
            UIMenuItem itemSupuestos = new UIMenuItem("Arts. 490/492 LECrim: Supuestos Legales", "In fraganti, intento de delito, fuga o requisitoria judicial.");
            UIMenuItem item520 = new UIMenuItem("Art. 520 LECrim: Lectura de Derechos", "Los 7 derechos irrenunciables de toda persona privada de libertad.");
            UIMenuItem itemReloj = new UIMenuItem("Régimen de 72 Horas de Custodia", "Plazo legal estricto antes de pasar a disposición judicial.");

            menu.AddItem(itemSupuestos);
            menu.AddItem(item520);
            menu.AddItem(itemReloj);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem item, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (item == itemSupuestos)
                {
                    ShowLegalCard("ARTS. 490 Y 492 LECRIM", "Causas Legales de Detención",
                        "~r~1.~w~ Al delincuente in fraganti.~n~" +
                        "~r~2.~w~ A quien fugare de custodia penal.~n~" +
                        "~r~3.~w~ A quien figure con orden judicial de busca y captura en SITEL.~n~" +
                        "~r~4.~w~ A quien no ofreciere fianza ni domicilio conocido tras cometer delito.");
                }
                else if (item == item520)
                {
                    ShowLegalCard("ART. 520 LECRIM", "Estatuto del Detenido",
                        "~y~1.~w~ Guardar silencio y no declarar.~n~" +
                        "~y~2.~w~ No declarar contra sí mismo ni confesarse culpable.~n~" +
                        "~y~3.~w~ Abogado particular o de oficio.~n~" +
                        "~y~4.~w~ Comunicación telefónica con familiar.~n~" +
                        "~y~5.~w~ Asistencia médica forense y consular.");
                }
                else if (item == itemReloj)
                {
                    ShowLegalCard("RÉGIMEN TEMPORAL", "Reloj de Custodia Policial (72h)",
                        "~b~Límite legal:~w~ La detención preventiva finalizará en el tiempo estrictamente necesario. En un ~r~máximo de 72 horas~w~ el detenido debe pasar a disposición del Juzgado de Guardia o en libertad con cargos.");
                }
            };
        }

        private static void PoblarCodigoPenal(UIMenu menu)
        {
            UIMenuItem item550 = new UIMenuItem("Art. 550 CP: Atentado a la Autoridad", "Agresión física o intimidación grave: Prisión de 1 a 4 años.");
            UIMenuItem item556 = new UIMenuItem("Art. 556 CP: Resistencia y Desobediencia", "Resistencia activa o pasiva grave tenaz: Prisión 3 meses a 1 año.");
            UIMenuItem item138 = new UIMenuItem("Arts. 138/139 CP: Homicidio y Asesinato", "Muerte dolosa o cualificada: Prisión de 10 a 25 años.");
            UIMenuItem itemUfam = new UIMenuItem("Arts. 153/468 CP: Violencia de Género UFAM", "Maltrato en ámbito familiar y quebrantamiento de alejamiento.");
            UIMenuItem item368 = new UIMenuItem("Art. 368 CP: Tráfico de Drogas", "Sustancias que causan grave daño (3-6 años) y menor daño (1-3 años).");
            UIMenuItem itemSeprona = new UIMenuItem("Delitos Ambientales SEPRONA", "Furtivismo (335), vertidos (325), incendios (351) y maltrato (340 bis).");

            menu.AddItem(item550);
            menu.AddItem(item556);
            menu.AddItem(item138);
            menu.AddItem(itemUfam);
            menu.AddItem(item368);
            menu.AddItem(itemSeprona);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem item, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (item == item550)
                {
                    ShowLegalCard("ARTÍCULO 550 CP", "Atentado a la Autoridad",
                        "~r~Pena:~w~ Prisión de 1 a 4 años y multa de 3 a 6 meses.~n~" +
                        "~y~Conducta típica:~w~ Agresión física o intimidación grave contra funcionarios policiales en el ejercicio de sus funciones.");
                }
                else if (item == item556)
                {
                    ShowLegalCard("ARTÍCULO 556 CP", "Resistencia Grave",
                        "~r~Pena:~w~ Prisión de 3 meses a 1 año o multa de 6 a 18 meses.~n~" +
                        "~y~Conducta:~w~ Oposición tenaz y reiterada a los mandatos legítimos de los agentes actuantes.");
                }
                else if (item == item138)
                {
                    ShowLegalCard("ARTS. 138 Y 139 CP", "Homicidio y Asesinato",
                        "~r~Homicidio:~w~ 10 a 15 años de prisión.~n~" +
                        "~r~Asesinato (alevosía, precio, ensañamiento):~w~ 15 a 25 años.~n~" +
                        "Competencia exclusiva: Audiencia Provincial con Tribunal del Jurado.");
                }
                else if (item == itemUfam)
                {
                    ShowLegalCard("LEGISLACIÓN UFAM", "Violencia sobre la Mujer y Familiar",
                        "~r~Art. 153 CP:~w~ Maltrato físico/psíquico. Prisión 6 meses a 1 año.~n~" +
                        "~r~Art. 468 CP:~w~ Quebrantamiento de orden de alejamiento. Detención forzosa in situ e informe urgente a UFAM.");
                }
                else if (item == item368)
                {
                    ShowLegalCard("ARTÍCULO 368 CP", "Salud Pública (Drogas)",
                        "~r~Grave daño a la salud:~w~ Cocaína, heroína, MDMA, metanfetamina (3 a 6 años de prisión).~n~" +
                        "~y~Menor daño:~w~ Cannabis, hachís (1 a 3 años de prisión).");
                }
                else if (item == itemSeprona)
                {
                    ShowLegalCard("LEGISLACIÓN SEPRONA", "Medio Ambiente y Animales",
                        "~g~Art. 325 CP:~w~ Vertidos tóxicos industriales (6 meses a 2 años).~n~" +
                        "~g~Art. 335 CP:~w~ Caza y pesca furtiva (Multa e inhabilitación).~n~" +
                        "~g~Art. 340 bis CP:~w~ Maltrato animal grave (3 a 18 meses).~n~" +
                        "~r~Art. 351 CP:~w~ Incendio forestal (10 a 20 años).");
                }
            };
        }

        private static void PoblarTrafico(UIMenu menu)
        {
            UIMenuItem itemCuadro = new UIMenuItem("Cuadro Comparativo Alcoholemia DGT", "0.25 mg/l general, 0.15 mg/l novel y >0.60 mg/l penal.");
            UIMenuItem item379_2 = new UIMenuItem("Art. 379.2 CP: Alcoholemia Penal", "Superar 0.60 mg/l en aire espirado: Delito y retirada de carnet.");
            UIMenuItem item383 = new UIMenuItem("Art. 383 CP: Negativa a Pruebas", "Delito con 6 meses a 1 año de prisión y privación de carnet.");
            UIMenuItem itemVel = new UIMenuItem("Art. 379.1 CP: Exceso Grave de Velocidad", "+60 km/h en vía urbana o +80 km/h en vía interurbana.");
            UIMenuItem itemTemeraria = new UIMenuItem("Arts. 380/381 CP: Conducción Temeraria y Kamikaze", "Manifiesto desprecio a la vida humana y prisión efectiva.");

            menu.AddItem(itemCuadro);
            menu.AddItem(item379_2);
            menu.AddItem(item383);
            menu.AddItem(itemVel);
            menu.AddItem(itemTemeraria);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem item, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (item == itemCuadro)
                {
                    ShowLegalCard("CUADRO DGT ALCOHOL", "Tasas en Aire Espirado",
                        "~g~General:~w~ 0.00 a 0.25 mg/l (Correcto).~n~" +
                        "~y~Infracción Administrativa:~w~ 0.25 a 0.60 mg/l (500€ a 1000€ y 4-6 puntos).~n~" +
                        "~r~Delito Penal:~w~ Más de 0.60 mg/l (Art. 379.2 CP).~n~" +
                        "~y~Noveles / Profesionales:~w~ Límite administrativo 0.15 mg/l.");
                }
                else if (item == item379_2)
                {
                    ShowLegalCard("ARTÍCULO 379.2 CP", "Delito contra la Seguridad Vial",
                        "~r~Tasa superior a 0.60 mg/l.~w~~n~" +
                        "Prisión de 3 a 6 meses, multa o TBC, y en todo caso ~y~retirada del permiso de conducir de 1 a 4 años~w~. Procede detención y atestado.");
                }
                else if (item == item383)
                {
                    ShowLegalCard("ARTÍCULO 383 CP", "Negativa a Someterse a Prueba",
                        "~r~Delito autónomo.~w~ Negarse a soplar en el etilómetro Dräger o al test salival se castiga con ~r~prisión de 6 meses a 1 año~w~ y retirada de carnet de 1 a 4 años.");
                }
                else if (item == itemVel)
                {
                    ShowLegalCard("ARTÍCULO 379.1 CP", "Velocidad Constitutiva de Delito",
                        "~r~Umbrales penales:~w~~n~" +
                        "- Superar en ~y~+60 km/h~w~ el límite en vía urbana.~n~" +
                        "- Superar en ~y~+80 km/h~w~ el límite en vía interurbana.~n~" +
                        "Pena: Prisión 3-6 meses y pérdida de carnet 1-4 años.");
                }
                else if (item == itemTemeraria)
                {
                    ShowLegalCard("ARTS. 380 Y 381 CP", "Temeridad Vial y Conducción Kamikaze",
                        "~r~Art. 380 CP:~w~ Conducción temeraria manifiesta (Prisión 6 meses a 2 años).~n~" +
                        "~r~Art. 381 CP:~w~ Manifiesto desprecio por la vida ajena (Prisión 2 a 5 años y comiso definitivo del vehículo).");
                }
            };
        }

        private static void PoblarFuerza(UIMenu menu)
        {
            UIMenuItem itemEscala = new UIMenuItem("Escala de los 5 Niveles de Fuerza", "Desde la presencia física hasta el uso de armas de fuego.");
            UIMenuItem itemOc = new UIMenuItem("Spray Pimienta (OC): Protocolo de Empleo", "Distancia operativa (1-3m) y prohibiciones en espacios cerrados.");
            UIMenuItem itemTaser = new UIMenuItem("Dispositivo Taser X26P (NMI)", "Incapacitación neuromuscular y zonas anatómicas recomendadas.");
            UIMenuItem itemLetal = new UIMenuItem("Uso del Arma de Fuego Letal", "Legítima defensa, proporcionalidad y riesgo inminente grave.");

            menu.AddItem(itemEscala);
            menu.AddItem(itemOc);
            menu.AddItem(itemTaser);
            menu.AddItem(itemLetal);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem item, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (item == itemEscala)
                {
                    ShowLegalCard("ESCALA DE FUERZA", "Instrucción Secretaría de Estado",
                        "~b~Nivel 1:~w~ Presencia policial y patrulla.~n~" +
                        "~b~Nivel 2:~w~ Persuasión y diálogo verbal.~n~" +
                        "~y~Nivel 3:~w~ Reducción física y técnicas de control.~n~" +
                        "~y~Nivel 4:~w~ Medios intermedios (Spray OC / Taser / Bastón).~n~" +
                        "~r~Nivel 5:~w~ Fuerza letal (Arma de fuego).");
                }
                else if (item == itemOc)
                {
                    ShowLegalCard("SPRAY GAS PIMIENTA (OC)", "Medio Intermedio No Letal",
                        "~y~Alcance idóneo:~w~ 1 a 3 metros.~n~" +
                        "~y~Efectos:~w~ Irritación conjuntival severa y cierre de párpados.~n~" +
                        "~r~Prohibición:~w~ No usar a menos de 50 cm ni en recintos cerrados sin ventilación.");
                }
                else if (item == itemTaser)
                {
                    ShowLegalCard("TASER X26P (NMI)", "Incapacitación Neuromuscular",
                        "~y~Mecanismo:~w~ Descarga de alto voltaje y bajo amperaje para anular la respuesta motriz voluntaria.~n~" +
                        "~g~Zonas de impacto:~w~ Torso bajo y espalda.~n~" +
                        "~r~Evitar:~w~ Cabeza, ojos, cuello y genitales.");
                }
                else if (item == itemLetal)
                {
                    ShowLegalCard("ARMA DE FUEGO", "Última Ratio Operativa",
                        "~r~Exclusivamente ante agresión ilegítima~w~ que ponga en peligro inminente y grave la vida o integridad del policía o de terceros.~n~" +
                        "Disparos intimidatorios al aire solo si no existe riesgo para terceras personas.");
                }
            };
        }

        private static void PoblarTactico(UIMenu menu)
        {
            UIMenuItem itemCqb = new UIMenuItem("Asalto Táctico CQB en Inmuebles", "Stack de asalto, brecha mecánica/explosiva y aturdidora flashbang.");
            UIMenuItem itemIfak = new UIMenuItem("Botiquín Táctico IFAK (Protocolo MARCH)", "Torniquete CAT, vendaje hemostático, DEA y Naloxona.");

            menu.AddItem(itemCqb);
            menu.AddItem(itemIfak);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem item, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (item == itemCqb)
                {
                    ShowLegalCard("TÁCTICA CQB", "Protocolo de Asalto en Inmuebles",
                        "~r~1. Stack:~w~ Columna pegada al muro junto a la puerta.~n~" +
                        "~r~2. Brecha:~w~ Ariete táctico, escopeta o carga explosiva C4.~n~" +
                        "~y~3. Flashbang:~w~ Desorientación sensorial durante 5 segundos.~n~" +
                        "~b~4. Entrada dinámica:~w~ Dominación cruzada de esquinas.");
                }
                else if (item == itemIfak)
                {
                    ShowLegalCard("BOTIQUÍN TÁCTICO IFAK", "Protocolo Sanitario MARCH",
                        "~r~M (Massive Bleeding):~w~ Torniquete CAT a 5-7 cm sobre la herida.~n~" +
                        "~y~A (Airway):~w~ Apertura y desobstrucción de vía aérea.~n~" +
                        "~b~R (Respiration):~w~ Sellado de herida torácica.~n~" +
                        "~g~C (Circulation):~w~ Desfibrilador DEA o Naloxona.~n~" +
                        "~w~H (Hypothermia):~w~ Manta térmica y evacuación SAMUR.");
                }
            };
        }

        private static void PoblarAtestados(UIMenu menu)
        {
            UIMenuItem itemEstructura = new UIMenuItem("Estructura Oficial del Atestado CNP", "Diligencia de inicio, TIP, lectura de derechos y remisión judicial.");
            UIMenuItem itemCustodia = new UIMenuItem("Cadena de Custodia de Efectos", "Precinto, registro y preservación de piezas de convicción.");

            menu.AddItem(itemEstructura);
            menu.AddItem(itemCustodia);

            menu.OnItemSelect += delegate (UIMenu sender, UIMenuItem item, int index)
            {
                AudioEngine.PlayMenuSelect();
                if (item == itemEstructura)
                {
                    ShowLegalCard("ATESTADO POLICIAL", "Diligencias Previas en SPI_Data/",
                        "~b~1.~w~ Diligencia de inicio y exposición de hechos.~n~" +
                        "~b~2.~w~ Identificación de agentes por carnet profesional (TIP).~n~" +
                        "~b~3.~w~ Acta formal de lectura de derechos (Art. 520 LECrim).~n~" +
                        "~b~4.~w~ Calificación penal preliminar y remisión al Juzgado de Guardia.");
                }
                else if (item == itemCustodia)
                {
                    ShowLegalCard("CADENA DE CUSTODIA", "Preservación Probatoria",
                        "~y~Garantía procesal:~w~ Toda arma, dinero, droga o sustancia incautada debe ser inventariada con número de serie, embolsada con precinto judicial y remitida a la Policía Científica sin alteraciones.");
                }
            };
        }

        private static void ShowLegalCard(string titulo, string subtitulo, string contenido)
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~" + titulo, "~g~" + subtitulo, contenido);
            Game.DisplaySubtitle("~b~[MANUAL JURÍDICO] ~w~" + titulo + " - " + subtitulo, 5000);
            Game.LogTrivial("ManualJuridicoMenu: Mostrando ficha legal " + titulo + " (" + subtitulo + ")");
        }
    }
}
