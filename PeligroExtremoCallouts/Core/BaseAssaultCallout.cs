using System;
using System.Drawing;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;

namespace PeligroExtremoCallouts.Core
{
    /// <summary>
    /// Especializacion para situaciones de tiroteo urbano y asaltos a mano armada.
    /// Incorpora guerra de bandas con facciones independientes, emboscada a patrullas con policia
    /// que devuelve fuego, vehiculos de escape dinamicos, y peligros ambientales.
    /// </summary>
    public abstract class BaseAssaultCallout : BaseCallout
    {
        protected Vehicle GetawayVehicle { get; set; }
        protected bool GetawayTriggered { get; set; }
        protected bool IsGangWarScenario { get; set; }

        protected RelationshipGroup HostileGroup { get; set; }
        protected RelationshipGroup FriendlyGroup { get; set; }

        protected BaseAssaultCallout()
        {
            try
            {
                HostileGroup = new RelationshipGroup("ASSAULT_HOSTILES");
                FriendlyGroup = new RelationshipGroup("ASSAULT_FRIENDLIES");

                RelationshipGroup playerGroup = new RelationshipGroup("PLAYER");
                RelationshipGroup copGroup = new RelationshipGroup("COP");

                SetupMutualHate(HostileGroup, playerGroup);
                SetupMutualHate(HostileGroup, copGroup);
                SetupMutualHate(HostileGroup, FriendlyGroup);

                Game.SetRelationshipBetweenRelationshipGroups(FriendlyGroup, playerGroup, Relationship.Companion);
                Game.SetRelationshipBetweenRelationshipGroups(playerGroup, FriendlyGroup, Relationship.Companion);
                Game.SetRelationshipBetweenRelationshipGroups(FriendlyGroup, copGroup, Relationship.Companion);
                Game.SetRelationshipBetweenRelationshipGroups(copGroup, FriendlyGroup, Relationship.Companion);
            }
            catch { }
        }

        protected override void OnPlayerArrival()
        {
            Game.DisplaySubtitle("~r~[DISPAROS ACTIVOS]~w~ " + DisplayName + ". Sospechosos abriendo fuego.");
            EngageHostiles();
        }

        protected virtual void EngageHostiles()
        {
            Ped player = Game.LocalPlayer.Character;
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive)
                {
                    s.BlockPermanentEvents = false;
                    s.KeepTasks = true;

                    if (IsGangWarScenario)
                    {
                        NativeFunction.Natives.TASK_COMBAT_HATED_TARGETS_IN_AREA(s, 150f, 0);
                    }
                    else
                    {
                        Ped target = GetNearestAlliedOfficer(s.Position) ?? player;
                        if (target != null && target.Exists())
                        {
                            s.Tasks.FightAgainst(target);
                        }
                    }
                }
            }

