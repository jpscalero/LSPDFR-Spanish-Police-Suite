using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.RuralSeprona
{
    [CalloutInfo("AL_SenderistaExtraviado", CalloutProbability.High)]
    public class AL_SenderistaExtraviado : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_SenderistaExtraviado"; } }
        public override string DisplayName { get { return "Guardia Civil GREIM: Rescate de Excursionista en Montana"; } }
        public override string DispatchSubtitle { get { return "Llamada SOS de senderista desorientado con principio de hipotermia en sendero escarpado."; } }
        public override int ThreatLevel { get { return 1; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-420.5f, 1120.4f, 325.8f), new Vector3(-750.2f, 1280.5f, 328.5f), new Vector3(720.4f, 1250.2f, 360.5f), new Vector3(-2250.4f, 250.2f, 169.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("BLAZER", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("a_m_y_hiker_01", SpawnPoint.Around(2f), 0f);

            if (_mainSuspect != null)
            {
                AttachBlipToSuspect(_mainSuspect, System.Drawing.Color.Orange, 0.65f);
                Suspects.Add(_mainSuspect);
            }
        }

        protected override void OnPlayerArrival()
        {
            if (_actionTriggered) return;
            _actionTriggered = true;

            switch (SelectedOutcome)
            {
                case EAlternativeOutcome.PeacefulCompliance:
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Senderista localizado deshidratado; se le suministran liquidos y se le acompaña a zona segura.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_UNARMED"), "~r~[RESISTENCIA Y AGRESION]~w~ El senderista, bajo shock termico severo, forcejea desorientado con los rescatadores.");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El supuesto senderista era un delincuente escondido y echa a correr al ver uniformes!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El individuo salta a un quad y acelera por senderos de descenso montañoso!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡El excursionista habia sido tomado como rehen por furtivos armados que abren fuego!");
                    break;
            }
        }

        protected override void OnScenarioTick()
        {
            if (SelectedOutcome == EAlternativeOutcome.PeacefulCompliance)
            {
                Ped player = Game.LocalPlayer.Character;
                if (player != null && player.Exists() && _mainSuspect != null && _mainSuspect.Exists())
                {
                    if (player.Position.DistanceTo(_mainSuspect.Position) < 3.5f)
                    {
                        Game.DisplayHelp("Presiona ~INPUT_CONTEXT~ para ~g~completar la mediacion y archivar la actuacion~w~.");
                        if (Game.IsControlJustPressed(0, GameControl.Context))
                        {
                            NotifyCode4();
                        }
                    }
                }
            }
        }

        protected override void CheckCompletionCriteria()
        {
            if (SelectedOutcome == EAlternativeOutcome.PeacefulCompliance) return;

            bool activeHostiles = false;
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive && !LSPD_First_Response.Mod.API.Functions.IsPedArrested(s))
                {
                    activeHostiles = true;
                    break;
                }
            }

            if (!activeHostiles)
            {
                NotifyCode4();
            }
        }
    }
}
