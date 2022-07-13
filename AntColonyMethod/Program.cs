﻿using System;
using System.Collections.Generic;

namespace AntColonyMethod
{
    class GrafParams
    {
        public int NumParam { get; set; }
        public int Pheromones { get; set; }

        public bool FlagChoice { get; set; }
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
            
            int K = 10; //Количество муравьев          

            int[,] TestData = { { 20,15,15,40},
                                { 20,21,20,30},
                                { 19,10,23,45} };//Массив тестовых значений феромонов


            List<GrafParams> Graf = new List<GrafParams>(); //Список элементов Graf
            //Заполнение списка элементов списка 
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {

                    Graf.Add(new GrafParams() { NumParam = i, Pheromones = TestData[j, i], FlagChoice = false });
                }
            }
            foreach (GrafParams grafParams in Graf)
            {
                Console.WriteLine(grafParams);
                //Console.WriteLine("Num: {grafParams.NumParam}; Znach: {grafParams.Pheromones}");
            }

            for (int i = 0; i < K; i++)
            {
                ColonyMethod(N, M, Graf);
            }                 
        }

        public static int ColonyMethod(int n, int m, List<GrafParams> graf)
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
                for (int j = 0; j < m; j++) {
                    Pij[j] = Convert.ToDouble(graf[i+j].Pheromones) / Convert.ToDouble(SumPheromones);
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
            }

            for (int j = 0; j < n; j++)
            {
                Console.Write(" " + Ways[j, 1]);
            }
            Console.WriteLine();

            //Пересчет феромонов
            int Func = Function(n, m, Ways);
            int Q = 0; //Общее число феромонов
            int Delta = Q / Func;

            for (int i = 0; i < graf.Count; i++) {
                if (graf[i].FlagChoice == true) {
                    graf[i].Pheromones += Delta;
                    graf[i].FlagChoice = false;
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
