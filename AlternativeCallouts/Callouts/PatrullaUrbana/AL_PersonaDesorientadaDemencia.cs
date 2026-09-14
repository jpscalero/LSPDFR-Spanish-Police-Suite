using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.PatrullaUrbana
{
    [CalloutInfo("AL_PersonaDesorientadaDemencia", CalloutProbability.High)]
    public class AL_PersonaDesorientadaDemencia : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_PersonaDesorientadaDemencia"; } }
        public override string DisplayName { get { return "Patrulla ZETA: Asistencia a Anciano Extraviado"; } }
        public override string DispatchSubtitle { get { return "Comerciantes informan de persona mayor desorientada deambulando sin rumbo."; } }
        public override int ThreatLevel { get { return 1; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(300.5f, -1400.2f, 32.5f), new Vector3(360.4f, -585.2f, 28.8f), new Vector3(-450.2f, -340.5f, 34.5f), new Vector3(-875.4f, -305.2f, 39.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("INGOT", SpawnPoint, 0f);
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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Anciano identificado gracias a su pulsera teleasistida. Acompañado sano y salvo a su hogar.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_UNARMED"), "~r~[RESISTENCIA Y AGRESION]~w~ El anciano sufre un brote de agitacion y confunde a los agentes; requiere reduccion suave.");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El anciano se asusta con las sirenas y corre hacia una avenida de gran circulacion!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El anciano confuso se monta en un turismo en marcha y acelera erráticamente!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_KNIFE"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡El anciano estaba siendo coaccionado en un portal por un atracador armado con navaja!");
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
