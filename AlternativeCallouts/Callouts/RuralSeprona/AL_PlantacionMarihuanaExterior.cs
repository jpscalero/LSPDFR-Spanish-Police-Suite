using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.RuralSeprona
{
    [CalloutInfo("AL_PlantacionMarihuanaExterior", CalloutProbability.High)]
    public class AL_PlantacionMarihuanaExterior : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_PlantacionMarihuanaExterior"; } }
        public override string DisplayName { get { return "Guardia Civil SEPRONA: Plantacion Guerrilla de Cannabis en Monte"; } }
        public override string DispatchSubtitle { get { return "Vigilancia aerea o senderistas alertan de cultivo exterior oculto entre pinos y maleza."; } }
        public override int ThreatLevel { get { return 4; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-1600.4f, -420.2f, 40.5f), new Vector3(-680.2f, 320.5f, 83.2f), new Vector3(-450.4f, 750.2f, 185.5f), new Vector3(-780.2f, 950.4f, 215.2f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("REBEL", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("g_m_y_mexgoon_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Pequeño cultivo artesanal sin custodios armados. Se procede a la incautacion y corte de plantas.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_MACHETE"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El vigilante del cultivo salta de una caseta armado con un machete de poda!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡Dos sospechosos arrojan las herramientas de riego y huyen ladera arriba!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡Los cultivadores abordan una camioneta 4x4 cargada de fardos y huyen a toda prisa!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Macroplantacion custodiada por cartel con fusiles de asalto y trampas perimetrales!");
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
