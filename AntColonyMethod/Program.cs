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
        public int valueParam { get; set; }
        public int pheromones { get; set; }

        public int selectNum { get; set; }

        public override string ToString()
        {
            return "IdParam: " + idParam + "   NumParam: " + numParam + "   ValueParam: " + valueParam + "   Pheromones: " + pheromones;
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

        public static int THREADS_COUNT = 3; //Количество потоков

        static void Main(string[] args)
        {
            //Входные переменные
            int N = 4; //Количество параметров
            //int[] M = new int[N]; //Массив для хранения количество значений параметров
            int[] M = { 3, 1, 2, 5 }; //Тестовые значения
            int iterationCount = 10; // Число итераций в алгоритме
            int antCount = 3; //Количество муравьев

            //Создание Хэш-таблицы
            Hashtable hashTable = new Hashtable();

            //Получение входных данных
            GettingInputData(N, M, iterationCount, antCount);

            //Создание графа  
            List<GrafParams> Graf = new List<GrafParams>(); //Список элементов Graf
            CreateGraf(N, M, Graf);

            //Список всех возможных путей
            //List<int[]> WayList = new List<int[]>();
            //CreateListWay(N, M, WayList, Graf);

            //Прохождение всех итераций
            for (int j = 0; j < iterationCount; j++)
            {
                //Создание группы агентов
                List<AgentGroup> Agent = new List<AgentGroup>(); //Список агентов
                                                                 //Прохождение K агентов
                for (int i = 0; i < antCount; i++)
                {
                    Agent.Add(new AgentGroup() { idAgent = i });

                    //Определение пути агента
                    int[] newWayAgent;
                    newWayAgent = AgentMoving(N, M, Graf);
                   
                    do
                    {                       
                        //Вычисление Хэша пути
                        string hashWay = GetHash(newWayAgent);
                        Console.WriteLine(hashWay);

                        //Сравнение Хэша со значениями таблицы
                        if (!hashTable.ContainsKey(hashWay))
                        {
                            hashTable.Add(hashWay, newWayAgent); //Добавление нового ключа в таблицй                            
                            break;
                        }
                        else 
                        {
                            newWayAgent = AgentMoving(N, M, Graf);

                            //Поиск нового пути по бинарному дереву
                            //newWayAgent = FindNewWay(N, Graf);
                        }

                    } while (true);


                    //Сохранение пути агента
                    Agent[i].wayAgent.AddRange(newWayAgent);
                    Console.WriteLine(Agent[i]);

                    TargetFunction(Agent[i].wayAgent); //Вычисление значений критериев

                }

                //Занесение феромона
                for (int i = 0; i < antCount; i++)
                {
                    Agent[i].delta = AddPheromone(N, M, Agent[i].wayAgent, Graf);
                }

                //Испарение феромонов
                PheromoneEvaporation(Graf, Agent);
            }

        }

        public static int GettingInputData(int n, int[] m, int iterationCount, int antCount) //Получение исходныхи данных
        {
            return 0;
        }
 
        public static int CreateGraf(int n, int[] m, List<GrafParams> graf) //Создание графа 
        {
            //Заполнение списка элементов графа 
            int id = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m[i]; j++)
                {
                    graf.Add(new GrafParams() { idParam = id, numParam = i, pheromones = 1, selectNum = 0 });
                    id++;
                }
            }

            foreach (GrafParams element in graf)
            {
                Console.WriteLine(element);
                //Console.WriteLine("Num: {grafParams.NumParam}; Znach: {grafParams.Pheromones}");
            }

            return 0;
        }

        public static int[] AgentMoving(int n, int[] m, List<GrafParams> graf)
        {

            int[] way = new int[n]; //Выбранный путь           

            int i = 0;
            int k = 0;
            while (i < graf.Count)
            {
                //Выбор следующей вершины
                ChoiceNextVertex(i, m[k], graf, way);
                i += m[k];
                k++;
            }

            for (int j = 0; j < n; j++)
            {
                Console.Write(" " + way[j]);
            }
            Console.WriteLine();

            return way;
        }

        public static int ChoiceNextVertex(int i, //Точка начала просмотра графа 
                                           int m, List<GrafParams> graf, int[] way) //Выбор следующей вершины
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

        public static double AddPheromone(int n, int[] m, List<int> way, List<GrafParams> graf) //Добавление феромонов
        {
            int func = TargetFunction(way); //Значение целевой фцнкции
            int q = 100; //Общее число феромонов
            int delta = q / func;

            for (int i = 0; i < n; i++)
            {
                graf[way[i]].pheromones += delta;
            }
            return delta;
        }

        public static int TargetFunction(List<int> way) //Подсчет целивой функции
        {
            int Value = 1;
            return Value;
        }

        public static int PheromoneEvaporation(List<GrafParams> graf, List<AgentGroup> agent) //Испарение феромона
        {
            double L = 0.2;
            foreach (GrafParams grafElem in graf)
            {
                double Evaporation = L * Convert.ToDouble(grafElem.pheromones);

                foreach (AgentGroup agentElem in agent)
                {
                    foreach (int wayElem in agentElem.wayAgent)
                    {
                        if (wayElem == grafElem.idParam)
                            Evaporation += (1 - L) * agentElem.delta;
                    }
                }               
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

        public static int CreateListWay(int n, int[] m, List<int[]> wayList, List<GrafParams> graf) //Создание списка всех возможных путей 
        {           
            //Определение длины списка
            int listLength = 1;
            foreach (int elem in m) {
                listLength = listLength * elem;
            }

            int[] way = new int[n];

            return 0;
        }

        public static int[] FindNewWay(int n, List<GrafParams> graf) {
            int[] newWay = new int[n];
            return newWay;
        }
    }
}