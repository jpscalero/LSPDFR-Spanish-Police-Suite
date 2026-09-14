using System;
using System.Collections.Generic;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;
using RadioYJusticia.Core;

namespace RadioYJusticia.Justice
{
    public class DetaineeRecord
    {
        public Ped DetainedPed { get; set; }
        public string Name { get; set; }
        public string Dni { get; set; }
        public string Crime { get; set; }
        public DateTime ArrestTime { get; set; }
        public int Stage { get; set; }
        public DateTime LastStageTime { get; set; }
        public bool SentToJudge { get; set; }

        public DetaineeRecord(Ped ped, string name, string dni, string crime)
        {
            DetainedPed = ped;
            Name = name;
            Dni = dni;
            Crime = crime;
            ArrestTime = DateTime.Now;
            Stage = 0;
            LastStageTime = DateTime.Now;
            SentToJudge = false;
        }
    }

    public static class CustodyManager
    {
        private static readonly List<DetaineeRecord> activeDetainees = new List<DetaineeRecord>();
        private static readonly object listLock = new object();
        private static readonly Random random = new Random();

        public static void RegisterDetainee(Ped ped, string customCrime)
        {
            if (ped == null || !ped.Exists()) return;

            lock (listLock)
            {
                for (int i = 0; i < activeDetainees.Count; i++)
                {
                    if (activeDetainees[i].DetainedPed == ped)
                    {
                        return; // Ya registrado
                    }
                }

                string nombre = "Individuo sin documentar";
                try
                {
                    Persona persona = Functions.GetPersonaForPed(ped);
                    if (persona != null && !string.IsNullOrEmpty(persona.FullName))
                    {
                        nombre = persona.FullName;
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.CustodyManager.RegisterArrestedPed: " + ex.Message);
                }

                string dni = GenerarDniEspanol(nombre);
                string delito = customCrime;
                if (string.IsNullOrEmpty(delito))
                {
                    PenalCrimeDefinition crimeDef = SentenceManager.GetRandomCrimeDefinition();
                    delito = crimeDef.Delito + " (" + crimeDef.Articulo + ")";
                }

                DetaineeRecord record = new DetaineeRecord(ped, nombre, dni, delito);
                activeDetainees.Add(record);

                Game.LogTrivial("RadioYJusticia: Nuevo detenido en custodia policial: " + nombre + " (" + dni + ") por " + delito);

                string msg = string.Format("~w~Detenido: ~y~{0}~w~~n~DNI: ~y~{1}~w~~n~Delito: {2}~n~~g~Custodia policial activada (Plazo maximo legal 72 horas).",
                    nombre, dni, delito);

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~SALA DE CUSTODIA (COMISARIA)", "~y~Ingreso de Detenido", msg);
            }
        }

        public static void Process()
        {
            Game.LogTrivial("RadioYJusticia: Bucle de CustodyManager iniciado.");

            while (true)
            {
                GameFiber.Sleep(2000);

                try
                {
                    // 1. Escaneo periódico de sospechosos detenidos cerca del jugador
                    ScanNearbyArrestedPeds();

                    // 2. Progresión del reloj de custodia legal (72 horas)
                    UpdateCustodyStages();
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia: Error en ciclo de CustodyManager: " + ex.Message);
                }
            }
        }

        private static void ScanNearbyArrestedPeds()
        {
            Ped player = Game.LocalPlayer.Character;
            if (player == null || !player.Exists()) return;

            Ped[] nearbyPeds = World.GetEntities(player.Position, 25f, GetEntitiesFlags.ConsiderHumanPeds) as Ped[];
            if (nearbyPeds == null) return;

            for (int i = 0; i < nearbyPeds.Length; i++)
            {
                Ped p = nearbyPeds[i];
                if (p == null || !p.Exists() || p == player || !p.IsHuman || !p.IsAlive) continue;

                bool isArrested = false;
                try
                {
                    isArrested = Functions.IsPedArrested(p);
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("RadioYJusticia.CustodyManager.FindNearestArrestedPed (IsPedArrested): " + ex.Message);
                }

                if (!isArrested)
                {
                    try
                    {
                        isArrested = NativeFunction.Natives.IS_PED_CUFFED<bool>(p);
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("RadioYJusticia.CustodyManager.FindNearestArrestedPed (IS_PED_CUFFED): " + ex.Message);
                    }
                }

                if (isArrested)
                {
                    bool yaRegistrado = false;
                    lock (listLock)
                    {
                        for (int j = 0; j < activeDetainees.Count; j++)
                        {
                            if (activeDetainees[j].DetainedPed == p)
                            {
                                yaRegistrado = true;
                                break;
                            }
                        }
                    }

                    if (!yaRegistrado)
                    {
                        RegisterDetainee(p, null);
                    }
                }
            }
        }

        private static void UpdateCustodyStages()
        {
            // Tiempo total de custodia legal (72h) ajustado a minutos reales
            int speedMinutes = ConfigManager.JudicialSpeedMinutes;
            if (speedMinutes < 1) speedMinutes = 2;

            double stageDurationSeconds = (speedMinutes * 60.0) / 3.0;
            List<DetaineeRecord> readyForJudge = new List<DetaineeRecord>();

            lock (listLock)
            {
                DateTime now = DateTime.Now;

                for (int i = 0; i < activeDetainees.Count; i++)
                {
                    DetaineeRecord record = activeDetainees[i];
                    double elapsedInStage = (now - record.LastStageTime).TotalSeconds;

                    if (elapsedInStage >= stageDurationSeconds)
                    {
                        record.Stage++;
                        record.LastStageTime = now;

                        if (record.Stage == 1)
                        {
                            // 24 Horas: Diligencias policiales
                            string msg = string.Format("~w~Detenido: ~y~{0}~w~ (DNI {1})~n~Asistencia letrada ratificada e interrogatorio policial instruido con abogado de oficio.",
                                record.Name, record.Dni);
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~CUSTODIA POLICIAL (24h)", "~w~Diligencias Policiales", msg);
                        }
                        else if (record.Stage == 2)
                        {
                            // 48 Horas: Informe pericial y atestado
                            string msg = string.Format("~w~Detenido: ~y~{0}~w~ (DNI {1})~n~Atestado policial y cadena de custodia remitidos a la Fiscalia de Guardia.",
                                record.Name, record.Dni);
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~CUSTODIA POLICIAL (48h)", "~w~Cierre de Atestado", msg);
                        }
                        else if (record.Stage >= 3)
                        {
                            // 72 Horas: Puesta a disposición judicial del Juez de Instrucción de Guardia
                            string msg = string.Format("~w~Detenido: ~y~{0}~w~ (DNI {1})~n~Plazo maximo constitucional de detencion alcanzado.~n~~y~Pase a disposicion judicial inmediata.",
                                record.Name, record.Dni);
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~LIMITE LEGAL 72 HORAS", "~y~Pase a Disposicion Judicial", msg);

                            record.SentToJudge = true;
                            readyForJudge.Add(record);
                        }
                    }
                }

                // Remover procesados de la lista activa
                for (int k = 0; k < readyForJudge.Count; k++)
                {
                    activeDetainees.Remove(readyForJudge[k]);
                }
            }

            // Llamar al juez para los que alcanzaron las 72h
            for (int k = 0; k < readyForJudge.Count; k++)
            {
                JudgeDecision.DecideCase(readyForJudge[k]);
            }
        }

        public static string GenerarDniEspanol(string nombre)
        {
            int seed = !string.IsNullOrEmpty(nombre) ? nombre.GetHashCode() : Environment.TickCount;
            Random r = new Random(seed + DateTime.Now.Millisecond);
            int numero = r.Next(10000000, 99999999);
            string letras = "TRWAGMYFPDXBNJZSQVHLCKE";
            char letra = letras[numero % 23];
            return numero.ToString() + letra;
        }

        public static List<DetaineeRecord> GetActiveDetainees()
        {
            lock (listLock)
            {
                return new List<DetaineeRecord>(activeDetainees);
            }
        }
    }
}
