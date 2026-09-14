using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.PatrullaUrbana
{
    [CalloutInfo("AL_FalsoPolicia", CalloutProbability.High)]
    public class AL_FalsoPolicia : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_FalsoPolicia"; } }
        public override string DisplayName { get { return "Patrulla ZETA: Usurpacion de Funciones Publicas (Falso Policia)"; } }
        public override string DispatchSubtitle { get { return "Aviso de turismo civil con luces estroboscopicas ilegales dando el alto a conductores."; } }
        public override int ThreatLevel { get { return 3; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-450.4f, -320.2f, 34.5f), new Vector3(220.2f, -800.5f, 30.5f), new Vector3(-1050.5f, -450.4f, 36.5f), new Vector3(450.4f, -1020.2f, 28.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("STANIER", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("s_m_y_cop_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Aficionado al motor confiesa haber instalado rotativos por diversión. Se incauta el material y se sanciona.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_BAT"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El falso policia se pone violento, alega inmunidad y arremete con una porra!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡Al ver el coche patrulla oficial, el sospechoso abandona su vehiculo y escapa a pie!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El falso policia activa sus sirenas piratas y acelera huyendo por la avenida!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_COMBATPISTOL"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Banda de 'policias falsos' dedicada al asalto de furgones! ¡Disparan con pistolas reales!");
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
