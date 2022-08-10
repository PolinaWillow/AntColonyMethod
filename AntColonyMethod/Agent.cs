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
        public int idAgent { get; set; }
        public double delta { get; set; }

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
        public static int[] FindAgentWay_Method2(int n, List<int> m, Graf graf, Hashtable hashTable, int controlCount) {
            int[] wayAgent;
            do
            {
                wayAgent = graf.FindWay(n, m);                
                //Вычисление Хэша пути
                string hashWay = Hash.GetHash(wayAgent);
                //Console.WriteLine("Хэш выбранного пути: "+hashWay);

                //Сравнение Хэша со значениями таблицы
                if (!hashTable.ContainsKey(hashWay))
                {
                    hashTable.Add(hashWay, wayAgent); //Добавление нового ключа в таблицй                            
                    break;
                }
                if (controlCount == hashTable.Count)
                {
                    break;
                }
            } while (true) ;

            return wayAgent;
        }
        public static int[] FindAgentWay_Method1(int n, List<int> m, Graf graf, Hashtable hashTable) {
            int[] wayAgent;
            wayAgent = graf.FindWay(n, m); //Генерация первичного пути
                                           //Вычисление Хэша пути
            string hashWay = Hash.GetHash(wayAgent);
            if (!hashTable.ContainsKey(hashWay))
            {
                hashTable.Add(hashWay, wayAgent); //Добавление нового ключа в таблицй                            
            }
            else
            {
                int[] newWayAgent = FindAlternativeWay(n, graf, wayAgent, hashTable, hashWay);
                hashWay = Hash.GetHash(newWayAgent);
                hashTable.Add(hashWay, newWayAgent);
                Array.Copy(newWayAgent, 0, wayAgent, 0, n);

            }

            return wayAgent;

        }

        public static int NextWay(int[] way, int nomParam, Graf graf)
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

        public static int[] FindAlternativeWay(int n, Graf graf, int[] startWay, Hashtable hashTable, string hashWay)
        {
            int nomParam = 0; //Номер параметра
            int[] newWay = new int[n]; //Новый путь
            Array.Copy(startWay, 0, newWay, 0, n);

            while (hashTable.ContainsKey(hashWay) && (nomParam < n))
            {
                NextWay(newWay, nomParam, graf);

                if (newWay[nomParam] == startWay[nomParam] && nomParam < n)
                {
                    while (newWay[nomParam] == startWay[nomParam] && nomParam < n)
                    {
                        nomParam += 1;
                        NextWay(newWay, nomParam, graf);
                    }
                    nomParam = 0;
                }
                hashWay = Hash.GetHash(newWay);
            }

            return newWay;
        }


    }
}
