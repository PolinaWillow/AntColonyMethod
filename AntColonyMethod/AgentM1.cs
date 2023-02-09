using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntColonyLib;
using AntColonyLib.DataModel;
using AntColonyLib.Processing;

namespace AntColonyMethod
{
    public class AgentM1
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
        /// Определениие пути агента
        /// </summary>
        /// <param name="dataTask">Набор входных данных</param>
        /// <returns></returns>
        public int[] FindAgentWay_Method(DataTask dataTask, StatisticsCollection statistics)
        {
            int[] wayAgent;
            wayAgent = dataTask.graphWorkCopy.FindWay(dataTask); //Генерация первичного пути

            //Вычисление Хэша пути
            Hash hash = new Hash();
            string hashWay = hash.GetHash(wayAgent);
            if (ChangeableParams.HASH_SAVE)
            {
                if (dataTask.squliteBD.FindHashInTable(hashWay) == 1/*!dataTask.hashTable.ContainsKey(hashWay)*/)
                {
                    hash.AddNewHash(hashWay, wayAgent, dataTask);//hashTable); //Добавление нового ключа в таблицй                            
                }
                else
                {
                    int[] newWayAgent = FindAlternativeWay(dataTask, wayAgent, hashWay, statistics);
                    hashWay = hash.GetHash(newWayAgent);
                    hash.AddNewHash(hashWay, wayAgent, dataTask);//hashTable);
                    Array.Copy(newWayAgent, 0, wayAgent, 0, dataTask.graphWorkCopy.paramCount);

                }
            }
            else {
                if (!dataTask.hashTable.ContainsKey(hashWay))
                {
                    hash.AddNewHash(hashWay, wayAgent, dataTask);//hashTable); //Добавление нового ключа в таблицй                            
                }
                else
                {
                    int[] newWayAgent = FindAlternativeWay(dataTask, wayAgent, hashWay, statistics);
                    hashWay = hash.GetHash(newWayAgent);
                    hash.AddNewHash(hashWay, wayAgent, dataTask);//hashTable);
                    Array.Copy(newWayAgent, 0, wayAgent, 0, dataTask.graphWorkCopy.paramCount);

                }

            }
           

            return wayAgent;

        }
       
        /// <summary>
        /// Новый путь агента
        /// </summary>
        /// <param name="way">Путь агента</param>
        /// <param name="nomParam">Номер параметро</param>
        /// <param name="graph">Граф значений</param>
        /// <returns></returns>
        private int NextWay(int[] way, int nomParam, Graph graph)
        {
            //Создаем и заполняем сиписок слоя
            List<int> Layer = new List<int>();
            foreach (GraphParams elem in graph.Params)
            {
                if (elem.numParamFact == nomParam) { Layer.Add(elem.idParam); }
            }

            //Увелечение значения параметра на 1
            way[nomParam] += 1;
            if (way[nomParam] > Layer[Layer.Count - 1]) { way[nomParam] = Layer[0]; }

            return 0;
        }

        /// <summary>
        /// Поиск альтернативного путя агента (Вариант 1)
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="startWay">Изменяемый путь</param>
        /// <param name="hashWay">Хэш пути</param>
        /// <returns></returns>
        private int[] FindAlternativeWay(DataTask dataTask, int[] startWay, string hashWay, StatisticsCollection statistics)
        {
            int nomParam = 0; //Номер параметра
            int[] newWay = new int[dataTask.graphWorkCopy.paramCount]; //Новый путь
            Hash hash = new Hash();
            Array.Copy(startWay, 0, newWay, 0, dataTask.graphWorkCopy.paramCount);

            if (ChangeableParams.HASH_SAVE)
            {
                while ((dataTask.squliteBD.FindHashInTable(hashWay) == 0)/*hashTable.ContainsKey(hashWay)*/ && (nomParam < dataTask.graphWorkCopy.paramCount))
                {
                    //Сохраняем количество переборов
                    statistics.KolEnumI++;

                    NextWay(newWay, nomParam, dataTask.graphWorkCopy);

                    if (newWay[nomParam] == startWay[nomParam] && nomParam < dataTask.graphWorkCopy.paramCount)
                    {
                        while (newWay[nomParam] == startWay[nomParam] && nomParam < (dataTask.graphWorkCopy.paramCount - 1))
                        {
                            nomParam += 1;
                            NextWay(newWay, nomParam, dataTask.graphWorkCopy);
                        }
                        nomParam = 0;
                    }
                    hashWay = hash.GetHash(newWay);
                }
            }
            else {
                while (dataTask.hashTable.ContainsKey(hashWay) && (nomParam < dataTask.graphWorkCopy.paramCount))
                {
                    //Сохраняем количество переборов
                    statistics.KolEnumI++;

                    NextWay(newWay, nomParam, dataTask.graphWorkCopy);

                    if (newWay[nomParam] == startWay[nomParam] && nomParam < dataTask.graphWorkCopy.paramCount)
                    {
                        while (newWay[nomParam] == startWay[nomParam] && nomParam < (dataTask.graphWorkCopy.paramCount - 1))
                        {
                            nomParam += 1;
                            NextWay(newWay, nomParam, dataTask.graphWorkCopy);
                        }
                        nomParam = 0;
                    }
                    hashWay = hash.GetHash(newWay);
                }
            }

            

            return newWay;
        }      
    }
}
