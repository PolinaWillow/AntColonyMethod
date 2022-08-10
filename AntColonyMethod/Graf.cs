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

        public List<GrafParams> Params = new List<GrafParams>();

        public int InitialGraf()
        {
            foreach (GrafParams element in Params)
            {
                element.InitialState();
            }
            return 0;
        }

        public int PrintGraf()
        {
            foreach (GrafParams element in Params)
            {
                Console.WriteLine(element);
            }
            Console.WriteLine();
            return 0;
        }

        public int CreateGraf(int n, List<int> m, List<string> valueData) //Создание графа 
        {
            //Заполнение списка элементов графа 
            int id = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m[i]; j++)
                {
                    Params.Add(new GrafParams() { idParam = id, numParam = i, pheromones = 1, selectNum = 0 });

                    //Опредление типа значения параметра
                    if (double.TryParse(valueData[id], out double res))
                    {
                        Params[id].typeParam = TypeNumerator.Double;
                    }
                    else { Params[id].typeParam = TypeNumerator.String; }
                    Params[id].valueParam = valueData[id];

                    id++;
                }
            }

            PrintGraf();

            return 0;
        }

        public int[] FindWay(int n, List<int> m)
        {

            int[] way = new int[n]; //Выбранный путь           

            int i = 0;
            int k = 0;
            while (i < Params.Count)
            {
                //Выбор следующей вершины
                ChoiceNextVertex(i, m[k], way);
                i += m[k];
                k++;
            }

            //for (int j = 0; j < n; j++)
            //{
            //    Console.Write(" " + way[j]);
            //}
            //Console.WriteLine();

            return way;
        }

        public int ChoiceNextVertex(int i, //Точка начала просмотра графа 
                                           int m, int[] way) //Выбор следующей вершины
        {

            double sumPheromones = 0;
            double[] Pij = new double[m];//Массив вероятности попадания
            int[] Idij = new int[m];//Массив Id рассматриваемых узлов

            for (int j = 0; j < m; j++)
            {
                sumPheromones += Params[i + j].pheromones;
            }
            //Console.WriteLine("Raram: " + graf[i].numParam + " SumPheromones: "+ sumPheromones);

            //Подсчет вероятности попадания
            for (int j = 0; j < m; j++)
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
            double[] intervals = new double[m + 1]; //Определение интервалов попадания
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

        public double AddPheromone(int n, List<int> m, List<int> way, double function) //Добавление феромонов
        {
            double eps =0.0000000000000001;
            double delta = Q / (function + eps);

            for (int i = 0; i < n; i++)
            {
                Params[way[i]].pheromones += delta;
            }

            return delta;
        }

        public int PheromoneEvaporation(List<Agent> agent) //Испарение феромона
        {
            //double L = 0.2;
            foreach (GrafParams grafElem in Params)
            {
                double Evaporation = L * Convert.ToDouble(grafElem.pheromones);

                foreach (Agent agentElem in agent)
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
