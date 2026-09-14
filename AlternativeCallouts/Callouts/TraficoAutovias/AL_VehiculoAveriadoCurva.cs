using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.TraficoAutovias
{
    [CalloutInfo("AL_VehiculoAveriadoCurva", CalloutProbability.High)]
    public class AL_VehiculoAveriadoCurva : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_VehiculoAveriadoCurva"; } }
        public override string DisplayName { get { return "Trafico ATGC: Vehiculo Averiado en Curva sin Visibilidad"; } }
        public override string DispatchSubtitle { get { return "Peligro grave en calzada por turismo inmovilizado en curva ciega sin iluminacion."; } }
        public override int ThreatLevel { get { return 2; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-750.2f, 1280.4f, 328.2f), new Vector3(760.4f, 1275.2f, 360.5f), new Vector3(-430.4f, 950.2f, 240.2f), new Vector3(1850.2f, 3680.4f, 40.2f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("ASTEROPE", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("a_m_y_stbla_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Vehiculo protegido con rotativos policiales; grua de auxilio llega y retira el turismo.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_WRENCH"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El conductor ebrio empuña una llave inglesa y amenaza a los guardias de Trafico!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El individuo que desvalijaba el coche averiado huye corriendo ladera abajo!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El conductor consigue arrancar a empujones y emprende la huida a trompicones!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_PUMPSHOTGUN"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡El coche habia participado en un tiroteo! ¡Los ocupantes ocultos abren fuego con escopeta!");
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
