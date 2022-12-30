using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyLib
{
    public class TargetFunction
    {
        public SenderData sender { get; set; }

        /// <summary>
        /// Значение целефой функции
        /// </summary>
        public double valueTarget { get; set; }

        public TargetFunction()
        {
            sender = new SenderData(); 
            valueTarget = 0;
        }

        /// <summary>
        /// Подсчет значения целефой функции
        /// </summary>
        /// <param name="way">Выбранный путь (id значений параметров)</param>
        /// <param name="graph">Граф</param>
        /// <param name="paramCount">Количество параметров</param>
        /// <returns></returns>
        public double FindValue(List<int> way, Graph graph, int paramCount) //Подсчет целивой функции
        {
            //Отсортировка пути в исходную последовательность
            //Console.WriteLine();
            //foreach (int elem in way) {
            //    Console.Write(elem + "\t");
            //}
            //Console.WriteLine();
            int[] wayCopy = new int[way.Count];

            for (int i = 0; i < way.Count; i++)
            {
                int k = graph.Params[way[i]].numParam;
                wayCopy[k] = way[i];           
            }
            //foreach (int elem in wayCopy)
            //{
            //    Console.Write(elem + "\t");
            //}

            double[] path = new double[paramCount - 1];
            for (int i = 0; i < paramCount - 1; i++)
            {
                path[i] = Convert.ToDouble(graph.Params[wayCopy[i]].valueParam);
            }

            double Value = path[0] - path[1] + 2 * path[2] + path[3] + 2 * path[4] + 0.5 * path[5] - 0.12 * path[6] - path[7] + 80 * path[8] + 0.00001 * path[9];

            if (string.Compare(graph.Params[wayCopy[10]].valueParam, "Сильное") == 0)
            {
                Value += 20;
            }
            return Value;
        }

        /// <summary>
        /// Поиск максимума функции
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="agent">Проверяемый агент</param>
        /// <param name="max">Значене максимума</param>
        /// <param name="maxFunction">Массив для хранения максимума и пути его получения</param>
        /// <param name="wayAgent">Путь агента</param>
        /// <returns></returns>
        public double FindMaxFunction(DataTask dataTask, List<int> way, double max, string[] maxFunction, int[] wayAgent)
        {
            double valueFunction = FindValue(way, dataTask.graphWorkCopy, dataTask.graphWorkCopy.paramCount); //Вычисление значений критериев
                                                                                                         //Console.WriteLine("Значение критерия: " + valueFunction + "\n");
            if (valueFunction >= max)
            {
                maxFunction[0] = Convert.ToString(valueFunction);
                for (int k = 1; k < dataTask.graphWorkCopy.paramCount + 1; k++)
                {
                    maxFunction[k] = dataTask.graphWorkCopy.Params[wayAgent[k - 1]].valueParam;
                }
                max = valueFunction;
            }

            return max;
        }

        /// <summary>
        /// Поиск минимума функции
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="agent">Проверяемый агент</param>
        /// <param name="min">Значене минимума</param>
        /// <param name="minFunction">Массив для хранения минимума и пути его получения</param>
        /// <param name="wayAgent">Путь агента</param>
        /// <returns></returns>
        public double FindMinFunction(DataTask dataTask, List<int> way, double min, string[] minFunction, int[] wayAgent)
        {
            double valueFunction = FindValue(way, dataTask.graphWorkCopy, dataTask.graphWorkCopy.paramCount); //Вычисление значений критериев

            if (valueFunction <= min)
            {
                minFunction[0] = Convert.ToString(valueFunction);
                for (int k = 1; k < dataTask.graphWorkCopy.paramCount + 1; k++)
                {
                    minFunction[k] = dataTask.graphWorkCopy.Params[wayAgent[k - 1]].valueParam;
                }
                min = valueFunction;
            }

            return min;
        }

        public double FindValue_Send(List<int> way, Graph graph, int paramCount) {
            double resultValue = 0;
            //Получение массива значений параметров и их типов
            List<string[]> path = new List<string[]>();

            return resultValue;
        }

        private List<string[]> getPath(List<int> way, Graph graph)
        {
            List<string[]> path = new List<string[]>();
            foreach (int elem in way)
            {
                path.Add(graph.GetValueById(elem));
            }

            return path;
        }

    }
}
