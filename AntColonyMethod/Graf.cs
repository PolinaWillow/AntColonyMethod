using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class Graf
    {
        
        public static int Q = 100;  //Общее число феромонов
        public static double L = 0.9; //Коэфициент пересчета испарения феромонов

        /// <summary>
        /// Список параметров
        /// </summary>
        public List<GrafParams> Params = new List<GrafParams>();

        /// <summary>
        /// Инициализация графа
        /// </summary>
        /// <returns></returns>
        public int InitialGraf()
        {
            foreach (GrafParams element in Params)
            {
                element.InitialState();
            }
            return 0;
        }

        /// <summary>
        /// Печать графа
        /// </summary>
        /// <returns></returns>
        public int PrintGraf()
        {
            foreach (GrafParams element in Params)
            {
                Console.WriteLine(element);
            }
            Console.WriteLine();
            return 0;
        }

        /// <summary>
        /// Заполнение графа
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <returns></returns>
        public int CreateGraf(DataTask dataTask) //Создание графа 
        {
            //Заполнение списка элементов графа 
            int id = 0;
            for (int i = 0; i < dataTask.paramCount; i++)
            {
                for (int j = 0; j < dataTask.valueCount[i]; j++)
                {
                    Params.Add(new GrafParams() { idParam = id, numParam = i, pheromones = 1, selectNum = 0 });

                    //Опредление типа значения параметра
                    if (double.TryParse(dataTask.valueData[id], out double res))
                    {
                        Params[id].typeParam = TypeNumerator.Double;
                    }
                    else { Params[id].typeParam = TypeNumerator.String; }
                    Params[id].valueParam = dataTask.valueData[id];

                    id++;
                }
            }

            PrintGraf();

            return 0;
        }

        /// <summary>
        /// Поиск пути в графе
        /// </summary>
        /// <param name="dataTask"></param>
        /// <returns></returns>
        public int[] FindWay(DataTask dataTask)
        {

            int[] way = new int[dataTask.paramCount]; //Выбранный путь           

            int i = 0;
            int k = 0;
            while (i < Params.Count)
            {
                //Выбор следующей вершины
                ChoiceNextVertex(i, dataTask.valueCount[k], way);
                i += dataTask.valueCount[k];
                k++;
            }

            //for (int j = 0; j < n; j++)
            //{
            //    Console.Write(" " + way[j]);
            //}
            //Console.WriteLine();

            return way;
        }

        /// <summary>
        /// Выбор следцющей вершины
        /// </summary>
        /// <param name="i">Точка начала просмотра графа </param>
        /// <param name="valueCount">Список для хранения количества значений параметров</param>
        /// <param name="way">Путь</param>
        /// <returns></returns>
        public int ChoiceNextVertex(int i, int valueCount, int[] way) //Выбор следующей вершины
        {

            double sumPheromones = 0;
            double[] Pij = new double[valueCount];//Массив вероятности попадания
            int[] Idij = new int[valueCount];//Массив Id рассматриваемых узлов

            for (int j = 0; j < valueCount; j++)
            {
                sumPheromones += Params[i + j].pheromones;
            }
            //Console.WriteLine("Raram: " + graf[i].numParam + " SumPheromones: "+ sumPheromones);

            //Подсчет вероятности попадания
            for (int j = 0; j < valueCount; j++)
            {
                Pij[j] = Params[i + j].pheromones / sumPheromones;
                Idij[j] = Params[i + j].idParam;
            }

            //Console.Write("Вероятности попадания: ");
            //foreach (double elem in Pij) 
            //{
            //    Console.Write(elem + "  ");

            //}
            //Console.WriteLine();

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

            way[Params[i].numParam] = Idij[parametrNum - 1];
            //Поднятие флага выбора параметра           
            Params[i + parametrNum - 1].selectNum++;

            return 0;
        }

        /// <summary>
        /// Добавление феромонов
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="way">Путь агента</param>
        /// <param name="function">Значение искомой функции</param>
        /// <returns></returns>
        public double AddPheromone(DataTask dataTask, List<int> way, double functionValue) //Добавление феромонов
        {
            double eps =0.0000000000000001;
            double delta = Q / (functionValue + eps);

            for (int i = 0; i < dataTask.paramCount; i++)
            {
                Params[way[i]].pheromones += delta;
            }

            return delta;
        }

        /// <summary>
        /// Изменение феромонов
        /// </summary>
        /// <param name="agent">Список агентов</param>
        /// <returns></returns>
        public int PheromoneEvaporation(List<Agent> agents) //Испарение феромона
        {
            //double L = 0.2;
            foreach (GrafParams grafElem in Params)
            {
                double Evaporation = L * Convert.ToDouble(grafElem.pheromones);

                foreach (Agent agentElem in agents)
                {
                    foreach (int wayElem in agentElem.wayAgent)
                    {
                        if (wayElem == grafElem.idParam)
                        {
                            Evaporation += (1 - L) * agentElem.delta;
                            //Console.WriteLine(Evaporation);
                        }
                    }
                }
                grafElem.pheromones = Evaporation;
            }

            //PrintGraf();

            return 0;
        }
    }
}
