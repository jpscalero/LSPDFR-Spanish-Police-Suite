using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.RuralSeprona
{
    [CalloutInfo("AL_CazaFurtivaParque", CalloutProbability.High)]
    public class AL_CazaFurtivaParque : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_CazaFurtivaParque"; } }
        public override string DisplayName { get { return "Guardia Civil SEPRONA: Furtivismo en Parque Natural"; } }
        public override string DispatchSubtitle { get { return "Disparos no autorizados en coto de caza protegido durante periodo de veda."; } }
        public override int ThreatLevel { get { return 3; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-680.5f, 5800.2f, 17.5f), new Vector3(450.2f, 5550.4f, 780.2f), new Vector3(-1150.4f, 4950.2f, 220.5f), new Vector3(1500.2f, 6450.4f, 35.2f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("bodhi2", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("a_m_m_hillbilly_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Cazadores con permisos pero fuera de horario: entregan armas y aceptan la infraccion de SEPRONA.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_MACHETE"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El furtivo saca un machete de caza y arremete contra los guardias de SEPRONA!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡Los furtivos sueltan las piezas cobradas y corren hacia el bosque cerrado!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡Los furtivos abordan un todoterreno y huyen a toda prisa por pistas forestales!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_SNIPERRIFLE"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Furtivos profesionales de trofeos protegidos abren fuego con rifles de precision!");
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
