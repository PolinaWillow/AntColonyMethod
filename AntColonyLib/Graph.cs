using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyLib
{
    public class Graph
    {
        /// <summary>
        /// Список параметров
        /// </summary>
        public List<GraphParams> Params { get; set; }

        /// <summary>
        /// Список id первых значений параметра
        /// </summary>
        public List<int> IDFirstValueParam = new List<int>();

        /// <summary>
        /// Количество параметров
        /// </summary>
        public int paramCount { get; set; }

        /// <summary>
        /// Список для хранения количества значений параметров
        /// </summary>
        public List<int> valueCount { get; set; }

        /// <summary>
        /// Инициализация графа
        /// </summary>
        /// <returns></returns>
        public int InitialGraph()
        {
            foreach (GraphParams element in Params)
            {
                element.InitialState();
            }
            return 0;
        }

        /// <summary>
        /// Печать графа
        /// </summary>
        /// <returns></returns>
        public int PrintGraph()
        {
            foreach (GraphParams element in Params)
            {
                Console.WriteLine(element);
            }
            Console.WriteLine();
            return 0;
        }               

        /// <summary>
        /// Поиск пути в графе
        /// </summary>
        /// <param name="dataTask"></param>
        /// <returns></returns>
        public int[] FindWay(DataTask dataTask)
        {

            int[] way = new int[paramCount]; //Выбранный путь           

            int i = 0;
            int k = 0;
            while (i < Params.Count)
            {
                //Выбор следующей вершины
                ChoiceNextVertex(i, valueCount[k], way);
                i += valueCount[k];
                k++;
            }
            return way;
        }

        /// <summary>
        /// Выбор следцющей вершины
        /// </summary>
        /// <param name="i">Точка начала просмотра графа </param>
        /// <param name="valueCount">Список для хранения количества значений параметров</param>
        /// <param name="way">Путь</param>
        /// <returns></returns>
        private int ChoiceNextVertex(int i, int valueCount, int[] way) //Выбор следующей вершины
        {

            double sumPheromones = 0;
            double[] Pij = new double[valueCount];//Массив вероятности попадания
            int[] Idij = new int[valueCount];//Массив Id рассматриваемых узлов

            for (int j = 0; j < valueCount; j++)
            {
                sumPheromones += Params[i + j].pheromones;
            }

            //Подсчет вероятности попадания
            for (int j = 0; j < valueCount; j++)
            {
                Pij[j] = Params[i + j].pheromones / sumPheromones;
                Idij[j] = Params[i + j].idParam;
            }

            //Переход к случайному параметру
            double[] intervals = new double[valueCount + 1]; //Определение интервалов попадания
            intervals[0] = 0;
            for (int j = 1; j < intervals.Length; j++)
            {
                intervals[j] = intervals[j - 1] + Pij[j - 1];
            }
            intervals[intervals.Length - 1] = 1;

            Random rnd = new Random();
            double value = rnd.NextDouble();

            int parametrNum = 0;
            int x = 1;
            while (parametrNum == 0)
            {
                if ((value < intervals[x]) && (value > intervals[x - 1]))
                {
                    parametrNum = x;
                }
                x++;
            }

            way[Params[i].numParamFact] = Idij[parametrNum - 1];
            //Поднятие флага выбора параметра           
            Params[i + parametrNum - 1].selectNum++;

            return 0;
        }

        public Graph() {
            Params = new List<GraphParams>();
            valueCount = new List<int>();
            paramCount = 0;
        }
        
    }
}
