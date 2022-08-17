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
        public static int ATTEMPTS_COUNT = 100000; //Количество попыток генерации нового пути

        static void Main(string[] args)
        {
            //Входные переменные
            UserParamsTask userParamsTask = new UserParamsTask();
            userParamsTask.iterationCount = 9; 
            userParamsTask.antCount = 7776;

            //Получение входных данных
            DataTask dataTask = new DataTask();
            DataReader dataReader = new DataReader();
            dataTask = dataReader.GettingInputData();

            //Создание графа  
            dataTask.graf.CreateGraf(dataTask);

            string[] maxFunction = new string[dataTask.paramCount + 1]; //Массив хранения максимума функции и значения параметров
            string[] minFunction = new string[dataTask.paramCount + 1]; //Массив хранения минимума функции и значения параметров

            double max = double.MinValue;
            double min = double.MaxValue;

            DataWriter dataWriter = new DataWriter();
            string outputFile = dataWriter.CreateOutputFile(dataTask); 



            int attempt = 0;
            //Прохождение всех итераций
            for (int j = 0; j < userParamsTask.iterationCount; j++)
            {
                Console.WriteLine("\nИтерация № " + (j + 1));
                //Создание группы агентов
                AgentGroup agentGroup = new AgentGroup(); //Список агентов
                //Прохождение K агентов
                int CountAgent = 0;

                //Проход муравьев по графу
                if (dataTask.availabilityThread)
                {
                    Parallel.For(0, userParamsTask.antCount, (i, state) =>
                    {
                        AgentPassage(dataTask, agentGroup, CountAgent, attempt, min, max, maxFunction, minFunction);
                        max = Convert.ToDouble(maxFunction[0]);
                        min = Convert.ToDouble(minFunction[0]);
                    });
                }
                else
                {
                    for (int i = 0; i < userParamsTask.antCount; i++)
                    {
                        AgentPassage(dataTask, agentGroup, CountAgent, attempt, min, max, maxFunction, minFunction);
                        max = Convert.ToDouble(maxFunction[0]);
                        min = Convert.ToDouble(minFunction[0]);
                        //Console.WriteLine("min = " + min);
                    }
                }

                TargetFunction targetFun = new TargetFunction();
                //Занесение феромона
                for (int i = 0; i < userParamsTask.antCount; i++)
                {
                    double functionValue = targetFun.FindValue(agentGroup.Agents[i].wayAgent, dataTask.graf.Params, dataTask.paramCount);
                    agentGroup.Agents[i].delta = dataTask.graf.AddPheromone(dataTask, agentGroup.Agents[i].wayAgent, functionValue);
                }

                //Испарение феромонов
                dataTask.graf.PheromoneEvaporation(agentGroup.Agents);

                //Занесение результатов прохода итерации в файл
                dataWriter.GettingOutputData(outputFile, (j+1), maxFunction, minFunction);

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

        public static void AgentPassage(DataTask dataTask, AgentGroup agentGroup, int CountAgent, int attempt, double min, double max, string[] maxFunction, string[] minFunction)
        {
            CountAgent++;

            string id = Guid.NewGuid().ToString();
            agentGroup.AddNewAgent(id);

            Agent agent = agentGroup.FindAgent(id);

            //Определение пути агента
            int[] wayAgent = agent.FindAgentWay_Method1(dataTask);
            //int[] wayAgent = agent.FindAgentWay_Method2(N, M, graf, hashTable);
            attempt += 1;

            //Сохранение пути агента
            agentGroup.AddWayAgent(wayAgent, id);            

            //Поиск максимума и минимума
            TargetFunction targetFunction = new TargetFunction();

            targetFunction.FindMaxFunction(dataTask, agent, max, maxFunction, wayAgent);
            targetFunction.FindMinFunction(dataTask, agent, min, minFunction, wayAgent);            

            // Сброс феромонов
            if (attempt == ATTEMPTS_COUNT)
            {
                dataTask.graf.InitialGraf();
                attempt = 0;
            }
        }
    }
}