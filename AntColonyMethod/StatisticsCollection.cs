using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class StatisticsCollection
    {
        public int NumHitPercentage = 16; //Количество интервалов попадания 
        public StatisticsCollection() {
            numLaunches = 4;
            LaunchesCount = 0;
            optimalMax = 7 - 1 + 2 * 10 + 20 + 2 * 3 + 0.5 * 5.34 - 0.12 * 15.3 - 1 + 80 * 0.5 + 0.00001 * 600000 + 20;
            optimalMin = 1 - 5 + 2 * 1 + 1 + 2 * 1 + 0.5 * 5.24 - 0.12 * 16.9 - 10 + 80 * 0.01 + 0.00001 * 100000;
            MIteration = 0;
            DIteration = 0;
            MSolution = 0;
            DSolution = 0;
            UniqueSolutionCount = 0;

            PercentageList = new List<double>();
            PercentageList.Add(0.9999);
            for (int i = 1; i < NumHitPercentage; i++) {
                PercentageList.Add(1-i*0.01);
            }

            HitCount = new List<int>();
            for (int i = 0; i < NumHitPercentage; i++)
            {
                HitCount.Add(0);
            }
        }

        /// <summary>
        /// Количество запусков выполнения
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
        public long MIteration { get; set; }

        /// <summary>
        /// Дисперсия итераций
        /// </summary>
        public long DIteration { get; set; }

        /// <summary>
        /// Матожидание найденных уникальных решений
        /// </summary>
        public long MSolution { get; set; }

        /// <summary>
        /// Дисперсия найденных уникальных решений
        /// </summary>
        public long DSolution { get; set; }

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
        /// Количество найденных оптимальных решений максимума
        /// </summary>
        public long OptimalCountMax { get; set; }

        /// <summary>
        /// Сброс статистики на начало
        /// </summary>
        public void StartStatistics() {
            MIteration = 0;
            DIteration = 0;
            MSolution = 0;
            DSolution = 0;
            UniqueSolutionCount = 0;
        }

        /// <summary>
        /// Сброс части статистики
        /// </summary>
        public void ResetStatistics()
        {
            OptimalCountMax = 0;
            for (int i = 0; i < NumHitPercentage; i++)
            {
                HitCount[i] = 0;
            }

        }


        /// <summary>
        /// Сбор статистики
        /// </summary>
        /// <param name="IterationCount">Число итераций</param>
        /// <param name="SolutionCount">Число уникальных решений</param>
        public void CollectingStat(int IterationCount, int SolutionCount) {
            MIteration += IterationCount;
            DIteration = DIteration + IterationCount * IterationCount;
            MSolution += SolutionCount;
            DSolution = DSolution + SolutionCount* SolutionCount;
        }

        /// <summary>
        /// Определения количества попадания решения в интервалы
        /// </summary>
        /// <param name="targetFunction">Значение функции</param>
        public void FindOptimalCount(double targetFunction) {
            for (int i = 0; i < NumHitPercentage; i++) {
                if (((optimalMax - optimalMin) * PercentageList[i] + optimalMin) <= targetFunction) 
                {
                    HitCount[i]++;
                }
            }
            

            if (targetFunction == optimalMax) {
                OptimalCountMax++;
            }
        }
    }
}
