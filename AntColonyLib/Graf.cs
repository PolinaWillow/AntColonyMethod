using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyLib
{
    public class Graf
    {
        /// <summary>
        /// Список параметров
        /// </summary>
        public List<GrafParams> Params = new List<GrafParams>();

        /// <summary>
        /// Список id первых значений параметра
        /// </summary>
        public List<int> IDFirstValueParam = new List<int>();

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
                IDFirstValueParam.Add(id);
                for (int j = 0; j < dataTask.valueCount[i]; j++)
                {
                    Params.Add(new GrafParams() { idParam = id, numParamFact = i, numParam = i, pheromones = 1, selectNum = 0 });

                    //Опредление типа значения параметра
                    if (double.TryParse(dataTask.valueData[id], out double res))
                    {
                        Params[id].typeParam = TypeNumerator.Double;
                    }
                    else { Params[id].typeParam = TypeNumerator.String; }
                    Params[id].numValueParam = j;
                    Params[id].valueParam = dataTask.valueData[id];

                    id++;
                }
            }

            PrintGraf();
            return 0;
        }

        /// <summary>
        /// Создание Упорядоченного графа
        /// </summary>
        /// <param name="ParamsCopy">Упорядочевиемый граф</param>
        /// <param name="dataTask">Исходные данные</param>
        /// <param name="key">Ключ упорядочения</param>
        /// <param name="NumParam">Номер параметра</param>
        /// <returns></returns>
        public int CreateGrafClone(Graf GrafCopy, DataTask dataTask, string key, int NumParam)
        {
            if (string.Compare(key, "ParamsIncreasing") == 0)
            { //Упорядочивание № параметров по количеству значений по возрастанию 
                //Упорядочивание номеров параметров
                int[,] NumsParams = new int[dataTask.valueCount.Count, 2];
                for (int i = 0; i < dataTask.valueCount.Count; i++)
                {
                    NumsParams[i, 0] = i;
                    NumsParams[i, 1] = dataTask.valueCount[i];
                }

                //Сортировка номеров параметров
                for (int i = 1; i < dataTask.valueCount.Count; i++)
                {
                    for (int j = 0; j < dataTask.valueCount.Count - 1; j++)
                    {
                        if (NumsParams[j, 1] > NumsParams[j + 1, 1])
                        {
                            int num = NumsParams[j, 0];
                            int val = NumsParams[j, 1];
                            NumsParams[j, 0] = NumsParams[j + 1, 0];
                            NumsParams[j, 1] = NumsParams[j + 1, 1];
                            NumsParams[j + 1, 0] = num;
                            NumsParams[j + 1, 1] = val;
                        }
                    }
                }

                //Создание клона графа
                for (int i = 0; i < dataTask.valueCount.Count; i++)
                {
                    for (int j = 0; j < NumsParams[i, 1]; j++)
                    {
                        GrafCopy.Params.Add(dataTask.graf.Params[dataTask.graf.IDFirstValueParam[NumsParams[i, 0]] + j]);
                    }
                }

                //Перенумерация параметров
                int f = 0;
                int k = 0;
                int h = 0;
                while (k < dataTask.graf.Params.Count)
                {
                    h++;
                    if (h > NumsParams[f, 1])
                    {
                        f++;
                        h = 1;
                    }
                    GrafCopy.Params[k].numParamFact = f;

                    k++;
                }

                //GrafCopy.PrintGraf();
            }


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

            way[Params[i].numParam] = Idij[parametrNum - 1];
            //Поднятие флага выбора параметра           
            Params[i + parametrNum - 1].selectNum++;

            return 0;
        }

        
    }
}
