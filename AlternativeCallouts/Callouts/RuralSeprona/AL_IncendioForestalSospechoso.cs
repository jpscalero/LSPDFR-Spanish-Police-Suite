using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.RuralSeprona
{
    [CalloutInfo("AL_IncendioForestalSospechoso", CalloutProbability.High)]
    public class AL_IncendioForestalSospechoso : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_IncendioForestalSospechoso"; } }
        public override string DisplayName { get { return "Guardia Civil SEPRONA: Conato de Incendio Forestal Sospechoso"; } }
        public override string DispatchSubtitle { get { return "Columna de humo visible en zona boscosa protegida bajo alerta maxima de calor."; } }
        public override int ThreatLevel { get { return 3; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-450.4f, 2850.2f, 34.5f), new Vector3(1850.2f, 2650.4f, 45.2f), new Vector3(2550.4f, 3850.2f, 41.5f), new Vector3(-1100.2f, 4250.4f, 30.2f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("SANCHEZ", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("a_m_m_farmer_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Quema de rastrojos agricolas descontrolada; el agricultor colabora con batefuegos para extinguirla.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_HATCHET"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡Sujeto sospechoso de pirómano arremete con un hacha de mano contra la patrulla!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El piromano suelta una garrafa de gasolina y huye corriendo barranco abajo!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El sospechoso arranca una moto de cross y huye a gran velocidad por senderos!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_MOLOTOV"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Piromano armado arrojando cocteles molotov para cortar el avance de la Guardia Civil!");
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
