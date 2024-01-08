using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;

namespace WOW_Fusion.Services
{
    internal class DateService
    {      
        public static string CurrentShift(dynamic shifts, string resourceId)
        {
            string shiftWC = string.Empty;
            int shiftIndex = -1;
            for (int i = 0; i < (int)shifts["WorkCenterResource"]["count"]; i++)
            {
                //Buscar recurso (Máquina) de OT en las recursos del centro de trabajo.
                if (resourceId.Equals(shifts["WorkCenterResource"]["items"][i]["ResourceId"].ToString()))
                {
                    int countShifts = (int)shifts["WorkCenterResource"]["items"][i]["WorkCenterResourceShift"]["count"];
                    if (countShifts > 0)//Validar si tiene turnos
                    {
                        shiftIndex = i;
                        break;
                    }
                }
            }

            if (shiftIndex != -1)
            {
                dynamic currentShift = shifts["WorkCenterResource"]["items"][shiftIndex]["WorkCenterResourceShift"]["items"];

                for (int i = 0; i < (int)shifts["WorkCenterResource"]["items"][shiftIndex]["WorkCenterResourceShift"]["count"]; i++)
                {
                    try
                    {
                        DateTime startShift = DateTime.Parse(currentShift[i]["StartTime"].ToString());
                        float durationShift = string.IsNullOrEmpty(currentShift[i]["Duration"].ToString()) ? 0 : float.Parse(currentShift[i]["Duration"].ToString());
                        TimeSpan durationParse = TimeSpan.FromHours((double)(new decimal(durationShift)));
                        DateTime sDurationShift = DateTime.Parse(durationParse.ToString());
                        DateTime endShift = startShift.Add(sDurationShift.TimeOfDay);
                        DateTime currentHour = DateTime.Parse(DateTime.Now.ToString("HH:mm"));

                        if (startShift.TimeOfDay <= endShift.TimeOfDay)
                        {
                            //Inicio y fin de turno estan en el mismo día
                            if (currentHour.TimeOfDay >= startShift.TimeOfDay && currentHour.TimeOfDay <= endShift.TimeOfDay)
                            {
                                shiftWC = currentShift[i]["ShiftName"].ToString();
                            }
                            //SINO {currentHour.TimeOfDay} NO ESTA ENTRE {startShift.TimeOfDay}-{endShift.TimeOfDay}
                        }
                        else
                        {
                            //Inicio y fin de turno estan en diferentes días
                            if (currentHour.TimeOfDay >= startShift.TimeOfDay || currentHour.TimeOfDay <= endShift.TimeOfDay)
                            {
                                shiftWC = currentShift[i]["ShiftName"].ToString();
                            }
                            //SINO {currentHour.TimeOfDay} NO ES POSTERIOR {startShift.TimeOfDay} NI ANTERIOR A {endShift.TimeOfDay}
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{Today()} -> Error lectura de turno: {ex.Message}");
                    }
                }
            }
            else
            {
                NotifierController.Warning("No se encontraron datos del turno");
            }
            return shiftWC;
        }
        public static string ValidateShifth()
        {
            DateTime startShift = DateTime.Parse("18:00");
            float durationShift = float.Parse("12.0");
            TimeSpan durationParse = TimeSpan.FromHours((double)(new decimal(durationShift)));
            DateTime sDurationShift = DateTime.Parse(durationParse.ToString());
            DateTime endShift = startShift.Add(sDurationShift.TimeOfDay);
            DateTime currentHour = DateTime.Parse("07:00");

            if (startShift.TimeOfDay <= endShift.TimeOfDay)
            {
                if (currentHour.TimeOfDay >= startShift.TimeOfDay && currentHour.TimeOfDay <= endShift.TimeOfDay)
                {
                    MessageBox.Show($"{currentHour.TimeOfDay} POSTERIOR {startShift.TimeOfDay} && {currentHour.TimeOfDay} ANTERIOR {endShift.TimeOfDay}");
                }
                else
                {
                    MessageBox.Show($"{currentHour.TimeOfDay} NO ESTA ENTRE {startShift.TimeOfDay}-{endShift.TimeOfDay}");
                }
            }
            else
            {
                if (currentHour.TimeOfDay >= startShift.TimeOfDay)
                {
                    MessageBox.Show($"{currentHour.TimeOfDay} POSTERIOR {startShift.TimeOfDay}");
                }
                else if (currentHour.TimeOfDay <= endShift.TimeOfDay)
                {
                    MessageBox.Show($"{currentHour.TimeOfDay} ANTERIOR {endShift.TimeOfDay}");
                }
                else
                {
                    MessageBox.Show($"{currentHour.TimeOfDay} NO ES POSTERIOR {startShift.TimeOfDay} NI ANTERIOR A {endShift.TimeOfDay}");
                }
            }
            return string.Empty;
        }
        public static string Now()
        {
            //DateTime today = DateTime.UtcNow.Date;
            //DateTime today = DateTime.Today;
            DateTimeOffset today = DateTimeOffset.Now;
            return today.ToString("dd-MM-yyyy");
        }

        public static string Today()
        {
            DateTimeOffset today = DateTimeOffset.Now;
            return today.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public static string LocalDate(string dateISO8601)
        {
            DateTimeOffset dateTimeOffset, localTimeOffset;
            dateTimeOffset = DateTimeOffset.Parse(dateISO8601);
            localTimeOffset = dateTimeOffset.ToLocalTime();
            return localTimeOffset.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
