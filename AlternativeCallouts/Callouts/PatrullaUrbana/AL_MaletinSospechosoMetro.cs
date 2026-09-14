using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.PatrullaUrbana
{
    [CalloutInfo("AL_MaletinSospechosoMetro", CalloutProbability.High)]
    public class AL_MaletinSospechosoMetro : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_MaletinSospechosoMetro"; } }
        public override string DisplayName { get { return "Patrulla ZETA: Objeto Abandonado en Estacion de Metro"; } }
        public override string DispatchSubtitle { get { return "Personal de seguridad de transportes localiza maletin rigido sin dueño en anden."; } }
        public override int ThreatLevel { get { return 2; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(-595.4f, -280.2f, 50.5f), new Vector3(350.2f, -1620.4f, -40.5f), new Vector3(-45.2f, -650.4f, 15.5f), new Vector3(450.4f, -800.2f, -12.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("RUMPO", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("a_m_m_business_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ El propietario acude corriendo: se trataba de un olvido involuntario de material de oficina.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_UNARMED"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡Un ladron cercano intenta arrebatar el maletin y empuja a la patrulla!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El dueño del maletin divisa a los agentes, tira el telefono y huye por las escaleras mecanicas!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El sospechoso alcanza una furgoneta aparcada fuera y emprende la fuga!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡El maletin contenia fajos de dinero negro del cartel! ¡Tirador armado sale de los tuneles!");
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
