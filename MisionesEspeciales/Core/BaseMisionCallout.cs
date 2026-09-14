using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PoliceSuite.Common;

namespace MisionesEspeciales.Core
{
    public abstract class BaseMisionCallout : Callout
    {
        protected Vector3 SpawnPoint;
        protected Blip AreaBlip;
        protected List<Ped> Suspects;
        protected List<Ped> Victims;
        protected List<Ped> Allies;
        protected List<Vehicle> Vehicles;
        protected List<Blip> EntityBlips;
        protected Dictionary<Ped, Blip> SuspectBlips;

        protected bool IsScenarioArrived;
        protected bool IsScenarioFinished;
        protected int TickTimer;

        public BaseMisionCallout()
        {
            Suspects = new List<Ped>();
            Victims = new List<Ped>();
            Allies = new List<Ped>();
            Vehicles = new List<Vehicle>();
            EntityBlips = new List<Blip>();
            SuspectBlips = new Dictionary<Ped, Blip>();
            IsScenarioArrived = false;
            IsScenarioFinished = false;
            TickTimer = 0;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 40f);
            CalloutPosition = SpawnPoint;
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            AreaBlip = new Blip(SpawnPoint, 30f);
            AreaBlip.Color = System.Drawing.Color.Yellow;
            AreaBlip.Alpha = 0.5f;
            AreaBlip.IsRouteEnabled = true;

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~SALA 091 / 062", "~y~AVISO DESPACHADO", "~w~Proceda con precaución al punto indicado en el GPS.");
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET", false);
            PoliceSuite.Common.SpanishVoiceDispatchEngine.SpeakDispatch("Sala 091 a todas las unidades. Nuevo requerimiento especial asignado. Procedan al lugar.");

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!IsScenarioArrived)
            {
                if (Game.LocalPlayer.Character.DistanceTo(SpawnPoint) < 65f)
                {
                    IsScenarioArrived = true;
                    if (AreaBlip != null && AreaBlip.Exists())
                    {
                        AreaBlip.Delete();
                        AreaBlip = null;
                    }
                    OnPlayerArrived();
                }
                return;
            }

            // AI Tick throttle (cada ~400ms a 60 FPS = cada 25 frames)
            TickTimer++;
            if (TickTimer >= 25)
            {
                TickTimer = 0;

                // Limpieza dinamica de blips de sospechosos abatidos o arrestados
                CalloutBlips.UpdatePedBlips(SuspectBlips, true);

                OnScenarioTick();
            }
        }

        protected abstract void OnPlayerArrived();
        protected abstract void OnScenarioTick();

        protected Ped SpawnSuspect(Vector3 position, string modelName, string weaponName, int health, int armor)
        {
            Ped ped = new Ped(modelName, position, 0f);
            if (ped != null && ped.Exists())
            {
                ped.IsPersistent = true;
                ped.BlockPermanentEvents = false;
                ped.Health = health;
                ped.MaxHealth = health;
                ped.Armor = armor;

                if (!string.IsNullOrEmpty(weaponName))
                {
                    ped.Inventory.GiveNewWeapon(weaponName, 500, true);
                }

                Blip b = ped.AttachBlip();
                b.Color = System.Drawing.Color.Red;
                b.Scale = 0.75f;
                EntityBlips.Add(b);
                SuspectBlips[ped] = b;
                Suspects.Add(ped);
            }
            return ped;
        }

        protected Ped SpawnVictim(Vector3 position, string modelName)
        {
            Ped ped = new Ped(modelName, position, 0f);
            if (ped != null && ped.Exists())
            {
                ped.IsPersistent = true;
                ped.BlockPermanentEvents = true;

                Blip b = ped.AttachBlip();
                b.Color = System.Drawing.Color.Orange;
                b.Scale = 0.65f;
                EntityBlips.Add(b);
                Victims.Add(ped);
            }
            return ped;
        }

        protected Vehicle SpawnCalloutVehicle(Vector3 position, string modelName, float heading)
        {
            Vehicle veh = new Vehicle(modelName, position, heading);
            if (veh != null && veh.Exists())
            {
                veh.IsPersistent = true;
                Vehicles.Add(veh);
            }
            return veh;
        }

        protected void CompleteMission(string title, string summary)
        {
            if (IsScenarioFinished) return;
            IsScenarioFinished = true;

            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "FLIGHT_SCHOOL_LESSON_PASSED", "HUD_AWARDS", false);
            PoliceSuite.Common.SpanishVoiceDispatchEngine.AnnounceCode4();
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~g~MISIÓN COMPLETADA", string.Format("~y~{0}", title), summary);

            End();
        }

        public override void End()
        {
            base.End();

            if (AreaBlip != null && AreaBlip.Exists())
            {
                AreaBlip.Delete();
                AreaBlip = null;
            }

            foreach (Blip b in EntityBlips)
            {
                if (b != null && b.Exists()) b.Delete();
            }
            EntityBlips.Clear();
            SuspectBlips.Clear();

            foreach (Ped p in Suspects)
            {
                if (p != null && p.Exists()) p.Dismiss();
            }
            Suspects.Clear();

            foreach (Ped v in Victims)
            {
                if (v != null && v.Exists()) v.Dismiss();
            }
            Victims.Clear();

            foreach (Ped a in Allies)
            {
                if (a != null && a.Exists()) a.Dismiss();
            }
            Allies.Clear();

            foreach (Vehicle veh in Vehicles)
            {
                if (veh != null && veh.Exists()) veh.Dismiss();
            }
            Vehicles.Clear();
        }
    }
}
