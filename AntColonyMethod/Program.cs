using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AntColonyMethod
{
    class GrafParams
    {
        public int idParam { get; set; }
        public int numParam { get; set; }
        public int pheromones { get; set; }

        public int selectNum { get; set; }

        //public int Delta { get; set; }

        public override string ToString()
        {
            return "IdParam: " + idParam + "   NumParam: " + numParam + "   Pheromones: " + pheromones;
        }
    }

    class AgentGroup
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
    }

    class Program
    {
        public static int ANT_COUNT = 3; //Количество муравьев
        public static int THREADS_COUNT = ANT_COUNT; //Количество потоков

        static void Main(string[] args)
        {
            //Массив феромонов для графа 3*4
            int N = 4; //Количество параметров
            int M = 3; //Количество значений параметров            

            //Создание Хэш-таблицы
            Hashtable hashTable = new Hashtable();

            List<GrafParams> Graf = new List<GrafParams>(); //Список элементов Graf
            //Заполнение списка элементов графа 
            int id = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Graf.Add(new GrafParams() { idParam = id, numParam = i, pheromones = 1, selectNum = 0 });
                    id++;
                }
            }
            foreach (GrafParams element in Graf)
            {
                Console.WriteLine(element);
                //Console.WriteLine("Num: {grafParams.NumParam}; Znach: {grafParams.Pheromones}");
            }


            //Создание группы агентов
            List<AgentGroup> Agent = new List<AgentGroup>(); //Список агентов
            //Прохождение K агентов
            for (int i = 0; i < ANT_COUNT; i++)
            {
                Agent.Add(new AgentGroup() { idAgent = i });

                //Определение пути агента
                int[] newWayAgent;
                bool flagFindWay = false;
                do
                {
                    newWayAgent = AgentMoving(N, M, Graf);
                    //Вычисление Хэша пути
                    string hashWay = GetHash(newWayAgent);
                    Console.WriteLine(hashWay);

                    //Сравнение Хэша со значениями таблицы
                    if (!hashTable.ContainsKey(hashWay))
                    {
                        hashTable.Add(hashWay, newWayAgent); //Добавление нового ключа в таблицй
                        flagFindWay = true;
                    }     
                    
                } while (!flagFindWay);               


                //Сохранение пути агента
                Agent[i].wayAgent.AddRange(newWayAgent);
                Console.WriteLine(Agent[i]);

                
            }

            //Занесение феромона
            for (int i = 0; i < ANT_COUNT; i++)
            {
                Agent[i].delta = AddPheromone(N, M, Agent[i].wayAgent, Graf);
            }

            //Испарение феромонов
            PheromoneEvaporation(N, M, Graf, Agent);


        }


        public static int[] AgentMoving(int n, int m, List<GrafParams> graf)
        {

            int[] way = new int[n]; //Выбранный путь           

            for (int i = 0; i < graf.Count; i += m)
            {
                //Выбор следующей вершины
                ChoiceNextVertex(i, m, graf, way);
            }

            for (int j = 0; j < n; j++)
            {
                Console.Write(" " + way[j]);
            }
            Console.WriteLine();

            return way;
        }

        public static int ChoiceNextVertex(int i, int m, List<GrafParams> graf, int[] way) //Выбор следующей вершины
        {

            int sumPheromones = 0;
            double[] Pij = new double[m];//Массив вероятности попадания
            int[] Idij = new int[m];//Массив Id рассматриваемых узлов

            for (int j = 0; j < m; j++)
            {
                sumPheromones += graf[i + j].pheromones;
            }
            //Console.WriteLine("Raram: " + graf[i].NumParam + " SumPheromones: "+ SumPheromones);

            //Подсчет вероятности попадания
            for (int j = 0; j < m; j++)
            {
                Pij[j] = Convert.ToDouble(graf[i + j].pheromones) / Convert.ToDouble(sumPheromones);
                Idij[j] = graf[i + j].idParam;
            }

            //Переход к случайному параметру
            double[] intervals = new double[m + 1]; //Определение интервалов попадания
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

            way[graf[i].numParam] = Idij[parametrNum - 1];
            //Поднятие флага выбора параметра           
            graf[i + parametrNum - 1].selectNum++;

            return 0;
        }

        public static double AddPheromone(int n, int m, List<int> way, List<GrafParams> graf) //Добавление феромонов
        {
            int func = Function(n, m, way); //Значение целевой фцнкции
            int q = 100; //Общее число феромонов
            int delta = q / func;

            for (int i = 0; i < n; i++)
            {
                graf[way[i]].pheromones += delta;
            }
            return delta;
        }

        public static int Function(int n, int m, List<int> way) //Подсчет целивой функции
        {
            int Value = 1;
            return Value;
        }

        public static int PheromoneEvaporation(int n, int m, List<GrafParams> graf, List<AgentGroup> agent) //Испарение феромона
        {
            double L = 0.2;
            foreach (GrafParams grafElem in graf)
            {
                double Evaporation = L * Convert.ToDouble(grafElem.pheromones);

                foreach (AgentGroup agentElem in agent)
                {
                    foreach (int wayElem in agentElem.wayAgent) {
                        if (wayElem == grafElem.idParam)
                            Evaporation += (1 - L) * agentElem.delta;
                    }
                }

                //element.Pheromones = Convert.ToInt32(Evaporation);
            }
            
            return 0;
        }

        public static string GetHash(int[] way) //Получение Хэша
        {
            string StringWay = string.Join(",", way); //Получение строки
            byte[] StrSource = ASCIIEncoding.ASCII.GetBytes(StringWay); //Получение массива байтов 
            byte[] StrHash = new MD5CryptoServiceProvider().ComputeHash(StrSource); //Получение Хэша
            //Преобразование Хэша в строку
            string HashWay = ByteArrayToString(StrHash);

            return HashWay;
        }
        
        public static string ByteArrayToString(byte[] arrInput) //Преобрахование Хэша в строку
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        } 
    }
}
