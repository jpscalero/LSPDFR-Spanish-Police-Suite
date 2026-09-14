using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.TraficoAutovias
{
    [CalloutInfo("AL_ConductorKamikaze", CalloutProbability.High)]
    public class AL_ConductorKamikaze : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_ConductorKamikaze"; } }
        public override string DisplayName { get { return "Trafico ATGC: Conductor en Sentido Contrario (Kamikaze)"; } }
        public override string DispatchSubtitle { get { return "Multiples llamadas al 112 alertando de vehiculo circulando a contramano por la autovia."; } }
        public override int ThreatLevel { get { return 4; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1750.2f, 2600.4f, 45.5f), new Vector3(2450.4f, 3120.2f, 48.5f), new Vector3(-1050.4f, 2800.2f, 22.4f), new Vector3(2100.2f, 2150.5f, 54.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("FUGITIVE", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("a_m_m_mexcntry_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Conductor de avanzada edad desorientado atiende a las señales y se detiene en el arcen con seguridad.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_UNARMED"), "~r~[RESISTENCIA Y AGRESION]~w~ Conductor ebrio detiene el coche pero agrede violentamente a los agentes de Tráfico.");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El conductor frena en seco, salta el quitamiedos y corre campo a traves!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El infractor acelera a 180 km/h esquivando el tráfico en dirección contraria!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_COMBATPISTOL"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Turismo robado por delincuente en busca y captura! ¡Embiste a la patrulla y dispara!");
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
