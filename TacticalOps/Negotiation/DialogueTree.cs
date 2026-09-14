using System;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace TacticalOps.Negotiation
{
    public static class DialogueTree
    {
        private static MenuPool pool;
        private static UIMenu menu;
        private static int currentPhase;
        private static bool isProcessingChoice;

        public static int CurrentPhase
        {
            get { return currentPhase; }
        }

        static DialogueTree()
        {
            currentPhase = 1;
            isProcessingChoice = false;
        }

        public static void Start(Ped suspect, Ped hostage)
        {
            currentPhase = 1;
            isProcessingChoice = false;

            if (pool == null)
            {
                pool = new MenuPool();
            }

            BuildPhaseMenu();
            menu.Visible = true;
            Game.LogTrivial("TacticalOps: DialogueTree iniciado en Fase 1.");
        }

        public static void Close()
        {
            if (pool != null)
            {
                pool.CloseAllMenus();
            }
            isProcessingChoice = false;
            currentPhase = 1;
        }

        public static void Process()
        {
            if (pool != null && TensionSystem.IsActive)
            {
                pool.ProcessMenus();
            }
        }

        private static void BuildPhaseMenu()
        {
            string subtitle = "FASE " + currentPhase + " DE 4";
            if (currentPhase == 1)
            {
                subtitle = "FASE 1: TOMA DE CONTACTO";
            }
            else if (currentPhase == 2)
            {
                subtitle = "FASE 2: EMPATIA Y CALMA";
            }
            else if (currentPhase == 3)
            {
                subtitle = "FASE 3: GESTION DE EXIGENCIAS";
            }
            else if (currentPhase == 4)
            {
                subtitle = "FASE 4: ENTREGA PACIFICA";
            }

            menu = new UIMenu("NEGOCIACION TACTICA", "~b~" + subtitle);
            pool = new MenuPool();
            pool.Add(menu);

            UIMenuItem itemCorrect = null;
            UIMenuItem itemNeutral = null;
            UIMenuItem itemBad = null;

            if (currentPhase == 1) // Contacto
            {
                itemCorrect = new UIMenuItem("Voz Tranquilizadora (Megafono)", "«Habla la Policia. Nadie va a disparar, queremos escucharte.»");
                itemNeutral = new UIMenuItem("Aviso Estandar de Presencia", "«Sujeto armado, estas rodeado. Mantente donde estas.»");
                itemBad = new UIMenuItem("Orden Intimidatoria Agresiva", "«¡Tira el arma ahora mismo o entramos con todo!»");
            }
            else if (currentPhase == 2) // Empatia
            {
                itemCorrect = new UIMenuItem("Validacion Emocional y Escucha", "«Se que estas pasando por un infierno, pero esto tiene solucion.»");
                itemNeutral = new UIMenuItem("Enfoque Racional y Consecuencias", "«Piensa con la cabeza fria, herir a alguien empeorara tu situacion.»");
                itemBad = new UIMenuItem("Reproche y Descalificacion", "«¡Deja de quejarte como un cobarde y asume tus actos!»");
            }
            else if (currentPhase == 3) // Exigencias
            {
                itemCorrect = new UIMenuItem("Oferta de Ayuda Medica / Agua", "«Te garantizamos asistencia medica y agua si bajas el arma.»");
                itemNeutral = new UIMenuItem("Postura Neutral sobre Demandas", "«No puedo prometerte nada hasta que no hablemos en calma.»");
                itemBad = new UIMenuItem("Rechazo Rotundo e Inflexible", "«¡No vamos a negociar absolutamente nada con un delincuente!»");
            }
            else // Entrega pacifica
            {
                itemCorrect = new UIMenuItem("Garantia de Integridad Fisica", "«Tienes mi palabra de honor de que saldras ileso. Confia en mi.»");
                itemNeutral = new UIMenuItem("Instrucciones Estandar de Salida", "«Camina despacio hacia la salida con las manos en la nuca.»");
                itemBad = new UIMenuItem("Ultimatum de Entrada Dinamica", "«¡Los GEO van a reventar la puerta en diez segundos!»");
            }

            menu.AddItem(itemCorrect);
            menu.AddItem(itemNeutral);
            menu.AddItem(itemBad);

            menu.OnItemSelect += delegate(UIMenu sender, UIMenuItem selectedItem, int index)
            {
                if (isProcessingChoice)
                {
                    return;
                }

                menu.Visible = false;
                isProcessingChoice = true;

                GameFiber.StartNew(delegate
                {
                    ExecuteDialogueChoice(selectedItem == itemCorrect, selectedItem == itemNeutral, selectedItem == itemBad);
                });
            };
        }

        private static void ExecuteDialogueChoice(bool isCorrect, bool isNeutral, bool isBad)
        {
            Ped suspect = NegotiationManager.SuspectPed;

            if (isCorrect)
            {
                PlayOfficerDialogue(currentPhase, 1);
                GameFiber.Sleep(2600);

                PlaySuspectDialogue(currentPhase, 1);
                TensionSystem.ReduceTension(15.0f);
                GameFiber.Sleep(2000);

                if (currentPhase < 4)
                {
                    currentPhase++;
                    BuildPhaseMenu();
                    menu.Visible = true;
                }
                else
                {
                    // Ha completado las 4 fases con exito
                    if (TensionSystem.Tension <= 35.0f)
                    {
                        NegotiationManager.TriggerSurrender();
                    }
                    else
                    {
                        Game.DisplaySubtitle("~b~Negociador:~w~ «Ya casi estamos. Manten la calma y suelta el arma.»", 3000);
                        BuildPhaseMenu();
                        menu.Visible = true;
                    }
                }
            }
            else if (isNeutral)
            {
                PlayOfficerDialogue(currentPhase, 2);
                GameFiber.Sleep(2600);

                PlaySuspectDialogue(currentPhase, 2);
                GameFiber.Sleep(2000);

                BuildPhaseMenu();
                menu.Visible = true;
            }
            else if (isBad)
            {
                PlayOfficerDialogue(currentPhase, 3);
                GameFiber.Sleep(2600);

                PlaySuspectDialogue(currentPhase, 3);
                TensionSystem.AddTension(20.0f);
                GameFiber.Sleep(2000);

                if (TensionSystem.Tension < 100.0f)
                {
                    BuildPhaseMenu();
                    menu.Visible = true;
                }
            }

            isProcessingChoice = false;
        }

        private static void PlayOfficerDialogue(int phase, int type)
        {
            string text = "";
            if (phase == 1)
            {
                if (type == 1) text = "~b~Negociador (Megafono):~w~ «Habla la Policia. Nadie va a disparar, queremos escucharte.»";
                else if (type == 2) text = "~b~Negociador (Megafono):~w~ «Sujeto armado, estas rodeado. Mantente donde estas.»";
                else text = "~r~Oficial:~w~ «¡Tira el arma ahora mismo o entramos con todo!»";
            }
            else if (phase == 2)
            {
                if (type == 1) text = "~b~Negociador (Megafono):~w~ «Se que estas pasando por un infierno, pero esto tiene solucion.»";
                else if (type == 2) text = "~b~Negociador (Megafono):~w~ «Piensa con la cabeza fria, herir a alguien empeorara tu situacion.»";
                else text = "~r~Oficial:~w~ «¡Deja de quejarte como un cobarde y asume tus actos!»";
            }
            else if (phase == 3)
            {
                if (type == 1) text = "~b~Negociador (Megafono):~w~ «Te garantizamos asistencia medica y agua si bajas el arma.»";
                else if (type == 2) text = "~b~Negociador (Megafono):~w~ «No puedo prometerte nada hasta que no hablemos en calma.»";
                else text = "~r~Oficial:~w~ «¡No vamos a negociar absolutamente nada con un delincuente!»";
            }
            else
            {
                if (type == 1) text = "~b~Negociador (Megafono):~w~ «Tienes mi palabra de honor de que saldras ileso. Confia en mi.»";
                else if (type == 2) text = "~b~Negociador (Megafono):~w~ «Camina despacio hacia la salida con las manos en la nuca.»";
                else text = "~r~Oficial:~w~ «¡Los GEO van a reventar la puerta en diez segundos!»";
            }

            Game.DisplaySubtitle(text, 3000);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
        }

        private static void PlaySuspectDialogue(int phase, int type)
        {
            string text = "";
            if (phase == 1)
            {
                if (type == 1) text = "~y~Sospechoso:~w~ «¡No... no os acerqueis! ¡Solo quiero que me escuchen!»";
                else if (type == 2) text = "~y~Sospechoso:~w~ «¡Se perfectamente donde estoy! ¡No intenteis nada raro!»";
                else text = "~r~Sospechoso:~w~ «¡¿Que vais a entrar?! ¡Acercate y le vuelo la cabeza!»";
            }
            else if (phase == 2)
            {
                if (type == 1) text = "~y~Sospechoso:~w~ «Nadie me hacia caso... me lo han quitado todo...»";
                else if (type == 2) text = "~y~Sospechoso:~w~ «¡No me des sermones morales, no sabes nada de mi vida!»";
                else text = "~r~Sospechoso:~w~ «¡¿Cobarde yo?! ¡Ahora vais a ver de lo que soy capaz!»";
            }
            else if (phase == 3)
            {
                if (type == 1) text = "~y~Sospechoso:~w~ «Necesito agua... tengo mucha sed... dejadme pensar...»";
                else if (type == 2) text = "~y~Sospechoso:~w~ «¡Si no hay garantias, de aqui no sale nadie vivo!»";
                else text = "~r~Sospechoso:~w~ «¡¿Sin negociacion?! ¡Pues se acabo la conversacion!»";
            }
            else
            {
                if (type == 1) text = "~g~Sospechoso:~w~ «Esta bien... no dispareis por favor... voy a soltarla...»";
                else if (type == 2) text = "~y~Sospechoso:~w~ «Dadme un minuto... quiero ver que no hay francotiradores apuntandome.»";
                else text = "~r~Sospechoso:~w~ «¡¡Pues que vengan!! ¡¡Moriremos todos aqui dentro!!»";
            }

            Game.DisplaySubtitle(text, 3000);
            if (type == 3)
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Bed", "WastedSounds", false);
            }
            else if (type == 1)
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "MEDAL_UP", "HUD_MINI_GAME_SOUNDSET", false);
            }
        }
    }
}
