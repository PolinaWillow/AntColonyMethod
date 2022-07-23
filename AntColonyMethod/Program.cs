using System;
using System.Collections.Generic;

namespace AntColonyMethod
{
    class GrafParams
    {
        public int NumParam { get; set; }
        public int Pheromones { get; set; }

        public bool FlagChoice { get; set; }

        public int SelectNum { get; set; }

        public int Delta { get; set; }
        //public double ValParam { get; set; }

        public override string ToString()
        {
            return "NumParam: " + NumParam + "   Pheromones: " + Pheromones;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //Массив феромонов для графа 3*4
            int N = 4; //Количество параметров
            int M = 3; //Количество значений параметров

            int K = 2; //Количество муравьев

            List<GrafParams> Graf = new List<GrafParams>(); //Список элементов Graf
            //Заполнение списка элементов графа 
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {

                    Graf.Add(new GrafParams() { NumParam = i, Pheromones = 1, FlagChoice = false, SelectNum = 0, Delta = 0 });
                }
            }
            foreach (GrafParams element in Graf)
            {
                Console.WriteLine(element);
                //Console.WriteLine("Num: {grafParams.NumParam}; Znach: {grafParams.Pheromones}");
            }

            //Прохождение K муравьев
            for (int i = 0; i < K; i++)
            {
                AgentMoving(N, M, Graf);

                foreach (GrafParams element in Graf)
                {
                    Console.WriteLine(element);
                    //Console.WriteLine("Num: {grafParams.NumParam}; Znach: {grafParams.Pheromones}");
                }
            }

            //Испарение феромонов
            PheromoneEvaporation(N, M, Graf);

        }

        public static int AgentMoving(int n, int m, List<GrafParams> graf)
        {

            int[,] Ways = new int[n, 2]; //Выбранный путь
            for (int i = 0; i < n; i++)
            {
                Ways[i, 0] = i + 1;
                Ways[i, 1] = 0;
            }

            for (int i = 0; i < graf.Count; i += m)
            {
                int SumPheromones = 0;
                double[] Pij = new double[m];//Массив вероятности попадания

                for (int j = 0; j < m; j++)
                {
                    SumPheromones += graf[i + j].Pheromones;
                }
                //Console.WriteLine("Raram: " + graf[i].NumParam + " SumPheromones: "+ SumPheromones);

                //Подсчет вероятности попадания
                for (int j = 0; j < m; j++)
                {
                    Pij[j] = Convert.ToDouble(graf[i + j].Pheromones) / Convert.ToDouble(SumPheromones);
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

                Ways[graf[i].NumParam, 1] = ParametrNum;
                //Поднятие флага выбора параметра
                graf[i + ParametrNum - 1].FlagChoice = true;
                graf[i + ParametrNum - 1].SelectNum++;
            }

            for (int j = 0; j < n; j++)
            {
                Console.Write(" " + Ways[j, 1]);
            }
            Console.WriteLine();

            //Пересчет феромонов
            AddPheromone(n, m, Ways, graf);

            return 0;
        }

        public static int ChoiceNext() //Выбор следующей вершины
        {

            return 0;
        }

        public static int AddPheromone(int n, int m, int[,] ways, List<GrafParams> graf) //Добавление феромонов
        {
            int Func = Function(n, m, ways); //Значение целевой фцнкции
            int Q = 100; //Общее число феромонов
            int Delta = Q / Func;

            for (int i = 0; i < graf.Count; i++)
            {
                if (graf[i].FlagChoice == true)
                {
                    graf[i].Delta += Delta;
                    graf[i].Pheromones += Delta;
                    graf[i].FlagChoice = false;
                }
            }
            return 0;
        }

        public static int Function(int n, int m, int[,] ways) //Подсчет целивой функции
        {
            int Value = 1;
            return Value;
        }

        public static int PheromoneEvaporation(int n, int m, List<GrafParams> graf) //Испарение феромона
        {
            double L = 0.2;
            foreach (GrafParams element in graf)
            {
                double Evaporation = L * Convert.ToDouble(element.Pheromones) + (1 - L) * element.Delta;
                element.Pheromones = Convert.ToInt32(Evaporation);
            }

            return 0;
        }
    }
}
