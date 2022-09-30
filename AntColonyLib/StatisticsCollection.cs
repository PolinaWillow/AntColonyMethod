using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyLib
{
    public class StatisticsCollection
    {
        public StatisticsCollection()
        {
            NumHitPercentage = ChangeableParams.NUM_HIT_PERCENTAGE;
            numLaunches = ChangeableParams.NUM_LAUNCHES;
            LaunchesCount = 0;
            optimalMax = 7 - 1 + 2 * 10 + 20 + 2 * 3 + 0.5 * 5.34 - 0.12 * 15.3 - 1 + 80 * 0.5 + 0.00001 * 600000 + 20;
            optimalMin = 1 - 5 + 2 * 1 + 1 + 2 * 1 + 0.5 * 5.24 - 0.12 * 16.9 - 10 + 80 * 0.01 + 0.00001 * 100000;
            MIteration = 0;
            DIteration = 0;
            MSolution = 0;
            DSolution = 0;
            AntEnumI = 0;
            KolEnumI = 0;
            AntEnumIProc = 0;


            UniqueSolutionCount = 0;

            PercentageList = new List<double>();
            PercentageList.Add(0.9999);
            for (int i = 1; i < ChangeableParams.NUM_HIT_PERCENTAGE; i++)
            {
                PercentageList.Add(1 - i * 0.01);
            }

            HitCount = new List<int>();
            МFinctionI = new List<double>();
            DFinctionI = new List<double>();
            МFinctionS = new List<double>();
            DFinctionS = new List<double>();
            Kol = new List<int>();

            for (int i = 0; i < ChangeableParams.NUM_HIT_PERCENTAGE; i++)
            {
                HitCount.Add(0);
                МFinctionI.Add(0);
                DFinctionI.Add(0);
                МFinctionS.Add(0);
                DFinctionS.Add(0);
                Kol.Add(0);
            }

            WorkTime = new TimeSpan();
            TimeStart = new DateTime();
            TimeEnd = new DateTime();
        }

        /// <summary>
        /// ПРоцент попадания
        /// </summary>
        public int NumHitPercentage { get; set; }

        /// <summary>
        /// Время работы алгоритма
        /// </summary>
        public TimeSpan WorkTime { get; set; }

        /// <summary>
        /// Начало отсчета времени выполнения
        /// </summary>
        public DateTime TimeStart { get; set; }

        /// <summary>
        /// Конец отсчета времени выполнения
        /// </summary>
        public DateTime TimeEnd { get; set; }

        /// <summary>
        /// Число запусков программы
        /// </summary>
        public int numLaunches { get; set; }

        /// <summary>
        /// Номер текущего запуска
        /// </summary>
        public int LaunchesCount { get; set; }

        /// <summary>
        /// Оптимальное значение максимума
        /// </summary>
        public double optimalMax { get; set; }

        /// <summary>
        /// Оптимальное значение минимума
        /// </summary>
        public double optimalMin { get; set; }


        /// <summary>
        /// Матожидание итераций
        /// </summary>
        public double MIteration { get; set; }

        /// <summary>
        /// Дисперсия итераций
        /// </summary>
        public double DIteration { get; set; }

        /// <summary>
        /// Среднее число найденных уникальных решений
        /// </summary>
        public double MSolution { get; set; }

        /// <summary>
        /// Дисперсия найденных уникальных решений
        /// </summary>
        public double DSolution { get; set; }

        /// <summary>
        /// Номер итерации, на котором найдено оптимальное решение
        /// </summary> 
        public List<double> МFinctionI { get; set; }

        /// <summary>
        /// Дисперсия попадание в интервал (итерации)
        /// </summary>
        public List<double> DFinctionI { get; set; }

        /// <summary>
        /// Номер агента, нашедшего решение
        /// </summary>
        public List<double> МFinctionS { get; set; }

        /// <summary>
        /// Дисперсия попадание в интервал (решение)
        /// </summary>
        public List<double> DFinctionS { get; set; }

        /// <summary>
        /// Среднее количество переборов путей за итерацию
        /// </summary>
        public double AntEnumI { get; set; }

        public double AntEnumIProc { get; set; }

        /// <summary>
        /// Количество переборов за итерацию
        /// </summary>
        public int KolEnumI { get; set; }



        /// <summary>
        /// Сколько раз собрали статистику по матожиданию и дисперсии
        /// </summary>
        public List<int> Kol { get; set; }

        /// <summary>
        /// Список хранения процентов
        /// </summary>
        public List<double> PercentageList { get; set; }

        /// <summary>
        /// Список количества найденных решений, попавших в определенный интервал
        /// </summary>
        public List<int> HitCount { get; set; }

        /// <summary>
        /// Число уникальных решений, найденных за итерацию
        /// </summary>
        public int UniqueSolutionCount { get; set; }

        /// <summary>
        /// Сброс статистики на начало
        /// </summary>
        public void StartStatistics()
        {
            LaunchesCount = 0;

            MIteration = 0;
            DIteration = 0;
            MSolution = 0;
            DSolution = 0;

            UniqueSolutionCount = 0;
        }

        /// <summary>
        /// Сброс статистики перед каждым новым прогоном
        /// </summary>
        public void ResetStatistics()
        {
            for (int i = 0; i < ChangeableParams.NUM_HIT_PERCENTAGE; i++)
            {
                HitCount[i] = 0;
            }

            for (int i = 0; i < ChangeableParams.NUM_HIT_PERCENTAGE; i++)
            {
                МFinctionI[i] = 0;
                DFinctionI[i] = 0;
                МFinctionS[i] = 0;
                DFinctionS[i] = 0;
                Kol[i] = 0;
            }

            AntEnumI = 0;
            KolEnumI = 0;
            AntEnumIProc = 0;


        }


        /// <summary>
        /// Определение среднего числа итераций за все прогоны
        /// </summary>
        /// <param name="IterationCount">Номер итерации</param>
        /// <param name="SolutionCount">Число уникальных решений</param>
        public void CollectingStat(int IterationCount, int SolutionCount)
        {
            IterationCount += 1;
            MIteration += IterationCount;
            DIteration = DIteration + IterationCount * IterationCount;
            MSolution += SolutionCount;
            DSolution = DSolution + SolutionCount * SolutionCount;
        }

        /// <summary>
        /// Определения количества попадания решения в интервалы
        /// </summary>
        /// <param name="targetFunction">Значение функции</param>
        public void FindOptimalCount(double targetFunction, int nomIteration, int nomSolution)
        {
            for (int i = 0; i < ChangeableParams.NUM_HIT_PERCENTAGE; i++)
            {
                if ((((optimalMax - optimalMin) * PercentageList[i] + optimalMin) <= targetFunction) && (HitCount[i] == 0))
                {
                    //Подсчет матожидания и дисперсии
                    МFinctionI[i] = МFinctionI[i] + nomIteration;
                    DFinctionI[i] = DFinctionI[i] + nomIteration * nomSolution;
                    МFinctionS[i] = МFinctionS[i] + nomSolution;
                    DFinctionS[i] = DFinctionS[i] + nomSolution * nomSolution;

                    Kol[i] = Kol[i] + 1;
                }

                if (((optimalMax - optimalMin) * PercentageList[i] + optimalMin) <= targetFunction)
                {
                    HitCount[i]++;
                }
            }

        }

        /// <summary>
        /// Определение среднее количества переборов за решение (Сколько раз агенты перебирают пути за итерацию в среднем)
        /// </summary>
        /// <param name="IterationCount">Число итераций</param>
        public void EmunStatI(long IterationCount)
        {
            AntEnumI = KolEnumI / IterationCount;
            AntEnumIProc = KolEnumI / (KolEnumI + IterationCount);
        }

        /// <summary>
        /// Определение времени работы программы
        /// </summary>
        public void WorkTimeLaunch()
        {
            WorkTime = TimeEnd - TimeStart;
        }

    }

}
