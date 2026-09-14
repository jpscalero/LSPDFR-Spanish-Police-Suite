using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.PatrullaUrbana
{
    [CalloutInfo("AL_OkupacionVivienda", CalloutProbability.High)]
    public class AL_OkupacionVivienda : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_OkupacionVivienda"; } }
        public override string DisplayName { get { return "Patrulla ZETA: Okupacion Ilegal Flagrante"; } }
        public override string DispatchSubtitle { get { return "Llamada urgente de propietario alertando de intento de acceso no autorizado a vivienda."; } }
        public override int ThreatLevel { get { return 3; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(450.2f, -1820.4f, 28.5f), new Vector3(1120.4f, -1450.2f, 34.5f), new Vector3(-850.5f, -1250.4f, 5.8f), new Vector3(750.2f, -850.5f, 24.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("SURFER", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("a_m_m_tramp_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Mediacion exitosa: los ocupantes acceden a retirar sus enseres y desalojar voluntariamente.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_BAT"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El ocupante enarbola un bate de beisbol y se niega a abandonar la finca!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡Los sospechosos escapan por una ventana trasera hacia los patios colindantes!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡Los ocupantes cargan una furgoneta y emprenden la huida a trompicones!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Era una tapadera de banda armada! ¡Disparos de subfusil desde el interior!");
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
