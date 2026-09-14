using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;

namespace TacticalAndFederalCallouts.Core
{
    /// <summary>
    /// Motor de realismo balistico, supresion de fuego, dano corporal, triaje de heridas y combate tactico.
    /// </summary>
    public static class CombatRealismManager
    {
        private static readonly Dictionary<Ped, int> _previousArmor = new Dictionary<Ped, int>();
        private static readonly HashSet<Ped> _injuredPeds = new HashSet<Ped>();
        private static readonly HashSet<Ped> _crawlingPeds = new HashSet<Ped>();
        private static readonly Random _rnd = new Random();

        public static void InitializePedRealism(Ped ped, bool isNightRaid = false)
        {
            if (ped == null || !ped.Exists()) return;

            try
            {
                _previousArmor[ped] = ped.Armor;

                // Activacion de linterna de arma tactica para operaciones en oscuridad
                if (isNightRaid || World.TimeOfDay.Hours >= 20 || World.TimeOfDay.Hours < 6)
                {
                    NativeFunction.Natives.SET_FLASH_LIGHT_ENABLED(ped, true);
                }

                NativeFunction.Natives.SET_PED_CAN_USE_COVER(ped, true);
                NativeFunction.Natives.SET_PED_CAN_PEEK_IN_COVER(ped, true);
                NativeFunction.Natives.SET_PED_FIRING_PATTERN(ped, unchecked((int)0xC6EE6B4C)); // FullAutomatic
            }
            catch (Exception ex)
            {
                Game.LogTrivial("CombatRealismManager.ConfigureTacticalCombatant: " + ex.Message);
            }
        }

        public static void ProcessCombatTick(List<Ped> combatants, Ped player)
        {
            if (combatants == null || combatants.Count == 0) return;

            for (int i = 0; i < combatants.Count; i++)
            {
                Ped ped = combatants[i];
                if (ped == null || !ped.Exists() || !ped.IsAlive) continue;

                try
                {
                    // 1. Simulacion de absorcion de impacto en chaleco antibalas (Kevlar/Placas)
                    int currentArmor = ped.Armor;
                    int prevArmor = _previousArmor.ContainsKey(ped) ? _previousArmor[ped] : currentArmor;

                    if (currentArmor < prevArmor)
                    {
                        _previousArmor[ped] = currentArmor;
                        TriggerArmorImpactEffect(ped);
                    }

                    // 2. Degradacion por heridas (Cojera y Shock)
                    if (ped.Health < 65 && !_injuredPeds.Contains(ped))
                    {
                        _injuredPeds.Add(ped);
                        ApplyInjuredWalkStyle(ped);
                    }

                    // 3. Herida critica (Cae de rodillas / Transicion a pistola secundaria)
                    if (ped.Health < 30 && !_crawlingPeds.Contains(ped))
                    {
                        _crawlingPeds.Add(ped);
                        ApplyCriticalWoundBehavior(ped, player);
                    }

                    // 4. Supresion balistica y gritos de combate realistas
                    if (ped.IsInCombat && _rnd.Next(0, 100) < 12)
                    {
                        TriggerTacticalSpeech(ped);
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("TacticalAndFederalCallouts.CombatRealismManager.ProcessPeds: " + ex.Message);
                }
            }
        }

        private static void TriggerArmorImpactEffect(Ped ped)
        {
            try
            {
                NativeFunction.Natives.REQUEST_NAMED_PTFX_ASSET("core");
                if (NativeFunction.Natives.HAS_NAMED_PTFX_ASSET_LOADED<bool>("core"))
                {
                    NativeFunction.Natives.USE_PARTICLE_FX_ASSET("core");
                    NativeFunction.Natives.START_PARTICLE_FX_NON_LOOPED_ON_PED_BONE(
                        "bullet_sparks", ped, 0f, 0.1f, 0.2f, 0f, 0f, 0f,
                        0x2E28, 1.0f, false, false, false
                    );
                }
                NativeFunction.Natives.PLAY_PED_AMBIENT_SPEECH_NATIVE(ped, "GENERIC_OUCH", "SPEECH_PARAMS_FORCE_SHOUTED");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts.CombatRealismManager.TriggerArmorImpactEffect: " + ex.Message);
            }
        }

        private static void ApplyInjuredWalkStyle(Ped ped)
        {
            try
            {
                NativeFunction.Natives.REQUEST_ANIM_SET("move_m@injured");
                if (NativeFunction.Natives.HAS_ANIM_SET_LOADED<bool>("move_m@injured"))
                {
                    NativeFunction.Natives.SET_PED_MOVEMENT_CLIPSET(ped, "move_m@injured", 1.0f);
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts.CombatRealismManager.ApplyInjuredWalkStyle: " + ex.Message);
            }
        }

        private static void ApplyCriticalWoundBehavior(Ped ped, Ped player)
        {
            try
            {
                NativeFunction.Natives.SET_PED_DROPS_WEAPON_WHEN_DEAD(ped, true);
                uint pistolHash = NativeFunction.Natives.GET_HASH_KEY<uint>("WEAPON_PISTOL");
                if (ped.Inventory.Weapons.Contains((WeaponHash)pistolHash))
                {
                    ped.Inventory.EquippedWeapon = (WeaponHash)pistolHash;
                }
                NativeFunction.Natives.SET_PED_ACCURACY(ped, 25);
                NativeFunction.Natives.PLAY_PED_AMBIENT_SPEECH_NATIVE(ped, "GENERIC_CURSE_MED", "SPEECH_PARAMS_FORCE_SHOUTED");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts.CombatRealismManager.ApplyCriticalWoundBehavior: " + ex.Message);
            }
        }

        private static void TriggerTacticalSpeech(Ped ped)
        {
            try
            {
                string[] shouts = new string[] { "COVER_ME", "RELOAD", "GENERIC_WAR_CRY", "NEED_BACKUP" };
                string shout = shouts[_rnd.Next(shouts.Length)];
                NativeFunction.Natives.PLAY_PED_AMBIENT_SPEECH_NATIVE(ped, shout, "SPEECH_PARAMS_FORCE_SHOUTED");
            }
            catch (Exception ex)
            {
                Game.LogTrivial("TacticalAndFederalCallouts.CombatRealismManager.TriggerTacticalSpeech: " + ex.Message);
            }
        }

        public static void CleanupPed(Ped ped)
        {
            if (ped == null) return;
            if (_previousArmor.ContainsKey(ped)) _previousArmor.Remove(ped);
            if (_injuredPeds.Contains(ped)) _injuredPeds.Remove(ped);
            if (_crawlingPeds.Contains(ped)) _crawlingPeds.Remove(ped);
        }

        public static void ClearAll()
        {
            _previousArmor.Clear();
            _injuredPeds.Clear();
            _crawlingPeds.Clear();
        }
    }
}
