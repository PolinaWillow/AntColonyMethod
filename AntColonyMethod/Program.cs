using System;

namespace AntColonyMethod
{
    class Program
    {
        class Graf
        {
            public int NumParam;
            public double ValParam;
            public int Pheromones;
        }
        static void Main(string[] args)
        {
            //Массив феромонов для графа 3*4
            int N = 3; //Получается при вводе данных
            int M = 4;
            int K = 10; //Количество муравьев

            //Вместо массива Graf сделать массив феромонов графа с сохранением их значений
            int[,] Graf = { { 15,15,15,40},
                            { 20,21,20,30},
                            { 19,10,23,45} };



            Console.WriteLine("Исходный граф");
            for (int j = 0; j < N; j++)
            {
                for (int i = 0; i < M; i++)
                {
                    Console.Write(Graf[j, i] + " ");
                }
                Console.WriteLine("");
            }

            ColonyMethod(N, M, Graf, K);

        }

        public static int ColonyMethod(int n, int m, int[,] graf, int k)
        {
            for (int h = 0; h < k; h++)
            {
                int[,] Ways = new int[m, 2];
                for (int i = 0; i < m; i++)
                {
                    Ways[i, 0] = i + 1;
                    Ways[i, 1] = 0;
                }
                //Определение пути по графу
                for (int j = 0; j < m; j++)
                {
                    int SumPheromones = 0;
                    //Массив вероятности попадания
                    double[] Pij = new double[n];

                    //Подсчет общего числа феромонов
                    for (int i = 0; i < n; i++)
                    {
                        SumPheromones += graf[i, j];
                    }

                    //Подсчет вероятности попадания
                    //Console.Write("\nВероятности попадания для параметра " + (j + 1) + ": ");
                    for (int i = 0; i < n; i++)
                    {
                        Pij[i] = Convert.ToDouble(graf[i, j]) / Convert.ToDouble(SumPheromones);
                        //Console.Write(Pij[i] + " ");
                    }
                    //Console.WriteLine("");

                    //Переход к случайному параметру
                    double[] Intervals = new double[n + 1];
                    Intervals[0] = 0;
                    for (int i = 1; i < Intervals.Length; i++)
                    {
                        Intervals[i] = Intervals[i - 1] + Pij[i - 1];
                    }
                    Intervals[Intervals.Length - 1] = 1;
                    //Console.WriteLine(string.Join(" ", Intervals));

                    Random rnd = new Random();
                    double value = rnd.NextDouble();
                    //Console.WriteLine("rnd = " + value);

                    int ParametrNum = 0;
                    int x = 0;
                    while (ParametrNum == 0) {
                        if ((value < Intervals[x]) && (value > Intervals[x - 1]))
                        {
                            ParametrNum = x;
                        }
                        x++;
                    }
                    //for (int i = 1; i < Intervals.Length; i++)
                    //{
                    //    if ((value < Intervals[i]) && (value > Intervals[i - 1]))
                    //    {
                    //        ParametrNum = i;
                    //    }
                    //}
                    Ways[j, 1] = ParametrNum;
                    //Console.WriteLine("Номер выбранного значения текущего параметра = " + Ways[j, 1]);
                }

                Console.Write("Путь " + (h + 1) + ": ");
                for (int j = 0; j < m; j++)
                {
                    Console.Write(" " + Ways[j, 1]);
                }
                Console.WriteLine();

                //Пересчет феромонов
                int Func = Function(n, m, Ways);
                int Q = 0; //Общее число феромонов
                int Delta = Q / Func;

                for (int g = 0; g < n; g++)
                {
                    graf[g, Ways[g, 1]] += Delta;
                }

            }
            return 0;
        }

        public static int Function(int n, int m, int[,] ways)
        {
            int Value = 0;
            return Value;
        }
    }
}
