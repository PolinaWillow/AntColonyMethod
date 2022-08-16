using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class Program
    {

        public static int THREADS_COUNT = 3; //Количество потоков

        public static int ATTEMPTS_COUNT = 100000; //Количество попыток генерации нового пути

        public static string PATH_TEST_FILE_DATA = "TestData1.txt"; //Файл тестовых значений


        static void Main(string[] args)
        {
            //Входные переменные
            int N; //Количество параметров
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

            int attempt = 0;
            //Прохождение всех итераций
            for (int j = 0; j < iterationCount; j++)
            {
                Console.WriteLine("\nИтерация № " + (j + 1));
                //Создание группы агентов
                AgentGroup agentGroup = new AgentGroup(); //Список агентов
                //Прохождение K агентов
                int CountAgent = 0;
                
                
                Parallel.For(0, antCount, (i, state) =>
               {
                   CountAgent++;
                   agentGroup.AddNewAgent(i);

                   Agent agent = agentGroup.FindAgent(i);
                   //Определение пути агента
                   if (agent == null)
                   {
                       state.Break();
                   }
                   int[] wayAgent = agent.FindAgentWay_Method1(N, M, graf, hashTable);
                   //int[] wayAgent = agent.FindAgentWay_Method2(N, M, graf, hashTable);
                   attempt += 1;

                   //Сохранение пути агента
                   agentGroup.AddWayAgent(wayAgent, i);
                   //Console.WriteLine(Agent[i]);

                   TargetFunction targetFunction = new TargetFunction();

                   double valueFunction = targetFunction.FindValue(agent.wayAgent, graf.Params, N); //Вычисление значений критериев
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
                   if (attempt == ATTEMPTS_COUNT)
                   {
                       graf.InitialGraf();
                       attempt = 0;
                   }

               });

                TargetFunction targetFun = new TargetFunction();
                //Занесение феромона
                for (int i = 0; i < antCount; i++)
                {
                    double function = targetFun.FindValue(agentGroup.Agents[i].wayAgent, graf.Params, N);
                    agentGroup.Agents[i].delta = graf.AddPheromone(N, M, agentGroup.Agents[i].wayAgent, function);
                }

                //Испарение феромонов
                graf.PheromoneEvaporation(agentGroup.Agents);


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

            }

            Console.Write("\n\nМаксимум:  ");
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