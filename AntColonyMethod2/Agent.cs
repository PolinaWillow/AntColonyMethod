using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntColonyLib;

namespace AntColonyMethod2
{
    public class Agent
    {
        /// <summary>
        /// Идентификатор агента
        /// </summary>
        public string idAgent { get; set; }

        /// <summary>
        /// Дельта
        /// </summary>
        public double delta { get; set; }

        /// <summary>
        /// Путь Агента
        /// </summary>
        public List<int> wayAgent = new List<int>();

        public override string ToString()
        {
            string result = "NumAgent: " + idAgent + "   Way: ";
            foreach (int elem in wayAgent)
            {
                result += elem + "; ";
            }
            return result;
        }

        /// <summary>
        /// Определениие пути агента (Вариант 3)
        /// </summary>
        /// <param name="dataTask">Набор входных данных</param>
        /// <returns></returns>
        public int[] FindAgentWay_Method(DataTask dataTask, StatisticsCollection statistics)
        {
            int[] wayAgent;
            wayAgent = dataTask.graf.FindWay(dataTask); //Генерация первичного пути

            //Вычисление Хэша пути
            Hash hash = new Hash();
            string hashWay = hash.GetHash(wayAgent);
            if (!dataTask.hashTable.ContainsKey(hashWay))
            {
                hash.AddNewHash(hashWay, wayAgent, dataTask.hashTable); //Добавление нового ключа в таблицй                            
            }
            else
            {
                //Создание упорядоченной копии графа
                dataTask.grafCopy.CreateGrafClone(dataTask.grafCopy, dataTask, "ParamsIncreasing", 0);

                int[] newWayAgent = FindAlternativeWay(dataTask, wayAgent, hashWay, statistics);
                hashWay = hash.GetHash(newWayAgent);
                hash.AddNewHash(hashWay, wayAgent, dataTask.hashTable);
                Array.Copy(newWayAgent, 0, wayAgent, 0, dataTask.paramCount);

            }

            return wayAgent;

        }

        /// <summary>
        /// Новый путь агента
        /// </summary>
        /// <param name="way">Путь агента</param>
        /// <param name="nomParam">Номер параметро</param>
        /// <param name="graf">Граф значений</param>
        /// <returns></returns>
        private int NextWay(int[] way, int nomParam, Graf graf)
        {
            //Создаем и заполняем сиписок слоя
            List<int> Layer = new List<int>();
            foreach (GrafParams elem in graf.Params)
            {
                if (elem.numParamFact == nomParam) { Layer.Add(elem.idParam); }
            }

            //Увелечение значения параметра на 1
            way[nomParam] += 1;
            if (way[nomParam] > Layer[Layer.Count - 1]) { way[nomParam] = Layer[0]; }

            return 0;
        }


        /// <summary>
        /// Поиск альтернативного путя агента (Вариант 2)
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="startWay">Изменяемый путь</param>
        /// <param name="hashWay">Хэш пути</param>
        /// <returns></returns>
        private int[] FindAlternativeWay(DataTask dataTask, int[] startWay, string hashWay, StatisticsCollection statistics)
        {
            int nomParam = 0; //Номер параметра
            int[] newWay = new int[dataTask.paramCount]; //Новый путь
            Hash hash = new Hash();
            Array.Copy(startWay, 0, newWay, 0, dataTask.paramCount);


            while (dataTask.hashTable.ContainsKey(hashWay) && (nomParam < dataTask.paramCount))
            {
                //Сохраняем количество переборов
                statistics.KolEnumI++;

                NextWay(newWay, nomParam, dataTask.grafCopy);

                if (newWay[nomParam] == startWay[nomParam] && nomParam < dataTask.paramCount)
                {
                    while (newWay[nomParam] == startWay[nomParam] && nomParam < (dataTask.paramCount - 1))
                    {
                        nomParam += 1;
                        NextWay(newWay, nomParam, dataTask.grafCopy);
                    }
                    nomParam = 0;
                }
                hashWay = hash.GetHash(newWay);
            }

            return newWay;
        }

    }
}
