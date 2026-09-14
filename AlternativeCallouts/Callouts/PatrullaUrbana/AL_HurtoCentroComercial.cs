using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.PatrullaUrbana
{
    [CalloutInfo("AL_HurtoCentroComercial", CalloutProbability.High)]
    public class AL_HurtoCentroComercial : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_HurtoCentroComercial"; } }
        public override string DisplayName { get { return "Patrulla ZETA: Hurto en Centro Comercial"; } }
        public override string DispatchSubtitle { get { return "Vigilancia privada solicita apoyo policial ante la retencion de un sospechoso de sustraccion."; } }
        public override int ThreatLevel { get { return 2; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-550.2f, -180.5f, 38.2f), new Vector3(-680.4f, -250.2f, 38.5f), new Vector3(-1380.2f, -480.4f, 33.2f), new Vector3(-850.4f, -420.2f, 37.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("PCJ", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("g_m_y_famca_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ El hurtador entrega los articulos sustraidos pacíficamente. Se redacta acta de denuncia.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_KNIFE"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El hurtador saca una navaja y amenaza a los vigilantes y a la policia!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El sospechoso se zafa y echa a correr zigzagueando por la zona comercial!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El sospechoso corre a su motocicleta en el parking y escapa a toda velocidad!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_PISTOL"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡La mochila hurtada contenia un arma de fuego ilegal y joyas robadas! ¡Abran fuego!");
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
