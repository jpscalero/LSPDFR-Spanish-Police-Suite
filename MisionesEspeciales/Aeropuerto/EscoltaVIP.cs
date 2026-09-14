using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.Aeropuerto
{
    [CalloutInfo("Policía Aeroportuaria: Escolta de Comitiva VIP y Emboscada", CalloutProbability.Medium)]
    public class EscoltaVIP : BaseMisionCallout
    {
        private Vehicle vipLimo;
        private Ped vip;
        private bool attackersSpawned;

        public EscoltaVIP()
        {
            attackersSpawned = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Policía Aeroportuaria: Dispositivo de escolta y seguridad estática para comitiva diplomática VIP.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[UNIDAD CENTRAL DE PROTECCIÓN]~w~ Proporcione cobertura a la limusina presidencial.", 6000);

            vipLimo = SpawnCalloutVehicle(SpawnPoint, "stretch", 90f);
            vip = SpawnVictim(SpawnPoint.Around(2f), "u_m_m_promourn_01");

            // Escolta policial
            Ped bodyguard = new Ped("s_m_m_armoured_01", SpawnPoint.Around(3f), 0f);
            if (bodyguard != null && bodyguard.Exists())
            {
                bodyguard.IsPersistent = true;
                bodyguard.Inventory.GiveNewWeapon("WEAPON_COMBATPISTOL", 200, true);
                Allies.Add(bodyguard);
            }
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            if (!attackersSpawned && Game.LocalPlayer.Character.DistanceTo(SpawnPoint) < 15f)
            {
                attackersSpawned = true;
                Game.DisplayNotification("~r~¡EMBOSCADA A LA COMITIVA!~w~ Vehículo con sicarios armados intercepta el convoy.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);

                SpawnCalloutVehicle(SpawnPoint.Around(25f), "dubsta", 270f);

                SpawnSuspect(SpawnPoint.Around(22f), "g_m_m_armgoon_01", "WEAPON_ASSAULTRIFLE", 140, 100);
                SpawnSuspect(SpawnPoint.Around(24f), "g_m_m_armgoon_01", "WEAPON_MICROSMG", 120, 50);

                foreach (Ped s in Suspects)
                {
                    if (s != null && s.Exists() && s.IsAlive)
                    {
                        s.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
                return;
            }

            if (attackersSpawned)
            {
                bool anyAlive = false;
                foreach (Ped s in Suspects)
                {
                    if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                    {
                        anyAlive = true;
                        break;
                    }
                }

                if (!anyAlive)
                {
                    CompleteMission("Comitiva VIP Puesta a Salvo", "Atacantes neutralizados y personalidad protegida trasladada al hotel en cápsula de seguridad.");
                }
            }
        }
    }
}
