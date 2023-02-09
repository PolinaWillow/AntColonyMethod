using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntColonyLib.DataModel;
using AntColonyLib.Processing;

namespace AntColonyLib
{
    /// <summary>
    /// Класс, служащий для хранения 
    /// </summary>
    public class DataTask
    {       
        /// <summary>
        /// Контрольное число путей
        /// </summary>
        public int controlCount { get; set; }       

        /// <summary>
        /// Список входных значений
        /// </summary>
        public List<string> valueData { get; set; }

        /// <summary>
        ///  Хэш-таблица для хранения хэшей путей
        /// </summary>
        public Hashtable hashTable { get; set; }

        /// <summary>
        /// База данных для временного хранения Хэшей путей
        /// </summary>
        public SqliteWork squliteBD { get; set; }

        /// <summary>
        /// Оригинальный граф параметров
        /// </summary>
        public Graph graphOriginal { get; set; }

        /// <summary>
        /// Граф параметров (Упорядоченная копия)
        /// </summary>
        public Graph graphWorkCopy { get; set; }

        /// <summary>
        /// Число итераций в алгоритме
        /// </summary>
        public long iterationCount { get; set; }

        /// <summary>
        /// Количество агентов
        /// </summary>
        public int antCount { get; set; }

        /// <summary>
        /// Наличие или отсутствие потоков
        /// </summary>
        public bool availabilityThread { get; set; }

        public DataTask()
        {
            
            valueData = new List<string>();
            graphOriginal = new Graph();
            graphWorkCopy = new Graph();
            availabilityThread = false;
            antCount = ChangeableParams.ANT_COUNT;
            iterationCount = 0;
            controlCount = 1;
            if (ChangeableParams.HASH_SAVE)
            {
                squliteBD = new SqliteWork();
                squliteBD.ConnectionToBd();
            }
            else {
                hashTable = new Hashtable();
            }
            

           

        }

        /// <summary>
        /// Обнуление изменений входных данных и свойства hashTable
        /// </summary>
        public void ResetDatatTask()
        {
            graphOriginal.InitialGraph();
            graphWorkCopy.InitialGraph();
            if (ChangeableParams.HASH_SAVE)
            {
                squliteBD.ClearTable();
            }
            else {
                hashTable.Clear();
            }
        }

        /// <summary>
        /// Заполнение графа
        /// </summary>       
        public int CreateGraph() //Создание графа 
        {
            //Заполнение списка элементов графа 
            int id = 0;
            for (int i = 0; i < graphOriginal.paramCount; i++)
            {
                graphOriginal.IDFirstValueParam.Add(id);
                for (int j = 0; j < graphOriginal.valueCount[i]; j++)
                {
                    graphOriginal.Params.Add(new GraphParams() { idParam = id, numParamFact = i, numParam = i, pheromones = 1, selectNum = 0 });

                    //Опредление типа значения параметра
                    if (double.TryParse(valueData[id], out double res))
                    {
                        graphOriginal.Params[id].typeParam = TypeNumerator.Double;
                    }
                    else { graphOriginal.Params[id].typeParam = TypeNumerator.String; }
                    graphOriginal.Params[id].numValueParam = j;
                    graphOriginal.Params[id].valueParam = valueData[id];

                    id++;
                }
            }
            graphOriginal.PrintGraph();
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
        public int CreateGraphClone(string key, int NumParam)
        {
            graphWorkCopy.paramCount = graphOriginal.paramCount;
            if (string.Compare(key, "ParamsIncreasing") == 0)
            { //Упорядочивание № параметров по количеству значений по возрастанию 
                //Упорядочивание номеров параметров
                int[,] NumsParams = new int[graphOriginal.valueCount.Count, 2];
                for (int i = 0; i < graphOriginal.valueCount.Count; i++)
                {
                    NumsParams[i, 0] = i;
                    NumsParams[i, 1] = graphOriginal.valueCount[i];
                }

                //Сортировка номеров параметров
                for (int i = 1; i < graphOriginal.valueCount.Count; i++)
                {
                    for (int j = 0; j < graphOriginal.valueCount.Count - 1; j++)
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
                for (int i = 0; i < graphOriginal.valueCount.Count; i++)
                {
                    graphWorkCopy.IDFirstValueParam.Add(graphOriginal.IDFirstValueParam[NumsParams[i, 0]]);
                    for (int j = 0; j < NumsParams[i, 1]; j++)
                    {
                        graphWorkCopy.Params.Add(graphOriginal.Params[graphOriginal.IDFirstValueParam[NumsParams[i, 0]] + j]);
                    }
                    graphWorkCopy.valueCount.Add(NumsParams[i, 1]);
                }
                //Переинициализация Id значений
                for (int i = 0; i < graphWorkCopy.Params.Count; i++) {                      
                    graphWorkCopy.Params[i].idParam = i;
                }
                int g = -1;
                for (int i = 0; i < graphWorkCopy.Params.Count; i += graphWorkCopy.valueCount[g]) 
                {
                    graphWorkCopy.IDFirstValueParam[g+1]=i;
                    g++;
                    if (g >= graphWorkCopy.valueCount.Count) { break; }
                }

                //Перенумерация параметров
                int f = 0;
                int k = 0;
                int h = 0;
                while (k < graphOriginal.Params.Count)
                {
                    h++;
                    if (h > NumsParams[f, 1])
                    {
                        f++;
                        h = 1;
                    }
                    graphWorkCopy.Params[k].numParamFact = f;

                    k++;
                }
            } 
            else if (string.Compare(key, "NoSort") == 0) {
                graphWorkCopy.Params.AddRange(graphOriginal.Params);
                graphWorkCopy.IDFirstValueParam.AddRange(graphOriginal.IDFirstValueParam);
                graphWorkCopy.valueCount.AddRange(graphOriginal.valueCount);
            }

            graphWorkCopy.PrintGraph();
            Console.WriteLine(graphWorkCopy.paramCount);
            foreach (int elem in graphWorkCopy.IDFirstValueParam)
            {
                Console.Write("\t" + elem);
            }
            Console.WriteLine();
            foreach (int elem in graphWorkCopy.valueCount)
            {
                Console.Write("\t" + elem);
            }

            return 0;
        }


    }
}
