using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Граф параметров
        /// </summary>
        public Graf graf { get; set; }

        /// <summary>
        /// Граф параметров (Упорядоченная копия)
        /// </summary>
        public Graf grafCopy { get; set; }

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
            hashTable = new Hashtable();
            graf = new Graf();
            grafCopy = new Graf();
            availabilityThread = false;
            antCount = ChangeableParams.ANT_COUNT;
            iterationCount = 0;
            controlCount = 1;
            
        }

        /// <summary>
        /// Обнуление изменений входных данных и свойства hashTable
        /// </summary>
        public void ResetDatatTask()
        {
            graf.InitialGraf();
            hashTable.Clear();
        }

        /// <summary>
        /// Заполнение графа
        /// </summary>       
        public int CreateGraf() //Создание графа 
        {
            //Заполнение списка элементов графа 
            int id = 0;
            for (int i = 0; i < graf.paramCount; i++)
            {
                graf.IDFirstValueParam.Add(id);
                for (int j = 0; j < graf.valueCount[i]; j++)
                {
                    graf.Params.Add(new GrafParams() { idParam = id, numParamFact = i, numParam = i, pheromones = 1, selectNum = 0 });

                    //Опредление типа значения параметра
                    if (double.TryParse(valueData[id], out double res))
                    {
                        graf.Params[id].typeParam = TypeNumerator.Double;
                    }
                    else { graf.Params[id].typeParam = TypeNumerator.String; }
                    graf.Params[id].numValueParam = j;
                    graf.Params[id].valueParam = valueData[id];

                    id++;
                }
            }
            graf.PrintGraf();
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
        public int CreateGrafClone(string key, int NumParam)
        {
            if (string.Compare(key, "ParamsIncreasing") == 0)
            { //Упорядочивание № параметров по количеству значений по возрастанию 
                //Упорядочивание номеров параметров
                int[,] NumsParams = new int[graf.valueCount.Count, 2];
                for (int i = 0; i < graf.valueCount.Count; i++)
                {
                    NumsParams[i, 0] = i;
                    NumsParams[i, 1] = graf.valueCount[i];
                }

                //Сортировка номеров параметров
                for (int i = 1; i < graf.valueCount.Count; i++)
                {
                    for (int j = 0; j < graf.valueCount.Count - 1; j++)
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
                for (int i = 0; i < graf.valueCount.Count; i++)
                {
                    for (int j = 0; j < NumsParams[i, 1]; j++)
                    {
                        grafCopy.Params.Add(graf.Params[graf.IDFirstValueParam[NumsParams[i, 0]] + j]);
                    }
                }

                //Перенумерация параметров
                int f = 0;
                int k = 0;
                int h = 0;
                while (k < graf.Params.Count)
                {
                    h++;
                    if (h > NumsParams[f, 1])
                    {
                        f++;
                        h = 1;
                    }
                    grafCopy.Params[k].numParamFact = f;

                    k++;
                }

                //GrafCopy.PrintGraf();
            }


            return 0;
        }


    }
}
