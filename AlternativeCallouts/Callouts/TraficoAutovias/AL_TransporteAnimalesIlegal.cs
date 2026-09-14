using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.TraficoAutovias
{
    [CalloutInfo("AL_TransporteAnimalesIlegal", CalloutProbability.High)]
    public class AL_TransporteAnimalesIlegal : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_TransporteAnimalesIlegal"; } }
        public override string DisplayName { get { return "Trafico ATGC: Transporte Ilegal de Animales Vivos"; } }
        public override string DispatchSubtitle { get { return "Furgoneta sospechosa de transportar ganado sin guia de origen ni condiciones sanitarias."; } }
        public override int ThreatLevel { get { return 2; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1730.5f, 3280.2f, 41.1f), new Vector3(2135.4f, 4780.2f, 41.0f), new Vector3(-450.4f, 2850.2f, 34.5f), new Vector3(1850.2f, 2650.4f, 45.2f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("BURRITO", SpawnPoint, 0f);
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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ El transportista presenta documentacion zootecnica y subsana el defecto de precinto.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_KNIFE"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El transportista ilegal extrae un cuchillo de campo y arremete contra los agentes!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El infractor abandona la furgoneta y corre a campo abierto entre los matorrales!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El transportista acelera por caminos de tierra intentando despistar a la patrulla!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_ASSAULTRIFLE"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Red de trafico ilegal de especies protegidas! ¡Traficantes armados defienden la carga!");
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
