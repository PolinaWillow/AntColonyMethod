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
        public static int ATTEMPTS_COUNT = 100; //Количество попыток генерации нового пути

        public static string PATH_TEST_FILE_DATA = "TestData1.txt"; //Файл тестовых значений


        static void Main(string[] args)
        {
            //Входные переменные
            int N = 0; //Количество параметров
            List<int> M = new List<int>(); //Список для хранения количества значений параметров
            List<string> valueData = new List<string>(); //Список входных значений

            int iterationCount = 9; // Число итераций в алгоритме
            int antCount = 7776; //Количество муравьев

            //Создание Хэш-таблицы
            Hashtable hashTable = new Hashtable();

            //Получение входных данных
            N = GettingInputData(M, iterationCount, antCount, valueData);

            int controlCount = 1;
            foreach (int elem in M) //Подсчет количества всех путей
            {
                controlCount = controlCount * elem;
            }

            //Создание графа  
            Graf graf = new Graf(); //Список элементов Graf
            graf.CreateGraf(N, M, valueData);

            string[] maxFunction = new string[N + 1]; //Массив хранения максимума функции и значения параметров
            string[] minFunction = new string[N + 1]; //Массив хранения минимума функции и значения параметров

            double min = double.MinValue;
            double max = double.MaxValue;

            //Прохождение всех итераций
            for (int j = 0; j < iterationCount; j++)
            {
                Console.WriteLine("\nИтерация № " + (j + 1));
                //Создание группы агентов
                AgentGroup agentGroup = new AgentGroup(); //Список агентов
                //Прохождение K агентов
                int CountAgent = 0;
                for (int i = 0; i < antCount; i++)
                {
                    CountAgent++;
                    agentGroup.Agent.Add(new Agent() { idAgent = i });
                    //Console.WriteLine("\nАгент № " + (i + 1));
                    //Определение пути агента

                    int[] wayAgent;
                    int attempt = 1;

                    wayAgent = graf.FindWay(N, M); //Генерация первичного пути
                    //Вычисление Хэша пути
                    string hashWay = Hash.GetHash(wayAgent);
                    if (!hashTable.ContainsKey(hashWay))
                    {
                        hashTable.Add(hashWay, wayAgent); //Добавление нового ключа в таблицй                            
                    }
                    else
                    {
                        int[] newWayAgent = FindAlternativeWay(N, graf, wayAgent, hashTable, hashWay);
                        hashWay = Hash.GetHash(newWayAgent);
                        hashTable.Add(hashWay, newWayAgent);
                        Array.Copy(newWayAgent, 0, wayAgent, 0, N);

                    }
                    //Console.WriteLine(CountAgent);

                    //int[] newWayAgent;
                    //int attempt = 0;
                    //do
                    //{
                    //    newWayAgent = AgentMoving(N, M, graf.Params);
                    //    attempt++;
                    //    //Вычисление Хэша пути
                    //    string hashWay = Hash.GetHash(newWayAgent);
                    //    //Console.WriteLine("Хэш выбранного пути: "+hashWay);

                    //    //Сравнение Хэша со значениями таблицы
                    //    if (!hashTable.ContainsKey(hashWay))
                    //    {
                    //        hashTable.Add(hashWay, newWayAgent); //Добавление нового ключа в таблицй                            
                    //        break;
                    //    }
                    //    if (controlCount == hashTable.Count)
                    //    {
                    //        controlCount = 0;
                    //        break;
                    //    }
                    //    //if (ATTEMPTS_COUNT == attempt) { graf.InitialGraf(); }
                    //} while (true);


                    //Сохранение пути агента
                    agentGroup.Agent[i].wayAgent.AddRange(wayAgent);
                    //Console.WriteLine(Agent[i]);

                    double valueFunction = Function.TargetFunction(agentGroup.Agent[i].wayAgent, graf.Params, N); //Вычисление значений критериев
                    //Console.WriteLine("Значение критерия: " + valueFunction + "\n");

                    //Поиск минимума и максимума критериев                   
                    if (valueFunction <= max)
                    {
                        minFunction[0] = Convert.ToString(valueFunction);
                        for (int k = 1; k < N + 1; k++)
                        {
                            minFunction[k] = graf.Params[wayAgent[k - 1]].valueParam;
                        }
                        max = valueFunction;
                    }

                    if (valueFunction >= min)
                    {
                        maxFunction[0] = Convert.ToString(valueFunction);
                        for (int k = 1; k < N + 1; k++)
                        {
                            maxFunction[k] = graf.Params[wayAgent[k - 1]].valueParam;
                        }
                        min = valueFunction;
                    }

                    // Сброс феромонов
                    if (attempt == ATTEMPTS_COUNT) { graf.InitialGraf(); }

                }

                //Занесение феромона
                for (int i = 0; i < antCount; i++)
                {
                    double function = Function.TargetFunction(agentGroup.Agent[i].wayAgent, graf.Params, N);
                    agentGroup.Agent[i].delta = graf.AddPheromone(N, M, agentGroup.Agent[i].wayAgent, function, Q);
                }

                //Испарение феромонов
                graf.PheromoneEvaporation(agentGroup.Agent, L);


                Console.Write("Максимум:  ");
                foreach (string elem in maxFunction)
                {
                    Console.Write(elem + " ");
                }
                Console.Write("\nМинимум:  ");
                foreach (string elem in minFunction)
                {
                    Console.Write(elem + " ");
                }

                //Console.Write("\n");
                //graf.PrintGraf();
                //Console.Write("\n");



            }

            Console.Write("\nМаксимум:  ");
            foreach (string elem in maxFunction)
            {
                Console.Write(elem + " ");
            }
            Console.Write("\nМинимум:  ");
            foreach (string elem in minFunction)
            {
                Console.Write(elem + " ");
            }
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

    }
}