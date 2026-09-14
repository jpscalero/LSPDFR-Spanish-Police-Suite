using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;

namespace SistemaPolicialIntegral.Services
{
    public static class ForensicJudicialVan
    {
        private static HashSet<uint> processedBodies = new HashSet<uint>();

        public static void RequestForensicUnit(Vector3 location)
        {
            GameFiber.StartNew(delegate
            {
                Game.DisplaySubtitle("~b~[SIRDEE]~w~ Solicitando unidad de Policía Científica al lugar.", 4000);
                GameFiber.Sleep(2000);

                Vector3 spawnPos = World.GetNextPositionOnStreet(location.Around(100f));
                Vehicle van = new Vehicle("policet", spawnPos);
                if (!van.Exists()) return;
                van.IsPersistent = true;
                van.IsSirenOn = true;

                Ped forensic1 = new Ped("s_m_m_scientist_01", spawnPos, 0f);
                Ped forensic2 = new Ped("s_m_m_scientist_01", spawnPos, 0f);

                if (forensic1.Exists()) { forensic1.IsPersistent = true; forensic1.WarpIntoVehicle(van, -1); }
                if (forensic2.Exists()) { forensic2.IsPersistent = true; forensic2.WarpIntoVehicle(van, 0); }

                if (forensic1.Exists())
                {
                    forensic1.Tasks.DriveToPosition(van, location, 20f, VehicleDrivingFlags.Emergency, 8f).WaitForCompletion(40000);
                }

                while (van.DistanceTo(location) > 15f)
                {
                    GameFiber.Sleep(1000);
                    if (!van.Exists()) return;
                }
                
                van.IsSirenOn = false;

                if (forensic1.Exists()) forensic1.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion();
                if (forensic2.Exists()) forensic2.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion();

                Ped[] allPeds = World.GetAllPeds();
                List<Ped> deadPeds = new List<Ped>();
                foreach (Ped p in allPeds)
                {
                    if (p.Exists() && p.DistanceTo(location) < 20f && p.IsDead)
                    {
                        if (!processedBodies.Contains(p.Handle))
                        {
                            deadPeds.Add(p);
                        }
                    }
                }

                foreach (Ped body in deadPeds)
                {
                    if (!body.Exists()) continue;
                    
                    Ped activeForensic = forensic1.Exists() ? forensic1 : (forensic2.Exists() ? forensic2 : null);
                    if (activeForensic != null)
                    {
                        activeForensic.Tasks.GoToOffsetFromEntity(body, 1.0f, 0f, 1.0f).WaitForCompletion();
                        activeForensic.Tasks.PlayAnimation("amb@medic@standing@kneel@base", "base", 2f, AnimationFlags.Loop).WaitForCompletion(5000);
                        
                        Game.DisplaySubtitle("~b~[POLICÍA CIENTÍFICA]~w~ Levantamiento del cadáver completado. Acta judicial redactada.", 4000);
                        processedBodies.Add(body.Handle);
                        
                        if (body.Exists()) body.Delete();
                    }
                }
                
                GameFiber.Sleep(2000);

                if (forensic1.Exists()) forensic1.Tasks.EnterVehicle(van, -1);
                if (forensic2.Exists()) forensic2.Tasks.EnterVehicle(van, 0);

                GameFiber.Sleep(5000);
                if (forensic1.Exists()) forensic1.Tasks.CruiseWithVehicle(van, 15f, VehicleDrivingFlags.Normal);

                GameFiber.Sleep(60000);
                if (forensic1.Exists()) forensic1.Delete();
                if (forensic2.Exists()) forensic2.Delete();
                if (van.Exists()) van.Delete();
            });
        }
    }
}
