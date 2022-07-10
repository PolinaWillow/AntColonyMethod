using System;

namespace AntColonyMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            //Массив феромонов для графа 3*4
            int N = 3;
            int M = 4;
            int[,] Graf = { { 15,15,15,40},
                            { 20,21,20,30},
                            { 19,10,23,45} };
            
            Console.WriteLine("Исходный граф");
            for (int j = 0; j < N; j++)
            {
                for (int i = 0; i < M; i++)
                {
                    Console.Write(Graf[j,i]+ " ");
                }
                Console.WriteLine("");
            }

            ColonyMethod(N, M, Graf);

        }

        public static int ColonyMethod(int n, int m, int[,] graf)
        {
            //Определение пути по графу
            for (int j = 0; j < m; j++)
            {
                int SumPheromones = 0;
                //Массив вероятности попадания
                double[] Pij = new double[n];

                //Подсчет общего числа феромонов
                for (int i = 0; i < n; i++)
                {                   
                    SumPheromones += graf[i,j];
                }

                //Подсчет вероятности попадания
                Console.Write("\nВероятности попадания для параметра " + (j + 1) + ": ");
                for (int i = 0; i < n; i++)
                {
                    Pij[i] = Convert.ToDouble(graf[i, j]) / Convert.ToDouble(SumPheromones);
                    Console.Write(Pij[i] + " ");
                }                             
                Console.WriteLine("");

                //Переход к случайному параметру
                double[] Intervals = new double[n+1];
                Intervals[0] = 0;
                for (int i = 1; i < Intervals.Length; i++) {
                    Intervals[i] = Intervals[i - 1] + Pij[i - 1];
                }
                Intervals[Intervals.Length-1] = 1;
                Console.WriteLine(string.Join(" ", Intervals));

                Random rnd = new Random();
                double value = rnd.NextDouble();
                Console.WriteLine("rnd = "+ value);

                int ParametrNum=0;
                for (int i = 1; i < Intervals.Length; i++) {
                    if((value < Intervals[i])&&(value > Intervals[i - 1]))
                    {
                        ParametrNum = i;
                    }
                }
                Console.WriteLine("Номер выбранного значения текущего параметра = " + ParametrNum);
            }
            return 0;
        }
    }
}
