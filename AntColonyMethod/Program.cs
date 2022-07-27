using System;
using System.Collections.Generic;

namespace AntColonyMethod
{
    class GrafParams
    {
        public int IdParam { get; set; }
        public int NumParam { get; set; }
        public int Pheromones { get; set; }

        public int SelectNum { get; set; }

        //public int Delta { get; set; }

        public override string ToString()
        {
            return "IdParam: " + IdParam + "   NumParam: " + NumParam + "   Pheromones: " + Pheromones;
        }
    }

    class AgentGroup
    {
        public int IdAgent { get; set; }
        public double Delta { get; set; }

        public List<int> WayAgent = new List<int>();

        public override string ToString()
        {
            string result = "NumAgent: " + IdAgent + "   Way: ";
            foreach (int elem in WayAgent)
            {
                result += elem + "; ";
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Массив феромонов для графа 3*4
            int N = 4; //Количество параметров
            int M = 3; //Количество значений параметров

            int K = 3; //Количество муравьев

            List<GrafParams> Graf = new List<GrafParams>(); //Список элементов Graf
            //Заполнение списка элементов графа 
            int id = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Graf.Add(new GrafParams() { IdParam = id, NumParam = i, Pheromones = 1, SelectNum = 0 });
                    id++;
                }
            }
            foreach (GrafParams element in Graf)
            {
                Console.WriteLine(element);
                //Console.WriteLine("Num: {grafParams.NumParam}; Znach: {grafParams.Pheromones}");
            }


            //Создание группы агентов
            List<AgentGroup> Agent = new List<AgentGroup>(); //Список агентов
            //Прохождение K агентов
            for (int i = 0; i < K; i++)
            {
                Agent.Add(new AgentGroup() { IdAgent = i });

                //Добавление пути агента
                //int[] m = AgentMoving(N, M, Graf);
                Agent[i].WayAgent.AddRange(AgentMoving(N, M, Graf));

                Console.WriteLine(Agent[i]);
            }

            //Занесение феромона
            for (int i = 0; i < K; i++)
            {
                Agent[i].Delta = AddPheromone(N, M, Agent[i].WayAgent, Graf);
            }

            //Испарение феромонов
            PheromoneEvaporation(N, M, Graf, Agent);


        }

        public static int[] AgentMoving(int n, int m, List<GrafParams> graf)
        {

            int[] Way = new int[n]; //Выбранный путь           

            for (int i = 0; i < graf.Count; i += m)
            {
                //Выбор следующей вершины
                ChoiceNextVertex(i, m, graf, Way);
            }

            for (int j = 0; j < n; j++)
            {
                Console.Write(" " + Way[j]);
            }
            Console.WriteLine();

            return Way;
        }

        public static int ChoiceNextVertex(int i, int m, List<GrafParams> graf, int[] way) //Выбор следующей вершины
        {

            int SumPheromones = 0;
            double[] Pij = new double[m];//Массив вероятности попадания
            int[] Idij = new int[m];//Массив Id рассматриваемых узлов

            for (int j = 0; j < m; j++)
            {
                SumPheromones += graf[i + j].Pheromones;
            }
            //Console.WriteLine("Raram: " + graf[i].NumParam + " SumPheromones: "+ SumPheromones);

            //Подсчет вероятности попадания
            for (int j = 0; j < m; j++)
            {
                Pij[j] = Convert.ToDouble(graf[i + j].Pheromones) / Convert.ToDouble(SumPheromones);
                Idij[j] = graf[i + j].IdParam;
            }

            //Переход к случайному параметру
            double[] Intervals = new double[m + 1]; //Определение интервалов попадания
            Intervals[0] = 0;
            for (int j = 1; j < Intervals.Length; j++)
            {
                Intervals[j] = Intervals[j - 1] + Pij[j - 1];
            }
            Intervals[Intervals.Length - 1] = 1;

            Random rnd = new Random();
            double value = rnd.NextDouble();

            int ParametrNum = 0;
            int x = 1;
            while (ParametrNum == 0)
            {
                if ((value < Intervals[x]) && (value > Intervals[x - 1]))
                {
                    ParametrNum = x;
                }
                x++;
            }

            way[graf[i].NumParam] = Idij[ParametrNum - 1];
            //Поднятие флага выбора параметра           
            graf[i + ParametrNum - 1].SelectNum++;

            return 0;
        }

        public static double AddPheromone(int n, int m, List<int> way, List<GrafParams> graf) //Добавление феромонов
        {
            int Func = Function(n, m, way); //Значение целевой фцнкции
            int Q = 100; //Общее число феромонов
            int Delta = Q / Func;

            for (int i = 0; i < n; i++)
            {
                graf[way[i]].Pheromones += Delta;
            }
            return Delta;
        }

        public static int Function(int n, int m, List<int> way) //Подсчет целивой функции
        {
            int Value = 1;
            return Value;
        }

        public static int PheromoneEvaporation(int n, int m, List<GrafParams> graf, List<AgentGroup> agent) //Испарение феромона
        {
            double L = 0.2;
            foreach (GrafParams grafElem in graf)
            {
                double Evaporation = L * Convert.ToDouble(grafElem.Pheromones);

                foreach (AgentGroup agentElem in agent)
                {
                    foreach (int wayElem in agentElem.WayAgent) {
                        if (wayElem == grafElem.IdParam)
                            Evaporation += (1 - L) * agentElem.Delta;
                    }
                }

                //element.Pheromones = Convert.ToInt32(Evaporation);
            }
            
            return 0;
        }
    }
}
