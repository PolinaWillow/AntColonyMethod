using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AntColonyMethod
{
    class Program
    {

        public static int THREADS_COUNT = 3; //Количество потоков
        public static int Q = 100;  //Общее число феромонов
        public static double L = 0.2; //Коэфициент пересчета испарения феромонов        

        public static string PATH_TEST_FILE_DATA = "TestData1.txt"; //Файл тестовых значений


        static void Main(string[] args)
        {
            //Входные переменные
            int N = 0; //Количество параметров
            List<int> M = new List<int>(); //Список для хранения количества значений параметров

            //Тестовые значения
            //int N = 4; 
            //int[] M = { 3, 1, 2, 5 }; 

            int iterationCount = 10000; // Число итераций в алгоритме
            int antCount = 503118; //Количество муравьев

            //Создание Хэш-таблицы
            Hashtable hashTable = new Hashtable();

            //Получение входных данных
            List<string> valueData = new List<string>();
            N = GettingInputData(M, iterationCount, antCount, valueData);

            //Создание графа  
            List<GrafParams> Graf = new List<GrafParams>(); //Список элементов Graf
            CreateGraf(N, M, Graf, valueData);

            string[] maxFunction = new string[N + 1];
            string[] minFunction = new string[N + 1];

            //Прохождение всех итераций
            for (int j = 0; j < iterationCount; j++)
            {
                Console.WriteLine("\nИтерация № " + (j + 1));
                //Создание группы агентов
                List<AgentGroup> Agent = new List<AgentGroup>(); //Список агентов
                                                                 //Прохождение K агентов
                for (int i = 0; i < antCount; i++)
                {
                    Agent.Add(new AgentGroup() { idAgent = i });
                    //Console.WriteLine("\nАгент № " + (i + 1));
                    //Определение пути агента
                    int[] newWayAgent;

                    do
                    {
                        newWayAgent = AgentMoving(N, M, Graf);
                        //Вычисление Хэша пути
                        string hashWay = GetHash(newWayAgent);
                        //Console.WriteLine("Хэш выбранного пути: "+hashWay);

                        //Сравнение Хэша со значениями таблицы
                        if (!hashTable.ContainsKey(hashWay))
                        {
                            hashTable.Add(hashWay, newWayAgent); //Добавление нового ключа в таблицй                            
                            break;
                        }
                    } while (true);


                    //Сохранение пути агента
                    Agent[i].wayAgent.AddRange(newWayAgent);
                    //Console.WriteLine(Agent[i]);

                    double valueFunction = TargetFunction(Agent[i].wayAgent, Graf, N); //Вычисление значений критериев
                    //Console.WriteLine("Значение критерия: " + valueFunction + "\n");

                    //Поиск минимума и максимума критериев
                    double max = double.MaxValue;                    
                    
                    if (valueFunction <= max)
                    {
                        minFunction[0] = Convert.ToString(valueFunction);
                        for (int k = 1; k < N + 1; k++) {
                            minFunction[k] =Graf[newWayAgent[k-1]].valueParam;
                        }
                    }

                    double min = double.MinValue;
                    
                    if (valueFunction >= min)
                    {
                        maxFunction[0] = Convert.ToString(valueFunction);
                        for (int k = 1; k < N + 1; k++)
                        {
                            maxFunction[k] = Graf[newWayAgent[k - 1]].valueParam;
                        }
                    }
                }

                //Занесение феромона
                for (int i = 0; i < antCount; i++)
                {
                    Agent[i].delta = AddPheromone(N, M, Agent[i].wayAgent, Graf);
                }

                //Console.WriteLine("После добавления феромонов");
                //foreach (GrafParams element in Graf)
                //{
                //    Console.WriteLine(element);
                //    //Console.WriteLine("Num: {grafParams.NumParam}; Znach: {grafParams.Pheromones}");
                //}

                //Испарение феромонов
                PheromoneEvaporation(Graf, Agent);
            }

            Console.Write("\nМаксимум:  ");
            foreach (string elem in maxFunction) {
                Console.Write(elem+" ");
            }
            Console.Write("\nМинимум:  ");
            foreach (string elem in minFunction)
            {
                Console.Write(elem + " ");
            }
        }

        public static int GettingInputData(List<int> m, int iterationCount, int antCount, List<string> valueData) //Получение исходныхи данных
        {
            int n = 0;
            using (var sr = new StreamReader(PATH_TEST_FILE_DATA))
            {
                //Чтение числа пораметров
                string countParam = sr.ReadLine();
                n = Convert.ToInt32(countParam);
                //Console.WriteLine(n + "\n");

                //Определение массива количества значений параметров
                string countValueParam = sr.ReadLine();
                m.AddRange(countValueParam.Split(' ').Select(x => Convert.ToInt32(x)).ToList());

                //foreach (int mElem in m)
                //{
                //    Console.Write(mElem + " ");
                //}

                //Считывание всех значений параметров 
                for (int i = 0; i < n; i++)
                {
                    string valueParam = sr.ReadLine();
                    valueData.AddRange(valueParam.Split(' '));
                }
                //Console.WriteLine();
                //foreach (string elem in valueData)
                //{
                //    Console.Write(elem + " ");
                //}

                sr.Close();
            }
            return n;
        }

        public static int CreateGraf(int n, List<int> m, List<GrafParams> graf, List<string> valueData) //Создание графа 
        {
            //Заполнение списка элементов графа 
            int id = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m[i]; j++)
                {
                    graf.Add(new GrafParams() { idParam = id, numParam = i, pheromones = 1, selectNum = 0 });

                    //Опредление типа значения параметра
                    if (double.TryParse(valueData[id], out double res))
                    {
                        graf[id].typeParam = TypeNumerator.Double;
                    }
                    else { graf[id].typeParam = TypeNumerator.String; }
                    graf[id].valueParam = valueData[id];

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

        public static int[] AgentMoving(int n, List<int> m, List<GrafParams> graf)
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

            //for (int j = 0; j < n; j++)
            //{
            //    Console.Write(" " + way[j]);
            //}
            //Console.WriteLine();

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
            //Console.WriteLine("Raram: " + graf[i].numParam + " SumPheromones: "+ sumPheromones);

            //Подсчет вероятности попадания
            for (int j = 0; j < m; j++)
            {
                Pij[j] = Convert.ToDouble(graf[i + j].pheromones) / Convert.ToDouble(sumPheromones);
                Idij[j] = graf[i + j].idParam;
            }

            //Console.Write("Вероятности попадания: ");
            //foreach (double elem in Pij) 
            //{
            //    Console.Write(elem + "  ");

            //}
            //Console.WriteLine();

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

        public static double AddPheromone(int n, List<int> m, List<int> way, List<GrafParams> graf) //Добавление феромонов
        {
            int func = Convert.ToInt32(TargetFunction(way, graf, n)); //Значение целевой фцнкции
            //int q = 100; //Общее число феромонов

            if (func == 0) {
                func = 1;
                Console.Write("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                foreach (int elem in way) {
                    Console.Write(elem + " ");
                }
                Console.Write("\n");
            }

            int delta = Q / func;

            for (int i = 0; i < n; i++)
            {
                graf[way[i]].pheromones += delta;
            }

            return delta;
        }

        public static double TargetFunction(List<int> way, List<GrafParams> graf, int n) //Подсчет целивой функции
        {
            //way.Clear();
            //way.AddRange(new[] { 1, 9, 13, 23, 42, 45, 63, 73, 75, 86, 98 });

            double[] path = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                path[i] = Convert.ToDouble(graf[way[i]].valueParam);
            }

            double Value = path[0] - path[1] + 2 * path[2] + path[3] + 2 * path[4] + 0.5 * path[5] - 0.12 * path[6] - path[7] + 80 * path[8] + 0.00001 * path[9];

            if (string.Compare(graf[way[10]].valueParam, "Сильное") == 0)
            {
                Value += 20;
            }
            return Value;
        }

        public static int PheromoneEvaporation(List<GrafParams> graf, List<AgentGroup> agent) //Испарение феромона
        {
            //double L = 0.2;
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
    }
}