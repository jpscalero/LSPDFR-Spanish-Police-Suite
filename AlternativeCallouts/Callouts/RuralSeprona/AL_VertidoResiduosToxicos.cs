using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.RuralSeprona
{
    [CalloutInfo("AL_VertidoResiduosToxicos", CalloutProbability.High)]
    public class AL_VertidoResiduosToxicos : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_VertidoResiduosToxicos"; } }
        public override string DisplayName { get { return "Guardia Civil SEPRONA: Vertido Clandestino de Residuos Contaminantes"; } }
        public override string DispatchSubtitle { get { return "Furgoneta o camion sospechoso arrojando bidones en barranco cercano a cauce fluvial."; } }
        public override int ThreatLevel { get { return 3; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1650.5f, 20.2f, 165.5f), new Vector3(1710.2f, 85.4f, 165.5f), new Vector3(1580.4f, -45.2f, 165.5f), new Vector3(1690.2f, 120.5f, 165.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("RUBBLE", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("s_m_m_trucker_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Infractor acata la orden de paralizacion y recojo de escombros. Se instruye acta por delito contra el medio ambiente.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_CROWBAR"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El conductor arremete con una barra de desencofrar contra la patrulla de SEPRONA!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El sujeto abandona la furgoneta y corre campo a traves ocultandose en un cañizal!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El camion pesado acelera por la pista de tierra derramando residuos toxicos en su huida!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_CARBINERIFLE"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Vertido de residuos quimicos de narcotrafico! ¡Sicarios de custodia armados disparan!");
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
