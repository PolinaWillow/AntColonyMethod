using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class Function
    {
        public static double TargetFunction(List<int> way, List<GrafParams> graf, int n) //Подсчет целивой функции
        {
            //way.Clear();
            //way.AddRange(new[] { 1, 9, 13, 23, 42, 45, 63, 73, 75, 86, 98 });

            double[] path = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                path[i] = Convert.ToDouble(graf[way[i]].valueParam);
            }

            double Value = path[0] - path[1] + 2 * path[2] + path[3] + 2 * path[4] + 0.5 * path[5] - 0.12 * path[6] - path[7] + 80 * path[8] + 0.00001 * path[9];

            if (string.Compare(graf[way[10]].valueParam, "Сильное") == 0)
            {
                Value += 20;
            }
            return Value;
        }
    }
}
