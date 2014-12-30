using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticHybrid
{
    public class Kod
    {
        public static double[] code(double[] array)
        {
            double[] newArray = new double[array.Length]; // budet nash zakodirovanniy massiv
            double[] tempSortedArray = (double[])array.Clone();  // dlia sozdania uporiadochennoi kollektsii
            Array.Sort(tempSortedArray);
            Dictionary<double, double> map = new Dictionary<double, double>(); // uporiadochennaya kollektsia elementov nashego nezakodirovonnogo massiva
            foreach (var v in tempSortedArray)
            {
                map.Add(v, v); // zapolnenie
            }

            //proidemsia po kazhdomu elementu nashego isxodnogo massiva
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = map[Convert.ToInt32(array[i])]; // zakodirovanniy i-ый element - eto element uporiadochennoi kollektsii, sootvetstvuyushiy kluchu = i-тому elementu isxodnogo masiva

                double[] keysArray = map.Keys.ToArray(); // kollektsia kluchei, chtoby udobnee bylo proitis po kollektsii map i modifitsirovat ee

                for (int j = keysArray.Length - 1; keysArray[j] > array[i]; j--) // proxodim po kazhdomu kluchu kollektsii map v obratnom poriadke dlia prostoty
                {
                    map[keysArray[j]] = map[keysArray[j - 1]]; // sdvig kollektsii napravo
                }

                map.Remove(array[i]); // udaliaem ispolzovanniy kluch
            }

            return newArray;
        }

        public static double[] uncode(double[] array)
        {
            double[] newArray = new double[array.Length];
            Dictionary<double, double> map = new Dictionary<double, double>(); // uporiadochennaya kollektsia elementov nashego nezakodirovonnogo massiva

            for (int i = 1; i <= array.Length; i++)
            {
                map.Add(i, i);
            }

            //proidemsia po kazhdomu elementu nashego isxodnogo massiva
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = map.FirstOrDefault(x => x.Value.Equals(array[i])).Key; // map[Convert.ToInt32(array[i])];

                double[] keysArray = map.Keys.ToArray(); // kollektsia kluchei, chtoby udobnee bylo proitis po kollektsii map i modifitsirovat ee

                for (int j = keysArray.Length - 1; keysArray[j] > map.FirstOrDefault(x => x.Value.Equals(array[i])).Key; j--) // proxodim po kazhdomu kluchu kollektsii map v obratnom poriadke dlia prostoty
                {
                    map[keysArray[j]] = map[keysArray[j - 1]]; // sdvig kollektsii napravo
                }

                map.Remove(map.FirstOrDefault(x => x.Value.Equals(array[i])).Key); // udaliaem ispolzovanniy kluch
            }

            return newArray;
        }
    }
}