            // Oficiales aliados responden al fuego
            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped ally = FriendlyPeds[i];
                if (ally != null && ally.Exists() && ally.IsAlive)
                {
                    ally.BlockPermanentEvents = false;
                    ally.KeepTasks = true;

                    Ped target = GetNearestSuspect(ally.Position);
                    if (target != null && target.Exists())
                    {
                        ally.Tasks.FightAgainst(target);
                    }
                }
            }
        }

        protected override void OnScenarioTick()
        {
            Ped player = Game.LocalPlayer.Character;

            // Retargeting dinamico de sospechosos si no estan en combate activo
            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    if (!s.IsInCombat)
                    {
                        if (IsGangWarScenario)
                        {
                            NativeFunction.Natives.TASK_COMBAT_HATED_TARGETS_IN_AREA(s, 150f, 0);
                        }
                        else
                        {
                            Ped target = GetNearestAlliedOfficer(s.Position) ?? player;
                            if (target != null && target.Exists())
                            {
                                s.Tasks.FightAgainst(target);
                            }
                        }
                    }
                }
            }

            // Refrescar combate de aliados si su objetivo cayo o no estan en combate
            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped ally = FriendlyPeds[i];
                if (ally != null && ally.Exists() && ally.IsAlive)
                {
                    if (!ally.IsInCombat)
                    {
                        Ped target = GetNearestSuspect(ally.Position);
                        if (target != null && target.Exists())
                        {
                            ally.Tasks.FightAgainst(target);
                        }
                    }
                }
            }
        }

        protected override void CheckCompletionCriteria()
        {
            bool anyHostileActive = false;
            Ped player = Game.LocalPlayer.Character;
            Vector3 playerPos = (player != null && player.Exists()) ? player.Position : SpawnPoint;

            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    // Si el sospechoso huyo muy lejos de la escena y del jugador, se descarta para evitar soft-lock
                    if (s.Position.DistanceTo(SpawnPoint) > 300f && s.Position.DistanceTo(playerPos) > 200f)
                    {
                        if (SuspectBlips.ContainsKey(s) && SuspectBlips[s] != null && SuspectBlips[s].Exists())
                        {
                            SuspectBlips[s].Delete();
                        }
                        s.Dismiss();
                        continue;
                    }
                    anyHostileActive = true;
                    break;
                }
            }

            if (!anyHostileActive)
            {
                NotifyCode4();
            }
        }

        // ---------------- Helpers de Asaltos y Tiroteos ----------------

        protected Ped CreateAssaultSuspect(string model, Vector3 pos, float heading, WeaponHash weapon, int armor = 80)
        {
            Ped p = SpawnPed(model, pos, heading);
            if (p != null)
            {
                p.Inventory.Weapons.Add(weapon).Ammo = -1;
                ConfigurePedCombat(p, ConfigManager.SuspectAccuracy, armor, true);
                p.RelationshipGroup = HostileGroup;
                AttachBlipToSuspect(p, Color.Red, 0.65f);
                Suspects.Add(p);
            }
            return p;
        }

        protected Ped CreateVictim(string model, Vector3 pos, float heading, bool autoCower = true)
        {
            Ped p = SpawnPed(model, pos, heading);
            if (p != null)
            {
                p.BlockPermanentEvents = false;
                if (autoCower)
                {
                    p.Tasks.Cower(-1);
                }
                AttachBlipToVictim(p, Color.Green, 0.55f);
                Victims.Add(p);
            }
            return p;
        }

        protected Ped CreateAlliedOfficer(string model, Vector3 pos, float heading, WeaponHash weapon)
        {
            Ped p = SpawnPed(model, pos, heading);
            if (p != null)
            {
                p.Inventory.Weapons.Add(weapon).Ammo = -1;
                p.Armor = 100;
                p.RelationshipGroup = FriendlyGroup;
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(p, 46, true);
                NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(p, 2); // Advance
                NativeFunction.Natives.SET_PED_ACCURACY(p, 65);
                Blip b = p.AttachBlip();
                b.Color = Color.Blue;
                b.Scale = 0.55f;
                Cleaner.RegisterBlip(b);
                FriendlyPeds.Add(p);
            }
            return p;
        }

        public void SetupMutualHate(RelationshipGroup g1, RelationshipGroup g2)
        {
            try
            {
                Game.SetRelationshipBetweenRelationshipGroups(g1, g2, Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups(g2, g1, Relationship.Hate);
            }
            catch { }
        }

        protected void SetupGangWarFaction(Ped ped, RelationshipGroup myGroup, RelationshipGroup enemyGroup)
        {
            if (ped == null || !ped.Exists()) return;
            ped.RelationshipGroup = myGroup;
            SetupMutualHate(myGroup, enemyGroup);
        }

        protected Ped GetNearestSuspect(Vector3 position)
        {
            Ped nearest = null;
            float minDistance = float.MaxValue;

            for (int i = 0; i < Suspects.Count; i++)
            {
                Ped s = Suspects[i];
                if (s != null && s.Exists() && s.IsAlive && !Functions.IsPedArrested(s))
                {
                    float d = s.Position.DistanceTo(position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = s;
                    }
                }
            }
            return nearest;
        }

        protected Ped GetNearestAlliedOfficer(Vector3 position)
        {
            Ped nearest = null;
            float minDistance = float.MaxValue;

            for (int i = 0; i < FriendlyPeds.Count; i++)
            {
                Ped a = FriendlyPeds[i];
                if (a != null && a.Exists() && a.IsAlive)
                {
                    float d = a.Position.DistanceTo(position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = a;
                    }
                }
            }
            return nearest;
        }
    }
}
