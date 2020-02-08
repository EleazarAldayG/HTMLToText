using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;


namespace Fase2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Dirección donde se encuentran todos los archivos HTML
            Console.WriteLine("Introduce la ruta donde se encuentran los archivos HTML:");
            var path = Console.ReadLine();


            Stopwatch tiempoTotalDePrograma = Stopwatch.StartNew();

            //Tiempo total en abrir archivos
            double tiempoTotalAperturaArchivos = 0;
            char[] producto;
            bool comillaDobleAbierta = false;
            Nullable<int> aperturaTag = 0;
            int cierreTag = 0;
            // Loop que busca todos los archivos '.html' en el path
            // Método en el que se crea el archivo donde se escribirán los resultados de consola
            FileStream nuevoArchivo;
            FileStream archivoLog = new FileStream("a2_matricula.txt", FileMode.Create);
            var logWritter = new StreamWriter(archivoLog);

            logWritter.AutoFlush = true;
            Console.SetOut(logWritter);
            Console.SetError(logWritter);

            foreach (string file in Directory.GetFiles(path, "*.html"))
            {
                comillaDobleAbierta = false;
                aperturaTag = null;
                cierreTag = 0;
                Stopwatch sw = Stopwatch.StartNew();
                // Lee cada archivo HTML del path
                string contents = File.ReadAllText(file);
                // Obtiene el nombre de cada archivo HTML
                string name = Path.GetFileName(file);
                nuevoArchivo = new FileStream("html_removed("+name+").txt", FileMode.Create);
                var nuevoArchivoWritter = new StreamWriter(nuevoArchivo);
                nuevoArchivoWritter.AutoFlush = true;
                Console.SetOut(nuevoArchivoWritter);
                Console.SetError(nuevoArchivoWritter);
                char[] charsContenido = contents.ToCharArray();
                producto = new char[charsContenido.Length];
                for(int index = 0; index < charsContenido.Length; index++) //Lectura de cada char
                {
                    if (charsContenido[index].Equals('"')) //Si el char es una comilla doble
                    {
                        if (index > 0) //Valida que el index sea mayo a 0...
                        {
                            if(!charsContenido[index - 1].Equals('\\'))//...para que se pueda ver el char anterior
                            {                                           // y validar que dicha comilla no se 'escape'
                                if (comillaDobleAbierta) //Si la comilla ya estaba abierta...
                                {
                                    comillaDobleAbierta = false; //Se señala que se esta cerrando
                                }
                                else // Y si no estaba abierta...
                                {
                                    comillaDobleAbierta = true;//Se señala que se esta abriendo
                                }
                            }//Cierre validación escape
                        }//Cierre validación index
                    }//Cierre validación comilla

                    if (!comillaDobleAbierta)//Si no hay comillas abiertas, o en otras palabras, el texto no se interpretará como texto
                    {
                        if (charsContenido[index].Equals('<'))//Se valida la apertura de tag
                        {
                            aperturaTag = index;
                        }
                        else
                        {
                            if(aperturaTag != null) // Se valida si ya había un tag abierto
                            {
                                if (charsContenido[index].Equals('>')) //Se valida si esta cerrando tag
                                {
                                    cierreTag = index; // De ser así, entonces se sabe que se tiene ya un tag HTML
                                    for(int index2 = aperturaTag.Value;index2 <= cierreTag; index2++) // Se procede a "eliminar" todos los chars 
                                    {                                                           // que se encuentren desde la apertura del tag hasta su cierre
                                        charsContenido[index2] = '\0';
                                    }
                                    aperturaTag = null; 
                                    cierreTag = 0; //Y finalmente se settea a 0 la ubicación de los indices
                                }
                            }
                        }
                    }
                } // Fin del for de lectura de chars

                for(int index = 0; index < charsContenido.Length; index++) //Se cicla sobre los chars seleccionados
                {
                    if(charsContenido[index] != '\0')// Si el char es diferente de los "null"...
                    {
                        producto[index] = charsContenido[index]; //.. se agregarán al producto final
                    }
                }

                contents = new string(producto); //Finalmente, se convierte el char array final a String

                // Texto que aparecerá en el nuevo archivo
                Console.WriteLine(contents);
                sw.Stop();
                //Después de crear archivo y llenar contenido, se cambia al log
                Console.SetOut(logWritter);
                Console.SetError(logWritter);
                
                Console.WriteLine(name + " tardó en abrirse: {0}ms", sw.Elapsed.TotalMilliseconds);
            }

            //Detiene el cronómetro del programa
            tiempoTotalDePrograma.Stop();
            Console.WriteLine("Tiempo total de ejecución: {0}ms", tiempoTotalDePrograma.Elapsed.TotalMilliseconds);
        }
    }
}



