﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;

namespace WOW_Fusion.Services
{
    internal class DateService
    {      
        public static string CurrentShift(dynamic shifts, string resourceId)
        {
            string shiftWC = string.Empty;
            int shiftIndex = -1;
            for (int i = 0; i < (int)shifts.WorkCenterResource.count; i++)
            {
                //Buscar recurso (Máquina) de OT en las recursos del centro de trabajo.
                if (resourceId.Equals(shifts.WorkCenterResource.items[i].ResourceId.ToString()))
                {
                    int countShifts = (int)shifts.WorkCenterResource.items[i].WorkCenterResourceShift.count;
                    if (countShifts > 0)//Validar si tiene turnos
                    {
                        shiftIndex = i;
                        break;
                    }
                }
            }

            if (shiftIndex != -1)
            {
                dynamic currentShift = shifts.WorkCenterResource.items[shiftIndex].WorkCenterResourceShift.items;

                for (int i = 0; i < (int)shifts.WorkCenterResource.items[shiftIndex].WorkCenterResourceShift.count; i++)
                {
                    try
                    {
                        DateTime startShift = DateTime.Parse(currentShift[i].StartTime.ToString());
                        float durationShift = string.IsNullOrEmpty(currentShift[i].Duration.ToString()) ? 0 : float.Parse(currentShift[i].Duration.ToString());
                        TimeSpan durationParse = TimeSpan.FromHours((double)(new decimal(durationShift)));
                        DateTime sDurationShift = DateTime.Parse(durationParse.ToString());
                        DateTime endShift = startShift.Add(sDurationShift.TimeOfDay);
                        DateTime currentHour = DateTime.Parse(DateTime.Now.ToString("HH:mm"));

                        if (startShift.TimeOfDay <= endShift.TimeOfDay)
                        {
                            //Inicio y fin de turno estan en el mismo día
                            if (currentHour.TimeOfDay >= startShift.TimeOfDay && currentHour.TimeOfDay <= endShift.TimeOfDay)
                            {
                                shiftWC = currentShift[i].ShiftName.ToString();
                            }
                            //SINO {currentHour.TimeOfDay} NO ESTA ENTRE {startShift.TimeOfDay}-{endShift.TimeOfDay}
                        }
                        else
                        {
                            //Inicio y fin de turno estan en diferentes días
                            if (currentHour.TimeOfDay >= startShift.TimeOfDay || currentHour.TimeOfDay <= endShift.TimeOfDay)
                            {
                                shiftWC = currentShift[i].ShiftName.ToString();
                            }
                            //SINO {currentHour.TimeOfDay} NO ES POSTERIOR {startShift.TimeOfDay} NI ANTERIOR A {endShift.TimeOfDay}
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error lectura de turno: {ex.Message} [{Today()}]", Color.Red);
                    }
                }
            }
            else
            {
                Console.WriteLine($"No se encontraron datos del turno [{Today()}]", Color.Red);
            }
            return shiftWC;
        }

        public static bool IsBetweenDates(DateTime start, DateTime end)
        {
            bool isBetweenDates = false;
            DateTimeOffset now = DateTimeOffset.Now;
            if (start <= end)
            {
                //Inicio y fin estan en el mismo día
                isBetweenDates = (now >= start && now <= end) ? true : false;
            }
            else
            {
                //Inicio y fin estan en diferentes días
                isBetweenDates = (now >= start || now <= end) ? true : false;
            }
            return isBetweenDates;
        }

        public static string Now()
        {
            //DateTime today = DateTime.UtcNow.Date;
            //DateTime today = DateTime.Today;
            DateTimeOffset today = DateTimeOffset.Now;
            return today.ToString("dd/MM/yyyy");
        }

        public static string Today()
        {
            DateTimeOffset today = DateTimeOffset.Now;
            return today.ToString("dd/MM/yy HH:mm:ss");
        }

        public static string LocalDate(string dateISO8601)
        {
            DateTimeOffset dateTimeOffset, localTimeOffset;
            dateTimeOffset = DateTimeOffset.Parse(dateISO8601);
            localTimeOffset = dateTimeOffset.ToLocalTime();
            return localTimeOffset.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static string EpochTime()
        {
            // DateTime Unix epoch [Segundos]
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime currentTime = DateTime.UtcNow;

            // Calcular deferencia entre tiempo actual y EPOCH Obtener en segundos
            TimeSpan timeSinceEpoch = currentTime - epoch;
            long epochTimeInSeconds = (long)timeSinceEpoch.TotalSeconds;

            // Convertir EPOCH a fecha y hora estándar
            DateTime epochTimeDateTime = epoch.AddSeconds(epochTimeInSeconds);

            //Retornar en segundos [10 caracteres] - [13 en ms]
            return epochTimeInSeconds.ToString();
        }
    }
}
