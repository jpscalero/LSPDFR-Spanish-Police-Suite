using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;

namespace SistemaPolicialIntegral.Services
{
    public static class SamurAmbulanceService
    {
        public static void RequestAmbulance(Vector3 location)
        {
            GameFiber.StartNew(delegate
            {
                Game.DisplaySubtitle("~b~[SIRDEE]~w~ Solicitando unidad de Soporte Vital Avanzado (SAMUR).", 4000);
                GameFiber.Sleep(2000);

                Vector3 spawnPos = World.GetNextPositionOnStreet(location.Around(100f));
                Vehicle ambulance = new Vehicle("ambulance", spawnPos);
                if (!ambulance.Exists()) return;
                ambulance.IsPersistent = true;
                ambulance.IsSirenOn = true;

                Ped paramedic1 = new Ped("s_m_m_paramedic_01", spawnPos, 0f);
                Ped paramedic2 = new Ped("s_m_m_paramedic_01", spawnPos, 0f);

                if (paramedic1.Exists()) { paramedic1.IsPersistent = true; paramedic1.WarpIntoVehicle(ambulance, -1); }
                if (paramedic2.Exists()) { paramedic2.IsPersistent = true; paramedic2.WarpIntoVehicle(ambulance, 0); }

                if (paramedic1.Exists())
                {
                    paramedic1.Tasks.DriveToPosition(ambulance, location, 20f, VehicleDrivingFlags.Emergency, 8f).WaitForCompletion(40000);
                }

                while (ambulance.DistanceTo(location) > 10f)
                {
                    GameFiber.Sleep(1000);
                    if (!ambulance.Exists()) return;
                }
                
                ambulance.IsSirenOn = false;

                if (paramedic1.Exists()) paramedic1.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion();
                if (paramedic2.Exists()) paramedic2.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion();

                Ped[] allPeds = World.GetAllPeds();
                List<Ped> patients = new List<Ped>();
                foreach (Ped p in allPeds)
                {
                    if (p.Exists() && p.DistanceTo(location) < 20f && (p.Health < 50 || p.IsDead))
                    {
                        if (p != Game.LocalPlayer.Character)
                        {
                            patients.Add(p);
                        }
                    }
                }

                foreach (Ped patient in patients)
                {
                    if (!patient.Exists()) continue;
                    
                    Ped activeParamedic = paramedic1.Exists() ? paramedic1 : (paramedic2.Exists() ? paramedic2 : null);
                    if (activeParamedic != null)
                    {
                        activeParamedic.Tasks.GoToOffsetFromEntity(patient, 1.0f, 0f, 1.0f).WaitForCompletion();
                        activeParamedic.Tasks.PlayAnimation("mini@cpr@char_a", "cpr_pumpchest", 2f, AnimationFlags.Loop).WaitForCompletion(6000);
                        
                        if (!patient.IsDead)
                        {
                            patient.Health = patient.MaxHealth;
                            Game.DisplaySubtitle("~g~[SAMUR 061]~w~ Paciente estabilizado. Soporte Vital Avanzado completado.", 4000);
                        }
                        else
                        {
                            Game.DisplaySubtitle("~r~[SAMUR 061]~w~ Paciente fallecido. Se certifica el óbito in situ.", 4000);
                        }
                    }
                }
                
                GameFiber.Sleep(2000);

                if (paramedic1.Exists()) paramedic1.Tasks.EnterVehicle(ambulance, -1);
                if (paramedic2.Exists()) paramedic2.Tasks.EnterVehicle(ambulance, 0);

                GameFiber.Sleep(5000);
                if (paramedic1.Exists()) paramedic1.Tasks.CruiseWithVehicle(ambulance, 15f, VehicleDrivingFlags.Normal);

                GameFiber.Sleep(60000);
                if (paramedic1.Exists()) paramedic1.Delete();
                if (paramedic2.Exists()) paramedic2.Delete();
                if (ambulance.Exists()) ambulance.Delete();
            });
        }
    }
}
