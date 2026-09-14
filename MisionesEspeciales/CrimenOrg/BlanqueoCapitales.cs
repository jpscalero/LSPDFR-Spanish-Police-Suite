using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MisionesEspeciales.Core;

namespace MisionesEspeciales.CrimenOrg
{
    [CalloutInfo("Blanqueo de Capitales y Empresa Pantalla", CalloutProbability.Medium)]
    public class BlanqueoCapitales : BaseMisionCallout
    {
        private bool docsSearched;

        public BlanqueoCapitales()
        {
            docsSearched = false;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "UDEF / Audiencia Nacional: Entrada y registro en asesoría mercantil por presunto blanqueo.";
            return base.OnBeforeCalloutDisplayed();
        }

        protected override void OnPlayerArrived()
        {
            Game.DisplaySubtitle("~b~[UDEF]~w~ Proceda al inmueble con orden judicial. Identifique al testaferro y asegure documentación.", 6000);

            // Testaferro y contable
            SpawnSuspect(SpawnPoint.Around(2f), "a_m_m_prolhost_01", "", 100, 0);
            SpawnSuspect(SpawnPoint.Around(4f), "a_m_m_og_boss_01", "WEAPON_PISTOL", 100, 0);

            // Guardaespaldas privado
            SpawnSuspect(SpawnPoint.Around(6f), "s_m_m_armoured_01", "WEAPON_COMBATPISTOL", 120, 100);

            SpawnCalloutVehicle(SpawnPoint.Around(8f), "cognoscenti", 0f);

            foreach (Ped s in Suspects)
            {
                if (s != null && s.Exists() && s.IsAlive)
                {
                    if (s.Inventory.Weapons.Count > 0)
                    {
                        s.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                    else
                    {
                        s.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                    }
                }
            }
        }

        protected override void OnScenarioTick()
        {
            if (IsScenarioFinished) return;

            if (!docsSearched && Game.LocalPlayer.Character.DistanceTo(SpawnPoint) < 4f)
            {
                docsSearched = true;
                Game.DisplayNotification("~g~[UDEF]~w~ Localizada caja fuerte con libros de contabilidad 'B' y 1.200.000€ en efectivo.");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);
            }

            bool anyAlive = false;
            foreach (Ped suspect in Suspects)
            {
                if (suspect != null && suspect.Exists() && suspect.IsAlive && !Functions.IsPedArrested(suspect))
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive && docsSearched)
            {
                CompleteMission("Entrada y Registro UDEF Completada", "Testaferros detenidos y registros contables incautados a disposición judicial.");
            }
        }
    }
}
