using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MathLib.MyMath;

/**
 * Výpočítává vyběrovou směrodatnou odchylku.
 *
 * Jedná se konzolovou aplikaci, která bere vstup z příkazového řádku.
 *
 * @note Program podporuje čísla s desetinou čárkou.
 * @attention Pro ukončení zadávání čísel musí být vložen znak konce souboru na novém řádku.
 */

namespace Profiling
{
    internal class ProfilingProgram
    {
        private static int Main()
        {
            char[] whitespace = new char[] { ' ', '\t', '\n', '\r' };

            List<decimal> numbers = new();

            string line;
            while ((line = Console.ReadLine()) != null) {
                string[] splited = line.Split(whitespace);

                Parallel.ForEach(
                    splited,
                    new ParallelOptions { MaxDegreeOfParallelism = 16},
                    split => {
                    if (decimal.TryParse(split, out decimal num)) {
                        lock (splited) {
                            numbers.Add(num);
                        }
                    }
                });
            }
            //all numbers loaded to memory
            int count = numbers.Count;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("count       :{0}", count);
#endif
            if (count == 0) {
                Console.WriteLine("No numbers has been entered.");
                return 1;
            }

            if (count == 1) {
                Console.WriteLine("Cant calculated standard deviation from one number.");
                return 1;
            }

            decimal _x = numbers.Average();
#if DEBUG
            System.Diagnostics.Debug.WriteLine("avg         :{0}", _x);
#endif
            decimal N_x2 = Mult(count, Pow(_x, 2));

            //calculate pow 2 for all numbers
            Parallel.For(
                0,
                count, i => {
                numbers[i] = Pow(numbers[i], 2);
            });

            decimal sumX2 = numbers.Sum();
            decimal finalSum = Sub(sumX2, N_x2);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("final sum   :{0}", finalSum);
#endif
            decimal s = Sqrt(Div(finalSum, count - 1));

            Console.WriteLine(s);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("S           :{0}", s);
#endif
            return 0;
        }
    }
}
