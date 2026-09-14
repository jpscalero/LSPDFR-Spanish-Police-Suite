using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using AlternativeCallouts.Core;

namespace AlternativeCallouts.Callouts.PatrullaUrbana
{
    [CalloutInfo("AL_SeguimientoMotoRobada", CalloutProbability.High)]
    public class AL_SeguimientoMotoRobada : BaseAlternativeCallout
    {
        public override string CalloutIdentifier { get { return "AL_SeguimientoMotoRobada"; } }
        public override string DisplayName { get { return "Patrulla ZETA: Localizacion por GPS de Motocicleta Sustraida"; } }
        public override string DispatchSubtitle { get { return "Propietario aporta senal GPS en tiempo real de ciclomotor sustraido hace 30 minutos."; } }
        public override int ThreatLevel { get { return 2; } }

        private Ped _mainSuspect;
        private Vehicle _sceneVehicle;
        private bool _actionTriggered = false;

        protected override Vector3 SelectSpawnLocation()
        {
            Vector3[] locations = new Vector3[]
            {
                new Vector3(120.4f, -1450.2f, 29.5f), new Vector3(-850.2f, -1200.4f, 6.2f), new Vector3(450.4f, -1850.2f, 28.5f), new Vector3(-1350.2f, -850.4f, 15.5f)
            };
            Random rnd = new Random();
            return locations[rnd.Next(locations.Length)];
        }

        protected override void InitializeScenario()
        {
            _sceneVehicle = SpawnVehicle("SANCHEZ", SpawnPoint, 0f);
            _mainSuspect = SpawnPed("g_m_y_korean_01", SpawnPoint.Around(2f), 0f);

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
                    TriggerPeacefulMediation(_mainSuspect, "~g~[RESOLUCION PACIFICA]~w~ Motocicleta localizada abandonada en un descampado. Recuperada sin incidencias.");
                    break;

                case EAlternativeOutcome.ResistAndFight:
                    TriggerResistAndFight(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_DAGGER"), "~r~[RESISTENCIA Y AGRESION]~w~ ¡El ladron sorprendido forzando el contacto arremete con un destornillador!");
                    break;

                case EAlternativeOutcome.FootPursuit:
                    TriggerFootPursuit(_mainSuspect, "~r~[FUGA A PIE]~w~ ¡El ladron suelta la moto y escapa a la carrera saltando muros entre edificios!");
                    break;

                case EAlternativeOutcome.VehiclePursuit:
                    TriggerVehiclePursuit(_mainSuspect, _sceneVehicle, "~r~[FUGA EN VEHICULO]~w~ ¡El ladron arranca la moto robada y huye a gran velocidad por aceras y callejones!");
                    break;

                case EAlternativeOutcome.SevereCrimeTwist:
                    TriggerSevereCrimeTwist(_mainSuspect, (WeaponHash)Game.GetHashKey("WEAPON_SMG"), "~r~[GIRO CRITICO - ALTO RIESGO]~w~ ¡Taller clandestino de despiece (chop shop)! ¡3 mecanicos armados abren fuego!");
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
