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

            int iterationCount = 10; // Число итераций в алгоритме
            int antCount = 3; //Количество муравьев

            //Создание Хэш-таблицы
            Hashtable hashTable = new Hashtable();

            //Получение входных данных
            List<string> valueData = new List<string>();
            N = GettingInputData(M, iterationCount, antCount, valueData);

            //Создание графа  
            List<GrafParams> Graf = new List<GrafParams>(); //Список элементов Graf
            CreateGraf(N, M, Graf, valueData);

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
                            break;
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

        public static double AddPheromone(int n, List<int> m, List<int> way, List<GrafParams> graf) //Добавление феромонов
        {
            int func = TargetFunction(way); //Значение целевой фцнкции
            //int q = 100; //Общее число феромонов
            int delta = Q / func;

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