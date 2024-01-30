using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Services;

namespace WOW_Fusion.Controllers
{
    internal class FileController
    {
        public static async Task Write(string key, string path)
        {

            //Encuentra el último índice de '\' y nombre de directorio
            string directoryPath = path.Substring(0, path.LastIndexOf('\\'));
            //Encuentra el último índice de '\' y nombre del archivo
            string fileName = path.Substring(path.LastIndexOf('\\') + 1);

            try
            {
                if (Directory.Exists(directoryPath))
                {
                    if (File.Exists(path))
                    {
                        using (StreamWriter writer = new StreamWriter(path, append : true, Encoding.UTF8))
                        {
                            // Escribir el contenido en una nueva línea en el archivo
                            await writer.WriteLineAsync(key);
                        }
                    }
                    else
                    {
                        //Crear archivo
                        using (FileStream fs = File.Create(path))
                        {
                            fs.Close();
                        }
                        Console.WriteLine($"{DateService.Today()} > Archivo OT escritura creado");
                        await Write(key, path);
                    }
                }
                else
                {
                    Directory.CreateDirectory(directoryPath);
                    await Write(key, path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateService.Today()} > Error de escritura. {ex.Message}");
            }
        }

        public static List<string> ContentFile(string path)
        {
            List<string> orders = new List<string>();

            string directoryPath = path.Substring(0, path.LastIndexOf('\\'));

            try
            {
                if (Directory.Exists(directoryPath))
                {
                    if (File.Exists(path))
                    {
                        // Leer todas las líneas del archivo
                        string[] lineas = File.ReadAllLines(path);

                        // Buscar coincidencia
                        foreach (string linea in lineas)
                        {
                            orders.Add(linea.Trim());
                        }
                        return orders;
                    }
                    else
                    {
                        //Crear archivo
                        using (FileStream fs = File.Create(path))
                        {
                            fs.Close();
                        }
                        Console.WriteLine($"{DateService.Today()} > Archivo OT lectura creado");

                        ContentFile(path);
                        return orders;
                    }
                }
                else
                {
                    Directory.CreateDirectory(directoryPath);
                    ContentFile(path); 
                    return orders;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateService.Today()} > Error de lectura. {ex.Message}");
                return orders;
            }
        }

        public static bool IsOrderPrinted(List<string> ordersPrinted, string order)
        {
            bool status = false;
            if (ordersPrinted.Count > 0)
            {
                foreach (string orderNumber in ordersPrinted)
                {
                    if (orderNumber.Equals(order))
                    {
                        status = true;
                    }
                }
            }
            return status;
        }

        public static bool Search(string key, string path)
        {
            bool result = false;
            //Encuentra el último índice de '\' y nombre de directorio
            string directoryPath = path.Substring(0, path.LastIndexOf('\\'));
            //Encuentra el último índice de '\' y nombre del archivo
            string fileName = path.Substring(path.LastIndexOf('\\') + 1);

            try
            {
                if (Directory.Exists(directoryPath))
                {
                    if (File.Exists(path))
                    {
                        // Leer todas las líneas del archivo
                        string[] lineas = File.ReadAllLines(path);

                        // Eliminar tab y linebreak
                        key = key.Replace("\r", "").Replace("\n", "");

                        // Buscar coincidencia
                        foreach (string linea in lineas)
                        {
                            if (string.Equals(linea.Trim(), key.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                result = true;
                            }
                        }
                    }
                    else
                    {
                        //Crear archivo
                        using (FileStream fs = File.Create(path))
                        {
                            fs.Close();
                        }
                        Console.WriteLine($"{DateService.Today()} > Archivo OT lectura creado");
                        Search(key, path);
                    }
                }
                else
                {
                    Directory.CreateDirectory(directoryPath);
                    Search(key, path);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateService.Today()} > Error de lectura. {ex.Message}");
                return false;
            }
        }
    }
}
