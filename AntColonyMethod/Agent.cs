using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class Agent
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
        /// Определениие пути агента (Вариант 2)
        /// </summary>
        /// <param name="dataTask">Набор входных данных</param>
        /// <returns></returns>
        public int[] FindAgentWay_Method2(DataTask dataTask) {
            int[] wayAgent;
            do
            {
                Hash hash = new Hash();
                wayAgent = dataTask.graf.FindWay(dataTask);                
                //Вычисление Хэша пути
                string hashWay = hash.GetHash(wayAgent);
                //Console.WriteLine("Хэш выбранного пути: "+hashWay);

                //Сравнение Хэша со значениями таблицы
                if (!dataTask.hashTable.ContainsKey(hashWay))
                {
                    hash.AddNewHash(hashWay, wayAgent, dataTask.hashTable); //Добавление нового ключа в таблицй                            
                    break;
                }
                if (dataTask.controlCount == dataTask.hashTable.Count)
                {
                    break;
                }
            } while (true) ;

            return wayAgent;
        }

        /// <summary>
        /// Определениие пути агента (Вариант 1)
        /// </summary>
        /// <param name="dataTask">Набор входных данных</param>
        /// <returns></returns>
        public int[] FindAgentWay_Method1(DataTask dataTask) {
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
                int[] newWayAgent = FindAlternativeWay(dataTask, wayAgent, hashWay);
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
        public int NextWay(int[] way, int nomParam, Graf graf)
        {
            //Создаем и заполняем сиписок слоя
            List<int> Layer = new List<int>();
            foreach (GrafParams elem in graf.Params)
            {
                if (elem.numParam == nomParam) { Layer.Add(elem.idParam); }
            }

            //Увелечение значения параметра на 1
            way[nomParam] += 1;
            if (way[nomParam] > Layer[Layer.Count - 1]) { way[nomParam] = Layer[0]; }

            return 0;
        }

        /// <summary>
        /// Поиск альтернативного путя агента
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="startWay">Изменяемый путь</param>
        /// <param name="hashWay">Хэш пути</param>
        /// <returns></returns>
        public int[] FindAlternativeWay(DataTask dataTask, int[] startWay, string hashWay)
        {
            int nomParam = 0; //Номер параметра
            int[] newWay = new int[dataTask.paramCount]; //Новый путь
            Hash hash = new Hash();
            Array.Copy(startWay, 0, newWay, 0, dataTask.paramCount);

            while (dataTask.hashTable.ContainsKey(hashWay) && (nomParam < dataTask.paramCount))
            {
                NextWay(newWay, nomParam, dataTask.graf);

                if (newWay[nomParam] == startWay[nomParam] && nomParam < dataTask.paramCount)
                {
                    while (newWay[nomParam] == startWay[nomParam] && nomParam < (dataTask.paramCount-1))
                    {
                        nomParam += 1;                       
                        NextWay(newWay, nomParam, dataTask.graf);
                    }
                    nomParam = 0;
                }
                hashWay = hash.GetHash(newWay);
            }

            return newWay;
        }


    }
}
