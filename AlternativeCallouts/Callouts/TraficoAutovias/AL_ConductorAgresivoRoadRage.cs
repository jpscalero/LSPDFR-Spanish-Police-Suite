using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.TraficoAutovias
{
    [CalloutInfo("AL_ConductorAgresivoRoadRage", CalloutProbability.High)]
    public class AL_ConductorAgresivoRoadRage : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_ConductorAgresivoRoadRage"; } }
        public override string DisplayName { get { return "Trafico ATGC: Furia al Volante con Intimidacion"; } }
        public override string DispatchSubtitle { get { return "Conductor persigue y acosa a otro turismo tras una discusion en incorporacion a la autovia."; } }
        public override int ThreatLevel { get { return 3; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1200.4f, 1850.4f, 52.4f), new Vector3(1850.4f, 2600.5f, 45.6f), new Vector3(2450.2f, 3100.8f, 48.2f), new Vector3(-420.5f, 3850.4f, 40.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("BALLER", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("g_m_m_armgoon_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ La presencia policial calma los animos. Los dos conductores deponen su actitud y se dispersan.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_CROWBAR"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El conductor agresivo empuña una barra de hierro y ataca a la victima y a la policia!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El acosador frena, abandona su todoterreno y huye corriendo por un puente peatonal!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El agresor embiste al coche de la victima y se da a la fuga a toda velocidad!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL50"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡El agresor saca una pistola y dispara contra la luna delantera del otro turismo!");
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
