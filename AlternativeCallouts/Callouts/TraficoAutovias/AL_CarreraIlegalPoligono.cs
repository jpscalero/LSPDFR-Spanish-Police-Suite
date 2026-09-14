using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.TraficoAutovias
{
    [CalloutInfo("AL_CarreraIlegalPoligono", CalloutProbability.High)]
    public class AL_CarreraIlegalPoligono : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_CarreraIlegalPoligono"; } }
        public override string DisplayName { get { return "Trafico ATGC: Concentracion de Carreras Ilegales Nocturnas"; } }
        public override string DispatchSubtitle { get { return "Agrupacion de decenas de jovenes realizando carreras de aceleracion en poligono industrial."; } }
        public override int ThreatLevel { get { return 3; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(1010.5f, -2530.2f, 28.3f), new Vector3(880.4f, -2180.5f, 30.5f), new Vector3(720.4f, -1950.2f, 29.5f), new Vector3(-350.5f, -2600.2f, 6.2f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("SULTAN", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("a_m_y_skater_02", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Los organizadores colaboran, apagan motores y desalojan la zona tras ser apercibidos.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_BAT"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡Varios espectadores agresivos arrojan piedras y bates contra los vehiculos policiales!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El piloto abandona su coche tuneado y corre entre las naves industriales!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡Dos deportivos modificados aceleran a la vez desatando una persecucion multiple!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_MICROSMG"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Apostadores de bandas criminales abren fuego para cubrir la retirada de su cabecilla!");
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
